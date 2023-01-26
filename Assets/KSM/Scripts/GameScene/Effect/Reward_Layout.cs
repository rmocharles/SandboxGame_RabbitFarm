using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Reward_Layout : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text itemCountText;

    [System.Serializable]
    public class ItemList
    {
        public string itemName;
        public int itemCode;
        public Sprite itemSprite;
    }

    public ItemList[] itemLists;

    public void Initialize(int itemCode, int itemCount, float duration = 0)
    {
        for (int i = 0; i < itemLists.Length; i++)
        {
            if (itemLists[i].itemCode == itemCode)
            {
                itemImage.sprite = itemLists[i].itemSprite;
                itemImage.GetComponent<RectTransform>().sizeDelta = new Vector2(itemLists[i].itemSprite.textureRect.width, itemLists[i].itemSprite.textureRect.height);

                if (itemLists[i].itemCode == 18 || itemLists[i].itemCode == 19 || itemLists[i].itemCode == 20 || itemLists[i].itemCode == 21)
                    itemImage.GetComponent<RectTransform>().sizeDelta = new Vector2(256, 256);
            }
        }

        for (int i = 0; i < 2; i++)
        {
            GetComponentsInChildren<DOTweenAnimation>()[i].duration = duration - 0.3f;
            GetComponentsInChildren<DOTweenAnimation>()[i].delay = 0.3f;
        }
        
        itemCountText.text = "x " + itemCount.ToString();
        
        Invoke("DestroyObject", duration);
    }

    private void DestroyObject()
    {
        Destroy(this.gameObject);
    }
}
