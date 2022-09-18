using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class FarmUI : MonoBehaviour
{
    [Header("[ Inventory Panel ]")]
    public GameObject inventoryPanel;
    public GameObject inventoryCategoryObject;

    public GameObject inventoryListView;
    public GameObject inventoryListPrefab;

    public Sprite[] inventoryHarvestImage;
    public Sprite[] inventorySeedImage;

    public void ActiveInventoryList()
    {
        inventoryPanel.SetActive(true);
        ChangeInventoryCategory(0);
    }

    //0 : 수확물, 1 : 씨앗, 2 : 가구, 3 : 의류, 4 : 펫
    public void ChangeInventoryCategory(int num)
    {
        DestroySelectPrefab();

        for (int i = 0; i < inventoryCategoryObject.transform.childCount; i++)
        {
            if (num == i)
            {
                inventoryCategoryObject.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
                inventoryCategoryObject.transform.GetChild(i).GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                inventoryCategoryObject.transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
                inventoryCategoryObject.transform.GetChild(i).GetChild(1).gameObject.SetActive(false);
            }
        }

        for (int j = 0; j < inventoryListView.transform.GetChild(0).GetChild(0).childCount; j++)
            Destroy(inventoryListView.transform.GetChild(0).GetChild(0).GetChild(j).gameObject);

        switch (num)
        {
            case 0:     //수확물
                for(int i = 0; i < 9; i++)
                {
                    if(BackendServerManager.GetInstance().myInfo.harvest[i] > 0)
                    {
                        GameObject inventoryListObject = Instantiate(inventoryListPrefab, inventoryListView.transform.GetChild(0).GetChild(0).transform);

                        inventoryListObject.transform.GetChild(0).GetComponent<Image>().sprite = inventoryHarvestImage[i];

                        inventoryListObject.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(inventoryHarvestImage[i].textureRect.width, inventoryHarvestImage[i].textureRect.height);
                        inventoryListObject.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(0.75f, 0.75f, 0.75f);

                        inventoryListObject.transform.GetChild(1).GetComponent<Text>().text = PlayerPrefs.GetString("Langauge") == "ko" ? BackendServerManager.GetInstance().langaugeSheet[7 + i].ko : BackendServerManager.GetInstance().langaugeSheet[7 + i].en;
                        inventoryListObject.transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = BackendServerManager.GetInstance().myInfo.harvest[i].ToString();

                        switch (i)
                        {
                            case 0: //당근

                                for(int j = 0; j < 2; j++)
                                {
                                    if(BackendServerManager.GetInstance().myInfo.harvest[j + 9] > 0)
                                    {
                                        GameObject inventoryListObject2 = Instantiate(inventoryListPrefab, inventoryListView.transform.GetChild(0).GetChild(0).transform);

                                        inventoryListObject2.transform.GetChild(0).GetComponent<Image>().sprite = inventoryHarvestImage[j + 9];

                                        inventoryListObject2.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(inventoryHarvestImage[j + 9].textureRect.width, inventoryHarvestImage[j + 9].textureRect.height);
                                        inventoryListObject2.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(0.75f, 0.75f, 0.75f);

                                        inventoryListObject2.transform.GetChild(1).GetComponent<Text>().text = PlayerPrefs.GetString("Langauge") == "ko" ? BackendServerManager.GetInstance().langaugeSheet[j + 16].ko : BackendServerManager.GetInstance().langaugeSheet[j + 16].en;
                                        inventoryListObject2.transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = BackendServerManager.GetInstance().myInfo.harvest[j + 9].ToString();
                                    }
                                }
                                break;

                            case 1: //감자
                                for (int j = 0; j < 2; j++)
                                {
                                    if (BackendServerManager.GetInstance().myInfo.harvest[j + 11] > 0)
                                    {
                                        GameObject inventoryListObject2 = Instantiate(inventoryListPrefab, inventoryListView.transform.GetChild(0).GetChild(0).transform);

                                        inventoryListObject2.transform.GetChild(0).GetComponent<Image>().sprite = inventoryHarvestImage[j + 11];

                                        inventoryListObject2.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(inventoryHarvestImage[j + 11].textureRect.width, inventoryHarvestImage[j + 11].textureRect.height);
                                        inventoryListObject2.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(0.75f, 0.75f, 0.75f);

                                        inventoryListObject2.transform.GetChild(1).GetComponent<Text>().text = PlayerPrefs.GetString("Langauge") == "ko" ? BackendServerManager.GetInstance().langaugeSheet[j + 18].ko : BackendServerManager.GetInstance().langaugeSheet[j + 18].en;
                                        inventoryListObject2.transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = BackendServerManager.GetInstance().myInfo.harvest[j + 11].ToString();
                                    }
                                }
                                break;

                            case 2: //토마토
                                for (int j = 0; j < 2; j++)
                                {
                                    if (BackendServerManager.GetInstance().myInfo.harvest[j + 13] > 0)
                                    {
                                        GameObject inventoryListObject2 = Instantiate(inventoryListPrefab, inventoryListView.transform.GetChild(0).GetChild(0).transform);

                                        inventoryListObject2.transform.GetChild(0).GetComponent<Image>().sprite = inventoryHarvestImage[j + 13];

                                        inventoryListObject2.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(inventoryHarvestImage[j + 13].textureRect.width, inventoryHarvestImage[j + 13].textureRect.height);
                                        inventoryListObject2.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(0.75f, 0.75f, 0.75f);

                                        inventoryListObject2.transform.GetChild(1).GetComponent<Text>().text = PlayerPrefs.GetString("Langauge") == "ko" ? BackendServerManager.GetInstance().langaugeSheet[j + 20].ko : BackendServerManager.GetInstance().langaugeSheet[j + 20].en;
                                        inventoryListObject2.transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = BackendServerManager.GetInstance().myInfo.harvest[j + 13].ToString();
                                    }
                                }
                                break;

                            case 3: //오이
                                if (BackendServerManager.GetInstance().myInfo.harvest[15] > 0)
                                {
                                    GameObject inventoryListObject2 = Instantiate(inventoryListPrefab, inventoryListView.transform.GetChild(0).GetChild(0).transform);

                                    inventoryListObject2.transform.GetChild(0).GetComponent<Image>().sprite = inventoryHarvestImage[15];

                                    inventoryListObject2.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(inventoryHarvestImage[15].textureRect.width, inventoryHarvestImage[15].textureRect.height);
                                    inventoryListObject2.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(0.75f, 0.75f, 0.75f);

                                    inventoryListObject2.transform.GetChild(1).GetComponent<Text>().text = PlayerPrefs.GetString("Langauge") == "ko" ? BackendServerManager.GetInstance().langaugeSheet[22].ko : BackendServerManager.GetInstance().langaugeSheet[22].en;
                                    inventoryListObject2.transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = BackendServerManager.GetInstance().myInfo.harvest[15].ToString();
                                }
                                break;

                            case 4: //복숭아
                                if (BackendServerManager.GetInstance().myInfo.harvest[16] > 0)
                                {
                                    GameObject inventoryListObject2 = Instantiate(inventoryListPrefab, inventoryListView.transform.GetChild(0).GetChild(0).transform);

                                    inventoryListObject2.transform.GetChild(0).GetComponent<Image>().sprite = inventoryHarvestImage[16];

                                    inventoryListObject2.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(inventoryHarvestImage[16].textureRect.width, inventoryHarvestImage[16].textureRect.height);
                                    inventoryListObject2.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(0.75f, 0.75f, 0.75f);

                                    inventoryListObject2.transform.GetChild(1).GetComponent<Text>().text = PlayerPrefs.GetString("Langauge") == "ko" ? BackendServerManager.GetInstance().langaugeSheet[23].ko : BackendServerManager.GetInstance().langaugeSheet[23].en;
                                    inventoryListObject2.transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = BackendServerManager.GetInstance().myInfo.harvest[16].ToString();
                                }
                                break;

                            case 5: //사과
                                if (BackendServerManager.GetInstance().myInfo.harvest[17] > 0)
                                {
                                    GameObject inventoryListObject2 = Instantiate(inventoryListPrefab, inventoryListView.transform.GetChild(0).GetChild(0).transform);

                                    inventoryListObject2.transform.GetChild(0).GetComponent<Image>().sprite = inventoryHarvestImage[17];

                                    inventoryListObject2.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(inventoryHarvestImage[17].textureRect.width, inventoryHarvestImage[17].textureRect.height);
                                    inventoryListObject2.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(0.75f, 0.75f, 0.75f);

                                    inventoryListObject2.transform.GetChild(1).GetComponent<Text>().text = PlayerPrefs.GetString("Langauge") == "ko" ? BackendServerManager.GetInstance().langaugeSheet[24].ko : BackendServerManager.GetInstance().langaugeSheet[24].en;
                                    inventoryListObject2.transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = BackendServerManager.GetInstance().myInfo.harvest[17].ToString();
                                }
                                break;
                        }
                    }
                    
                }
                
                break;

            case 1:     //씨앗

                if (BackendServerManager.GetInstance().myInfo.fertilizer > 0)
                {
                    GameObject inventoryListObject = Instantiate(inventoryListPrefab, inventoryListView.transform.GetChild(0).GetChild(0).transform);
                    inventoryListObject.transform.GetChild(0).GetComponent<Image>().sprite = inventorySeedImage[9];
                    inventoryListObject.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(inventorySeedImage[9].textureRect.width, inventorySeedImage[9].textureRect.height);
                    inventoryListObject.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    if (inventoryListObject.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x < 100)
                        inventoryListObject.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(1.5f, 1.5f, 1.5f);

                    inventoryListObject.transform.GetChild(1).GetComponent<Text>().text = PlayerPrefs.GetString("Langauge") == "ko" ? BackendServerManager.GetInstance().langaugeSheet[34].ko : BackendServerManager.GetInstance().langaugeSheet[34].en;
                    inventoryListObject.transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = BackendServerManager.GetInstance().myInfo.fertilizer.ToString();
                }

                if (BackendServerManager.GetInstance().myInfo.fertilizer2 > 0)
                {
                    GameObject inventoryListObject = Instantiate(inventoryListPrefab, inventoryListView.transform.GetChild(0).GetChild(0).transform);
                    inventoryListObject.transform.GetChild(0).GetComponent<Image>().sprite = inventorySeedImage[10];
                    inventoryListObject.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(inventorySeedImage[10].textureRect.width, inventorySeedImage[10].textureRect.height);
                    inventoryListObject.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    if (inventoryListObject.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x < 100)
                        inventoryListObject.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(1.5f, 1.5f, 1.5f);

                    inventoryListObject.transform.GetChild(1).GetComponent<Text>().text = PlayerPrefs.GetString("Langauge") == "ko" ? BackendServerManager.GetInstance().langaugeSheet[35].ko : BackendServerManager.GetInstance().langaugeSheet[35].en;
                    inventoryListObject.transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = BackendServerManager.GetInstance().myInfo.fertilizer2.ToString();
                }

                for (int i = 0; i < 9; i++)
                {
                    if(BackendServerManager.GetInstance().myInfo.seed[i] > 0)
                    {
                        GameObject inventoryListObject = Instantiate(inventoryListPrefab, inventoryListView.transform.GetChild(0).GetChild(0).transform);
                        inventoryListObject.transform.GetChild(0).GetComponent<Image>().sprite = inventorySeedImage[i];
                        inventoryListObject.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(inventorySeedImage[i].textureRect.width, inventorySeedImage[i].textureRect.height);
                        inventoryListObject.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 0.5f);
                        if (inventoryListObject.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x < 100)
                            inventoryListObject.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(1.5f, 1.5f, 1.5f);

                        inventoryListObject.transform.GetChild(1).GetComponent<Text>().text = PlayerPrefs.GetString("Langauge") == "ko" ? BackendServerManager.GetInstance().langaugeSheet[25 + i].ko : BackendServerManager.GetInstance().langaugeSheet[25 + i].en;
                        inventoryListObject.transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = BackendServerManager.GetInstance().myInfo.seed[i].ToString();
                    }
                }
                break;

            case 2:     //가구
                break;

            case 3:     //의류
                break;

            case 4:     //펫
                break;
        }

        if (inventoryListView.transform.GetChild(1).GetComponent<Scrollbar>())
            inventoryListView.transform.GetChild(1).GetComponent<Scrollbar>().value = 0;
    }
}
