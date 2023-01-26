using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text itemCountText;

    [System.Serializable]
    public class ItemList
    {
        public string itemName;
        public int itemCode;
        public Sprite itemSprite;
        public int maxCount;
    }

    public ItemList[] itemLists;

    public void Initialize(int itemCode, int slotNumber)
    {
        for (int i = 0; i < itemLists.Length; i++)
        {
            if (itemLists[i].itemCode == itemCode)
            {
                itemImage.sprite = itemLists[i].itemSprite;
                
                itemImage.GetComponent<RectTransform>().sizeDelta = new Vector2(itemLists[i].itemSprite.textureRect.width, itemLists[i].itemSprite.textureRect.height);
                
                if (itemCode == 18 || itemCode == 19 || itemCode == 20 || itemCode == 21)
                {
                    itemImage.GetComponent<RectTransform>().sizeDelta = new Vector2(256, 256);
                }
            }
        }

        if (StaticManager.Backend.backendGameData.InventoryData.GetItemCount(itemLists[itemCode].itemName) > 99)
            itemCountText.text = "99+";
        else
        {
            itemCountText.text = StaticManager.Backend.backendGameData.InventoryData.GetItemCount(itemLists[itemCode].itemName).ToString();
        }
        
        itemImage.GetComponent<Button>().onClick.AddListener(() =>
        {
            for (int i = 0; i < itemLists.Length; i++)
            {
                if (itemLists[i].itemCode == itemCode)
                {
                    if (StaticManager.Backend.backendGameData.InventoryData.GetItemCount(itemLists[i].itemName) >= itemLists[i].maxCount)
                    {
                        StaticManager.Backend.backendGameData.MartData.SetItem(slotNumber, i, itemLists[i].maxCount);
                        StaticManager.Backend.backendGameData.InventoryData.AddItem(itemLists[i].itemName, -itemLists[i].maxCount);
                    }
                    else
                    {
                        StaticManager.Backend.backendGameData.MartData.SetItem(slotNumber, i, StaticManager.Backend.backendGameData.InventoryData.GetItemCount(itemLists[i].itemName));
                        StaticManager.Backend.backendGameData.InventoryData.AddItem(itemLists[i].itemName, -StaticManager.Backend.backendGameData.InventoryData.GetItemCount(itemLists[i].itemName));
                    }
                    
                    Destroy(GameManager.Mart.slotUI);
                    GameManager.Mart.InitializeData(slotNumber);
                    GameManager.Instance.SaveAllData();
                }
            }
        });
    }
}
