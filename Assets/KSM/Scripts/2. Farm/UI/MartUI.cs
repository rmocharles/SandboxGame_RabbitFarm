using System.Collections;
using System.Collections.Generic;
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

    public Sprite[] wantImage;
    public Sprite[] wantTypeImage;

    [Header("[ Table Setting ]")]
    public Transform startPosition;
    public Transform stopPosition;
    public Transform[] exitPosition;
    public Transform[] tablePosition;
    public Transform[] countPosition;

    public GameObject partTimerObject;

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
                    tableCanvas.transform.GetChild(i).GetComponent<Button>().interactable = false;

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
        tableSelectPanel.SetActive(true);

        if (BackendServerManager.GetInstance().TableType[tableNumber] >= 10) return;

        for(int i = 0; i < 18; i++)
        {
            switch (i)
            {
                case 0:

                    break;

                case 1:
                    break;
            }
        }
    }

    //    public void TableClick(int num)
    //    {
    //        if (BackendServerManager.GetInstance().TableType[num] >= 10) return;

    //        for(int i = 0; i < 9; i++)
    //        {
    //            if (BackendServerManager.GetInstance().myInfo.harvest[i] < 1)
    //                tableSelectCanvas.transform.GetChild(num).GetChild(i).GetComponent<Button>().interactable = false;
    //        }

    //        for(int i = 0; i < 9; i++)
    //        {
    //            if (BackendServerManager.GetInstance().TableType[i] >= 10)
    //                tableSelectCanvas.transform.GetChild(num).GetChild(BackendServerManager.GetInstance().TableType[i] - 10).GetComponent<Button>().interactable = false;
    //        }

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
}
