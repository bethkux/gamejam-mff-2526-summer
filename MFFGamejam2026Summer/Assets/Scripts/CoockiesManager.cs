using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CoockiesManager : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform cookieRect;
    [SerializeField] private CoockiePageManager pageManager;
    [SerializeField] private TextMeshProUGUI counterText;

    private float startScale;
    private int collectedCookies = 0;
    public int totalCookies;

    void Start()
    {
        counterText.text = $"Collected cookies:\n{collectedCookies} / {totalCookies}";
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        startScale = cookieRect.localScale.x;
        cookieRect.localScale = cookieRect.localScale * 0.7f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        cookieRect.localScale = Vector3.one * startScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        cookieRect.localScale = Vector3.one * startScale * 0.98f;
        collectedCookies++;
        counterText.text = $"Collected cookies:\n{collectedCookies} / {totalCookies}";

        if (collectedCookies == totalCookies)
        {
            Debug.Log("All cookies collected!");
            pageManager.ShowFinalPage();
        }
    }
}
