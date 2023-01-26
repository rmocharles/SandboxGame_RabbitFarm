using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackendData.Base;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class HarvestDragUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private Canvas uiCanvas;

    private int harvestCode;
    
    //드래그 관련
    private bool isDrag = false;
    private GameObject duplicateHarvestObject;
    private CanvasGroup canvasGroup;
    private GameObject duplicateInfoObject;
    void Awake()
    {
        uiCanvas = GameManager.Instance.harvestUICanvas;
        
        //버튼 관련 애니메이션 처리
        EventTrigger.Entry entry_PointerClick = new EventTrigger.Entry();
        entry_PointerClick.eventID = EventTriggerType.PointerClick;
        entry_PointerClick.callback.AddListener((data) =>
        {
            ButtonAnimation("Start");
        });
        GetComponent<EventTrigger>().triggers.Add(entry_PointerClick);
        
        EventTrigger.Entry entry_PointerEnter = new EventTrigger.Entry();
        entry_PointerEnter.eventID = EventTriggerType.PointerEnter;
        entry_PointerEnter.callback.AddListener((data) => { ButtonAnimation("Start"); });
        GetComponent<EventTrigger>().triggers.Add(entry_PointerEnter);

        EventTrigger.Entry entry_PointerExit = new EventTrigger.Entry();
        entry_PointerExit.eventID = EventTriggerType.PointerExit;
        entry_PointerExit.callback.AddListener((data) => { ButtonAnimation("End"); });
        GetComponent<EventTrigger>().triggers.Add(entry_PointerExit);

        EventTrigger.Entry entry_PointerUp = new EventTrigger.Entry();
        entry_PointerUp.eventID = EventTriggerType.PointerUp;
        entry_PointerUp.callback.AddListener((data) => { ButtonAnimation("End"); });
        GetComponent<EventTrigger>().triggers.Add(entry_PointerUp);
    }

    public void Initialize(int harvestCode)
    {
        this.harvestCode = harvestCode;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (StaticManager.Backend.backendGameData.UserData.Level >= StaticManager.Backend.backendChart.Harvest.harvestSheet[harvestCode].RequireLevel &&
            StaticManager.Backend.backendGameData.UserData.Gold >= StaticManager.Backend.backendChart.Harvest.harvestSheet[harvestCode].Price)
        {
            //심을 버니 정하기
            GameManager.Bunny.harvestBunnyNumber = Random.Range(0, 3);
        
            for(int i = 0; i < uiCanvas.transform.childCount; i++)
                if (uiCanvas.transform.GetChild(i).GetComponent<HarvestInfoUI>())
                    Destroy(uiCanvas.transform.GetChild(i).gameObject);

            duplicateHarvestObject = Instantiate(this.gameObject, uiCanvas.transform);
            duplicateHarvestObject.GetComponent<HarvestDragUI>().Initialize(harvestCode);
            duplicateHarvestObject.GetComponent<HarvestDragUI>().SetDrag(true);
            duplicateHarvestObject.transform.position = transform.position;
            duplicateHarvestObject.GetComponent<CanvasGroup>().blocksRaycasts = false;

            duplicateInfoObject = StaticManager.UI.OpenUI("Prefabs/GameScene/HarvestInfoUI", duplicateHarvestObject.transform);
            duplicateInfoObject.transform.localScale = Vector3.one / 0.55f;
            duplicateInfoObject.GetComponent<HarvestInfoUI>().Initialize(harvestCode);
            duplicateInfoObject.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, 300);
        }

        if (StaticManager.Backend.backendGameData.UserData.Level < StaticManager.Backend.backendChart.Harvest.harvestSheet[harvestCode].RequireLevel)
        {
            GameManager.Instance.MakeToast(StaticManager.Langauge.Localize(45));
        }

        if (StaticManager.Backend.backendGameData.UserData.Gold < StaticManager.Backend.backendChart.Harvest.harvestSheet[harvestCode].Price)
        {
            GameManager.Instance.MakeToast(StaticManager.Langauge.Localize(39));
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (duplicateHarvestObject == null) return;
        duplicateHarvestObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
        duplicateHarvestObject.GetComponent<HarvestDragUI>().SetDrag(false);
        Destroy(duplicateHarvestObject);
        
        GameManager.Field.selectHarvestUI.GetComponent<RectTransform>().DOMoveY(-50, 1f);
        Invoke("DestroyUI", .1f);

        GameManager.Instance.SaveAllData();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (duplicateHarvestObject == null) return;
        duplicateHarvestObject.GetComponent<RectTransform>().anchoredPosition += eventData.delta / uiCanvas.scaleFactor;
        
    }


    private int amount = 0;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!isDrag) return;

        if (!col.name.Contains("Field")) return;

        //밭이 비어 있을 경우
        if (col.GetComponent<FieldInfo>().GetFieldLevel() == 0 && col.GetComponent<FieldInfo>().GetHarvestCode() == -1)
        {
            //작물 살 돈이 있을 경우
            if (StaticManager.Backend.backendGameData.UserData.Gold >= StaticManager.Backend.backendChart.Harvest.harvestSheet[harvestCode].Price)
            {
                amount += StaticManager.Backend.backendChart.Harvest.harvestSheet[harvestCode].Price;

                GameObject bubble = GameManager.Instance.GoldUI("-" + amount);
                //돈 차감
                StaticManager.Backend.backendGameData.UserData.SetGold(StaticManager.Backend.backendGameData.UserData.Gold - StaticManager.Backend.backendChart.Harvest.harvestSheet[harvestCode].Price);
                
                //밭 상태 변화
                GameManager.Field.SetField(col.GetComponent<FieldInfo>().GetFieldNumber(), 0, harvestCode);
                
                //수확 애니메이션
                GameManager.Bunny.ChangeBunnyState(GameManager.Bunny.harvestBunnyNumber, BunnyController.State.Harvest);
                GameManager.Bunny.bunnies[GameManager.Bunny.harvestBunnyNumber].MovePosition(col.transform.position);
                
                //사운드
                StaticManager.Sound.SetSFX("Seed");
            }
            else
            {
                for (int i = 0; i < GameManager.Instance.harvestUICanvas.transform.childCount; i++)
                {
                    Destroy(GameManager.Instance.harvestUICanvas.transform.GetChild(i).gameObject);
                }
                
                for (int i = 0; i < GameManager.Instance.goldText.transform.childCount; i++)
                    Destroy(GameManager.Instance.goldText.transform.GetChild(i).gameObject);
                
                GameManager.Instance.MakeToast(StaticManager.Langauge.Localize(39));
            }
        }
    }

    public void SetDrag(bool isDrag)
    {
        this.isDrag = isDrag;
    }

    private void DestroyUI()
    {
        if(GameManager.Field.selectHarvestUI.activeSelf)
            Destroy(GameManager.Field.selectHarvestUI);
    }
    private void ButtonAnimation(string state)
    {
        switch (state)
        {
            case "Start":
                transform.DOScale(Vector3.one * 0.55f, 0.1f);
            break;
            
            case "End":
                transform.DOScale(Vector3.one * 0.6f, 0.1f);
            break;
        }
    }
}
