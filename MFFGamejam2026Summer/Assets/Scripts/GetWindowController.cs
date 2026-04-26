using System.Collections;
using UnityEngine;


public class GetWindowController : MonoBehaviour
{
    
    private WindowController _windowController;
    private Coroutine _enableCloseCoroutine;

    private void Awake()
    {
        _windowController = GetComponentInParent<WindowController>();
    }
    
    private void Start()
    {
        if (_windowController != null)
        {
            _windowController.IsClosable = false;
        }
        else
        {
            Debug.LogWarning("CountdownController: no window controller found.");
        }
    }

    public void Close()
    {
        if (_windowController == null)
            return;

        _windowController.IsClosable = true;
        _windowController.Close();
    }

    public void EnableCloseAfter(float time)
    {
        if (_windowController == null)
            return;

        _windowController.IsClosable = false;

        if (_enableCloseCoroutine != null)
            StopCoroutine(_enableCloseCoroutine);

        _enableCloseCoroutine = StartCoroutine(EnableCloseAfterRoutine(time));
    }

    private IEnumerator EnableCloseAfterRoutine(float time)
    {
        yield return new WaitForSeconds(Mathf.Max(0f, time));

        if (_windowController != null)
            _windowController.IsClosable = true;

        _enableCloseCoroutine = null;
    }
}
