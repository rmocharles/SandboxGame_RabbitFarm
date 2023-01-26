using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MiniBubbleUI : MonoBehaviour
{
    [SerializeField] private TMP_Text infoText;

    public void Initialize(string text)
    {
        infoText.text = text;
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        
        Destroy(this.gameObject);
    }
}
