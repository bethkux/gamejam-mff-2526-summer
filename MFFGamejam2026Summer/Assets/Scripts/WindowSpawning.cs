using UnityEngine;

public class WindowSpawner : MonoBehaviour
{
    [Header("Flow Spawn")]
    [SerializeField] private WindowFlowNodeAsset flowNode;

    public void SpawnWindow()
    {
        if (WindowManager.Instance == null)
        {
            Debug.LogError("WindowManager instance is not available.");
            return;
        }
        
        if (flowNode == null)
        {
            Debug.LogError("Flow node asset is not assigned.");
            return;
        }

        WindowManager.Instance.SpawnWindowFromNode(flowNode);
        return;
    }
}