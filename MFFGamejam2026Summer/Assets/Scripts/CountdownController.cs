using TMPro;
using System.Collections;
using UnityEngine;

public class CountdownController : MonoBehaviour
{
    [SerializeField] private TMP_Text closableText;
    [SerializeField] private int startCountdown = 3;

    private WindowController _windowController;

    private void Start()
    {
        _windowController = GetComponentInParent<WindowController>();
        if (_windowController != null)
        {
            _windowController.IsClosable = false;
        }
        else
        {
            Debug.LogWarning("CountdownController: no window controller found.");
        }

        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        int start = Mathf.Max(0, startCountdown);

        // Count down from start to 1 (e.g. 3, 2, 1)
        for (int i = start; i >= 1; i--)
        {
            closableText.text = $"You can close this window in {i} seconds";
            yield return new WaitForSeconds(1f);
        }

        // Count back up to start (e.g. 2, 3)
        for (int i = 2; i <= start; i++)
        {
            closableText.text = $"You can close this window in {i} seconds";
            yield return new WaitForSeconds(1f);
        }

        // Count down to 0 (e.g. 2, 1, 0)
        for (int i = Mathf.Max(0, start - 1); i >= 0; i--)
        {
            closableText.text = $"You can close this window in {i} seconds";
            yield return new WaitForSeconds(1f);
        }

        if (_windowController != null)
        {
            _windowController.IsClosable = true;
            closableText.text = "You can close this window now.";
        }
    }
}
