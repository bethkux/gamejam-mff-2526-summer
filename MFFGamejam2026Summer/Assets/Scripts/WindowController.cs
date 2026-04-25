using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WindowController : MonoBehaviour
{
    [SerializeField] TMP_Text titleText;
    [SerializeField] RectTransform clientArea;
    [SerializeField] Button closeButton;

    // ----------------------------------------------------------------
    // Factory
    // ----------------------------------------------------------------
    public static WindowController Create(
        WindowController prefab,
        Transform parent,
        string title,
        GameObject contentPrefab)
    {
        var win = Instantiate(prefab, parent);
        win.titleText.text = title;
        win.SpawnContent(contentPrefab);
        return win;
    }

    // ----------------------------------------------------------------
    // Lifecycle
    // ----------------------------------------------------------------
    void Awake()
    {
        closeButton.onClick.AddListener(Close);
    }
    
    public void SetTitle(string title) => titleText.text = title;

    public void Close() => Destroy(gameObject);

    public void Minimize() => gameObject.SetActive(false);

    public RectTransform GetClientArea() => clientArea;
    
    void SpawnContent(GameObject contentPrefab)
    {
        if (contentPrefab == null) return;

        var content = Instantiate(contentPrefab, clientArea);
        var rt = content.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}