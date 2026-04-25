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

        Instantiate(contentPrefab, clientArea);

        // Force the layout to recalculate immediately
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }
}