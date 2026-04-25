using UnityEngine;
using System.Collections.Generic;

public class ClippyManager : MonoBehaviour
{
    [SerializeField] private AnimationClip animationClip;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // if (animator.GetCurrentAnimatorStateInfo(0).IsName("Clippy_idle"))
        // {
        //     foreach (string animName in animations)
        //     {
        //         if (animName == "Clippy_weird")
        //         {
        //             animator.Play(animName);
        //             break;
        //         }
        //     }
        // }
    }
}
