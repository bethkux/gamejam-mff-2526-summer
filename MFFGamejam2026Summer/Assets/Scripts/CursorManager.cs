using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [Header("Cursor Textures")]
    [SerializeField] private Texture2D cursorDefault;
    [SerializeField] private Texture2D cursorLoading;
    [SerializeField] private Texture2D cursorBannedInteraction;
    [SerializeField] private Texture2D cursorResize;


    [Header("Fake Cursor")]
    [SerializeField] private RectTransform fakeCursorRect;  // UI RawImage GameObject
    [SerializeField] private RawImage fakeCursorImage;       // RawImage component on it
    [SerializeField] private Canvas uiCanvas;


    [Header("Lag Settings")]
    [Tooltip("Windows open at which freezes start happening")]
    [SerializeField] private int windowsForMaxLag = 15;

    [Tooltip("How long the cursor stays unfrozen between freezes (seconds). Shrinks as windows increase.")]
    [SerializeField] private float maxUnfreezeTime = 2f;   // few windows
    [SerializeField] private float minUnfreezeTime = 0.1f; // many windows

    [Tooltip("How long each freeze lasts (seconds). Grows as windows increase.")]
    [SerializeField] private float minFreezeTime = 0.05f;  // few windows
    [SerializeField] private float maxFreezeTime = 1.5f;   // many windows

    [SerializeField] private AnimationCurve lagCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Vector2 _fakeCursorPos;
    private bool _isFrozen;
    private float _freezeTimer;
    private float _unfreezeTimer;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        _fakeCursorPos = Mouse.current.position.ReadValue();
        SetCursorDefault();
    }

    private void Update()
    {
        UpdateLag();
        UpdateFakeCursorPosition();
    }

    private void OnDestroy()
    {
        Cursor.visible = true;
    }

    public void SetCursorDefault() => SetSprite(cursorDefault);
    public void SetCursorLoading() => SetSprite(cursorLoading);
    public void SetCursorBannedInteraction() => SetSprite(cursorBannedInteraction);
    public void SetCursorResize() => SetSprite(cursorResize);


    public void OnPointerEnter(PointerEventData eventData)
    {
        // Uncomment whichever you need:
        // SetCursorBannedInteraction();
        // SetCursorLoading();
        // SetCursorDefault();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetCursorDefault();
    }


    private void UpdateLag()
    {
        if (WindowManager.Instance == null) return;

        int count = WindowManager.Instance.OpenWindows.Count;
        float t = lagCurve.Evaluate(Mathf.Clamp01((float)count / windowsForMaxLag));

        if (_isFrozen)
        {
            _freezeTimer -= Time.deltaTime;
            if (_freezeTimer <= 0f)
            {
                // Unfreeze: snap to real cursor and start unfreeze timer
                _isFrozen = false;
                _fakeCursorPos = Mouse.current.position.ReadValue();
                _unfreezeTimer = Mathf.Lerp(maxUnfreezeTime, minUnfreezeTime, t);
            }
        }
        else
        {
            _unfreezeTimer -= Time.deltaTime;
            if (_unfreezeTimer <= 0f)
            {
                // Freeze: stop the cursor and start freeze timer
                _isFrozen = true;
                _freezeTimer = Mathf.Lerp(minFreezeTime, maxFreezeTime, t);
            }
        }
    }


    private void UpdateFakeCursorPosition()
    {
        if (fakeCursorRect == null || uiCanvas == null) return;

        if (!_isFrozen)
            _fakeCursorPos = Mouse.current.position.ReadValue();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            uiCanvas.transform as RectTransform,
            _fakeCursorPos,
            uiCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : uiCanvas.worldCamera,
            out Vector2 localPoint
        );

        fakeCursorRect.localPosition = localPoint;
        fakeCursorRect.SetAsLastSibling();
    }

    private readonly Vector2 _hotSpot = Vector2.zero;

    private void SetSprite(Texture2D texture)
    {
        if (fakeCursorImage != null)
            fakeCursorImage.texture = texture;
    }
}