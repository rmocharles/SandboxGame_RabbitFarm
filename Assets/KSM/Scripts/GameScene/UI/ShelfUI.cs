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
        if (StaticManager.Backend.backendGameData.InventoryData.GetItemCount("Egg") > 0)
        {
            bool isShelf = false;
            for (int i = 0; i < 2; i++)
            {
                if (StaticManager.Backend.backendGameData.MartData.Dictionary[i + 13].ItemCode == 20 &&
                    StaticManager.Backend.backendGameData.MartData.Dictionary[i + 13].ItemCount > 0)
                    isShelf = true;
            }

            if (!isShelf)
            {
                GameObject item = StaticManager.UI.OpenUI("Prefabs/GameScene/ItemUI", shelfGroup.transform);
                item.GetComponent<ItemUI>().Initialize(20, shelfNumber);
            }
        }
        
        if (StaticManager.Backend.backendGameData.InventoryData.GetItemCount("Golden_Egg") > 0)
        {
            bool isShelf = false;
            for (int i = 0; i < 2; i++)
            {
                if (StaticManager.Backend.backendGameData.MartData.Dictionary[i + 13].ItemCode == 19 &&
                    StaticManager.Backend.backendGameData.MartData.Dictionary[i + 13].ItemCount > 0)
                    isShelf = true;
            }

            if (!isShelf)
            {
                GameObject item = StaticManager.UI.OpenUI("Prefabs/GameScene/ItemUI", shelfGroup.transform);
                item.GetComponent<ItemUI>().Initialize(21, shelfNumber);
            }
        }
        
        
    }
}