using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MartManager : MonoBehaviour
{
    private GuestManager guest;

    public Button[] slotButtons;

    [System.Serializable]
    public class SlotImage
    {
        public string name;
        public Sprite[] sprites;
    }

    public SlotImage[] slotImages;

    [HideInInspector]
    public GameObject slotUI;

    public GuestManager Guest
    {
        get
        {
            return guest;
        }
    }

    public void Initialize()
    {
        guest = GetComponentInChildren<GuestManager>();

        Guest.Initialize();

        for (int i = 0; i < 15; i++)
        {
            int num = i;
            
            slotButtons[num].onClick.AddListener(() =>
            {
                if (!StaticManager.Backend.backendGameData.MartData.Dictionary[num].IsOpen)
                {
                    GameObject unLockUI = StaticManager.UI.OpenUI("Prefabs/GameScene/UnlockUI", GameManager.Instance.UICanvas.transform);
                    //테이블일 경우
                    if (num >= 0 && num < 9)
                    {
                        int count = 0;
                        //0 ~ 8
                        for (int j = 0; j < 9; j++)
                        {
                            if (StaticManager.Backend.backendGameData.MartData.Dictionary[j].IsOpen)
                                count++;
                        }
                        unLockUI.GetComponent<UnlockUI>().Initialize(num, StaticManager.Backend.backendChart.Price.GetPrice("Table_" + count), UnlockUI.Type.Table);
                    }
                    else if (num >= 9 && num < 13)
                    {
                        int count = 1;
                        //0 ~ 8
                        for (int j = 0; j < 4; j++)
                        {
                            if (StaticManager.Backend.backendGameData.MartData.Dictionary[j + 9].IsOpen)
                                count++;
                        }
                        unLockUI.GetComponent<UnlockUI>().Initialize(num, StaticManager.Backend.backendChart.Price.GetPrice("Refrigerator_" + count), UnlockUI.Type.Refrigerator);
                    }
                    else
                    {
                        int count = 1;
                        //0 ~ 8
                        for (int j = 0; j < 2; j++)
                        {
                            if (StaticManager.Backend.backendGameData.MartData.Dictionary[j + 13].IsOpen)
                                count++;
                        }
                        unLockUI.GetComponent<UnlockUI>().Initialize(num, StaticManager.Backend.backendChart.Price.GetPrice("Shelf_" + count), UnlockUI.Type.Shelf);
                    }
                }
                else
                {
                    StaticManager.Sound.SetSFX();
                    if (slotUI != null)
                        Destroy(slotUI);
                
                    slotUI = StaticManager.UI.OpenUI("Prefabs/GameScene/MartUI", GameManager.Instance.harvestUICanvas.transform);

                    if (num < 9)
                    {
                        slotUI.AddComponent<TableUI>().Initialize(num);
                    }
                    //우유
                    else if(num is >= 9 and <= 12)
                    {
                        slotUI.AddComponent<RefrigeratorUI>().Initialize(num);
                    }
                    //계란
                    else if (num is > 12 and <= 14)
                    {
                        slotUI.AddComponent<ShelfUI>().Initialize(num);
                    }
                }
            });

            InitializeData(i);
        }
    }

    void Update()
    {
        // GameManager.Field.HideMainUI(slotUI != null);
    }

    public void InitializeData(int slotNumber)
    {
        if (!StaticManager.Backend.backendGameData.MartData.Dictionary[slotNumber].IsOpen)
        {
            //slotButtons[slotNumber].interactable = false;
            slotButtons[slotNumber].transform.GetChild(0).gameObject.SetActive(false);    //물품 비활성화
            slotButtons[slotNumber].transform.GetChild(1).gameObject.SetActive(true);     //락 활성화
        }
        else
        {
            //slotButtons[slotNumber].interactable = false;
            slotButtons[slotNumber].transform.GetChild(0).gameObject.SetActive(false);
            slotButtons[slotNumber].transform.GetChild(1).gameObject.SetActive(false);

            if (StaticManager.Backend.backendGameData.MartData.Dictionary[slotNumber].ItemCount > 0)
            {
                //slotButtons[slotNumber].interactable = true;
                slotButtons[slotNumber].transform.GetChild(0).gameObject.SetActive(true);
                slotButtons[slotNumber].transform.GetChild(0).GetComponent<Image>().sprite =
                    slotImages[StaticManager.Backend.backendGameData.MartData.Dictionary[slotNumber].ItemCode]
                        .sprites[StaticManager.Backend.backendGameData.MartData.Dictionary[slotNumber].ItemCount - 1];
            }
            else if (StaticManager.Backend.backendGameData.MartData.Dictionary[slotNumber].ItemCount == 0)
            {
                //slotButtons[slotNumber].interactable = true;
            }
        }
    }
}
