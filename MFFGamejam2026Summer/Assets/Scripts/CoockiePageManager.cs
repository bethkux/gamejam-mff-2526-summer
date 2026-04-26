using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class CoockiePageManager : MonoBehaviour
{
    [SerializeField] private GameObject cookieWindow;
    [SerializeField] private GameObject cookiePage;
    [SerializeField] private GameObject acceptPage;
    [SerializeField] private GameObject finalPage;

    public UnityEvent finished;

    void Start()
    {
        ShowAcceptPage();
    }

    void ShowAcceptPage()
    {
        cookiePage.SetActive(false);
        acceptPage.SetActive(true);
        finalPage.SetActive(false);
    }

    public void ShowCookiePage()
    {
        cookiePage.SetActive(true);
        acceptPage.SetActive(false);
        finalPage.SetActive(false);
    }

    public void ShowFinalPage()
    {
        cookiePage.SetActive(false);
        acceptPage.SetActive(false);
        finalPage.SetActive(true);

        finished.Invoke();
        Destroy(cookieWindow, 3f);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
