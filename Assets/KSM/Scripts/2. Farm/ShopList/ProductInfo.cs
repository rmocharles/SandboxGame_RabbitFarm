using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;

public class ProductInfo : MonoBehaviour
{
    public int myNumber = 0;

    void Start()
    {
        
    }

    public void PurchaseProduct()
    {
        switch (BackendServerManager.GetInstance().shopSheet[myNumber].cash)
        {
            case "Money":
                switch (myNumber)
                {
                    case 0:     //??????1
                        InAppPurchaser.GetInstance().BuyItem(1);
                        break;

                    case 1:     //??????2
                        InAppPurchaser.GetInstance().BuyItem(2);
                        break;

                    case 2:     //??????3
                        InAppPurchaser.GetInstance().BuyItem(3);
                        break;

                    case 3:     //??????4
                        InAppPurchaser.GetInstance().BuyItem(4);
                        break;

                    case 4:     //??????5
                        InAppPurchaser.GetInstance().BuyItem(5);
                        break;

                    case 5:     //??????6
                        InAppPurchaser.GetInstance().BuyItem(6);
                        break;
                }
                break;

            case "Diamond":
                if (BackendServerManager.GetInstance().myInfo.diamond >= BackendServerManager.GetInstance().shopSheet[myNumber].price)
                {
                    FarmUI.GetInstance().ButtonClick(1);
                    BackendServerManager.GetInstance().myInfo.diamond -= BackendServerManager.GetInstance().shopSheet[myNumber].price;
                    BackendServerManager.GetInstance().SaveMyInfo();
                }
                else
                {
                    FarmUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? "다이아가 부족합니다." : "Not enough diamonds.");
                    return;
                }
                break;

            case "Gold":

                if(BackendServerManager.GetInstance().myInfo.gold >= BackendServerManager.GetInstance().shopSheet[myNumber].price)
                {
                    FarmUI.GetInstance().ButtonClick(1);
                    BackendServerManager.GetInstance().myInfo.gold -= BackendServerManager.GetInstance().shopSheet[myNumber].price;
                    BackendServerManager.GetInstance().SaveMyInfo();
                }
                else
                {
                    FarmUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? "골드가 부족합니다." : "Not enough golds.");
                    return;
                }
                break;
        }

        switch (BackendServerManager.GetInstance().shopSheet[myNumber].compensate_Type)
        {
            case "Gold":
                BackendServerManager.GetInstance().myInfo.gold += BackendServerManager.GetInstance().shopSheet[myNumber].compensate_Price;
                break;
            case "Fertilizer":
                BackendServerManager.GetInstance().myInfo.fertilizer += BackendServerManager.GetInstance().shopSheet[myNumber].compensate_Price;
                break;

            case "Fertilizer2":
                BackendServerManager.GetInstance().myInfo.fertilizer2 += BackendServerManager.GetInstance().shopSheet[myNumber].compensate_Price;
                break;

            case "Carrot":
                BackendServerManager.GetInstance().myInfo.seed_Carrot += BackendServerManager.GetInstance().shopSheet[myNumber].compensate_Price;
                break;

            case "Potato":
                BackendServerManager.GetInstance().myInfo.seed_Potato += BackendServerManager.GetInstance().shopSheet[myNumber].compensate_Price;
                break;

            case "Tomato":
                BackendServerManager.GetInstance().myInfo.seed_Tomato += BackendServerManager.GetInstance().shopSheet[myNumber].compensate_Price;
                break;

            case "Cucumber":
                BackendServerManager.GetInstance().myInfo.seed_Cucumber += BackendServerManager.GetInstance().shopSheet[myNumber].compensate_Price;
                break;

            case "Apple":
                BackendServerManager.GetInstance().myInfo.seed_Apple += BackendServerManager.GetInstance().shopSheet[myNumber].compensate_Price;
                break;

            case "Peach":
                BackendServerManager.GetInstance().myInfo.seed_Peach += BackendServerManager.GetInstance().shopSheet[myNumber].compensate_Price;
                break;

            case "Pumpkin":
                BackendServerManager.GetInstance().myInfo.seed_Pumpkin += BackendServerManager.GetInstance().shopSheet[myNumber].compensate_Price;
                break;

            case "Pear":
                BackendServerManager.GetInstance().myInfo.seed_Pear += BackendServerManager.GetInstance().shopSheet[myNumber].compensate_Price;
                break;

            case "Cherry":
                BackendServerManager.GetInstance().myInfo.seed_Cherry += BackendServerManager.GetInstance().shopSheet[myNumber].compensate_Price;
                break;
        }
    }
}
