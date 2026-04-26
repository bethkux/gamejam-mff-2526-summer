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

    private RectTransform rect;

    private void Start()
    {
        if (toggle == null)
            toggle = GetComponentInChildren<Toggle>();

        rect = GetComponent<RectTransform>();
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
        yield return StartCoroutine(SlideOutAndFade());
        NotifySiblingsAbove();

        Destroy(gameObject);
    }

    private void NotifySiblingsAbove()
    {
        int myIndex = transform.GetSiblingIndex();

        for (int i = 0; i < myIndex; i++)
        {
            Transform sibling = transform.parent.GetChild(i);
            AchievementInternal a = sibling.GetComponent<AchievementInternal>();
            if (a != null)
                a.SlideDown(slideDownBy, slideDuration);
        }
    }

    public void SlideDown(float distance, float duration)
    {
        StartCoroutine(SlideDownRoutine(distance, duration));
    }

    private IEnumerator SlideDownRoutine(float distance, float duration)
    {
        Vector2 startPos = rect.anchoredPosition;
        Vector2 endPos = startPos - new Vector2(0f, distance);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsed / duration);
            yield return null;
        }

        rect.anchoredPosition = endPos;
    }

    private IEnumerator SlideOutAndFade()
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg == null)
            cg = gameObject.AddComponent<CanvasGroup>();

        Vector2 startPos = rect.anchoredPosition;
        Vector2 endPos = startPos - new Vector2(300f, 0f);

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