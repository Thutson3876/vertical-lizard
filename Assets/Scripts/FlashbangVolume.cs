using System;
using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class FlashbangVolume : MonoBehaviour
{
    private PostProcessVolume _postProcessVolume;
    [SerializeField] private Animator _flashbangAnimator;
    
    private void Start()
    {
        _postProcessVolume = GetComponent<PostProcessVolume>();
    }

    public void StartFlashbang()
    {
        Action<float> tweenVolume = TweenVolume;
        Tween.Custom(0.0F, 1.0F, 1.0F, tweenVolume, Ease.InSine);
    }

    private void TweenVolume(float value)
    {
        _postProcessVolume.weight = value;

        if (Mathf.Approximately(value, 1.0F))
        {
            _flashbangAnimator.SetBool("Play", true);
        }
    }
}
