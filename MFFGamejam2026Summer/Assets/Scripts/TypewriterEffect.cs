using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TypewriterEffect : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private TMP_Text textComponent;

    [Header("Settings")]
    [Tooltip("Characters revealed per second")]
    [SerializeField] private float charsPerSecond = 20f;

    [Tooltip("Extra pause when hitting punctuation like . ! ? , ;")]
    [SerializeField] private bool pauseOnPunctuation = true;

    [SerializeField] private float punctuationPause = 0.15f;

    //[Tooltip("Start playing automatically on Start")]
    //[SerializeField] private bool playOnStart = false;

    [Header("Events")]
    public UnityEvent onComplete;

    private Coroutine _coroutine;
    private string _fullText;


    private void Awake()
    {
        if (textComponent == null)
            textComponent = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        Play("Test test hello is it working?");
    }


    /// <summary>Play typewriter on the text currently set in the component.</summary>
    public void Play(TMP_Text text)
    {
        Play(text);
    }

    /// <summary>Set new text and play typewriter from the beginning.</summary>
    public void Play(string text)
    {
        Stop();
        _fullText = text;
        _coroutine = StartCoroutine(TypeRoutine());
    }

    /// <summary>Stop mid-way and show full text immediately.</summary>
    public void Skip()
    {
        Stop();
        textComponent.text = _fullText;
        textComponent.maxVisibleCharacters = int.MaxValue;
        onComplete?.Invoke();
    }

    /// <summary>Stop and clear.</summary>
    public void Stop()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            textComponent.text = "";
            _coroutine = null;
        }
    }

    /// <summary>Is the typewriter currently running?</summary>
    public bool IsPlaying => _coroutine != null;


    private IEnumerator TypeRoutine()
    {
        textComponent.text = _fullText;
        textComponent.ForceMeshUpdate();
        textComponent.maxVisibleCharacters = 0;

        int total = textComponent.textInfo.characterCount;
        float secondsPerChar = 1f / Mathf.Max(charsPerSecond, 0.01f);
        float elapsed = 0f;
        int visible = 0;

        while (visible < total)
        {
            elapsed += Time.deltaTime;

            while (elapsed >= secondsPerChar && visible < total)
            {
                elapsed -= secondsPerChar;
                visible++;
                textComponent.maxVisibleCharacters = visible;

                // Pause on punctuation after revealing it
                if (pauseOnPunctuation && visible < total)
                {
                    char revealed = textComponent.textInfo.characterInfo[visible - 1].character;
                    if (revealed == '.' || revealed == '!' || revealed == '?' ||
                        revealed == ',' || revealed == ';' || revealed == ':')
                    {
                        yield return new WaitForSeconds(punctuationPause);
                        elapsed = 0f;
                    }
                }
            }

            yield return null;
        }

        _coroutine = null;
        onComplete?.Invoke();
    }
}