using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AchievementHandler : MonoBehaviour
{
    public static AchievementHandler Instance { get; private set; }

    [SerializeField] private List<AchievementInternal> achievements = new();

    private Dictionary<int, AchievementInternal> lookup = new();
    private Dictionary<string, int> counters = new();
    private HashSet<int> completed = new();

    private Dictionary<AchievementInternal, float> pendingSlide = new();
    private bool flushScheduled = false;

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
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Increment("clicks");
            CheckConditions("click");
        }
    }

    private void CheckConditions(string trigger)
    {
        switch (trigger)
        {
            case "click":
                if (GetCounter("clicks") >= 20)
                    TryComplete(20);
                if (GetCounter("clicks") == 1)
                    TryComplete(67);
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
        completed.Add(id);
        achievements.Remove(achievement);
        lookup.Remove(id);
        achievement.Complete();
        Debug.Log($"Completed: {achievement.Title} (ID {id})");
    }

    public void OnAchievementDying(AchievementInternal dying)
    {
        int dyingIndex = dying.transform.GetSiblingIndex();
        Transform parent = dying.transform.parent;


        for (int i = 0; i < parent.childCount; i++)
        {
            if (i >= dyingIndex) continue; // only slide ones visually above

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