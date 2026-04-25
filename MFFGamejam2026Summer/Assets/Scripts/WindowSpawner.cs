using UnityEngine;
using UnityEngine.UI;

public class WindowManager : MonoBehaviour
{
    public static WindowManager Instance { get; private set; }

    [SerializeField] private WindowController windowPrefab;
    [SerializeField] private RectTransform canvas;

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
        return window;
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

