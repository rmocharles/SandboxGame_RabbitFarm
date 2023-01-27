using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShelfUI : MonoBehaviour
{
    private GameObject shelfGroup;

    void Awake()
    {
        shelfGroup = transform.GetComponentInChildren<HorizontalLayoutGroup>().gameObject;
    }

    public void Initialize(int shelfNumber)
    {
        for (int i = 0; i < 2; i++)
        {
            int num = i;
            Debug.LogError($"ItemCode : {num + 20}, ItemCount : {StaticManager.Backend.backendGameData.InventoryData.GetItemCount(num + 20)}");
            GameObject item = StaticManager.UI.OpenUI("Prefabs/GameScene/ItemUI", shelfGroup.transform);
            item.GetComponentInChildren<Button>().interactable = StaticManager.Backend.backendGameData.InventoryData.GetItemCount(num + 20) > 0;
            item.GetComponent<ItemUI>().Initialize(num + 20, shelfNumber);
        }
    }
}