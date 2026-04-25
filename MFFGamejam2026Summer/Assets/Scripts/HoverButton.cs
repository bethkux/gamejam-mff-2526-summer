using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class HoverButton: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Images")]
    public Sprite normalImage;
    public Sprite hoverImage;

    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();

        // Ensure starting image is correct
        if (normalImage != null)
            _image.sprite = normalImage;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverImage != null)
            _image.sprite = hoverImage;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (normalImage != null)
            _image.sprite = normalImage;
    }
}