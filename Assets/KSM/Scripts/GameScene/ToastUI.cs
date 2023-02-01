using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ToastUI : MonoBehaviour
{
    [SerializeField] private TMP_Text infoText;

    public void Initialize(string infoText, float delay = 1.5f)
    {
        this.infoText.text = infoText;

        GetComponent<DOTweenAnimation>().duration = .3f;
        Invoke("DestroyUI", 1.5f);
    }

    private void DestroyUI()
    {
        Destroy(this.gameObject);
    }
}
