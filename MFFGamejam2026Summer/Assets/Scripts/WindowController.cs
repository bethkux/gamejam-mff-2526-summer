using UnityEngine;
using UnityEngine.UI;
using TMPro;


// Create State working, idle

public enum WindowState
{
    Idle,
    Busy,
}

public class WindowController : MonoBehaviour
{
    [SerializeField] TMP_Text titleText;
    [SerializeField] RectTransform clientArea;
    [SerializeField] Button closeButton;
    [SerializeField] bool isEvading = false;
    WindowState _state = WindowState.Idle;
    WindowCloseEvader _closeEvader;
    
    public void SetEvading(bool evade)  
    {
        if (_closeEvader != null)
            _closeEvader.enabled = evade;
    }
    
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
        _closeEvader = GetComponent<WindowCloseEvader>();
        _closeEvader.enabled = isEvading;
    }
    
    public void SetTitle(string title) => titleText.text = title;

    public void Close() => Destroy(gameObject);
    
    public WindowState GetState()  => _state;
    public void SetState(WindowState state) => _state = state;
    
    public RectTransform GetClientArea() => clientArea;
    public RectTransform GetCloseButtonRect() => closeButton != null ? closeButton.transform as RectTransform : null;

    void SpawnContent(GameObject contentPrefab)
    {
        if (contentPrefab == null) return;

        Instantiate(contentPrefab, clientArea);

        // Force the layout to recalculate immediately
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }
}