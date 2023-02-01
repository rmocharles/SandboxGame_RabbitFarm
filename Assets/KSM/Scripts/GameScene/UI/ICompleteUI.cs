using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ICompleteUI : MonoBehaviour
{
    [SerializeField] private GameObject backgroundObject;
    [SerializeField] private Button closeButton;
    [SerializeField] private Sprite[] harvestSprites;
    [SerializeField] private Image harvestImage;
    [SerializeField] private Button completeButton;
    [SerializeField] private TMP_Text remainTimerText;
    [SerializeField] private TMP_Text completeInfoText;
    [SerializeField] private TMP_Text fertilizerCountText;
    [SerializeField] private Button fertilizerAdButton;
    [SerializeField] private TMP_Text specialInfoText;
    [SerializeField] private Button helpButton;

    private int fieldNumber;

    public void Initialize(int fieldNumber)
    {
        this.fieldNumber = fieldNumber;
        closeButton.onClick.AddListener(() =>
        {
            StaticManager.Sound.SetSFX();
            backgroundObject.GetComponent<RectTransform>().DOScale(Vector3.zero, 0.1f);
            Invoke("DestroyUI", 0.1f);
        });

        harvestImage.sprite = harvestSprites[StaticManager.Backend.backendGameData.FieldData.Dictionary[fieldNumber].HarvestCode];
        
        completeButton.onClick.AddListener(() =>
        {
            if (StaticManager.Backend.backendGameData.UserData.Diamond >= int.Parse(completeButton.GetComponentInChildren<TMP_Text>().text))
            {
                //완성
                StaticManager.Sound.SetSFX();
                StaticManager.Backend.backendGameData.UserData.AddDiamond(-int.Parse(completeButton.GetComponentInChildren<TMP_Text>().text));
                GameManager.Instance.DiamondUI("-" + completeButton.GetComponentInChildren<TMP_Text>().text);
                GameManager.Field.SetField(fieldNumber, DateTime.UtcNow.ToString());
            
                GameManager.Instance.SaveAllData();
            }
            else
            {
                StaticManager.Sound.SetSFX();
                GameManager.Instance.MakeToast(StaticManager.Langauge.Localize(47));
            }
            
        });

        completeInfoText.text = StaticManager.Langauge.Localize(32);

        fertilizerAdButton.onClick.AddListener(() =>
        {
            StaticManager.Sound.SetSFX();

            if (StaticManager.Backend.backendGameData.InventoryData.Dictionary["Fertilizer"] > 0)
            {
                //퀘스트
                StaticManager.Backend.backendGameData.QuestData.AddCount(2, 1);
                
                StaticManager.Backend.backendGameData.InventoryData.AddItem("Fertilizer", -1);
                GameManager.Field.SetField(fieldNumber, 1);
                
                backgroundObject.GetComponent<RectTransform>().DOScale(Vector3.zero, 0.1f);
                Invoke("DestroyUI", 0.1f);
            }
            else
            {
                //광고
                StaticManager.AD.ShowRewardAD(() =>
                {
                    StaticManager.Sound.SetSFX("Get");
                    StaticManager.Backend.backendGameData.InventoryData.AddItem("Fertilizer", 3);
                });
            }
            
        });

        specialInfoText.text = StaticManager.Langauge.Localize(33);
        
        helpButton.onClick.AddListener(() =>
        {
            StaticManager.Sound.SetSFX();
            GameManager.Instance.MakeToast2("?");
        });

        fertilizerAdButton.GetComponentInChildren<TMP_Text>().text = StaticManager.Langauge.Localize(172);
    }

    void Update()
    {
        TimeSpan remainTime = DateTime.Parse(StaticManager.Backend.backendGameData.FieldData.Dictionary[fieldNumber].RemainTimer) - GameManager.Instance.nowTime;
        int remainTimer = Mathf.FloorToInt((float)Math.Truncate(remainTime.TotalSeconds));
        //시, 분, 초로 변경
        if (remainTime.TotalSeconds > 0)
        {
            if (remainTime.TotalSeconds >= 3600)
            {
                if (PlayerPrefs.GetInt("LangIndex") == 0)
                    remainTimerText.text = remainTimer / 3600 + "시간" + remainTimer % 3600 + "분";
                else
                    remainTimerText.text = remainTimer / 3600 + "H" + remainTimer % 3600 + "m";
            }
            else if (remainTime.TotalSeconds >= 60 && remainTime.TotalSeconds < 3600)
            {
                if (PlayerPrefs.GetInt("LangIndex") == 0)
                    remainTimerText.text = remainTimer / 60 + "분" + remainTimer % 60 + "초";
                else
                    remainTimerText.text = remainTimer / 60 + "M" + remainTimer % 60 + "s";
            }
            else
            {
                if (PlayerPrefs.GetInt("LangIndex") == 0)
                    remainTimerText.text = remainTimer + "초";
                else
                    remainTimerText.text = remainTimer + "s";
            }

            if (StaticManager.Backend.backendGameData.InventoryData.Dictionary["Fertilizer"] > 99)
                fertilizerCountText.text = "99+";
            else
                fertilizerCountText.text = StaticManager.Backend.backendGameData.InventoryData.Dictionary["Fertilizer"].ToString();
        }
        //팝업이 열려 있는 도중 완성되었을 경우
        else
        {
            backgroundObject.GetComponent<RectTransform>().DOScale(Vector3.zero, 0.1f);
            Invoke("DestroyUI", 0.1f);
        }

        completeButton.GetComponentInChildren<TMP_Text>().text = (remainTimer / 60 + 1).ToString();
        
        //이미지
        fertilizerAdButton.transform.GetChild(0).gameObject.SetActive(StaticManager.Backend.backendGameData.InventoryData.Dictionary["Fertilizer"] < 1);
        //텍스트
        fertilizerAdButton.transform.GetChild(1).gameObject.SetActive(StaticManager.Backend.backendGameData.InventoryData.Dictionary["Fertilizer"] > 0);
        
        //비료가 사용된 밭 터치 방지
        fertilizerAdButton.interactable = StaticManager.Backend.backendGameData.FieldData.Dictionary[fieldNumber].FieldLevel == 0;
    }
    
    private void DestroyUI()
    {
        Destroy(this.gameObject);
    }
}
