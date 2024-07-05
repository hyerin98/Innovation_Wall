using UnityEngine;
using TMPro;
using DG.Tweening;
using OscSimpl;
using IMFINE.Utils.ConfigManager;
using System.Collections.Generic;
using UnityEngine.UI;

public class AnimatorControlExample : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayAnimation();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RewindAnimation();
        }
    }

    void PlayAnimation()
    {
        animator.Play("testtest2");
    }

    void RewindAnimation()
    {
        DOTween.Rewind(animator);
    }
}
