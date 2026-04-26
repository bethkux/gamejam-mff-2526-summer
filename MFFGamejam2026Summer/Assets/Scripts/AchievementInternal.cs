using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AchievementInternal : MonoBehaviour
{
    public int ID;
    public string Title;

    [SerializeField] private Toggle toggle;
    [SerializeField] private float slideDuration = 0.4f;
    [SerializeField] private float delayBeforeDie = 5f;
    [SerializeField] private float slideDownBy = 110f;

    public float SlideDownBy => slideDownBy;
    public float SlideDuration => slideDuration;

    private RectTransform rect;
    private Vector2 targetPos;
    private Coroutine slideCoroutine; 

    private void Start()
    {
        if (toggle == null)
            toggle = GetComponentInChildren<Toggle>();
        rect = GetComponent<RectTransform>();
        targetPos = rect.anchoredPosition;
    }

    public void Complete()
    {
        if (toggle != null)
            toggle.isOn = true;
        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        yield return new WaitForSeconds(delayBeforeDie);
        AchievementHandler.Instance.OnAchievementDying(this);
        yield return StartCoroutine(SlideOutAndFade());
        Destroy(gameObject);
    }

    public void SlideDown(float distance, float duration)
    {
        targetPos -= new Vector2(0f, distance);

        if (slideCoroutine != null)
            StopCoroutine(slideCoroutine);

        slideCoroutine = StartCoroutine(SlideDownRoutine(targetPos, duration));
    }

    private IEnumerator SlideDownRoutine(Vector2 endPos, float duration)
    {
        Vector2 startPos = rect.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsed / duration);
            yield return null;
        }

        rect.anchoredPosition = endPos;
        slideCoroutine = null;
    }

    private IEnumerator SlideOutAndFade()
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg == null)
            cg = gameObject.AddComponent<CanvasGroup>();

        Vector2 startPos = rect.anchoredPosition;
        Vector2 endPos = startPos + new Vector2(300f, 0f);
        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideDuration;
            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            cg.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }
    }
}