using UnityEngine;
using UnityEngine.InputSystem;

public class WindowCloseEvader : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform windowRect;
    [SerializeField] private RectTransform closeButtonRect;
    private RectTransform _canvasRect;
    private Canvas _canvas;

    [Header("Movement")]
    [SerializeField] private float triggerDistance = 120f;
    [SerializeField] private float maxSpeed = 650f;
    [SerializeField] private float edgePadding = 8f;

    private void Awake()
    {
        // Get the canvas rect from the WindowManager singleton
        _canvasRect = WindowManager.Instance.CanvasRect;

        if (windowRect == null)
            windowRect = transform as RectTransform;

        if (closeButtonRect == null)
        {
            var controller = GetComponent<WindowController>();
            if (controller != null)
                closeButtonRect = controller.GetCloseButtonRect();
        }

        if (_canvasRect == null)
        {
            var parentCanvas = GetComponentInParent<Canvas>();
            if (parentCanvas != null)
            {
                _canvas = parentCanvas.rootCanvas;
                _canvasRect = _canvas.transform as RectTransform;
            }
        }
        else
        {
            _canvas = _canvasRect.GetComponent<Canvas>();
        }
    }

    private void Update()
    {
        if (windowRect == null || closeButtonRect == null || _canvasRect == null)
        {
            Debug.LogWarning("WindowCloseEvader: Missing required references!");
            return;
        }

        if (!TryGetPointerScreenPosition(out Vector2 pointerScreenPos))
            return;

        Vector2 closeButtonScreenPos = GetCloseButtonScreenPosition();

        Vector2 awayFromMouse = closeButtonScreenPos - pointerScreenPos;
        float distance = awayFromMouse.magnitude;

        if (distance >= triggerDistance || distance < 0.001f)
            return;

        float proximity = 1f - Mathf.Clamp01(distance / triggerDistance);
        float speed = maxSpeed * proximity;
        Vector2 nextPosition = windowRect.anchoredPosition + awayFromMouse.normalized * (speed * Time.unscaledDeltaTime);

        windowRect.anchoredPosition = ClampInsideCanvas(nextPosition);
    }

    private Vector2 GetCloseButtonScreenPosition()
    {
        if (_canvas == null)
            return CanvasLocalToScreen(_canvasRect.InverseTransformPoint(closeButtonRect.position));

        switch (_canvas.renderMode)
        {
            // In overlay mode the rect's world position is already in screen pixels.
            case RenderMode.ScreenSpaceOverlay:
                return closeButtonRect.position;

            case RenderMode.ScreenSpaceCamera:
            {
                Camera cam = _canvas.worldCamera != null ? _canvas.worldCamera : Camera.main;
                if (cam != null)
                    return RectTransformUtility.WorldToScreenPoint(cam, closeButtonRect.position);
                return closeButtonRect.position;  // fallback: treat like overlay
            }

            case RenderMode.WorldSpace:
            {
                Camera cam = Camera.main;
                if (cam != null)
                    return RectTransformUtility.WorldToScreenPoint(cam, closeButtonRect.position);
                goto default;
            }

            default:
                return CanvasLocalToScreen(_canvasRect.InverseTransformPoint(closeButtonRect.position));
        }
    }

    private Vector2 CanvasLocalToScreen(Vector2 canvasLocalPos)
    {
        Vector2 canvasSize = _canvasRect.rect.size;
        Vector2 pivot = _canvasRect.pivot;

        float normalizedX = (canvasLocalPos.x + canvasSize.x * pivot.x) / canvasSize.x;
        float normalizedY = (canvasLocalPos.y + canvasSize.y * pivot.y) / canvasSize.y;

        return new Vector2(normalizedX * Screen.width, normalizedY * Screen.height);
    }

    private bool TryGetPointerScreenPosition(out Vector2 pointerScreenPosition)
    {
        pointerScreenPosition = default;

        if (Pointer.current == null)
            return false;

        pointerScreenPosition = Pointer.current.position.ReadValue();
        return true;
    }

    private Vector2 ClampInsideCanvas(Vector2 targetPosition)
    {
        Vector2 size = windowRect.rect.size;
        Vector2 pivot = windowRect.pivot;
        Rect canvasBounds = _canvasRect.rect;

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