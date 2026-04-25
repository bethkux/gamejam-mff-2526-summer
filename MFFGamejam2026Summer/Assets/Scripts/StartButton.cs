using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    [SerializeField] WindowController windowPrefab;
    [SerializeField] RectTransform canvas;
    [SerializeField] GameObject contentPrefab;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        if (_button != null)
            _button.onClick.AddListener(HandleClick);
    }

    private void OnDisable()
    {
        if (_button != null)
            _button.onClick.RemoveListener(HandleClick);
    }

    private void HandleClick()
    {
        WindowController.Create(windowPrefab, canvas, "My Documents", contentPrefab);
        Debug.Log("Start button clicked");
    }
}