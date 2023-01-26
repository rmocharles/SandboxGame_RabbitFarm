using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RefrigeratorUI : MonoBehaviour
{
    private GameObject refrigeratorGroup;

    void Awake()
    {
        refrigeratorGroup = transform.GetComponentInChildren<HorizontalLayoutGroup>().gameObject;
    }

    public void Initialize(int refigeratorNumber)
    {
        if (StaticManager.Backend.backendGameData.InventoryData.GetItemCount("Milk") > 0)
        {
            bool isRefrigerator = false;
            for (int i = 0; i < 4; i++)
            {
                if (StaticManager.Backend.backendGameData.MartData.Dictionary[i + 9].ItemCode == 18 &&
                    StaticManager.Backend.backendGameData.MartData.Dictionary[i + 9].ItemCount > 0)
                    isRefrigerator = true;
            }

            if (!isRefrigerator)
            {
                GameObject item = StaticManager.UI.OpenUI("Prefabs/GameScene/ItemUI", refrigeratorGroup.transform);
                item.GetComponent<ItemUI>().Initialize(18, refigeratorNumber);
            }
        }
        
        if (StaticManager.Backend.backendGameData.InventoryData.GetItemCount("Super_Milk") > 0)
        {
            bool isRefrigerator = false;
            for (int i = 0; i < 4; i++)
            {
                if (StaticManager.Backend.backendGameData.MartData.Dictionary[i + 9].ItemCode == 19 &&
                    StaticManager.Backend.backendGameData.MartData.Dictionary[i + 9].ItemCount > 0)
                    isRefrigerator = true;
            }

            if (!isRefrigerator)
            {
                GameObject item = StaticManager.UI.OpenUI("Prefabs/GameScene/ItemUI", refrigeratorGroup.transform);
                item.GetComponent<ItemUI>().Initialize(19, refigeratorNumber);
            }
        }
        
        
    }
}
