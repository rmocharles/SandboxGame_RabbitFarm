using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject backgroundObject;
    [SerializeField] private TMP_Text infoText;
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject itemGroup;
    [SerializeField] private GameObject bubblePool;
    [SerializeField] private ScrollRect scrollRect;

    void Start()
    {
        infoText.text = StaticManager.Langauge.Localize(34);
        closeButton.onClick.AddListener(() =>
        {
            backgroundObject.GetComponent<RectTransform>().DOScale(Vector3.zero, 0.1f);
            Invoke("DestroyUI", 0.1f);
        });

        //작물 인벤 추가
        for (int i = 0; i < StaticManager.Backend.backendGameData.InventoryData.harvestItem.GetLength(0); i++)
        {
            for (int j = 0; j < StaticManager.Backend.backendGameData.InventoryData.harvestItem.GetLength(1); j++)
            {
                if (!string.IsNullOrEmpty(StaticManager.Backend.backendGameData.InventoryData.harvestItem[i, j]))
                {
                    if (StaticManager.Backend.backendGameData.InventoryData.Dictionary[StaticManager.Backend.backendGameData.InventoryData.harvestItem[i, j]] > 0)
                    {
                        GameObject harvestItem = Instantiate(Resources.Load<GameObject>("Prefabs/GameScene/InventoryItem"), itemGroup.transform);
                        harvestItem.GetComponent<InventoryItem>().Initialize(StaticManager.Backend.backendGameData.InventoryData.harvestItem[i, j]);
                    }
                }
            }
        }
        
        //우유, 달걀 인벤 추가
        for (int i = 0; i < StaticManager.Backend.backendGameData.InventoryData.animalItem.GetLength(0); i++)
        {
            for (int j = 0; j < StaticManager.Backend.backendGameData.InventoryData.animalItem.GetLength(1); j++)
            {
                if (!string.IsNullOrEmpty(StaticManager.Backend.backendGameData.InventoryData.animalItem[i, j]))
                {
                    if (StaticManager.Backend.backendGameData.InventoryData.Dictionary[StaticManager.Backend.backendGameData.InventoryData.animalItem[i, j]] > 0)
                    {
                        GameObject animalItem = Instantiate(Resources.Load<GameObject>("Prefabs/GameScene/InventoryItem"), itemGroup.transform);
                        animalItem.GetComponent<InventoryItem>().Initialize(StaticManager.Backend.backendGameData.InventoryData.animalItem[i, j]);
                    }
                }
            }
        }
        
        //비료 인벤 추가
        if (StaticManager.Backend.backendGameData.InventoryData.Dictionary[StaticManager.Backend.backendGameData.InventoryData.Fertilizer] > 0)
        {
            GameObject fertilizerItem = Instantiate(Resources.Load<GameObject>("Prefabs/GameScene/InventoryItem"), itemGroup.transform);
            fertilizerItem.GetComponent<InventoryItem>().Initialize(StaticManager.Backend.backendGameData.InventoryData.Fertilizer);
        }
    }

    void Update()
    {
        scrollRect.onValueChanged.AddListener(DestroyBubble);
    }

    private void DestroyBubble(Vector2 pos)
    {
        for (int i = 0; i < bubblePool.transform.childCount; i++)
        {
            Destroy(bubblePool.transform.GetChild(i).gameObject);
        }
    }


    private void DestroyUI()
    {
        Destroy(this.gameObject);
    }
}
