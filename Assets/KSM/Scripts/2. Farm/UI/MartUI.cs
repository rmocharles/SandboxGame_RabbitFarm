using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class FarmUI : MonoBehaviour
{
    [Header("[ Table Select Canvas ]")]
    public GameObject tableCanvas;
    public GameObject tableSelectPanel;


    [System.Serializable]
    public class TableHarvestImage
    {
        public string name;
        public Sprite[] sprites;
    }
    public TableHarvestImage[] tableHarvestImage;

    public GameObject[] tableObject;
    public Sprite[] wantImage;
    public Sprite[] wantTypeImage;

    [Header("[ Table Setting ]")]
    public Transform startPosition;
    public Transform stopPosition;
    public Transform[] exitPosition;
    public Transform[] tablePosition;
    public Transform[] countPosition;

    public GameObject partTimerObject;
    public GameObject familyPartTimerObject;

    public GameObject successPrefab;

    private int selectTableNumber = 0;

    public void InitializeTable()
    {
        for(int i = 0; i < 9; i++)
        {
            if(BackendServerManager.GetInstance().TableType[i] == 0)
            {
                tableCanvas.transform.GetChild(i).GetComponent<Button>().interactable = true;
                tableCanvas.transform.GetChild(i).GetChild(1).gameObject.SetActive(false);
                tableCanvas.transform.GetChild(i).GetChild(2).gameObject.SetActive(true);
            }
            else
            {
                if(BackendServerManager.GetInstance().TableCount[i] > 0)
                {
                    //tableCanvas.transform.GetChild(i).GetComponent<Button>().interactable = false;

                    tableCanvas.transform.GetChild(i).GetChild(1).gameObject.SetActive(true);
                    tableCanvas.transform.GetChild(i).GetChild(2).gameObject.SetActive(false);

                    tableCanvas.transform.GetChild(i).GetChild(1).GetComponent<Image>().sprite = tableHarvestImage[BackendServerManager.GetInstance().TableType[i] - 10].sprites[BackendServerManager.GetInstance().TableCount[i] - 1];
                    switch(BackendServerManager.GetInstance().TableType[i] - 10)
                    {
                        case int n when (0 <= n && n <= 2 || n == 8):
                            tableCanvas.transform.GetChild(i).GetChild(1).GetComponent<RectTransform>().localScale = new Vector3(1.1f, 1.1f, 1.1f);
                            break;
                    }
                }
                else
                {
                    BackendServerManager.GetInstance().TableType[i] = 1;
                    tableCanvas.transform.GetChild(i).GetComponent<Button>().interactable = true;
                    tableCanvas.transform.GetChild(i).GetChild(1).gameObject.SetActive(false);
                    tableCanvas.transform.GetChild(i).GetChild(2).gameObject.SetActive(false);
                }
            }
        }
    }

    public void DisplayTable(int tableNumber)
    {
        //if (BackendServerManager.GetInstance().TableType[tableNumber] >= 10) return;

        selectTableNumber = tableNumber;

        tableSelectPanel.SetActive(true);

        //당근
        SetItem(0, 0);
        SetItem(1, 9);
        SetItem(2, 10);

        //감자
        SetItem(3, 1);
        SetItem(4, 11);
        SetItem(5, 12);

        //토마토
        SetItem(6, 2);
        SetItem(7, 13);
        SetItem(8, 14);

        //오이
        SetItem(9, 3);
        SetItem(10, 15);

        //복숭아
        SetItem(11, 4);
        SetItem(12, 16);

        //사과
        SetItem(13, 5);
        SetItem(14, 17);

        //호박
        SetItem(15, 6);

        //배
        SetItem(16, 7);

        //체리
        SetItem(17, 8);

    }

    //===================================
    private void SetItem(int order, int num)
    {
        tableSelectPanel.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(order).GetComponent<Button>().interactable = BackendServerManager.GetInstance().myInfo.harvest[num] > 0;

        for (int i = 0; i < 9; i++)
        {
            if (BackendServerManager.GetInstance().TableType[i] == num + 10)
                tableSelectPanel.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(order).GetComponent<Button>().interactable = false;
        }

        tableSelectPanel.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(order).GetComponentInChildren<TextMeshProUGUI>().text = BackendServerManager.GetInstance().myInfo.harvest[num] > BackendServerManager.GetInstance().martSheet[num].count ? BackendServerManager.GetInstance().martSheet[num].count + "+" : BackendServerManager.GetInstance().myInfo.harvest[num].ToString();
        tableSelectPanel.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(order).GetComponentInChildren<TextMeshProUGUI>().color = BackendServerManager.GetInstance().myInfo.harvest[num] == 0 ? new Color(1, 0, 0) : new Color(0, 0, 0);
    }

    //    public void TableClick(int num)
    //    {
    //        if (BackendServerManager.GetInstance().TableType[num] >= 10) return;

    //        for(int i = 0; i < 9; i++)
    //        {
    //            if (BackendServerManager.GetInstance().myInfo.harvest[i] < 1)
    //                tableSelectCanvas.transform.GetChild(num).GetChild(i).GetComponent<Button>().interactable = false;
    //        }

    //for(int i = 0; i< 9; i++)
    //{
    //    if (BackendServerManager.GetInstance().TableType[i] >= 10)
    //        tableSelectCanvas.transform.GetChild(num).GetChild(BackendServerManager.GetInstance().TableType[i] - 10).GetComponent<Button>().interactable = false;
    //}

    //        for (int i = 0; i < tableSelectCanvas.transform.childCount; i++)
    //        {
    //            if(i == num)
    //            {
    //                if(tableSelectCanvas.transform.GetChild(num).gameObject.activeSelf)
    //                    tableSelectCanvas.transform.GetChild(num).gameObject.SetActive(false);
    //                else
    //                    tableSelectCanvas.transform.GetChild(num).gameObject.SetActive(true);
    //            }

    //            else
    //                tableSelectCanvas.transform.GetChild(i).gameObject.SetActive(false);
    //        }

    //    }

    //    public void PutOnHarvest(int num)
    //    {
    //        int tableNumber = 0;
    //        int remainNumber = 5;

    //        for(int i = 0; i < 9; i++)
    //        {
    //            if (tableSelectCanvas.transform.GetChild(i).gameObject.activeSelf) tableNumber = i;

    //            tableSelectCanvas.transform.GetChild(i).gameObject.SetActive(false);
    //        }

    //        if(BackendServerManager.GetInstance().myInfo.harvest[num - 10] >= BackendServerManager.GetInstance().martSheet[num - 10].count)
    //        {
    //            remainNumber = BackendServerManager.GetInstance().martSheet[num - 10].count;
    //            BackendServerManager.GetInstance().myInfo.harvest[num - 10] -= BackendServerManager.GetInstance().martSheet[num - 10].count;
    //        }
    //        else
    //        {
    //            remainNumber = BackendServerManager.GetInstance().myInfo.harvest[num - 10];
    //            BackendServerManager.GetInstance().myInfo.harvest[num - 10] = 0;
    //        }

    //        BackendServerManager.GetInstance().TableType[tableNumber] = num;
    //        BackendServerManager.GetInstance().TableCount[tableNumber] = remainNumber;

    //        BackendServerManager.GetInstance().SaveMyInfo();
    //    }

    public void PutOnHarvest(int num)
    {
        tableSelectPanel.SetActive(false);

        if (BackendServerManager.GetInstance().TableCount[selectTableNumber] > 0)
        {
            int selectTableType = 0, selectTableCount = 0;

            selectTableType = BackendServerManager.GetInstance().TableType[selectTableNumber];
            selectTableCount = BackendServerManager.GetInstance().TableCount[selectTableNumber];


            BackendServerManager.GetInstance().myInfo.harvest[selectTableType - 10] += selectTableCount;
        }

        int remainNumber = 0;
        if (BackendServerManager.GetInstance().myInfo.harvest[num - 10] >= BackendServerManager.GetInstance().martSheet[num - 10].count)
        {
            remainNumber = BackendServerManager.GetInstance().martSheet[num - 10].count;
            BackendServerManager.GetInstance().myInfo.harvest[num - 10] -= BackendServerManager.GetInstance().martSheet[num - 10].count;
        }
        else
        {
            remainNumber = BackendServerManager.GetInstance().myInfo.harvest[num - 10];
            BackendServerManager.GetInstance().myInfo.harvest[num - 10] = 0;
        }

        BackendServerManager.GetInstance().TableType[selectTableNumber] = num;
        BackendServerManager.GetInstance().TableCount[selectTableNumber] = remainNumber;

        BackendServerManager.GetInstance().SaveMyInfo();
    }
}
