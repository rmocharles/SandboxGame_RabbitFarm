using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnlockUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject[] objects;
    [SerializeField] private Button PurchaseButton;
    [SerializeField] private TMP_Text infoText;

    public enum Type
    {
        Field, Table, Refrigerator, Shelf
    }

    public void Initialize(int index, int price, Type type)
    {
        objects[0].SetActive(type == Type.Field);
        objects[1].SetActive(type == Type.Table);
        objects[2].SetActive(type == Type.Shelf);
        objects[3].SetActive(type == Type.Refrigerator);
        
        closeButton.onClick.AddListener(() =>
        {
            StaticManager.Sound.SetSFX();
            Destroy(this.gameObject);
        });
        
        PurchaseButton.onClick.AddListener(() =>
        {
            if (StaticManager.Backend.backendGameData.UserData.Gold >= price)
            {
                StaticManager.Backend.backendGameData.UserData.AddGold(-price);
                StaticManager.Sound.SetSFX("Cash");
                switch (type)
                {
                    case Type.Field:
                        //StaticManager.Backend.backendGameData.FieldData.SetField(index, 0);
                        GameManager.Field.SetField(index, 0, false);
                        break;
                    
                    case Type.Table:
                        StaticManager.Backend.backendGameData.MartData.SetOpen(index, true);
                        GameManager.Mart.InitializeData(index);
                        break;
                    
                    case Type.Shelf:
                        StaticManager.Backend.backendGameData.MartData.SetOpen(index, true);
                        GameManager.Mart.InitializeData(index);
                        break;
                    
                    case Type.Refrigerator:
                        StaticManager.Backend.backendGameData.MartData.SetOpen(index, true);
                        GameManager.Mart.InitializeData(index);
                        break;
                }
                
                GameManager.Instance.SaveAllData();
                Destroy(this.gameObject);
            }
            else
            {
                GameManager.Instance.MakeToast(StaticManager.Langauge.Localize(39));
            }
        });

        PurchaseButton.GetComponentInChildren<TMP_Text>().text = price.ToString();

        infoText.text = StaticManager.Langauge.Localize((int)(52 + type));
    }
}
