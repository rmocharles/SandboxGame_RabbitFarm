using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class TableUI : MonoBehaviour
{
    private GameObject tableGroup;

    void Awake()
    {
        tableGroup = transform.GetComponentInChildren<HorizontalLayoutGroup>().gameObject;
    }

    private List<int> index = new List<int>();
    public void Initialize(int tableNumber)
    {
        for (int i = 0; i < 9; i++)
        {
            if (i >= 0 && i <= 8)
            {
                if (StaticManager.Backend.backendGameData.InventoryData.GetHarvestItemCount(i, 0) > 0)
                {
                    GameObject item = StaticManager.UI.OpenUI("Prefabs/GameScene/ItemUI", tableGroup.transform);
                    item.GetComponentInChildren<Button>().interactable = StaticManager.Backend.backendGameData.InventoryData.GetHarvestItemCount(i, 0) > 0;
                    item.GetComponent<ItemUI>().Initialize(i, tableNumber);
                }
                else
                {
                    index.Add(i);
                }
            }

            if (i >= 0 && i <= 5)
            {
                if (StaticManager.Backend.backendGameData.InventoryData.GetHarvestItemCount(i, 1) > 0)
                {
                    Debug.LogError(i + 9);
                    GameObject item = StaticManager.UI.OpenUI("Prefabs/GameScene/ItemUI", tableGroup.transform);
                    item.GetComponentInChildren<Button>().interactable = StaticManager.Backend.backendGameData.InventoryData.GetHarvestItemCount(i, 1) > 0;
                    item.GetComponent<ItemUI>().Initialize(i + 9, tableNumber);
                }
                else
                {
                    index.Add(i + 9);
                }
            }
            
            if (i >= 0 && i <= 2)
            {
                if (StaticManager.Backend.backendGameData.InventoryData.GetHarvestItemCount(i, 2) > 0)
                {
                    GameObject item = StaticManager.UI.OpenUI("Prefabs/GameScene/ItemUI", tableGroup.transform);
                    item.GetComponentInChildren<Button>().interactable = StaticManager.Backend.backendGameData.InventoryData.GetHarvestItemCount(i, 2) > 0;
                    item.GetComponent<ItemUI>().Initialize(i + 15, tableNumber);
                }
                else
                {
                    index.Add(i + 15);
                }
            }
            
        }

        for (int i = 0; i < index.Count; i++)
        {
            GameObject item = StaticManager.UI.OpenUI("Prefabs/GameScene/ItemUI", tableGroup.transform);
            item.GetComponentInChildren<Button>().interactable = false;
            item.GetComponent<ItemUI>().Initialize(index[i], tableNumber);
        }
    }
}
