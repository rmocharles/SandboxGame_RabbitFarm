using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PartTimeManager : MonoBehaviour
{
    [SerializeField] private Button partTimeButton;
    [SerializeField] private GameObject[] partTimeObjects;

    [SerializeField] private GameObject remainObject;

    void Start()
    {
        partTimeButton.onClick.AddListener(() =>
        {
            if(GameManager.Tutorial.clickObject.activeSelf)
                GameManager.Tutorial.clickObject.SetActive(false);
            
            StaticManager.UI.OpenUI("Prefabs/GameScene/PartTimeUI", GameManager.Instance.UICanvas.transform);
        });
        
        for(int i = 0; i < partTimeObjects.Length; i++)
            partTimeObjects[i].SetActive(false);
    }

    void Update()
    {
        remainObject.SetActive(StaticManager.Backend.backendGameData.PartTimeData.Type != -1 && StaticManager.Backend.backendGameData.PartTimeData.Type != 2);

        if (StaticManager.Backend.backendGameData.UserData.Level < 5)
        {
            partTimeButton.gameObject.SetActive(false);
        }
        else
        {
            partTimeButton.gameObject.SetActive(StaticManager.Backend.backendGameData.PartTimeData.Type == -1);
            
            //알바생 생성
            for (int i = 0; i < 3; i++)
            {
                partTimeObjects[i].SetActive(i == StaticManager.Backend.backendGameData.PartTimeData.Type);
                AdjustSortingLayer(i);
            }
            
            if (string.IsNullOrEmpty(StaticManager.Backend.backendGameData.PartTimeData.RemainTimer) || StaticManager.Backend.backendGameData.PartTimeData.Type == -1) return;
        
            //정규직인 경우
            if (StaticManager.Backend.backendGameData.PartTimeData.Type == 2)
            {
                return;
            }

            TimeSpan remainTime = DateTime.Parse(StaticManager.Backend.backendGameData.PartTimeData.RemainTimer) - GameManager.Instance.nowTime;
            int remainTimer = Mathf.FloorToInt((float)Math.Truncate(remainTime.TotalSeconds));
            
            int partTimeCoolTime = StaticManager.Backend.backendGameData.PartTimeData.Type == 0 ? 2 : 24;
            remainObject.GetComponentsInChildren<Image>()[1].fillAmount = (float)(1 - (remainTime.TotalSeconds) / (3600 * partTimeCoolTime));
            remainObject.GetComponentInChildren<TMP_Text>().text = string.Format("{0:D2}:{1:D2}:{2:D2}", remainTimer / 3600, remainTimer % 3600 / 60, remainTimer % 3600 % 60 );
            //알바생인 경우
            if(StaticManager.Backend.backendGameData.PartTimeData.Type < 2)
            {
                if (remainTime.TotalSeconds > 0)
                {
                    
                }
                else
                {
                    Debug.LogError("알바 종료");
                    StaticManager.Backend.backendGameData.PartTimeData.SetPartTime(-1);
                    GameManager.Instance.SaveAllData();
                }
            }
        }
    }

    public void SetWorkAnimation(int index)
    {
        partTimeObjects[index].GetComponent<SkeletonAnimation>().state.SetAnimation(0, "sell", false);
        partTimeObjects[index].GetComponent<SkeletonAnimation>().state.AddAnimation(0, "idle", true, 0);
    }
    
    public void AdjustSortingLayer(int index)
    {
        partTimeObjects[index].GetComponent<SortingGroup>().sortingOrder = (int)(partTimeObjects[index].transform.position.y * -100);
    }
}
