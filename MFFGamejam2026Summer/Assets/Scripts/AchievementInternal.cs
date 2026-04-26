using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AchievementInternal : MonoBehaviour
{
    [Header("Data")]
    public int ID;
    public string Title;

    [Header("References")]
    [SerializeField] private Toggle toggle;

    private void Start()
    {
        if (toggle == null)
            toggle = GetComponentInChildren<Toggle>();
    }

    public void Complete()
    {
        if (toggle != null)
            toggle.isOn = true;

        StartCoroutine(Kill(5f));
    }

    private IEnumerator Kill(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}