using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class AnimalUpgradeUI : MonoBehaviour
{
    [SerializeField] private GameObject backgroundObject;
    [SerializeField] private Button closeButton;
    [SerializeField] private Image itemImage;
    [SerializeField] private Button completeButton;
    [SerializeField] private TMP_Text remainTimerText;
    [SerializeField] private TMP_Text completeInfoText;
    [SerializeField] private TMP_Text nowLevelText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TMP_Text specialInfoText;

    public GameObject[] animalObject;
    

    public enum Animal
    {
        Cow,
        Chicken
    }

    public string nowAnimal;
    
    public void Initialize(Animal animal)
    {
        closeButton.onClick.AddListener(() =>
        {
            StaticManager.Sound.SetSFX();
            backgroundObject.GetComponent<RectTransform>().DOScale(Vector3.zero, 0.1f);
            Invoke("DestroyUI", 0.1f);
        });
        
        switch (animal)
        {
            case Animal.Chicken:
                nowAnimal = "Chicken";
                itemImage.sprite = Resources.Load<Sprite>("Sprites/Egg");
                animalObject[0].SetActive(true);
                specialInfoText.text = StaticManager.Langauge.Localize(192);
                break;
            
            case Animal.Cow:
                nowAnimal = "Cow";
                itemImage.sprite = Resources.Load<Sprite>("Sprites/Milk");
                animalObject[1].SetActive(true);
                specialInfoText.text = StaticManager.Langauge.Localize(191);
                break;
        }
        
        completeButton.onClick.AddListener(() =>
        {
            if (StaticManager.Backend.backendGameData.UserData.Diamond >= int.Parse(completeButton.GetComponentInChildren<TMP_Text>().text))
            {
                //완성
                StaticManager.Sound.SetSFX();
                StaticManager.Backend.backendGameData.UserData.AddDiamond(-int.Parse(completeButton.GetComponentInChildren<TMP_Text>().text));
                GameManager.Instance.DiamondUI("-" + completeButton.GetComponentInChildren<TMP_Text>().text);
            
                GameManager.Animal.SetAnimal(animal, DateTime.UtcNow.ToString());
            
                GameManager.Instance.SaveAllData();
            }
            else
            {
                StaticManager.Sound.SetSFX();
                GameManager.Instance.MakeToast(StaticManager.Langauge.Localize(47));
            }
        });

        completeInfoText.text = StaticManager.Langauge.Localize(32);

        upgradeButton.onClick.AddListener(() =>
        {
            StaticManager.Sound.SetSFX();
            StaticManager.Backend.backendGameData.UserData.AddGold(-int.Parse(upgradeButton.GetComponentInChildren<TMP_Text>().text));
            GameManager.Instance.GoldUI(upgradeButton.GetComponentInChildren<TMP_Text>().text);
            
            GameManager.Animal.InitializeAnimal(animal);
            
            StaticManager.Backend.backendGameData.AnimalData.SetAnimal(nowAnimal, StaticManager.Backend.backendGameData.AnimalData.Dictionary[nowAnimal].Upgrade + 1);
            
            GameManager.Instance.MakeToast(StaticManager.Langauge.Localize(41));
            
            GameManager.Instance.SaveAllData();
        });

        
    }

    void Update()
    {
        TimeSpan remainTime = DateTime.Parse(StaticManager.Backend.backendGameData.AnimalData.Dictionary[nowAnimal].RemainTimer) - GameManager.Instance.nowTime;
        int remainTimer = Mathf.FloorToInt((float)Math.Truncate(remainTime.TotalSeconds));
        //시, 분, 초로 변경
        if (remainTime.TotalSeconds > 0)
        {
            completeButton.interactable = true;
            
            if (remainTime.TotalSeconds >= 3600)
            {
                if (PlayerPrefs.GetInt("LangIndex") == 0)
                    remainTimerText.text = remainTimer / 3600 + "시간 " + remainTimer % 3600 + "분";
                else
                    remainTimerText.text = remainTimer / 3600 + "H " + remainTimer % 3600 + "m";
            }
            else if (remainTime.TotalSeconds >= 60 && remainTime.TotalSeconds < 3600)
            {
                if (PlayerPrefs.GetInt("LangIndex") == 0)
                    remainTimerText.text = remainTimer / 60 + "분 " + remainTimer % 60 + "초";
                else
                    remainTimerText.text = remainTimer / 60 + "M " + remainTimer % 60 + "s";
            }
            else
            {
                if (PlayerPrefs.GetInt("LangIndex") == 0)
                    remainTimerText.text = remainTimer + "초";
                else
                    remainTimerText.text = remainTimer + "s";
            }
        }
        //팝업이 열려 있는 도중 완성되었을 경우
        else
        {
            // backgroundObject.GetComponent<RectTransform>().DOScale(Vector3.zero, 0.1f);
            // Invoke("DestroyUI", 0.1f);
            remainTimerText.text = StaticManager.Langauge.Localize(40);
            completeButton.interactable = false;
            completeButton.GetComponentInChildren<TMP_Text>().text = string.Empty;
        }
        
        nowLevelText.text = "Lv." + StaticManager.Backend.backendGameData.AnimalData.Dictionary[nowAnimal].Upgrade;

        if (upgradeButton.interactable)
        {
            completeButton.GetComponentInChildren<TMP_Text>().text = (remainTimer / 60 + 1).ToString();
        }
        else
        {
            completeButton.GetComponentInChildren<TMP_Text>().text = string.Empty;
        }

        int upgradeLevel = StaticManager.Backend.backendGameData.AnimalData.Dictionary[nowAnimal].Upgrade;
        switch (nowAnimal)
        {
            case "Chicken":
                upgradeButton.GetComponentInChildren<TMP_Text>().text = upgradeLevel >= 20 ? StaticManager.Langauge.Localize(41) : StaticManager.Backend.backendChart.Animal.animalSheet[upgradeLevel].Chicken_Upgrade.ToString();
                break;
            
            case "Cow":
                upgradeButton.GetComponentInChildren<TMP_Text>().text = upgradeLevel >= 20 ? StaticManager.Langauge.Localize(41) : StaticManager.Backend.backendChart.Animal.animalSheet[upgradeLevel].Cow_Upgrade.ToString();
                break;
        }

        //만렙일 경우 업그레이드 방지
        if (upgradeLevel >= 20 || StaticManager.Backend.backendGameData.UserData.Gold < int.Parse(upgradeButton.GetComponentInChildren<TMP_Text>().text))
            upgradeButton.interactable = false;
        else
            upgradeButton.interactable = true;

    }
    
    private void DestroyUI()
    {
        Destroy(this.gameObject);
    }
}
