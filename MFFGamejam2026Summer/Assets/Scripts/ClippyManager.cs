using UnityEngine;
using System.Collections.Generic;

public class ClippyManager : MonoBehaviour
{
    [SerializeField] private TypewriterEffect typewriterEffect;
    private Animator animator;

    private AnimatorStateInfo state;

    
    void TrackProgress()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        think();
        speak("Hello, I'm Clippy! I can help you with your tasks. Just click on me to see what I can do!");
    }

    // Update is called once per frame
    void Update()
    {
        state = animator.GetCurrentAnimatorStateInfo(0);

        if (state.IsName("Clippy_leave") && state.normalizedTime >= 1f)
        {
            gameObject.SetActive(false);
        }
    }

    public void idle()
    {
        gameObject.SetActive(true);
        animator.Play("Clippy_idle");
    }

    public void leave()
    {
        gameObject.SetActive(true);
        animator.Play("Clippy_leave");
    }

    public void think()
    {
        gameObject.SetActive(true);
        animator.Play("Clippy_think");
    }

    public void penguin()
    {
        gameObject.SetActive(true);
        animator.Play("Penguin");
    }

    public void speak(string text)
    {
        typewriterEffect.Play(text);
    }
}
