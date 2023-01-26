using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeUI : MonoBehaviour
{
    private Image fadeImage;

    public delegate void AfterFade();

    private bool isFade = false;

    void Awake()
    {
        fadeImage = GetComponent<Image>();
    }

    void Update()
    {
        if (isFade)
        {
            StaticManager.Sound.SetVolume(0.5f - fadeImage.color.a);
        }
        else
        {
            StaticManager.Sound.SetVolume(.5f);
        }
    }

    public enum FadeType
    {
        ChangeToTransparent,
        ChangeToBlack,
    }

    public void FadeStart(FadeType fadeType, AfterFade afterFade, float duration = 1)
    {
        isFade = true;
        gameObject.SetActive(true);

        switch (fadeType)
        {
            case FadeType.ChangeToBlack:
                fadeImage.color = new Color(0, 0, 0, 0);
                break;
            case FadeType.ChangeToTransparent:
                fadeImage.color = new Color(0, 0, 0, 1);
                break;
        }
        
        DOTween.Sequence()
            .SetAutoKill(false)
            .Append(fadeImage.DOFade(fadeType == FadeType.ChangeToBlack ? 1 : 0, duration))
            .OnComplete(() =>
            {
                isFade = false;
                afterFade?.Invoke();
                //this.gameObject.SetActive(false);
            })
            .Restart();
    }
}
