using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowManager : MonoBehaviour
{
    public static WindowManager Instance { get; private set; }

    [SerializeField] private WindowController windowPrefab;
    [SerializeField] private RectTransform canvas;

    public RectTransform CanvasRect => canvas;
    
    
    // Read-only view of all currently open windows
    public IReadOnlyList<WindowController> OpenWindows => _openWindows;
    [SerializeField] private List<WindowController> _openWindows = new List<WindowController>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public WindowController SpawnWindow(string windowName, GameObject contentPrefab)
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

        return window;
    }

    private void HandleWindowClosed(WindowController window)
    {
        window.OnWindowClosed -= HandleWindowClosed;
        _openWindows.Remove(window);
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