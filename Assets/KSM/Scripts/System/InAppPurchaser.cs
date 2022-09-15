using BackEnd;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class InAppPurchaser : MonoBehaviour, IStoreListener
{

    private static InAppPurchaser instance;

    private static IStoreController storeController;
    private static IExtensionProvider extensionProvider;

    public const string DIAMOND1 = "com.touchtouch.sandboxgame_rabbit.diamond1";
    public const string DIAMOND2 = "com.touchtouch.sandboxgame_rabbit.diamond2";
    public const string DIAMOND3 = "com.touchtouch.sandboxgame_rabbit.diamond3";
    public const string DIAMOND4 = "com.touchtouch.sandboxgame_rabbit.diamond4";
    public const string DIAMOND5 = "com.touchtouch.sandboxgame_rabbit.diamond5";
    public const string DIAMOND6 = "com.touchtouch.sandboxgame_rabbit.diamond6";

    void Awake()
    {
        if (!instance) instance = this;
    }

    public static InAppPurchaser GetInstance()
    {
        if (instance == null) return null;
        return instance;
    }

    void Start()
    {
        InitializePurchasing();
    }

    bool IsInitialized()
    {
        return (storeController != null && extensionProvider != null);
    }

    void InitializePurchasing()
    {
        if (IsInitialized()) return;


        var module = StandardPurchasingModule.Instance();

        ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);

        builder.AddProduct(DIAMOND1, ProductType.Consumable);
        builder.AddProduct(DIAMOND2, ProductType.Consumable);
        builder.AddProduct(DIAMOND3, ProductType.Consumable);
        builder.AddProduct(DIAMOND4, ProductType.Consumable);
        builder.AddProduct(DIAMOND5, ProductType.Consumable);
        builder.AddProduct(DIAMOND6, ProductType.Consumable);


        UnityPurchasing.Initialize(this, builder);
        Debug.Log("##### InitializePurchasing : Initialize");
    }

    void BuyProductID(string productId)
    {
        if (IsInitialized())
        {
            Product product = storeController.products.WithID(productId);

            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                storeController.InitiatePurchase(product);
            }
            else
            {
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }

    public void OnInitialized(IStoreController _sc, IExtensionProvider _ep)
    {
        storeController = _sc;
        extensionProvider = _ep;
    }

    public void OnInitializeFailed(InitializationFailureReason reason)
    {
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        FarmUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? "결제 실패" : "Purchase Fail");
        Debug.LogError("???? ????");
    }

    //=============================================================================================
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        /*
        ???? ?????? ???? ????
        */
        BackendReturnObject validation = Backend.Receipt.IsValidateGooglePurchase(args.purchasedProduct.receipt, "receiptDescription", false);

        // ?????? ?????? ?????? ????
        if (validation.IsSuccess())
        {
            FarmUI.GetInstance().ButtonClick(1);

            if (String.Equals(args.purchasedProduct.definition.id, DIAMOND1, StringComparison.Ordinal))
            {
                BackendServerManager.GetInstance().myInfo.diamond += BackendServerManager.GetInstance().shopSheet[0].compensate_Price;
                BackendServerManager.GetInstance().SaveMyInfo();
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            }
            // Or ... a non-consumable product has been purchased by this user.
            else if (String.Equals(args.purchasedProduct.definition.id, DIAMOND2, StringComparison.Ordinal))
            {
                BackendServerManager.GetInstance().myInfo.diamond += BackendServerManager.GetInstance().shopSheet[1].compensate_Price;
                BackendServerManager.GetInstance().SaveMyInfo();
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            }
            else if (String.Equals(args.purchasedProduct.definition.id, DIAMOND3, StringComparison.Ordinal))
            {
                BackendServerManager.GetInstance().myInfo.diamond += BackendServerManager.GetInstance().shopSheet[2].compensate_Price;
                BackendServerManager.GetInstance().SaveMyInfo();
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            }
            else if (String.Equals(args.purchasedProduct.definition.id, DIAMOND4, StringComparison.Ordinal))
            {
                BackendServerManager.GetInstance().myInfo.diamond += BackendServerManager.GetInstance().shopSheet[3].compensate_Price;
                BackendServerManager.GetInstance().SaveMyInfo();
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            }
            else if (String.Equals(args.purchasedProduct.definition.id, DIAMOND5, StringComparison.Ordinal))
            {
                BackendServerManager.GetInstance().myInfo.diamond += BackendServerManager.GetInstance().shopSheet[4].compensate_Price;
                BackendServerManager.GetInstance().SaveMyInfo();
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            }
            else if (String.Equals(args.purchasedProduct.definition.id, DIAMOND6, StringComparison.Ordinal))
            {
                BackendServerManager.GetInstance().myInfo.diamond += BackendServerManager.GetInstance().shopSheet[5].compensate_Price;
                BackendServerManager.GetInstance().SaveMyInfo();
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            }
        }
        // ?????? ?????? ?????? ???? 
        else
        {
            // Or ... an unknown product has been purchased by this user. Fill in additional products here....
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
        }

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Complete;
    }

    public void BuyItem(int num)
    {
        switch (num)
        {
            case 1:     //?????? 1
                BuyProductID(DIAMOND1);
                break;

            case 2:     //?????? 2
                BuyProductID(DIAMOND2);
                break;

            case 3:     //?????? 3
                BuyProductID(DIAMOND3);
                break;

            case 4:     //?????? 4
                BuyProductID(DIAMOND4);
                break;

            case 5:     //?????? 5
                BuyProductID(DIAMOND5);
                break;

            case 6:     //?????? 6
                BuyProductID(DIAMOND6);
                break;
        }
    }
}
