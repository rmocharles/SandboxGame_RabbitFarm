using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PartTimeManager : MonoBehaviour
{
    [SerializeField] private Button partTimeButton;
    [SerializeField] private GameObject[] partTimeObjects;

    void Start()
    {
        partTimeButton.onClick.AddListener(() =>
        {
            StaticManager.UI.OpenUI("Prefabs/GameScene/PartTimeUI", GameManager.Instance.UICanvas.transform);
        });
        
        for(int i = 0; i < partTimeObjects.Length; i++)
            partTimeObjects[i].SetActive(false);
    }

    void Update()
    {
        if (StaticManager.Backend.backendGameData.UserData.Level < 5) return;
        //알바생 생성
        for (int i = 0; i < 3; i++)
        {
            partTimeObjects[i].SetActive(i == StaticManager.Backend.backendGameData.PartTimeData.Type);
            AdjustSortingLayer(i);
        }
        partTimeButton.gameObject.SetActive(StaticManager.Backend.backendGameData.PartTimeData.Type == -1);

        if (string.IsNullOrEmpty(StaticManager.Backend.backendGameData.PartTimeData.RemainTimer) || StaticManager.Backend.backendGameData.PartTimeData.Type == -1) return;
        
        //정규직인 경우
        if (StaticManager.Backend.backendGameData.PartTimeData.Type == 2)
        {
            return;
        }

        TimeSpan remainTime = DateTime.Parse(StaticManager.Backend.backendGameData.PartTimeData.RemainTimer) - DateTime.UtcNow;
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
