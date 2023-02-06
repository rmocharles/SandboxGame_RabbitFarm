using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartTimeUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text[] infoTexts;
    [SerializeField] private Button[] purchaseButtons;

    void Start()
    {
        closeButton.onClick.AddListener(() =>
        {
            StaticManager.Sound.SetSFX();
            Destroy(this.gameObject);
        });

        titleText.text = StaticManager.Langauge.Localize(58);
        infoTexts[0].text = StaticManager.Langauge.Localize(193);
        infoTexts[1].text = StaticManager.Langauge.Localize(59);
        infoTexts[2].text = StaticManager.Langauge.Localize(60);
        
        purchaseButtons[0].onClick.AddListener(() =>
        {
            Destroy(this.gameObject);
            StaticManager.Sound.SetSFX();
            StaticManager.AD.ShowRewardAD(() =>
            {
                //10분 알바 고용
                StaticManager.Backend.backendGameData.PartTimeData.SetPartTime(0);
                
                //모든 게스트 자동 구매 초기 확인
                for(int i = 0; i < GameManager.Mart.Guest.waitGuests.Count; i++)
                    GameManager.Mart.Guest.waitGuests[i].PurchaseInitial();
                
                GameManager.Instance.SaveAllData();
            });
        });
        
        purchaseButtons[1].onClick.AddListener(() =>
        {
            if (StaticManager.Backend.backendGameData.UserData.Diamond >= 50)
            {
                Destroy(this.gameObject);
                StaticManager.Sound.SetSFX("Cash");
                StaticManager.Backend.backendGameData.UserData.AddDiamond(-50);

                //24시간 알바 고용
                StaticManager.Backend.backendGameData.PartTimeData.SetPartTime(1);
                
                //모든 게스트 자동 구매 초기 확인
                for(int i = 0; i < GameManager.Mart.Guest.waitGuests.Count; i++)
                    GameManager.Mart.Guest.waitGuests[i].PurchaseInitial();
                
                GameManager.Instance.SaveAllData();
            }
            else
            {
                StaticManager.Sound.SetSFX();
                GameManager.Instance.MakeToast(StaticManager.Langauge.Localize(47));
            }
        });
    }

}
