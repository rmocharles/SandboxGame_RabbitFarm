using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    [SerializeField] private TMP_Text infoText;
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject[] layoutGroup;

    [SerializeField] private Sprite completeButtonSprite;
    [SerializeField] private Sprite inCompleteButtonSprite;


    private Color32 diamondColor = new Color32(228, 249, 252, 255);
    private Color32 goldColor = new Color32(253, 239, 176, 255);

    private Color32 completeColor = new Color32(71, 168, 197, 255);
    private Color32 inCompleteColor = new Color32(255, 0, 0, 255);

    void Awake()
    {
        infoText.text = StaticManager.Langauge.Localize(61);
        
        closeButton.onClick.AddListener(() =>
        {
            StaticManager.Sound.SetSFX();
            Destroy(this.gameObject);
        });

        Initialize();
    }

    void Initialize()
    {
        for (int i = 0; i < 8; i++)
        {
            //레이어 별 구분
            TMP_Text questInfoText = layoutGroup[i].GetComponentInChildren<TMP_Text>();
            TMP_Text currentCountText = layoutGroup[i].GetComponentsInChildren<TMP_Text>()[1];
            TMP_Text maxCountText = layoutGroup[i].GetComponentsInChildren<TMP_Text>()[2];
            TMP_Text completeCountText = layoutGroup[i].GetComponentsInChildren<TMP_Text>()[3];
            
            //해당 퀘스트가 골드 or 다이아 보상인지 체크 (레이어 색 변경)
            layoutGroup[i].GetComponent<Image>().color = StaticManager.Backend.backendChart.Quest.questSheet[i].Type == "Gold" ? goldColor : diamondColor;
            
            //퀘스트 번역
            questInfoText.text = StaticManager.Langauge.Localize(62 + i);
            
            //현재 카운트된 수
            currentCountText.text = StaticManager.Backend.backendGameData.QuestData.Dictionary[i].Count.ToString();
            
            //총 모아야 하는 수
            maxCountText.text = "/ " +
                                Mathf.Min(StaticManager.Backend.backendChart.Quest.questSheet[i].Upgrade * StaticManager.Backend.backendGameData.QuestData.Dictionary[i].Level,
                                    StaticManager.Backend.backendChart.Quest.questSheet[i].MaxUpgrade);
            
            //보상 카운트 수
            completeCountText.text = (Mathf.Min(StaticManager.Backend.backendChart.Quest.questSheet[i].Reward * StaticManager.Backend.backendGameData.QuestData.Dictionary[i].Level,
                StaticManager.Backend.backendChart.Quest.questSheet[i].MaxReward)).ToString();
            
            //보상 받을 수 있는지 여부에 따라 버튼 비활성화
            bool isOpen = StaticManager.Backend.backendGameData.QuestData.Dictionary[i].Count >= StaticManager.Backend.backendChart.Quest.questSheet[i].Upgrade *
                StaticManager.Backend.backendGameData.QuestData.Dictionary[i].Level;
            layoutGroup[i].GetComponentInChildren<Button>().interactable = isOpen;
            layoutGroup[i].GetComponentsInChildren<Image>()[2].sprite = isOpen ? completeButtonSprite : inCompleteButtonSprite;

            currentCountText.color = isOpen ? completeColor : inCompleteColor;

            //보상 관련
            int num = i;
            layoutGroup[num].GetComponentInChildren<Button>().onClick.RemoveAllListeners();
            layoutGroup[num].GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                StaticManager.Sound.SetSFX();
                //보상 여부
                bool isGold = StaticManager.Backend.backendChart.Quest.questSheet[num].Type == "Gold";
                int count = Mathf.Min(StaticManager.Backend.backendChart.Quest.questSheet[num].Upgrade * StaticManager.Backend.backendGameData.QuestData.Dictionary[num].Level,
                    StaticManager.Backend.backendChart.Quest.questSheet[num].MaxUpgrade);
                int reward = Mathf.Min(StaticManager.Backend.backendChart.Quest.questSheet[num].Reward * StaticManager.Backend.backendGameData.QuestData.Dictionary[num].Level,
                    StaticManager.Backend.backendChart.Quest.questSheet[num].MaxReward);


                switch (StaticManager.Backend.backendChart.Quest.questSheet[num].Type)
                {
                    case "Gold":
                        StaticManager.Backend.backendGameData.UserData.AddGold(reward);
                        GameManager.Instance.GoldUI("+" + reward);
                        break;
                    
                    case "Diamond":
                        StaticManager.Backend.backendGameData.UserData.AddDiamond(reward);
                        GameManager.Instance.DiamondUI("+" + reward);
                        break;
                    
                    case "Fertilizer":
                        StaticManager.Backend.backendGameData.InventoryData.AddItem("Fertilizer", reward);
                        break;
                }

                //업그레이드
                StaticManager.Backend.backendGameData.QuestData.SetQuest(num, -1);
                
                //개수 제거
                StaticManager.Backend.backendGameData.QuestData.AddCount(num, -count);
                
                //데이터 저장
                GameManager.Instance.SaveAllData();
                
                //초기화
                Initialize();
            });
        }
    }
}
