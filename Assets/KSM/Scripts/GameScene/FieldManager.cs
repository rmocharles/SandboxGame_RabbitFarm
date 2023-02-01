using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/*
 * FieldManager
 *
 * 1. 밭을 생성 (뒤끝 데이터)
 */
public class FieldManager : MonoBehaviour
{
    [SerializeField]
    private Vector3[] fieldPos =
    {
        new Vector3(0, -2.5f, 0),
        new Vector3(-2, -1.25f, 0),
        new Vector3(-4, 0, 0),
        new Vector3(2, -1.25f, 0),
        new Vector3(0, 0, 0),
        new Vector3(-2, 1.25f, 0),
        new Vector3(4, 0, 0),
        new Vector3(2, 1.25f, 0),
        new Vector3(0, 2.5f, 0)
    };
    
    public readonly Dictionary<int, FieldInfo> fields = new Dictionary<int, FieldInfo>();

    public Canvas fieldCanvas;
    public GameObject harvestBundle;
    public GameObject fxGroup;

    [HideInInspector]
    public GameObject selectHarvestUI, harvestInfoUI;

    void Update()
    {
        HideMainUI(selectHarvestUI != null || GameManager.Mart.slotUI != null);
    }
    public void Initialize()
    {
        //Field 생성 9개
        for (int i = 0; i < 9; i++)
        {
            //FX 오브젝트 생성
            GameObject fxObj = Instantiate(new GameObject(), fxGroup.transform);
            fxObj.transform.position = fieldPos[i] + new Vector3(0, 2, 0);
            fxObj.name = "FX_" + i;
            
            //Harvest배치
            GameObject harvestPool = Instantiate(new GameObject(), harvestBundle.transform);
            harvestPool.name = "Harvest_" + (i + 1);
            harvestPool.transform.position = new Vector3(fieldPos[i].x, fieldPos[i].y - 1, fieldPos[i].z);
            
            int num = i;
            GameObject fieldPrefab = Instantiate(Resources.Load<GameObject>("Prefabs/GameScene/Field"), fieldPos[i], Quaternion.identity, fieldCanvas.transform);
            fieldPrefab.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
            fieldPrefab.name = "Field_" + i;
            fieldPrefab.GetComponent<Button>().onClick.AddListener(() =>
            {
                StaticManager.Sound.SetSFX();
                //카메라 이동
                Camera.main.transform.DOMove(new Vector3(fieldPrefab.transform.position.x, fieldPrefab.transform.position.y, -10), 0.2f);
                
                SelectHarvestUI(num);
            });
            fieldPrefab.GetComponent<FieldInfo>().Initialize(num);

            fields.Add(i, fieldPrefab.GetComponent<FieldInfo>());
        }
    }

    //밭 상태 변경
    public void SetField(int fieldNumber, int fieldLevel, bool isCover = true)
    {
        StaticManager.Backend.backendGameData.FieldData.SetField(fieldNumber, fieldLevel);
        
        fields[fieldNumber].Initialize(fieldNumber, isCover);
        
        //GameManager.Instance.SaveAllData();
    }

    public void SetField(int fieldNumber, string remainTimer)
    {
        StaticManager.Backend.backendGameData.FieldData.SetField(fieldNumber, remainTimer);
        
        fields[fieldNumber].Initialize(fieldNumber, true);
        
        //GameManager.Instance.SaveAllData();
    }
    public void SetField(int fieldNumber, int fieldLevel, int harvestCode)
    {
        string remainTimer = string.Empty;
        if (harvestCode != -1)
        {
            remainTimer = DateTime.UtcNow.AddSeconds(StaticManager.Backend.backendChart.Harvest.harvestSheet[harvestCode].CoolTime).ToString();

            //비올때 쿨타임 50% 감소
            if (StaticManager.Backend.backendGameData.WeatherData.Type == 2)
            {
                remainTimer = DateTime.UtcNow.AddSeconds(StaticManager.Backend.backendChart.Harvest.harvestSheet[harvestCode].CoolTime / 2).ToString();
            }
        }
        StaticManager.Backend.backendGameData.FieldData.SetField(fieldNumber, fieldLevel, harvestCode, remainTimer);

        fields[fieldNumber].Initialize(fieldNumber);
        
        //GameManager.Instance.SaveAllData();
    }

    //밭을 눌렀을 때 잠금 여부 확인 및 UI 띄우기
    public void SelectHarvestUI(int fieldNumber)
    {
        //모든 켜져있는 UI 지우기
        CloseUI();
        
        //오픈 되어 있는지 뒤끝 정보 확인
        if (StaticManager.Backend.backendGameData.FieldData.Dictionary[fieldNumber].FieldLevel == -1)
        {
            StaticManager.Sound.SetSFX();
            
            GameObject unLockUI = StaticManager.UI.OpenUI("Prefabs/GameScene/UnlockUI", GameManager.Instance.UICanvas.transform);
            
            int count = 0;
            //0 ~ 8
            for (int i = 0; i < 9; i++)
            {
                if (StaticManager.Backend.backendGameData.FieldData.Dictionary[i].FieldLevel != -1)
                    count++;
            }

            unLockUI.GetComponent<UnlockUI>().Initialize(fieldNumber, StaticManager.Backend.backendChart.Price.GetPrice("Field_" + count), UnlockUI.Type.Field);
            return;
        }
        else
        {
            selectHarvestUI = StaticManager.UI.OpenUI("Prefabs/GameScene/SelectHarvestUI", GameManager.Instance.harvestUICanvas.transform);
            selectHarvestUI.GetComponent<SelectHarvestUI>().Initialize();
        }
    }
    
    //작물 Info UI
    public void HarvestInfoUI(int harvestCode, Transform parent)
    {
        StaticManager.Sound.SetSFX();

        if (harvestInfoUI != null)
            Destroy(harvestInfoUI);

        harvestInfoUI = StaticManager.UI.OpenUI("Prefabs/GameScene/HarvestInfoUI", parent);
        harvestInfoUI.GetComponent<HarvestInfoUI>().Initialize(harvestCode);

        harvestInfoUI.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, 250);
        harvestInfoUI.transform.SetParent(GameManager.Instance.harvestUICanvas.transform);

        harvestInfoUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(
            Mathf.Clamp(harvestInfoUI.GetComponent<RectTransform>().anchoredPosition.x, -(Screen.width / 2) + harvestInfoUI.GetComponent<RectTransform>().rect.width / 2 + 150,
                Screen.width / 2 - harvestInfoUI.GetComponent<RectTransform>().rect.width / 2 - 125), harvestInfoUI.GetComponent<RectTransform>().anchoredPosition.y);
    }
    
    //모든 UI 지우기
    public void CloseUI()
    {
        if (selectHarvestUI != null && selectHarvestUI.activeSelf || harvestInfoUI != null && harvestInfoUI.activeSelf)
        {
            Destroy(harvestInfoUI);
            Destroy(selectHarvestUI);
        }
    }
    
    public void HideMainUI(bool isActive)
    {
        float movePos = isActive ? -1000 : 0;
        GameManager.Instance.questButton.transform.DOMoveX(GameManager.Instance.originQuestPos.x + movePos, 0.5f);
        GameManager.Instance.bagButton.transform.DOMoveX(GameManager.Instance.originBagPos.x + movePos, 0.5f);
        GameManager.Instance.shopButton.transform.DOMoveX(GameManager.Instance.originShopPos.x + movePos, 0.5f);
        GameManager.Instance.moveButton.transform.DOMoveX(GameManager.Instance.originMovePos.x - movePos, 0.5f);
    }
}
