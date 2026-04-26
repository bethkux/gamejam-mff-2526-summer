using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AchievementHandler : MonoBehaviour
{
    [SerializeField] private List<AchievementInternal> achievements = new();

    private Dictionary<int, AchievementInternal> lookup = new();
    private Dictionary<string, int> counters = new();
    private HashSet<int> completed = new();


    private void Start()
    {
        // setup lookup dic for notifs
        lookup.Clear();
        foreach (var a in achievements)
        {
            if (lookup.ContainsKey(a.ID))
                Debug.Log("Duplicate: " + a.ID);
            else
                lookup[a.ID] = a;
        }
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)   // check if pressed
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
        if (completed.Contains(id)) return; // we dont need to do anything, it was completed

        if (!lookup.TryGetValue(id, out var achievement))   // if the value is not known
        {
            Debug.Log("ID not found:" + id);
            return;
        }

        completed.Add(id);
        achievements.Remove(achievement);
        achievement.Complete();

        Debug.Log($"Completed: {achievement.Title} (ID {id})");
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