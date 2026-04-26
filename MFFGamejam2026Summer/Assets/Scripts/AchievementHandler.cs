using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class AchievementHandler : MonoBehaviour
{
    public static AchievementHandler Instance { get; private set; }

    [SerializeField] private List<AchievementInternal> achievements = new();

    private Dictionary<int, AchievementInternal> lookup = new();
    private Dictionary<string, int> counters = new();
    private HashSet<int> completed = new();

    private Dictionary<AchievementInternal, float> pendingSlide = new();
    private bool flushScheduled = false;

    public int completedCount = 0;
    public int allAchCount = 0;

    public GameObject windowToBeSpawned;
    public GameObject windowToBeSpawned2;
    private bool DONE = false;


    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        lookup.Clear();
        foreach (var a in achievements)
        {
            if (lookup.ContainsKey(a.ID))
                Debug.LogWarning("Duplicate: " + a.ID);
            else
                lookup[a.ID] = a;
        }

        allAchCount = achievements.Count;
    }

    private void Start()
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg == null)
            cg = gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 0;
        StartCoroutine(SlideInRoutine());
        StartCoroutine(SpawnOneWindow());
    }

    private IEnumerator SpawnOneWindow()
    {
        yield return new WaitForSeconds(Random.Range(15f, 23f));
        windowToBeSpawned.gameObject.SetActive(true);

        yield return new WaitForSeconds(Random.Range(3f, 10f));
        windowToBeSpawned2.gameObject.SetActive(true);
    }

    private IEnumerator SlideInRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        RectTransform panel = GetComponent<RectTransform>();
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg == null)
            cg = gameObject.AddComponent<CanvasGroup>();

        Vector2 startPos = panel.anchoredPosition;
        Vector2 endPos = startPos + new Vector2(600, 0f);
        float duration = 0.6f;
        float elapsed = 0f;

        panel.anchoredPosition = startPos;
        cg.alpha = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            panel.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            cg.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        panel.anchoredPosition = endPos;
        cg.alpha = 1f;

        if (ClippyManager.Instance != null)
            ClippyManager.Instance.OnAchievementsAppeared();
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {

        }
    }

    public void ClickDetected()
    {
        Debug.Log("Click");
        Increment("clicks");
        CheckConditions("click");
    }

    private void CheckConditions(string trigger)
    {
        switch (trigger)
        {
            case "click":
                if (GetCounter("clicks") >= 20)
                    TryComplete(20);
                if (GetCounter("clicks") >= 2)
                    TryComplete(67);
                if (GetCounter("clicks") >= 50)
                    TryComplete(50);
                if (GetCounter("clicks") >= 100)
                    TryComplete(100);
                break;
        }
    }

    public void TryComplete(int id)
    {

        if (completed.Contains(id)) return;
        if (!lookup.TryGetValue(id, out var achievement))
        {
            Debug.LogWarning("ID not found: " + id);
            return;
        }
        completedCount++;
        completed.Add(id);
        achievements.Remove(achievement);
        lookup.Remove(id);
        achievement.Complete();
        Debug.Log($"Completed: {achievement.Title} (ID {id})");
        


        if (completedCount >= allAchCount)
        {

            if (ClippyManager.Instance != null && !DONE)
            {
                DONE = true;
                ClippyManager.Instance.OnDone();
            }
            WinGame();
            return;
        }

        if (ClippyManager.Instance != null)
            ClippyManager.Instance.OnAchievementCompleted(completedCount, allAchCount);
    }

    private IEnumerator LoadSceneUWU(string sceneName)
    {
        yield return new WaitForSeconds(5f);

        SceneManager.LoadScene(sceneName);
    }


    public void WinGame()
    {
        StartCoroutine(LoadSceneUWU("WinScene"));
    }

    public void OnAchievementDying(AchievementInternal dying)
    {
        int dyingIndex = dying.transform.GetSiblingIndex();
        Transform parent = dying.transform.parent;

        for (int i = 0; i < parent.childCount; i++)
        {
            if (i >= dyingIndex) continue;

            Transform sibling = parent.GetChild(i);
            if (sibling == dying.transform) continue;

            AchievementInternal a = sibling.GetComponent<AchievementInternal>();
            if (a == null) continue;

            if (!pendingSlide.ContainsKey(a))
                pendingSlide[a] = 0f;

            pendingSlide[a] += dying.SlideDownBy;
        }

        if (!flushScheduled)
        {
            flushScheduled = true;
            StartCoroutine(FlushSlidesEndOfFrame(dying.SlideDuration));
        }
    }

    private IEnumerator FlushSlidesEndOfFrame(float duration)
    {
        yield return new WaitForEndOfFrame();

        foreach (var kvp in pendingSlide)
        {
            if (kvp.Key != null)
                kvp.Key.SlideDown(kvp.Value, duration);
        }

        pendingSlide.Clear();
        flushScheduled = false;
    }

    public void Increment(string counter)
    {
        if (!counters.ContainsKey(counter))
            counters[counter] = 0;
        counters[counter]++;
    }

    public int GetCounter(string counter)
    {
        if (!counters.ContainsKey(counter))
            return 0;
        return counters[counter];
    }
}