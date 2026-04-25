using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private Texture2D cursorLoadingTexture;
    [SerializeField] private Texture2D cursorBannedInteractionTexture;

    [SerializeField] private Texture2D resizeTexture;
    private Vector2 hotSpot = Vector2.zero;
    void Start()
    {
        SetCursorDefault();
    }

    public void SetCursorDefault()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
    }
    public void SetCursorLoading()
    {
        Cursor.SetCursor(cursorLoadingTexture, hotSpot, CursorMode.Auto);
    }

    public void SetCursorBannedInteraction()
    {
        Cursor.SetCursor(cursorBannedInteractionTexture, hotSpot, CursorMode.Auto);
    }

    public void SetCursorResize()
    {
        Cursor.SetCursor(resizeTexture, hotSpot, CursorMode.Auto);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // On hover event, determine the appropriate cursor based on the context
        //SetCursorBannedInteraction();
        //SetCursorLoading();
        //SetCursorDefault();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        SetCursorDefault();
    }
}
