using System.Collections.Generic;
using UnityEngine;

public enum CloseSpawnMode
{
    SpawnAll,
    SpawnRandomWeighted,
}

[System.Serializable]
public class WindowFlowLink
{
    [Tooltip("Drag a WindowFlowNode asset here.")]
    public WindowFlowNodeAsset nextNode;

    [Min(0f)] public float weight = 1f;
}

[CreateAssetMenu(menuName = "Windows/Flow Node", fileName = "WindowFlowNode")]
public class WindowFlowNodeAsset : ScriptableObject
{
    [Tooltip("Title shown on the window title bar.")]
    public string title;

    public GameObject contentPrefab;
    public bool applyEvade;
    public bool isClosable = true;

    public CloseSpawnMode closeSpawnMode = CloseSpawnMode.SpawnAll;

    [Min(1)]
    [Tooltip("Used only by SpawnRandomWeighted. Spawns this many unique next nodes if available.")]
    public int randomSpawnCount = 1;

    public List<WindowFlowLink> nextWindows = new List<WindowFlowLink>();
}



