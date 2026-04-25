using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ClickableImage : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent OnImageClicked;

    private void Awake()
    {
        if (GetComponent<Image>() == null)
        {
            Debug.LogError("ClickableImage script requires an Image component on the same GameObject.");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Invoke the event when the image is clicked
        OnImageClicked?.Invoke();
    }
}
