using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CloseUI : MonoBehaviour
{
    [SerializeField] private GameObject backgroundObject;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_Text infoText;
    [SerializeField] private Button confirmButton;

    void Start()
    {
        closeButton.onClick.AddListener(() =>
        {
            backgroundObject.GetComponent<RectTransform>().DOScale(Vector3.zero, 0.1f);
            Invoke(nameof(DestroyUI), 0.1f);
        });

        infoText.text = PlayerPrefs.GetInt("LangIndex") == 0 ? "피곤해요!\n잠시 쉬었다 할게요!" : "I'm tired!\nI'll rest for a while!";
        
        confirmButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
    
    private void DestroyUI()
    {
        Destroy(this.gameObject);
    }
}
