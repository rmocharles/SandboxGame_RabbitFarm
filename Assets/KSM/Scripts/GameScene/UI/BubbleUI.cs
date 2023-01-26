using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class BubbleUI : MonoBehaviour
{
    [SerializeField] private TMP_Text bubbleText;
    private Vector2 pos;

    public void Initialize(string infoText)
    {
        bubbleText.text = infoText;

        GetComponent<DOTweenAnimation>().DORestart();
    }
}
