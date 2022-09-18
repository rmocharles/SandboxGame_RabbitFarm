using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public partial class FarmUI : MonoBehaviour
{
    [Header("[ Shop Panel ]")]
    public GameObject shopPanel;
    public GameObject shopCategoryObject;

    public GameObject shopListView;
    public GameObject shopListPrefab;

    public Sprite[] purchaseImage;

    public Sprite[] cashImage;
    public Sprite[] materialImage;
    public Sprite[] decoratingImage;
    public Sprite[] costumeImage;
    public Sprite[] petImage;

    public void ActiveShopList()
    {
        DestroySelectPrefab();

        if (inventoryPanel.activeSelf) inventoryPanel.SetActive(false);

        ChangeShopCategory(0);
    }

    //0 : 다이아, 1 : 씨앗, 2 : 가구, 3 : 의류, 4 : 펫
    public void ChangeShopCategory(int num)
    {
        int totalNumber = 0;

        for (int i = 0; i < shopCategoryObject.transform.childCount; i++)
        {
            if (num == i)
            {
                shopCategoryObject.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
                shopCategoryObject.transform.GetChild(i).GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                shopCategoryObject.transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
                shopCategoryObject.transform.GetChild(i).GetChild(1).gameObject.SetActive(false);
            }
        }

        for (int j = 0; j < shopListView.transform.GetChild(0).GetChild(0).childCount; j++)
            Destroy(shopListView.transform.GetChild(0).GetChild(0).GetChild(j).gameObject);

        for(int k = 0; k < BackendServerManager.GetInstance().shopSheet.Count; k++)
        {
            if(BackendServerManager.GetInstance().shopSheet[k].type == num)
            {
                switch (BackendServerManager.GetInstance().shopSheet[k].type)
                {
                    case 0:     //다이아 상점
                        GameObject shopListObject = Instantiate(shopListPrefab, shopListView.transform.GetChild(0).GetChild(0).transform);
                        shopListObject.transform.GetChild(2).GetComponent<Text>().text = PlayerPrefs.GetString("Langauge") == "ko" ? BackendServerManager.GetInstance().shopSheet[k].name : BackendServerManager.GetInstance().shopSheet[k].name_EN;

                        //보너스 표기
                        if (BackendServerManager.GetInstance().shopSheet[k].bonus != 0)
                        {
                            shopListObject.transform.GetChild(3).GetComponent<Text>().text = "+" + BackendServerManager.GetInstance().shopSheet[k].bonus;
                            shopListObject.transform.GetChild(1).gameObject.SetActive(true);
                            shopListObject.transform.GetChild(3).gameObject.SetActive(true);
                        }
                        else
                        {
                            shopListObject.transform.GetChild(1).gameObject.SetActive(false);
                            shopListObject.transform.GetChild(3).gameObject.SetActive(false);
                        }

                        //금액 표기
                        switch (BackendServerManager.GetInstance().shopSheet[k].cash)
                        {
                            case "Money":
                                shopListObject.transform.GetChild(4).GetComponent<Image>().sprite = PlayerPrefs.GetString("Langauge") == "ko" ? purchaseImage[0] : purchaseImage[1];
                                shopListObject.transform.GetChild(4).GetComponentInChildren<Text>().text = PlayerPrefs.GetString("Langauge") == "ko" ? string.Format("{0:#,###}", BackendServerManager.GetInstance().shopSheet[k].price) : BackendServerManager.GetInstance().shopSheet[k].price_EN.ToString();
                                break;
                            case "Diamond":
                                shopListObject.transform.GetChild(4).GetComponent<Image>().sprite = purchaseImage[2];
                                shopListObject.transform.GetChild(4).GetComponentInChildren<Text>().text = PlayerPrefs.GetString("Langauge") == "ko" ? string.Format("{0:#,###}", BackendServerManager.GetInstance().shopSheet[k].price) : BackendServerManager.GetInstance().shopSheet[k].price_EN.ToString();
                                break;
                            case "Gold":
                                shopListObject.transform.GetChild(4).GetComponent<Image>().sprite = purchaseImage[3];
                                shopListObject.transform.GetChild(4).GetComponentInChildren<Text>().text = PlayerPrefs.GetString("Langauge") == "ko" ? string.Format("{0:#,###}", BackendServerManager.GetInstance().shopSheet[k].price) : BackendServerManager.GetInstance().shopSheet[k].price_EN.ToString();
                                break;
                        }

                        shopListObject.transform.GetChild(4).GetComponent<ProductInfo>().myNumber = k;

                        shopListObject.transform.GetChild(0).GetComponent<Image>().sprite = cashImage[totalNumber];

                        shopListObject.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(cashImage[totalNumber].textureRect.width, cashImage[totalNumber].textureRect.height);
                        if(k == 3 || k == 5)
                        {
                            shopListObject.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(0.7f, 0.7f, 0.5f);
                        }
                        else if(k == 4 || k == 8)
                            shopListObject.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 0.5f);
                        else
                            shopListObject.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(0.9f, 0.9f, 0.5f);

                        if (shopListObject.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x < 100)
                            shopListObject.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(1.2f, 1.2f, 1.2f);

                        totalNumber++;
                        break;

                    case 1:     //씨앗 상점
                        GameObject shopListObject2 = Instantiate(shopListPrefab, shopListView.transform.GetChild(0).GetChild(0).transform);
                        shopListObject2.transform.GetChild(2).GetComponent<Text>().text = PlayerPrefs.GetString("Langauge") == "ko" ? BackendServerManager.GetInstance().shopSheet[k].name : BackendServerManager.GetInstance().shopSheet[k].name_EN;

                        //보너스 표기 X
                        shopListObject2.transform.GetChild(1).gameObject.SetActive(false);
                        shopListObject2.transform.GetChild(3).gameObject.SetActive(false);

                        //금액 표기
                        switch (BackendServerManager.GetInstance().shopSheet[k].cash)
                        {
                            case "Money":
                                shopListObject2.transform.GetChild(4).GetComponent<Image>().sprite = PlayerPrefs.GetString("Langauge") == "ko" ? purchaseImage[0] : purchaseImage[1];
                                shopListObject2.transform.GetChild(4).GetComponentInChildren<Text>().text = PlayerPrefs.GetString("Langauge") == "ko" ? string.Format("{0:#,###}", BackendServerManager.GetInstance().shopSheet[k].price) : BackendServerManager.GetInstance().shopSheet[k].price_EN.ToString();
                                break;
                            case "Diamond":
                                shopListObject2.transform.GetChild(4).GetComponent<Image>().sprite = purchaseImage[2];
                                shopListObject2.transform.GetChild(4).GetComponentInChildren<Text>().text = PlayerPrefs.GetString("Langauge") == "ko" ? string.Format("{0:#,###}", BackendServerManager.GetInstance().shopSheet[k].price) : BackendServerManager.GetInstance().shopSheet[k].price_EN.ToString();

                                break;
                            case "Gold":
                                shopListObject2.transform.GetChild(4).GetComponent<Image>().sprite = purchaseImage[3];
                                shopListObject2.transform.GetChild(4).GetComponentInChildren<Text>().text = PlayerPrefs.GetString("Langauge") == "ko" ? string.Format("{0:#,###}", BackendServerManager.GetInstance().shopSheet[k].price) : BackendServerManager.GetInstance().shopSheet[k].price_EN.ToString();
                                break;
                        }

                        shopListObject2.transform.GetChild(4).GetComponent<ProductInfo>().myNumber = k;

                        shopListObject2.transform.GetChild(0).GetComponent<Image>().sprite = materialImage[totalNumber];
                        
                        shopListObject2.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(materialImage[totalNumber].textureRect.width, materialImage[totalNumber].textureRect.height);
                        shopListObject2.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(0.7f, 0.7f, 1f);
                        if (shopListObject2.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x < 100)
                            shopListObject2.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(1.5f, 1.5f, 1.5f);
                        totalNumber++;
                        break;

                    case 2:     //가구 상점
                        break;

                    case 3:     //의류 상점
                        break;

                    case 4:     //펫 상점
                        break;
                }
            }
        }

        if(shopListView.transform.GetChild(1).GetComponent<Scrollbar>())
            shopListView.transform.GetChild(1).GetComponent<Scrollbar>().value = 0;

        shopPanel.SetActive(true);
    }
}
