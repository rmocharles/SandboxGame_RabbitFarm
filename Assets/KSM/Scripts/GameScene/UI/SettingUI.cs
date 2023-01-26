using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    [SerializeField] private Button backgroundButton;
    [SerializeField] private Button backgroundButton2;
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject backgroundGroup;
    [SerializeField] private GameObject effectGroup;
    [SerializeField] private Button langaugeButton;
    [SerializeField] private GameObject langaugeGroup;
    [SerializeField] private Button youtubeButton;
    [SerializeField] private TMP_Text userIDText;
    [SerializeField] private Button userIDButton;
    [SerializeField] private Button accountButton;

    private Color unCheckedColor = new Color32(142, 148, 152, 255);
    private Color checkedColor = new Color32(41, 119, 178, 255);

    void Start()
    {
        CloseLangaugeGroup();
        
        backgroundButton.onClick.AddListener(() =>CloseLangaugeGroup());
        backgroundButton2.onClick.AddListener(() =>CloseLangaugeGroup());
        
        closeButton.onClick.AddListener(() =>
        {
            StaticManager.Sound.SetSFX();
            backgroundButton2.GetComponent<RectTransform>().DOScale(Vector3.zero, 0.1f);
            Invoke("DestroyUI", 0.1f);
        });

        backgroundGroup.GetComponentsInChildren<Button>()[0].onClick.AddListener(() => StaticManager.Sound.SwitchBackgroundSound(1));
        backgroundGroup.GetComponentsInChildren<Button>()[1].onClick.AddListener(() => StaticManager.Sound.SwitchBackgroundSound(0));
        effectGroup.GetComponentsInChildren<Button>()[0].onClick.AddListener(() => StaticManager.Sound.SwitchEffectSound(1));
        effectGroup.GetComponentsInChildren<Button>()[1].onClick.AddListener(() => StaticManager.Sound.SwitchEffectSound(0));

        langaugeGroup.GetComponentsInChildren<Button>()[0].onClick.AddListener(() =>
        {
            StaticManager.Sound.SetSFX();
            StaticManager.Langauge.SwitchLangauge(0);
        });

        langaugeGroup.GetComponentsInChildren<Button>()[1].onClick.AddListener(() =>
        {
            StaticManager.Sound.SetSFX();
            StaticManager.Langauge.SwitchLangauge(1);
        });
        
        langaugeButton.onClick.AddListener(() =>
        {
            StaticManager.Sound.SetSFX();
            langaugeGroup.SetActive(!langaugeGroup.activeSelf);
        });
        
        youtubeButton.onClick.AddListener(() =>
        {
            CloseLangaugeGroup();
            Application.OpenURL("https://youtube.com/c/yoonmi0712");
        });

        userIDButton.GetComponentInChildren<TMP_Text>().text = Backend.UserInDate;
        userIDButton.onClick.AddListener(() =>
        {
            CloseLangaugeGroup();
            StaticManager.Sound.SetSFX();
            UniClipboard.SetText(Backend.UserInDate);
            StaticManager.UI.AlertUI.OpenUI(StaticManager.Langauge.Localize(28));
        });

    }

    void Update()
    {
        //배경음
        SwitchBackground(PlayerPrefs.GetInt("bgm") == 1);
        
        //효과음
        SwitchEffect(PlayerPrefs.GetInt("sfx") == 1);
        
        //언어
        langaugeGroup.GetComponentsInChildren<TMP_Text>()[0].color = PlayerPrefs.GetInt("LangIndex") == 0 ? checkedColor : unCheckedColor;
        langaugeGroup.GetComponentsInChildren<TMP_Text>()[1].color = PlayerPrefs.GetInt("LangIndex") == 1 ? checkedColor : unCheckedColor;
        

        userIDText.text = StaticManager.Langauge.Localize(29);
        
        accountButton.GetComponentInChildren<TMP_Text>().text = StaticManager.Langauge.Localize(49);
    }

    private void SwitchBackground(bool isOn)
    {
        if (isOn)
        {
            backgroundGroup.transform.GetChild(0).gameObject.SetActive(false);
            backgroundGroup.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            backgroundGroup.transform.GetChild(0).gameObject.SetActive(true);
            backgroundGroup.transform.GetChild(1).gameObject.SetActive(false);
        }
        
    }
    private void SwitchEffect(bool isOn)
    {
        if (isOn)
        {
            effectGroup.transform.GetChild(0).gameObject.SetActive(false);
            effectGroup.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            effectGroup.transform.GetChild(0).gameObject.SetActive(true);
            effectGroup.transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    private void CloseLangaugeGroup()
    {
        if(langaugeGroup.activeSelf)
            langaugeGroup.SetActive(false);
    }

    private void DestroyUI()
    {
        Destroy(this.gameObject);
    }
}
