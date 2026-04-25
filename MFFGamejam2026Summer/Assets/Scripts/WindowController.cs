using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public string WindowName { get; private set; }
    public event System.Action<WindowController> OnWindowClosed;

    public void SetEvading(bool evade)
    {
        if (_closeEvader != null)
            _closeEvader.enabled = evade;
    }

    public static WindowController Create(
        WindowController prefab,
        Transform parent,
        string title,
        GameObject contentPrefab)
    {
        var win = Instantiate(prefab, parent);
        win.WindowName = title;
        win.titleText.text = title;
        win.SpawnContent(contentPrefab);
        return win;
    }

    void Awake()
    {
        closeButton.onClick.AddListener(Close);
        _closeEvader = GetComponent<WindowCloseEvader>();
        if (_closeEvader != null )
            _closeEvader.enabled = isEvading;
    }

    public void SetTitle(string title)
    {
        WindowName = title;
        titleText.text = title;
    }

    public void Close()
    {
        OnWindowClosed?.Invoke(this);
        Destroy(gameObject);
    }

    public WindowState GetState() => _state;
    public void SetState(WindowState state) => _state = state;

    public RectTransform GetClientArea() => clientArea;
    public RectTransform GetCloseButtonRect() => closeButton != null ? closeButton.transform as RectTransform : null;

    void SpawnContent(GameObject contentPrefab)
    {
        if (contentPrefab == null) return;
        Instantiate(contentPrefab, clientArea);
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }
}