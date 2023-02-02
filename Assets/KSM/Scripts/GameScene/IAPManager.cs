using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.SceneManagement;

public class IAPManager : MonoBehaviour, IStoreListener
{
    private IStoreController storeController; 
    private IExtensionProvider storeExtensionProvider; 
    
    private const string DIA_1 = "rabbit_dia1";
    private const string DIA_2 = "rabbit_dia2";
    private const string DIA_3 = "rabbit_dia3";
    private const string DIA_4 = "rabbit_dia4";
    private const string EMPLOYEE = "employee";
    private const string PACKAGE_1 = "beginner_package";
    private const string PACKAGE_2 = "newyear_package";

    void Awake()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(DIA_1, ProductType.Consumable, new IDs{{DIA_1, GooglePlay.Name}});
        builder.AddProduct(DIA_2, ProductType.Consumable);
        builder.AddProduct(DIA_3, ProductType.Consumable);
        builder.AddProduct(DIA_4, ProductType.Consumable);
        builder.AddProduct(EMPLOYEE, ProductType.Consumable);
        builder.AddProduct(PACKAGE_1, ProductType.Consumable);
        builder.AddProduct(PACKAGE_2, ProductType.Consumable);


        UnityPurchasing.Initialize(this, builder);
        Debug.LogError("##### InitializePurchasing : Initialize");
        
        Debug.LogError($"초기화 : {IsInitialized()}");
    }

    bool IsInitialized()
    {
        return (storeController != null && storeExtensionProvider != null);
    }
    
    public void BuyProductID(string productId)
    {
        StaticManager.UI.SetLoading(true);
        if (IsInitialized())
        {
            Product product = storeController.products.WithID(productId);

            if (product != null && product.availableToPurchase)
            {
                Debug.LogError(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));

                storeController.InitiatePurchase(product);
            }
            else
            {
                StaticManager.UI.SetLoading(false);
                Debug.LogError($"구매 시도 불가 - {productId}");
            }
        }
        else
        {
            StaticManager.UI.SetLoading(false);
            Debug.LogError("BuyProductID FAIL. Not initialized.");
        }
    }

    public void OnInitialized(IStoreController _sc, IExtensionProvider _ep)
    {
        storeController = _sc;
        storeExtensionProvider = _ep;
        
        Debug.LogError("유니티 IAP 초기화 성공");
    }

    public void OnInitializeFailed(InitializationFailureReason reason)
    {
        Debug.LogError($"유니티 IAP 초기화 실패 {reason}");
    }

    // ====================================================================================================
    

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        /*
        뒤끝 영수증 검증 처리
        */
        Debug.LogError("Receipt");
        StaticManager.UI.SetLoading(false);

        Debug.Log(args.purchasedProduct.availableToPurchase);
        // 뒤끝 영수증 검증 처리
        BackendReturnObject validation = null;
#if UNITY_ANDROID || UNITY_EDITOR
        validation = Backend.Receipt.IsValidateGooglePurchase(args.purchasedProduct.receipt, "receiptDescriptionGoogle");
#elif UNITY_IOS
        validation = Backend.Receipt.IsValidateApplePurchase(args.purchasedProduct.receipt, "receiptDescriptionApple");
#endif

        if (validation.IsSuccess())
        {
            if (String.Equals(args.purchasedProduct.definition.id, DIA_1, StringComparison.Ordinal))
            {
                StaticManager.Backend.backendGameData.UserData.AddDiamond(100);
                StaticManager.UI.AlertUI.OpenUI(StaticManager.Langauge.Localize(101));
                GameManager.Instance.SaveAllData();
            }
            if (String.Equals(args.purchasedProduct.definition.id, DIA_2, StringComparison.Ordinal))
            {
                StaticManager.Backend.backendGameData.UserData.AddDiamond(450);
                StaticManager.UI.AlertUI.OpenUI(StaticManager.Langauge.Localize(102));
                GameManager.Instance.SaveAllData();
            }
            if (String.Equals(args.purchasedProduct.definition.id, DIA_3, StringComparison.Ordinal))
            {
                StaticManager.Backend.backendGameData.UserData.AddDiamond(850);
                StaticManager.UI.AlertUI.OpenUI(StaticManager.Langauge.Localize(103));
                GameManager.Instance.SaveAllData();
            }
            if (String.Equals(args.purchasedProduct.definition.id, DIA_4, StringComparison.Ordinal))
            {
                StaticManager.Backend.backendGameData.UserData.AddDiamond(1800);
                StaticManager.UI.AlertUI.OpenUI(StaticManager.Langauge.Localize(104));
                GameManager.Instance.SaveAllData();
            }
            if (String.Equals(args.purchasedProduct.definition.id, PACKAGE_1, StringComparison.Ordinal))
            {
                StaticManager.Backend.backendGameData.UserData.AddDiamond(500);
                StaticManager.Backend.backendGameData.UserData.AddGold(10000);
                StaticManager.Backend.backendGameData.InventoryData.AddItem("Fertilizer", 30);
                StaticManager.UI.AlertUI.OpenUI(StaticManager.Langauge.Localize(105));
                GameManager.Instance.SaveAllData();
            }
            if (String.Equals(args.purchasedProduct.definition.id, PACKAGE_2, StringComparison.Ordinal))
            {
                StaticManager.Backend.backendGameData.UserData.AddDiamond(1500);
                StaticManager.Backend.backendGameData.UserData.AddGold(20000);
                StaticManager.Backend.backendGameData.InventoryData.AddItem("Fertilizer", 75);
                StaticManager.UI.AlertUI.OpenUI(StaticManager.Langauge.Localize(106));
                GameManager.Instance.SaveAllData();
            }
            if (String.Equals(args.purchasedProduct.definition.id, EMPLOYEE, StringComparison.Ordinal))
            {
                for (int i = 0; i < GameManager.Instance.UICanvas.transform.childCount; i++)
                {
                    if(GameManager.Instance.UICanvas.transform.GetChild(i).GetComponent<PartTimeUI>())
                        Destroy(GameManager.Instance.UICanvas.transform.GetChild(i).gameObject);
                }
                StaticManager.Sound.SetSFX("Cash");
                StaticManager.Backend.backendGameData.PartTimeData.SetPartTime(2);
                
                //모든 게스트 자동 구매 초기 확인
                for(int i = 0; i < GameManager.Mart.Guest.waitGuests.Count; i++)
                    GameManager.Mart.Guest.waitGuests[i].PurchaseInitial();
                
                GameManager.Instance.SaveAllData();
            }
        }

        return PurchaseProcessingResult.Complete;
    }

    // ====================================================================================================	

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogError($"구매 실패 - {product.definition.id}, {failureReason}");
        StaticManager.UI.SetLoading(false);
        StaticManager.UI.AlertUI.OpenUI(StaticManager.Langauge.Localize(46));
    }
}
