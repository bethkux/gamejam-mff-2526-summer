using UnityEngine;
using System.Collections.Generic;

public class ClippyManager : MonoBehaviour
{
    public static ClippyManager Instance { get; private set; }

    [SerializeField] private TypewriterEffect typewriterEffect;
    [SerializeField] private float almostDoneThreshold = 0.75f;

    private Animator animator;
    private AnimatorStateInfo state;
    private bool saidAlmostDone = false;

    private readonly string[] onAchievementsAppear = new[]
    {
        "Hey! You have some achievements to complete. Get to it!",
        "Looks like you have tasks waiting. I believe in you!",
        "Oh! Achievements unlocked. You know what to do!",
    };

    private readonly string[] onAchievementCompleted = new[]
    {
        "Great job! Keep it up!",
        "Nice one! One down!",
        "You are on a roll!",
        "Look at you go!",
    };

    private readonly string[] onAlmostDone = new[]
    {
        "You are almost there, don't give up!",
        "So close! Just a little more!",
        "Nearly done! Finish strong!",
    };

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        think();
        speak("Hello, I am Clippy! I am here to help you... or distract you. We will see.", 30);
    }

    private void Update()
    {
        state = animator.GetCurrentAnimatorStateInfo(0);
        if (state.IsName("Clippy_leave") && state.normalizedTime >= 1f)
            gameObject.SetActive(false);
    }



    public void OnAchievementsAppeared()
    {
        idle();
        speak(Pick(onAchievementsAppear));
    }

    public void OnAchievementCompleted(int completedCount, int totalCount)
    {
        if (!saidAlmostDone && totalCount > 0 && (float)completedCount / totalCount >= almostDoneThreshold)
        {
            saidAlmostDone = true;
            idle();
            speak(Pick(onAlmostDone));
            return;
        }

        idle();
        speak(Pick(onAchievementCompleted));
    }


    public void idle() { gameObject.SetActive(true); animator.Play("Clippy_idle"); }
    public void leave() { gameObject.SetActive(true); animator.Play("Clippy_leave"); }
    public void think() { gameObject.SetActive(true); animator.Play("Clippy_think"); }
    public void penguin() { gameObject.SetActive(true); animator.Play("Penguin"); }

    public void speak(string text, float speed = 20) => typewriterEffect.Play(text, speed);


    private string Pick(string[] lines) => lines[Random.Range(0, lines.Length)];
}