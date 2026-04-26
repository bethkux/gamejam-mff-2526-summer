using UnityEngine;
using UnityEngine.EventSystems;

public class OnHoverDestroy : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Destroy(gameObject);
    }
}
