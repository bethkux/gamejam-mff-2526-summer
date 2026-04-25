using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void QuitGame()
    {
        StartCoroutine(QuitWithSound());
    }

    private IEnumerator QuitWithSound()
    {
        AudioSource source = AudioManager.Instance.PlaySFX("Shutdown");

        float delay = 1.5f;

        if (source != null && source.clip != null)
        {
            delay = source.clip.length / Mathf.Abs(source.pitch);
        }

        yield return new WaitForSeconds(delay);

        Debug.Log("QuitGame called");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }


    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneWithSound(sceneName));
    }

    private IEnumerator LoadSceneWithSound(string sceneName)
    {
        AudioSource source = AudioManager.Instance.PlaySFX("Startup");

        float delay = 1.5f;

        if (source != null && source.clip != null)
        {
            // account for pitch affecting duration
            delay = source.clip.length / Mathf.Abs(source.pitch);
        }

        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene(sceneName);
    }


    public void PlayGame()
    {
        LoadScene("GameScene");
    }
}