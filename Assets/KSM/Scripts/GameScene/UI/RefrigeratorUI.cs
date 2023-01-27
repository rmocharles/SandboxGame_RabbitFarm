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

    public void Initialize(int refrigeratorNumber)
    {
        for (int i = 0; i < 2; i++)
        {
            int num = i;
            GameObject item = StaticManager.UI.OpenUI("Prefabs/GameScene/ItemUI", refrigeratorGroup.transform);
            Debug.LogError(StaticManager.Backend.backendGameData.InventoryData.GetItemCount(num + 18));
            item.GetComponentInChildren<Button>().interactable = StaticManager.Backend.backendGameData.InventoryData.GetItemCount(num + 18) > 0;
            item.GetComponent<ItemUI>().Initialize(num + 18, refrigeratorNumber);
        }
    }
}
