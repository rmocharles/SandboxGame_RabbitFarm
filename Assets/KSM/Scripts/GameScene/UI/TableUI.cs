using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TableUI : MonoBehaviour
{
    private GameObject tableGroup;

    void Awake()
    {
        tableGroup = transform.GetComponentInChildren<HorizontalLayoutGroup>().gameObject;
    }
    public void Initialize(int tableNumber)
    {
        for (int i = 0; i < 9; i++)
        {
            if (StaticManager.Backend.backendGameData.InventoryData.GetHarvestItemCount(i, 0) > 0)
            {
                bool isTable = false;
                //테이블 위에 있는지 검사
                for (int j = 0; j < 9; j++)
                {
                    if (StaticManager.Backend.backendGameData.MartData.Dictionary[j].ItemCode == i && StaticManager.Backend.backendGameData.MartData.Dictionary[j].ItemCount > 0)
                        isTable = true;
                }

                if (!isTable)
                {
                    GameObject item = StaticManager.UI.OpenUI("Prefabs/GameScene/ItemUI", tableGroup.transform);
                    item.GetComponent<ItemUI>().Initialize(i, tableNumber);
                }
            }
            
            //당근, 감자, 토마토
            if (i >= 0 && i <= 5)
            {
                if (StaticManager.Backend.backendGameData.InventoryData.GetHarvestItemCount(i, 1) > 0)
                {
                    bool isTable = false;
                    //테이블 위에 있는지 검사
                    for (int j = 0; j < 9; j++)
                    {
                        if (StaticManager.Backend.backendGameData.MartData.Dictionary[j].ItemCode == i + 9 && StaticManager.Backend.backendGameData.MartData.Dictionary[j].ItemCount > 0)
                            isTable = true;
                    }

                    if (!isTable)
                    {
                        GameObject item = StaticManager.UI.OpenUI("Prefabs/GameScene/ItemUI", tableGroup.transform);
                        item.GetComponent<ItemUI>().Initialize(i + 9, tableNumber);
                    }
                }
            }

            if (i >= 0 && i <= 2)
            {
                if (StaticManager.Backend.backendGameData.InventoryData.GetHarvestItemCount(i, 2) > 0)
                {
                    bool isTable = false;
                    //테이블 위에 있는지 검사
                    for (int j = 0; j < 9; j++)
                    {
                        if (StaticManager.Backend.backendGameData.MartData.Dictionary[j].ItemCode == i + 15 && StaticManager.Backend.backendGameData.MartData.Dictionary[j].ItemCount > 0)
                            isTable = true;
                    }

                    if (!isTable)
                    {
                        GameObject item = StaticManager.UI.OpenUI("Prefabs/GameScene/ItemUI", tableGroup.transform);
                        item.GetComponent<ItemUI>().Initialize(i + 15, tableNumber);
                    }
                }
            }
        }
    }
}
