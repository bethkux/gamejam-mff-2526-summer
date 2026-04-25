using TMPro;
using System.Collections;
using UnityEngine;

public class CountdownController : MonoBehaviour
{
    [SerializeField] private TMP_Text closableText;

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
        int count = 10;
        while (count > 0)
        {
            closableText.text = $"You can close this window in {count} seconds";

            if (count == 2)
            {
                yield return StartCoroutine(CountUpRoutine());
            }

            count--;
            yield return new WaitForSeconds(1f);
        }

        if (_windowController != null)
        {
            _windowController.IsClosable = true;
            closableText.text = "hihi.";
        }
    }

    private IEnumerator CountUpRoutine()
    {
        int count = 2;
        var image = GetComponentInChildren<UnityEngine.UI.Image>(true);
        if (image != null)
        {
            image.enabled = true;
        }

        while (count <= 5)
        {
            closableText.text = $"You can close this window in {count} seconds";
            count++;
            yield return new WaitForSeconds(1f);
        }

        // Resume countdown from 5
        for (int i = 5; i > 0; i--)
        {
            closableText.text = $"You can close this window in {i} seconds";
            yield return new WaitForSeconds(1f);
        }
    }
}
