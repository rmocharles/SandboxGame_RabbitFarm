using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class FieldInfo : MonoBehaviour
{
    public GameObject lockObject;

    //밭의 기본 정보
    private int fieldNumber = 0;
    private int fieldLevel = 0;
    private int harvestCode = 0;
    private string remainTimer = string.Empty;

    [HideInInspector]
    public GameObject harvestObject;

    private int nowStep = 0;
    private bool isFirst = true;
    private float waitTimer = 0;

    void Start()
    {
    }

    public void Initialize(int fieldNumber, bool isCover = false)
    {
        this.fieldNumber = fieldNumber;
        this.harvestCode = StaticManager.Backend.backendGameData.FieldData.Dictionary[fieldNumber].HarvestCode;
        this.fieldLevel = StaticManager.Backend.backendGameData.FieldData.Dictionary[fieldNumber].FieldLevel;
        this.remainTimer = StaticManager.Backend.backendGameData.FieldData.Dictionary[fieldNumber].RemainTimer;
        
        GetComponent<Button>().interactable = StaticManager.Backend.backendGameData.FieldData.Dictionary[fieldNumber].HarvestCode == -1;
        
        lockObject.GetComponent<Button>().onClick.RemoveAllListeners();
        lockObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            GameObject unLockUI = StaticManager.UI.OpenUI("Prefabs/GameScene/UnlockUI", GameManager.Instance.UICanvas.transform);
            StaticManager.Sound.SetSFX();

            if (fieldNumber < 5)
            {
                unLockUI.GetComponent<UnlockUI>().Initialize(fieldNumber, 300 * (fieldNumber), UnlockUI.Type.Field);
            }
            else
            {
                unLockUI.GetComponent<UnlockUI>().Initialize(fieldNumber, 2000 * (fieldNumber - 4), UnlockUI.Type.Field);
            }

            
        });
        
        //이펙트 생성
        if (fieldLevel > 0)
        {
            GameObject fxObject = Instantiate(Resources.Load<GameObject>("Prefabs/GameScene/Effect/Fertilizer_FX"), GameManager.Field.fxGroup.transform.GetChild(fieldNumber));
        }
        else
        {
            for(int i = 0; i < GameManager.Field.fxGroup.transform.GetChild(fieldNumber).childCount; i++)
                Destroy(GameManager.Field.fxGroup.transform.GetChild(fieldNumber).GetChild(i).gameObject);
        }

        //밭 레벨 체크
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Atlas_1");
        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i].name == "FieldType_" + fieldLevel)
                GetComponent<Image>().sprite = sprites[i];
        }
        
        //작물 단계 체크 (처음 한번만)
        if (isFirst)
        {
            isFirst = false;
            
            if (string.IsNullOrEmpty(remainTimer) || harvestCode == -1)
            {
                nowStep = 0;
            }
            else
            {
                TimeSpan remainTime = DateTime.Parse(remainTimer) - DateTime.UtcNow;

                if(1 - (float)remainTime.TotalSeconds / StaticManager.Backend.backendChart.Harvest.harvestSheet[harvestCode].CoolTime < 0.7f && 
                   1 - (float)remainTime.TotalSeconds / StaticManager.Backend.backendChart.Harvest.harvestSheet[harvestCode].CoolTime >= 0.3f)
                {
                    nowStep = 1;
                }
                else if (1 - (float)remainTime.TotalSeconds / StaticManager.Backend.backendChart.Harvest.harvestSheet[harvestCode].CoolTime < 0.3f &&
                         1 - (float)remainTime.TotalSeconds / StaticManager.Backend.backendChart.Harvest.harvestSheet[harvestCode].CoolTime > 0f)
                {
                    nowStep = 2;
                }
            }
        }

        if (isCover)
        {
            harvestObject = GameManager.Field.harvestBundle.transform.GetChild(fieldNumber).GetChild(0).gameObject;
            harvestObject.GetComponent<HarvestInfo>().Initialize(fieldNumber);
            harvestObject.GetComponent<SortingGroup>().sortingOrder = (int)(transform.position.y * -100);
            Debug.LogError(StaticManager.Backend.backendGameData.FieldData.Dictionary[fieldNumber].FieldLevel);
        }
        else
        {
            //작물 체크 (농작물이 있을 경우)
            if (harvestCode != -1)
            {
                string harvestName = StaticManager.Backend.backendGameData.InventoryData.harvestItem[harvestCode, 0];
                harvestObject = StaticManager.UI.OpenUI("Prefabs/GameScene/Harvest/" + harvestName, GameManager.Field.harvestBundle.transform.GetChild(fieldNumber).transform);
                harvestObject.GetComponent<HarvestInfo>().Initialize(fieldNumber);
                harvestObject.GetComponent<SortingGroup>().sortingOrder = (int)(transform.position.y * -100);
            }
        }
        
    }

    void Update()
    {
        //잠김 여부 체크
        lockObject.SetActive(fieldLevel == -1);
        
        //농작물이 심어져 있는 상태일 경우
        if (harvestCode == -1) return;

        if (string.IsNullOrEmpty(remainTimer) || harvestObject == null || !harvestObject.activeSelf) return;

        if (waitTimer > 0) waitTimer -= Time.deltaTime;
        
        TimeSpan remainTime = DateTime.Parse(remainTimer) - DateTime.UtcNow;

        if (remainTime.TotalSeconds > 0)
        {
            //말풍선 비활성화
            harvestObject.GetComponent<HarvestInfo>().IsActive(false);

            //씨앗 단계
            if (1 - (float)remainTime.TotalSeconds / StaticManager.Backend.backendChart.Harvest.harvestSheet[harvestCode].CoolTime < 0.3f)
            {
                //처음 심었을 때 단 한번 실행
                if (nowStep == 0)
                {
                    nowStep = 1;
                    waitTimer = 0.5f;
                    harvestObject.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "step1_start", false);
                    harvestObject.GetComponent<SkeletonAnimation>().AnimationState.AddAnimation(0, "step1_idle", true, 0);
                }
                else
                {
                    if (waitTimer <= 0)
                    {
                        if(harvestObject.GetComponent<SkeletonAnimation>().AnimationName != "step1_idle")
                            harvestObject.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "step1_idle", true);
                    }
                }
            }
            

            //새싹 단계
            if (1 - (float)remainTime.TotalSeconds / StaticManager.Backend.backendChart.Harvest.harvestSheet[harvestCode].CoolTime < 0.7f &&
                1 - (float)remainTime.TotalSeconds / StaticManager.Backend.backendChart.Harvest.harvestSheet[harvestCode].CoolTime >= 0.3f)
            {
                if (nowStep == 1)
                {
                    nowStep = 2;
                    waitTimer = 0.567f;
                    harvestObject.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "step2_start", false);
                    harvestObject.GetComponent<SkeletonAnimation>().AnimationState.AddAnimation(0, "step2_idle", true, 0);
                }
                else
                {
                    if (waitTimer <= 0)
                    {
                        if(harvestObject.GetComponent<SkeletonAnimation>().AnimationName != "step2_idle")
                            harvestObject.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "step2_idle", true);
                    }
                }
            }
            
            //중간 새싹 단계
            if (1 - (float)remainTime.TotalSeconds / StaticManager.Backend.backendChart.Harvest.harvestSheet[harvestCode].CoolTime < 1f &&
                1 - (float)remainTime.TotalSeconds / StaticManager.Backend.backendChart.Harvest.harvestSheet[harvestCode].CoolTime >= 0.7f)
            {
                if (nowStep == 2)
                {
                    if (GameManager.Instance.nowMode == GameManager.Mode.Farm)
                    {
                        //사운드
                        GetComponent<AudioSource>().clip = StaticManager.Sound.SearchClip("Grow");
                        GetComponent<AudioSource>().Play();
                    }
                    
                    nowStep = 3;
                    waitTimer = 0.567f;
                    harvestObject.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "step3_start", false);
                    harvestObject.GetComponent<SkeletonAnimation>().AnimationState.AddAnimation(0, "step3_idle", true, 0);
                }
                else
                {
                    if (waitTimer <= 0)
                    {
                        if(harvestObject.GetComponent<SkeletonAnimation>().AnimationName != "step3_idle")
                            harvestObject.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "step3_idle", true);
                    }
                }
            }
        }
        //완성 단계
        else
        {
            harvestObject.GetComponent<HarvestInfo>().IsActive(true);
            
            if (nowStep == 3)
            {
                nowStep = 4;
                waitTimer = 1.1f;
                harvestObject.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "step4_start", false);
                harvestObject.GetComponent<SkeletonAnimation>().AnimationState.AddAnimation(0, "step4_idle", true, 0);
            }
            else
            {
                nowStep = 0;
                if (waitTimer <= 0)
                {
                    if(harvestObject.GetComponent<SkeletonAnimation>().AnimationName != "step4_idle")
                        harvestObject.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "step4_idle", true);
                }
            }
        }
    }

    public int GetFieldNumber()
    {
        return fieldNumber;
    }

    public int GetFieldLevel()
    {
        return fieldLevel;
    }

    public int GetHarvestCode()
    {
        return harvestCode;
    }

    public string GetRemainTimer()
    {
        return remainTimer;
    }
}
