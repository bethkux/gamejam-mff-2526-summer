using System.Collections;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GenericSetStuffScript : MonoBehaviour
{

    public bool killMePlease = false;
    public GameObject popups;
    private bool shouldDie = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (killMePlease) ShutdownMethod();
        if (popups.transform.childCount == 0) ShutdownMethod();

    }

    public void ShutdownMethod()
    {
        if (shouldDie) { 
            StartCoroutine(kill());
            shouldDie = false;
        }
    }
    IEnumerator kill()
    {
        AudioSource source = AudioManager.Instance.PlaySFX("Shutdown");

        float delay = 1.5f;

        if (source != null && source.clip != null)
        {
            // account for pitch affecting duration
            delay = source.clip.length / Mathf.Abs(source.pitch);
        }
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene("MainMenu");

    }
}
