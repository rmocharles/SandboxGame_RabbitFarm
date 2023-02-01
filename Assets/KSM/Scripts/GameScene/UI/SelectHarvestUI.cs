using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectHarvestUI : MonoBehaviour
{
    private GameObject harvestGroup;
    private GameObject harvestInfoUIObject;

    [SerializeField] private Button helpButton;
    
    public void Initialize()
    {
        helpButton.onClick.AddListener(() =>
        {
            StaticManager.Sound.SetSFX();
            GameObject tutorialUI = StaticManager.UI.OpenUI("Prefabs/GameScene/TutorialUI", GameManager.Instance.UICanvas.transform);
            tutorialUI.GetComponent<TutorialUI>().Initialize("Farm");
        });
        
        harvestGroup = transform.GetChild(0).GetChild(0).GetChild(0).gameObject;

        for (int i = 0; i < harvestGroup.transform.childCount; i++)
        {
            int num = i;
            GameObject lockObject = harvestGroup.transform.GetChild(i).GetChild(3).gameObject;
            GameObject goldObject = harvestGroup.transform.GetChild(i).GetChild(1).gameObject;
            GameObject costObject = harvestGroup.transform.GetChild(i).GetChild(2).gameObject;
            GameObject countObject = harvestGroup.transform.GetChild(i).GetChild(4).gameObject;
            GameObject warningObject = harvestGroup.transform.GetChild(i).GetChild(5).gameObject;

            Button harvestButton = harvestGroup.transform.GetChild(i).GetChild(0).GetComponent<Button>();
            
            harvestButton.GetComponent<HarvestDragUI>().Initialize(num);
            harvestButton.onClick.AddListener(() => GameManager.Field.HarvestInfoUI(num, harvestButton.transform.parent));
            
            //보유 여부 on/off
            // harvestButton.GetComponent<Button>().interactable =
            //     StaticManager.Backend.backendGameData.UserData.Level >= StaticManager.Backend.backendChart.Harvest.harvestSheet[i].RequireLevel &&
            //     StaticManager.Backend.backendGameData.UserData.Gold >= StaticManager.Backend.backendChart.Harvest.harvestSheet[i].Price;
            lockObject.SetActive(StaticManager.Backend.backendGameData.UserData.Level < StaticManager.Backend.backendChart.Harvest.harvestSheet[i].RequireLevel);
            goldObject.SetActive(StaticManager.Backend.backendGameData.UserData.Level >= StaticManager.Backend.backendChart.Harvest.harvestSheet[i].RequireLevel);
            costObject.SetActive(StaticManager.Backend.backendGameData.UserData.Level >= StaticManager.Backend.backendChart.Harvest.harvestSheet[i].RequireLevel);
            countObject.SetActive(StaticManager.Backend.backendGameData.UserData.Level >= StaticManager.Backend.backendChart.Harvest.harvestSheet[i].RequireLevel);
            warningObject.SetActive(StaticManager.Backend.backendGameData.UserData.Level < StaticManager.Backend.backendChart.Harvest.harvestSheet[i].RequireLevel);
            
            //레벨 체크
            warningObject.GetComponent<TMP_Text>().text = "Level " + StaticManager.Backend.backendChart.Harvest.harvestSheet[i].RequireLevel;
            
            //비용 체크
            costObject.GetComponentInChildren<TMP_Text>().text = StaticManager.Backend.backendChart.Harvest.harvestSheet[i].Price.ToString();
            
            //보유량 체크
            string harvestCount = string.Empty;
            if (StaticManager.Backend.backendGameData.InventoryData.Dictionary[StaticManager.Backend.backendGameData.InventoryData.harvestItem[i, 0]] > 99)
            {
                harvestCount = "99+";
            }
            else
                harvestCount = StaticManager.Backend.backendGameData.InventoryData.Dictionary[StaticManager.Backend.backendGameData.InventoryData.harvestItem[i, 0]].ToString();

            countObject.GetComponentInChildren<TMP_Text>().text = harvestCount;
        }
    }
}
