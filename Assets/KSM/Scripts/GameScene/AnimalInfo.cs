using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AnimalInfo : MonoBehaviour
{
    public AnimalUpgradeUI.Animal nowAnimal;

    public GameObject bubbleObject;
    public Button itemButton;
    public Button lockButton;

    private bool isInit = false;

    public void Initialize(AnimalUpgradeUI.Animal animal)
    {
        isInit = true;
        string unlock = string.Empty;
        
        nowAnimal = animal;
        switch (animal)
        {
            case AnimalUpgradeUI.Animal.Chicken:
                unlock = StaticManager.Langauge.Localize(56);
                break;
            
            case AnimalUpgradeUI.Animal.Cow:
                unlock = StaticManager.Langauge.Localize(57);
                break;
        }
        
        lockButton.onClick.AddListener(() =>
        {
            GameManager.Instance.MakeToast(unlock);
        });
        
        
        
        itemButton.onClick.RemoveAllListeners();
        itemButton.onClick.AddListener(() =>
        {
            GetComponent<AudioSource>().clip = StaticManager.Sound.SearchClip(nowAnimal == AnimalUpgradeUI.Animal.Chicken ? "Chicken" : "Cow");
            GetComponent<AudioSource>().Play();
            
            GameManager.Field.CloseUI();

            string animalName = nowAnimal == AnimalUpgradeUI.Animal.Chicken ? "Chicken" : "Cow";
            int upgradeLevel = 0;
            int speicalPercent = 0;
            int remainTimer = 0;

            switch (nowAnimal)
            {
                case AnimalUpgradeUI.Animal.Chicken:
                    upgradeLevel = StaticManager.Backend.backendChart.Animal.animalSheet[StaticManager.Backend.backendGameData.AnimalData.Dictionary[animalName].Upgrade - 1].Level;
                    speicalPercent = StaticManager.Backend.backendChart.Animal.animalSheet[upgradeLevel - 1].Chicken_Special;
                    remainTimer = StaticManager.Backend.backendChart.Animal.animalSheet[upgradeLevel - 1].Chicken_Speed;

                    if (Random.Range(1, 101) <= speicalPercent)
                    {
                        GameObject itemObject = Instantiate(Resources.Load<GameObject>("Prefabs/GameScene/Effect/RewardEffect"), GameManager.Instance.worldCanvas.transform);
                        itemObject.transform.position = bubbleObject.transform.position;
                        itemObject.GetComponent<RewardEffect>().Initialize(21, 1);
                        StaticManager.Backend.backendGameData.InventoryData.AddItem("Golden_Egg", 1);
                        StaticManager.Backend.backendGameData.UserData.AddExp(StaticManager.Backend.backendChart.Harvest.harvestSheet[10].Exp[1]);
                    }
                    else
                    {
                        GameObject itemObject = Instantiate(Resources.Load<GameObject>("Prefabs/GameScene/Effect/RewardEffect"), GameManager.Instance.worldCanvas.transform);
                        itemObject.transform.position = bubbleObject.transform.position;
                        itemObject.GetComponent<RewardEffect>().Initialize(20, 1);
                        StaticManager.Backend.backendGameData.InventoryData.AddItem("Egg", 1);
                        StaticManager.Backend.backendGameData.UserData.AddExp(StaticManager.Backend.backendChart.Harvest.harvestSheet[10].Exp[0]);
                    }
                    
                    //퀘스트
                    StaticManager.Backend.backendGameData.QuestData.AddCount(5, 1);
                    
                    break;
                
                case AnimalUpgradeUI.Animal.Cow:
                    upgradeLevel = StaticManager.Backend.backendChart.Animal.animalSheet[StaticManager.Backend.backendGameData.AnimalData.Dictionary[animalName].Upgrade - 1].Level;
                    speicalPercent = StaticManager.Backend.backendChart.Animal.animalSheet[upgradeLevel - 1].Cow_Special;
                    remainTimer = StaticManager.Backend.backendChart.Animal.animalSheet[upgradeLevel - 1].Cow_Speed;

                    if (Random.Range(1, 101) <= speicalPercent)
                    {
                        GameObject itemObject = Instantiate(Resources.Load<GameObject>("Prefabs/GameScene/Effect/RewardEffect"), GameManager.Instance.worldCanvas.transform);
                        itemObject.transform.position = bubbleObject.transform.position;
                        itemObject.GetComponent<RewardEffect>().Initialize(19, 1);
                        StaticManager.Backend.backendGameData.InventoryData.AddItem("Super_Milk", 1);
                        StaticManager.Backend.backendGameData.UserData.AddExp(StaticManager.Backend.backendChart.Harvest.harvestSheet[9].Exp[1]);
                    }
                    else
                    {
                        GameObject itemObject = Instantiate(Resources.Load<GameObject>("Prefabs/GameScene/Effect/RewardEffect"), GameManager.Instance.worldCanvas.transform);
                        itemObject.transform.position = bubbleObject.transform.position;
                        itemObject.GetComponent<RewardEffect>().Initialize(18, 1);
                        StaticManager.Backend.backendGameData.InventoryData.AddItem("Milk", 1);
                        StaticManager.Backend.backendGameData.UserData.AddExp(StaticManager.Backend.backendChart.Harvest.harvestSheet[9].Exp[0]);
                    }
                    
                    //퀘스트
                    StaticManager.Backend.backendGameData.QuestData.AddCount(6, 1);
                    break;
            }
            
            //업그레이드 레벨에 맞게 남는 시간 부여
            StaticManager.Backend.backendGameData.AnimalData.SetAnimal(animalName, GameManager.Instance.nowTime.AddSeconds(remainTimer).ToString());
            GameManager.Instance.SaveAllData();
            
        });
    }

    void Update()
    {
        if (!isInit) return;

        if (nowAnimal == AnimalUpgradeUI.Animal.Chicken)
        {
            lockButton.gameObject.SetActive(StaticManager.Backend.backendGameData.AnimalData.Dictionary["Chicken"].Upgrade == -1);
            transform.GetChild(0).gameObject.SetActive(StaticManager.Backend.backendGameData.AnimalData.Dictionary["Chicken"].Upgrade != -1);
            transform.GetChild(1).gameObject.SetActive(StaticManager.Backend.backendGameData.AnimalData.Dictionary["Chicken"].Upgrade != -1);

            GetComponent<Button>().interactable = StaticManager.Backend.backendGameData.AnimalData.Dictionary["Chicken"].Upgrade != -1;
            
            //맨 처음 생성 될 시
            if(string.IsNullOrEmpty(StaticManager.Backend.backendGameData.AnimalData.Dictionary["Chicken"].RemainTimer) && StaticManager.Backend.backendGameData.AnimalData.Dictionary["Chicken"].Upgrade > 0)
                GameManager.Animal.SetAnimal(AnimalUpgradeUI.Animal.Chicken, GameManager.Instance.nowTime.AddSeconds(StaticManager.Backend.backendChart.Animal.animalSheet[StaticManager.Backend.backendGameData.AnimalData.Dictionary["Chicken"].Upgrade].Chicken_Speed).ToString());
        }
        else
        {
            lockButton.gameObject.SetActive(StaticManager.Backend.backendGameData.AnimalData.Dictionary["Cow"].Upgrade == -1);
            
            transform.GetChild(0).gameObject.SetActive(StaticManager.Backend.backendGameData.AnimalData.Dictionary["Cow"].Upgrade != -1);
            transform.GetChild(1).gameObject.SetActive(StaticManager.Backend.backendGameData.AnimalData.Dictionary["Cow"].Upgrade != -1);
            
            GetComponent<Button>().interactable = StaticManager.Backend.backendGameData.AnimalData.Dictionary["Cow"].Upgrade != -1;
            //맨 처음 생성 될 시
            if(string.IsNullOrEmpty(StaticManager.Backend.backendGameData.AnimalData.Dictionary["Cow"].RemainTimer) && StaticManager.Backend.backendGameData.AnimalData.Dictionary["Cow"].Upgrade > 0)
                GameManager.Animal.SetAnimal(AnimalUpgradeUI.Animal.Cow, GameManager.Instance.nowTime.AddSeconds(StaticManager.Backend.backendChart.Animal.animalSheet[StaticManager.Backend.backendGameData.AnimalData.Dictionary["Cow"].Upgrade].Cow_Speed).ToString());
        }
        
        string animalName = nowAnimal == AnimalUpgradeUI.Animal.Chicken ? "Chicken" : "Cow";

        if (StaticManager.Backend.backendGameData.AnimalData.Dictionary[animalName].Upgrade == -1)
        {
            bubbleObject.SetActive(false);
            return;
        }
        
        TimeSpan remainTime = DateTime.Parse(StaticManager.Backend.backendGameData.AnimalData.Dictionary[animalName].RemainTimer) - GameManager.Instance.nowTime;

        if (remainTime.TotalSeconds > 0)
        {
            //말풍선 없애기
            bubbleObject.SetActive(false);
        }
        else
        {
            bubbleObject.SetActive(true);
        }
    }
}
