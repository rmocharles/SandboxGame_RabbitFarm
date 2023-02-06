using System.Collections;
using System.Collections.Generic;
using DG.DemiLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text itemCountText;
    private GameObject bubblePool;

    private string nowItemName;

    [System.Serializable]
    public class Item
    {
        public string name;
        public string name_ko;
        public string name_en;
        public int code;
        public Sprite itemSprite;
    }

    public Item[] items;
    
    void Start()
    {
        bubblePool = transform.parent.parent.parent.parent.GetChild(3).gameObject;
        GetComponent<Button>().onClick.AddListener(() =>
        {
            for (int i = 0; i < bubblePool.transform.childCount; i++)
            {
                Destroy(bubblePool.transform.GetChild(i).gameObject);
            }
            
            GameObject bubbleObject = StaticManager.UI.OpenUI("Prefabs/GameScene/BubbleUI", transform);
            bubbleObject.transform.SetParent(bubblePool.transform);

            bubbleObject.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, 50);

            bubbleObject.GetComponent<BubbleUI>().Initialize(nowItemName);
            
        });
    }

    public void Initialize(string itemName)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].name == itemName)
            {
                nowItemName = PlayerPrefs.GetInt("LangIndex") == 0 ? items[i].name_ko : items[i].name_en;
                itemImage.sprite = items[i].itemSprite;
                itemImage.GetComponent<RectTransform>().sizeDelta = new Vector2(items[i].itemSprite.textureRect.width, items[i].itemSprite.textureRect.height);
                
                if(items[i].code == 18 || items[i].code == 19 || items[i].code == 20 || items[i].code == 21)
                    itemImage.GetComponent<RectTransform>().sizeDelta = new Vector2(256, 256);
                
                else if(items[i].code == 22)
                    itemImage.GetComponent<RectTransform>().localScale *= 0.9f;
                if (StaticManager.Backend.backendGameData.InventoryData.GetItemCount(itemName) > 999)
                    itemCountText.text = "999+";
                else
                    itemCountText.text = StaticManager.Backend.backendGameData.InventoryData.GetItemCount(itemName).ToString();
            }
        }
    }
}
