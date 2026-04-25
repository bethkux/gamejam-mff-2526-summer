using UnityEngine;
using UnityEngine.EventSystems;

public class WindowDrag : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    // drag the root window, not the titlebar
    [SerializeField] RectTransform window; 
    
    private Vector2 dragOffset;

    public void OnPointerDown(PointerEventData e)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            window.parent as RectTransform,
            e.position,
            e.pressEventCamera,
            out Vector2 localPoint
        );
        dragOffset = window.anchoredPosition - localPoint;
    }

    public void OnDrag(PointerEventData e)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            window.parent as RectTransform,
            e.position,
            e.pressEventCamera,
            out Vector2 localPoint
        );
        window.anchoredPosition = localPoint + dragOffset;
    }
}