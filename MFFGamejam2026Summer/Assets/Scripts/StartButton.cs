using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    [SerializeField] private List<GameObject> contentPrefabs;
    [SerializeField] private List<string> windowNames;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        if (_button != null)
            _button.onClick.AddListener(HandleClick);
    }

    private void OnDisable()
    {
        if (_button != null)
            _button.onClick.RemoveListener(HandleClick);
    }

    private void HandleClick()
    {
        if (contentPrefabs.Count > 0 && windowNames.Count > 0 && contentPrefabs.Count == windowNames.Count)
        {
            int randomIndex = Random.Range(0, contentPrefabs.Count);
            GameObject selectedPrefab = contentPrefabs[randomIndex];
            string selectedWindowName = windowNames[randomIndex];

            // 5% chance to apply evade
            if (Random.Range(0, 100) < 5)
            {
                WindowManager.Instance.SpawnWindow(selectedWindowName, selectedPrefab, true);

            }
            else
            {
                WindowManager.Instance.SpawnWindow(windowNames[randomIndex], selectedPrefab, false);
            }
        }
        else
        {
            Debug.LogError("Content Prefabs and Window Names lists must be non-empty and of the same length.");
        }
    }
}