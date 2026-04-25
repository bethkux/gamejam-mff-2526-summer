using UnityEngine;

public class WindowCloseEvader : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform windowRect;
    [SerializeField] private RectTransform closeButtonRect;
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private Camera uiCamera;

    [Header("Movement")]
    [SerializeField] private float triggerDistance = 120f;
    [SerializeField] private float maxSpeed = 650f;
    [SerializeField] private float edgePadding = 8f;

    private void Awake()
    {
        if (windowRect == null)
            windowRect = transform as RectTransform;

        if (closeButtonRect == null)
        {
            var controller = GetComponent<WindowController>();
            if (controller != null)
                closeButtonRect = controller.GetCloseButtonRect();
        }

        if (canvasRect == null)
        {
            var parentCanvas = GetComponentInParent<Canvas>();
            if (parentCanvas != null)
                canvasRect = parentCanvas.transform as RectTransform;

            if (uiCamera == null && parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
                uiCamera = parentCanvas.worldCamera;
        }
    }

    private void Update()
    {
        if (windowRect == null || closeButtonRect == null || canvasRect == null)
            return;

        Vector2 closeButtonCenter = RectTransformUtility.WorldToScreenPoint(
            uiCamera,
            closeButtonRect.TransformPoint(closeButtonRect.rect.center));

        Vector2 awayFromMouse = closeButtonCenter - (Vector2)Input.mousePosition;
        float distance = awayFromMouse.magnitude;

        if (distance >= triggerDistance || distance < 0.001f)
            return;

        float proximity = 1f - Mathf.Clamp01(distance / triggerDistance);
        float speed = maxSpeed * proximity;
        Vector2 nextPosition = windowRect.anchoredPosition + awayFromMouse.normalized * speed * Time.unscaledDeltaTime;

        windowRect.anchoredPosition = ClampInsideCanvas(nextPosition);
    }

    private Vector2 ClampInsideCanvas(Vector2 targetPosition)
    {
        Vector2 size = windowRect.rect.size;
        Vector2 pivot = windowRect.pivot;
        Rect canvasBounds = canvasRect.rect;

        float minX = canvasBounds.xMin + size.x * pivot.x + edgePadding;
        float maxX = canvasBounds.xMax - size.x * (1f - pivot.x) - edgePadding;
        float minY = canvasBounds.yMin + size.y * pivot.y + edgePadding;
        float maxY = canvasBounds.yMax - size.y * (1f - pivot.y) - edgePadding;

        if (minX > maxX)
            targetPosition.x = (canvasBounds.xMin + canvasBounds.xMax) * 0.5f;
        else
            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);

        if (minY > maxY)
            targetPosition.y = (canvasBounds.yMin + canvasBounds.yMax) * 0.5f;
        else
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        return targetPosition;
    }
}

