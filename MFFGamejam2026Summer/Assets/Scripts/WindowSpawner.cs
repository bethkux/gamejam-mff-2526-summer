using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class WindowManager : MonoBehaviour
{
    public static WindowManager Instance { get; private set; }

    [SerializeField] private WindowController windowPrefab;
    [SerializeField] private RectTransform canvas;
    [Header("Close Flow")]
    [SerializeField] private bool enableCloseGraph = true;
    [SerializeField] private int maxWindowsSpawnedPerClose = 4;

    [FormerlySerializedAs("flowNode")]
    [Header("Lose State Flow")]
    [SerializeField] private List<WindowFlowNodeAsset> flowNodeList = new List<WindowFlowNodeAsset>();

    [SerializeField] private float timeUntilNewGraphStarts = 45.0f;
    private float _initialTimeUntilNewGraphStarts;
    private int _graphIndex = 0;

    public UnityEvent freezeMouse;
    
    public RectTransform CanvasRect => canvas;
    
    
    // Read-only view of all currently open windows
    public IReadOnlyList<WindowController> OpenWindows => _openWindows;
    [SerializeField] private List<WindowController> _openWindows = new List<WindowController>();
    private readonly Dictionary<WindowController, WindowFlowNodeAsset> _windowNodeMap = new Dictionary<WindowController, WindowFlowNodeAsset>();


    public int closedWindowsCount = 0;
    public UnityEvent closedWindows1;
    public UnityEvent closedWindows2;

    private bool completedA = false;
    private bool completedB = false;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        _initialTimeUntilNewGraphStarts = timeUntilNewGraphStarts;

    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Update()
    {
        timeUntilNewGraphStarts -= Time.deltaTime;
        if (timeUntilNewGraphStarts <= 0.0f)
        {
            timeUntilNewGraphStarts = _initialTimeUntilNewGraphStarts;
            SpawnWindowFromNode(flowNodeList[_graphIndex]);
            _graphIndex = (_graphIndex + 1) % flowNodeList.Count;
        }

        if (OpenWindows.Count > 20)
        {
            freezeMouse.Invoke();
            GameOver();
        }
    }

    private IEnumerator LoadSceneWithSound(string sceneName)
    {
        AudioSource source = AudioManager.Instance.PlaySFX("Shutdown");

        float delay = 1.5f;

        if (source != null && source.clip != null)
        {
            // account for pitch affecting duration
            delay = source.clip.length / Mathf.Abs(source.pitch);
        }

        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene(sceneName);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneWithSound(sceneName));
    }

    public void GameOver()
    {
        LoadScene("CreditsScene");
    }

    public WindowController SpawnWindow(string windowName, GameObject contentPrefab, bool evade = false)
    {
        if (windowPrefab == null || canvas == null)
        {
            Debug.LogError("WindowSpawner is missing references.");
            return null;
        }

        var window = WindowController.Create(windowPrefab, canvas, windowName, contentPrefab);
        PlaceRandomInsideCanvas(window.transform as RectTransform);

        _openWindows.Add(window);
        window.OnWindowClosed += HandleWindowClosed;
        
        window.SetEvading(evade);

        return window;
    }

    public WindowController SpawnWindowFromNode(WindowFlowNodeAsset node)
    {
        if (node == null)
        {
            Debug.LogWarning("Tried to spawn a null flow node.");
            return null;
        }

        var title = string.IsNullOrWhiteSpace(node.title) ? node.name : node.title;
        var window = SpawnWindow(title, node.contentPrefab, node.applyEvade);
        if (window != null)
        {
            window.IsClosable = node.isClosable;
            _windowNodeMap[window] = node;
        }

        return window;
    }

    private void HandleWindowClosed(WindowController window)
    {
        window.OnWindowClosed -= HandleWindowClosed;
        _openWindows.Remove(window);
        closedWindowsCount++;
        Debug.Log(closedWindowsCount);

        if (!completedA && closedWindowsCount >= 10)
        {
            closedWindows1.Invoke();
            completedA = true;
        }

        if (!completedB && closedWindowsCount >= 30)
        {
            closedWindows2.Invoke();
            completedB = true;
        }

        _windowNodeMap.TryGetValue(window, out var closedNode);
        _windowNodeMap.Remove(window);

        if (enableCloseGraph)
            SpawnGraphFollowers(closedNode);
    }

    private void SpawnGraphFollowers(WindowFlowNodeAsset closedNode)
    {
        if (closedNode == null)
            return;

        if (closedNode.nextWindows == null || closedNode.nextWindows.Count == 0)
            return;

        int safeLimit = Mathf.Max(1, maxWindowsSpawnedPerClose);
        int spawned = 0;

        if (closedNode.closeSpawnMode == CloseSpawnMode.SpawnAll)
        {
            for (int i = 0; i < closedNode.nextWindows.Count && spawned < safeLimit; i++)
            {
                var nextNode = closedNode.nextWindows[i] != null ? closedNode.nextWindows[i].nextNode : null;
                if (nextNode == null)
                    continue;

                if (SpawnWindowFromNode(nextNode) != null)
                    spawned++;
            }

            return;
        }

        int countToSpawn = Mathf.Min(Mathf.Max(1, closedNode.randomSpawnCount), safeLimit);
        var pool = new List<WindowFlowLink>(closedNode.nextWindows);

        while (pool.Count > 0 && spawned < countToSpawn)
        {
            var pick = PickWeighted(pool);
            if (pick == null)
                break;

            pool.Remove(pick);

            var nextNode = pick.nextNode;
            if (nextNode == null)
                continue;

            if (SpawnWindowFromNode(nextNode) != null)
                spawned++;
        }
    }

    private WindowFlowLink PickWeighted(List<WindowFlowLink> links)
    {
        float total = 0f;
        for (int i = 0; i < links.Count; i++)
            total += Mathf.Max(0f, links[i].weight);

        if (total <= 0f)
            return links.Count > 0 ? links[Random.Range(0, links.Count)] : null;

        float roll = Random.value * total;
        float cumulative = 0f;

        for (int i = 0; i < links.Count; i++)
        {
            cumulative += Mathf.Max(0f, links[i].weight);
            if (roll <= cumulative)
                return links[i];
        }

        return links[links.Count - 1];
    }


    public WindowController GetWindow(string windowName)
    {
        return _openWindows.Find(w => w.WindowName == windowName);
    }

    public bool IsWindowOpen(string windowName)
    {
        return _openWindows.Exists(w => w.WindowName == windowName);
    }

    public void CloseAllWindows()
    {
        foreach (var window in new List<WindowController>(_openWindows))
            window.Close();
    }

    private void PlaceRandomInsideCanvas(RectTransform windowRect)
    {
        if (windowRect == null)
            return;

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(windowRect);

        Vector2 windowSize = windowRect.rect.size;
        Rect canvasRect = canvas.rect;
        Vector2 pivot = windowRect.pivot;

        float minX = canvasRect.xMin + windowSize.x * pivot.x;
        float maxX = canvasRect.xMax - windowSize.x * (1f - pivot.x);
        float minY = canvasRect.yMin + windowSize.y * pivot.y;
        float maxY = canvasRect.yMax - windowSize.y * (1f - pivot.y);

        float randomX = minX <= maxX ? Random.Range(minX, maxX) : (canvasRect.xMin + canvasRect.xMax) * 0.5f;
        float randomY = minY <= maxY ? Random.Range(minY, maxY) : (canvasRect.yMin + canvasRect.yMax) * 0.5f;

        windowRect.anchoredPosition = new Vector2(randomX, randomY);
    }
}