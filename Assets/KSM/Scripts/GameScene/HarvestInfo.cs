using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class HarvestInfo : MonoBehaviour
{
    private Button completeButton;
    private Button skipButton;

    private int fieldNumber;
    private int harvestCode;
    private int fieldLevel;

    void Start()
    {
        completeButton = GetComponentsInChildren<Button>()[0];
        skipButton = GetComponentsInChildren<Button>()[1];
        
        //completeButton.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
        completeButton.onClick.RemoveAllListeners();
        completeButton.onClick.AddListener(() =>
        { 
            StaticManager.Sound.SetSFX("Get");
            GameManager.Field.CloseUI();

            //인근 보상 한번에 받기 (나랑 같은 작물이고, 완성된 작물인지 체크)
            for (int i = 0; i < 9; i++)
            {
                if (StaticManager.Backend.backendGameData.FieldData.Dictionary[i].HarvestCode == harvestCode)
                {
                    TimeSpan remainTime = DateTime.Parse(StaticManager.Backend.backendGameData.FieldData.Dictionary[i].RemainTimer) - DateTime.UtcNow;
                    if (remainTime.TotalSeconds <= 0)
                    {

                        int percent = Random.Range(1, 101);
            
                        int count = 0;
                        int exp = 0;
                        
                        #region 개수 정하기

                    if (percent > 0 && percent <= StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[0])
                        count = 1;
                    else if (percent > StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[0] &&
                             percent <= StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[0] + StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[1])
                        count = 2;
                    else if (percent > StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[0] + 
                             StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[1] && 
                             percent <= StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[0] + 
                             StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[1] + 
                             StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[2])
                        count = 3;
                    else if (percent > StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[0] + 
                             StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[1] + 
                             StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[2] &&
                             percent <= StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[0] + 
                             StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[1] + 
                             StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[2] + 
                             StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[3])
                        count = 4;
                    else if (percent > StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[0] + 
                             StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[1] + 
                             StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[2] + 
                             StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[3]
                             && percent <= StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[0] + 
                             StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[1] + 
                             StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[2] + 
                             StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[3] + 
                             StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[4])
                        count = 5;
                    
                    else if (percent > StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[0] + 
                             StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[1] + 
                             StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[2] + 
                             StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[3] + 
                             StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[4]
                             && percent <= StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[0] + 
                             StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[1] + 
                             StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[2] + 
                             StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[3] + 
                             StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[4] +
                             StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[5])
                        count = 6;
                    else if(percent > StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[0] + 
                            StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[1] + 
                            StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[2] + 
                            StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[3] + 
                            StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[4] +
                            StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Case[5])
                        count = 7;

                    #endregion

                    if (StaticManager.Backend.backendGameData.WeatherData.Type == 1)
                        count *= 2;

                        #region 유형 정하기

                    int[] version = {0, 0, 0};

                    if (StaticManager.Backend.backendGameData.FieldData.Dictionary[i].FieldLevel > 0)
                        { 
                            string type = StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Type;
                            Debug.LogError(harvestCode + ", " + type);
                            switch (type)
                        {
                            case "Upgrade":
                                for (int j = 0; j < count; j++)
                                {
                                    int special = Random.Range(1, 101);
                                    Debug.LogError(special);
                                    if (special > 0 && special <= StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Version[0])
                                    {
                                        version[0]++;
                                        exp += StaticManager.Backend.backendChart.Harvest.harvestSheet[harvestCode].Exp[0];
                                        StaticManager.Backend.backendGameData.InventoryData.AddItem(new []{harvestCode, 0}, 1);
                                    }
                                    else if (special > StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Version[0] &&
                                             special <= StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Version[0] +
                                             StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Version[1])
                                    {
                                        version[1]++;
                                        exp += StaticManager.Backend.backendChart.Harvest.harvestSheet[harvestCode].Exp[1];
                                        StaticManager.Backend.backendGameData.InventoryData.AddItem(new []{harvestCode, 1}, 1);
                                    }

                                    else if(special > StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Version[0] +
                                            StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Version[1])
                                    {
                                        version[2]++;
                                        exp += StaticManager.Backend.backendChart.Harvest.harvestSheet[harvestCode].Exp[2];
                                        StaticManager.Backend.backendGameData.InventoryData.AddItem(new []{harvestCode, 2}, 1);
                                    }
                                }
                                
                                //퀘스트
                                StaticManager.Backend.backendGameData.QuestData.AddCount(0, version[0]);
                                StaticManager.Backend.backendGameData.QuestData.AddCount(1, version[1] + version[2]);
                                break;
                
                            case "Amount":
                                int addPercent = Random.Range(1, 101);
                                int add = 0;
                                if (addPercent > 0 && addPercent <= StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Version[0])
                                    add = 1;
                                else if (addPercent > StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Version[0] &&
                                         addPercent <= StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Version[0] +
                                         StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Version[1])
                                    add = 2;
                                else if(addPercent > StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Version[0] +
                                        StaticManager.Backend.backendChart.Percent.percentSheet[harvestCode].Version[1])
                                    add = 3;

                                version[0] = count + add;
                                
                                //퀘스트
                                StaticManager.Backend.backendGameData.QuestData.AddCount(0, version[0]);
                                
                                StaticManager.Backend.backendGameData.InventoryData.AddItem(new []{harvestCode, 0}, count + add);
                                exp += StaticManager.Backend.backendChart.Harvest.harvestSheet[harvestCode].Exp[0] * version[0];
                                break;
                        }
                        }
                    
                    else if(StaticManager.Backend.backendGameData.FieldData.Dictionary[i].FieldLevel == 0)
                    {
                        version[0] = count;
                        //퀘스트
                        StaticManager.Backend.backendGameData.QuestData.AddCount(0, version[0]);
                        
                        exp += StaticManager.Backend.backendChart.Harvest.harvestSheet[harvestCode].Exp[0] * version[0];
                        StaticManager.Backend.backendGameData.InventoryData.AddItem(new []{harvestCode, 0}, version[0]);
                    }
                    
                    #endregion
                    
                    //경험치 추가
                    StaticManager.Backend.backendGameData.UserData.AddExp(exp);
                    
                    //이펙트 추가 (작물 종류, 개수, 경험치)

                    Dictionary<int, RewardEffect.Item> list = new Dictionary<int, RewardEffect.Item>();
                    for (int j = 0; j < version.Length; j++)
                    {
                        if (version[j] != 0)
                        {
                            if (j == 0)
                            {
                                RewardEffect.Item item = new RewardEffect.Item(harvestCode, version[0]);
                                list.Add(j, item);
                            }
                            else if (j == 1)
                            {
                                RewardEffect.Item item = new RewardEffect.Item(harvestCode + 9, version[1]);
                                list.Add(j, item);
                            }
                            else if (j == 2)
                            {
                                RewardEffect.Item item = new RewardEffect.Item(harvestCode + 15, version[2]);
                                list.Add(j, item);
                            }
                        }
                    }

                    GameObject effectObject = Instantiate(Resources.Load<GameObject>("Prefabs/GameScene/Effect/RewardEffect"), GameManager.Instance.worldCanvas.transform);
                    effectObject.transform.position = GameManager.Field.harvestBundle.transform.GetChild(i).transform.position + new Vector3(0, 1, 0);
                    
                    effectObject.GetComponent<RewardEffect>().Initialize(list);
                    
                    //밭 초기화
                    GameManager.Field.SetField(i, 0, -1);

                    //작물 제거
                    Destroy(GameManager.Field.fields[i].harvestObject);
                    }
                }
            }
            
            //첫 튜토리얼 달성
            if (StaticManager.Backend.backendGameData.UserData.Tutorial == 2)
            {
                StaticManager.Backend.backendGameData.UserData.SetTutorial(3);
                GameManager.Tutorial.SetTutorial(1);
            }
            
            GameManager.Instance.SaveAllData();
        });
        
        //스킵
        skipButton.onClick.RemoveAllListeners();
        skipButton.onClick.AddListener(() =>
        {
            GameManager.Field.CloseUI();
            GameObject completeUI = StaticManager.UI.OpenUI("Prefabs/GameScene/ICompleteUI", GameManager.Instance.UICanvas.transform);
            completeUI.GetComponent<ICompleteUI>().Initialize(fieldNumber);
        });
    }

    public void Initialize(int fieldNumber)
    {
        this.fieldNumber = fieldNumber;
        harvestCode = StaticManager.Backend.backendGameData.FieldData.Dictionary[fieldNumber].HarvestCode;
        fieldLevel = StaticManager.Backend.backendGameData.FieldData.Dictionary[fieldNumber].FieldLevel;
    }
    
    public void IsActive(bool isActive)
    {
        if (completeButton == null || skipButton == null) return;
        
        completeButton.transform.parent.gameObject.SetActive(isActive);
        skipButton.gameObject.SetActive(!isActive);
    }
}
