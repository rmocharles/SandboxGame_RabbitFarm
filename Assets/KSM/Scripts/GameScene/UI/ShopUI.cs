using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private GameObject backgroundObject;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_Text infoText;

    [SerializeField] private GameObject[] packageGroup;
    
    [SerializeField] private GameObject diamondButtonGroup;
    [SerializeField] private GameObject goldButtonGroup;
    [SerializeField] private GameObject petButtonGroup;

    [SerializeField] private GameObject diamondShopGroup;
    [SerializeField] private GameObject goldShopGroup;
    [SerializeField] private GameObject petShopGroup;

    [SerializeField] private Button[] diamondPurchaseButtons;
    [SerializeField] private Button[] goldPurchaseButtons;
    [SerializeField] private Button[] petPurchaseButtons;
    [SerializeField] private Button[] petHelpButtons;

    [SerializeField] private TMP_Text restoreText;

    [SerializeField] private TMP_Text[] langaugeTexts;

    [SerializeField] private Image[] packageImage;
    [SerializeField] private Sprite[] langaugeSprites;

    public enum Mode
    {
        Diamond,
        Gold,
        Pet
    };

    void Update()
    {
        petPurchaseButtons[0].interactable = StaticManager.Backend.backendGameData.UserData.Level >= 10;
        petPurchaseButtons[1].interactable = StaticManager.Backend.backendGameData.UserData.Level >= 20;
        petPurchaseButtons[2].interactable = StaticManager.Backend.backendGameData.UserData.Level >= 30;
    }

    void Start()
    {
        packageImage[0].sprite = PlayerPrefs.GetInt("LangIndex") == 0 ? langaugeSprites[0] : langaugeSprites[1];
        packageImage[1].sprite = PlayerPrefs.GetInt("LangIndex") == 0 ? langaugeSprites[2] : langaugeSprites[3];
        
        closeButton.onClick.AddListener(() =>
        {
            StaticManager.Sound.SetSFX();
            backgroundObject.GetComponent<RectTransform>().DOScale(Vector3.zero, 0.1f);
            Invoke("DestroyUI", 0.1f);
        });

        infoText.text = PlayerPrefs.GetInt("LangIndex") == 0 ? "상점" : "Shop";

        restoreText.text = StaticManager.Langauge.Localize(197);
        
        //패키지를 구매했을 경우 우선순위 뒤로 미루기
        for (int i = 0; i < StaticManager.Backend.backendGameData.ShopData.packageItem.Length; i++)
        {
            if (StaticManager.Backend.backendGameData.ShopData.GetItem(StaticManager.Backend.backendGameData.ShopData.packageItem[0]) > 0)
            {
                Debug.LogError($"{StaticManager.Backend.backendGameData.ShopData.packageItem[0]} 아이템은 구매했으므로 우선순위를 뒤로 미룹니다.");
                packageGroup[i].transform.SetAsLastSibling();

                packageGroup[i].GetComponentInChildren<Button>().interactable = false;
                packageGroup[i].GetComponentInChildren<TMP_Text>().text = StaticManager.Langauge.Localize(35);
            }
            else
            {
                packageGroup[i].GetComponentInChildren<Button>().interactable = true;
                //packageGroup[i].GetComponentInChildren<TMP_Text>().text = "구매 안함";
            }
        }

        for (int i = 0; i < 2; i++)
        {
            int num = i;
            diamondButtonGroup.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(() =>
            {
                StaticManager.Sound.SetSFX();
                ActiveMode(Mode.Diamond);
            });
            goldButtonGroup.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(() =>
            {
                StaticManager.Sound.SetSFX();
                ActiveMode(Mode.Gold);
            });
            petButtonGroup.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(() =>
            {
                StaticManager.Sound.SetSFX();
                ActiveMode(Mode.Pet);
            });
        }
        
        // diamondPurchaseButtons[0].onClick.AddListener(() =>
        // {
        //     StaticManager.Sound.SetSFX("Cash");
        //     StaticManager.Backend.backendGameData.UserData.AddDiamond(100);
        //     StaticManager.UI.AlertUI.OpenUI(StaticManager.Langauge.Localize(101));
        //     GameManager.Instance.SaveAllData();
        // });
        //
        // diamondPurchaseButtons[1].onClick.AddListener(() =>
        // {
        //     StaticManager.Sound.SetSFX("Cash");
        //     StaticManager.Backend.backendGameData.UserData.AddDiamond(450);
        //     StaticManager.UI.AlertUI.OpenUI(StaticManager.Langauge.Localize(102));
        //     GameManager.Instance.SaveAllData();
        // });
        //
        // diamondPurchaseButtons[2].onClick.AddListener(() =>
        // {
        //     StaticManager.Sound.SetSFX("Cash");
        //     StaticManager.Backend.backendGameData.UserData.AddDiamond(850);
        //     StaticManager.UI.AlertUI.OpenUI(StaticManager.Langauge.Localize(103));
        //     GameManager.Instance.SaveAllData();
        // });
        
        diamondPurchaseButtons[3].onClick.AddListener(() =>
        {
            StaticManager.Sound.SetSFX("Cash");
            StaticManager.Backend.backendGameData.UserData.AddDiamond(1800);
            StaticManager.UI.AlertUI.OpenUI(StaticManager.Langauge.Localize(104));
            GameManager.Instance.SaveAllData();
        });

        goldPurchaseButtons[0].onClick.AddListener(() =>
        {
            
            if (StaticManager.Backend.backendGameData.UserData.Diamond >= 100)
            {
                StaticManager.Sound.SetSFX("Cash");
                StaticManager.Backend.backendGameData.UserData.AddDiamond(-100);
                StaticManager.Backend.backendGameData.UserData.AddGold(2500);
                StaticManager.UI.AlertUI.OpenUI(StaticManager.Langauge.Localize(138));
                GameManager.Instance.SaveAllData();
            }
            else
            {
                StaticManager.Sound.SetSFX();
                GameManager.Instance.MakeToast(StaticManager.Langauge.Localize(47));
            }
        });
        goldPurchaseButtons[1].onClick.AddListener(() =>
        {
            if (StaticManager.Backend.backendGameData.UserData.Diamond >= 300)
            {
                StaticManager.Sound.SetSFX("Cash");
                StaticManager.Backend.backendGameData.UserData.AddDiamond(-300);
                StaticManager.Backend.backendGameData.UserData.AddGold(10000);
                StaticManager.UI.AlertUI.OpenUI(StaticManager.Langauge.Localize(139));
                GameManager.Instance.SaveAllData();
            }
            else
            {
                StaticManager.Sound.SetSFX();
                GameManager.Instance.MakeToast(StaticManager.Langauge.Localize(47));
            }
        });
        
        petPurchaseButtons[0].onClick.AddListener(() =>
        {
            if (StaticManager.Backend.backendGameData.UserData.Level >= 10)
            {
                if (StaticManager.Backend.backendGameData.UserData.Diamond >= 200)
                {
                    StaticManager.Sound.SetSFX("Cash");
                    StaticManager.Backend.backendGameData.UserData.AddDiamond(-200);
                    StaticManager.UI.AlertUI.OpenUI(StaticManager.Langauge.Localize(140));
                    GameManager.Pet.SetPet(2);
                    GameManager.Instance.SaveAllData();

                    ActiveMode(Mode.Pet);
                }
                else
                {
                    StaticManager.Sound.SetSFX();
                    GameManager.Instance.MakeToast(StaticManager.Langauge.Localize(47));
                }
            }
            else
            {
                StaticManager.Sound.SetSFX();
                GameManager.Instance.MakeToast(StaticManager.Langauge.Localize(45));
            }
            
        });
        
        petPurchaseButtons[1].onClick.AddListener(() =>
        {
            if (StaticManager.Backend.backendGameData.UserData.Level >= 20)
            {
                if (StaticManager.Backend.backendGameData.UserData.Diamond >= 300)
                {
                    StaticManager.Sound.SetSFX("Cash");
                    StaticManager.Backend.backendGameData.UserData.AddDiamond(-300);
                    StaticManager.UI.AlertUI.OpenUI(StaticManager.Langauge.Localize(141));
                    GameManager.Pet.SetPet(3);
                    GameManager.Instance.SaveAllData();
                    
                    ActiveMode(Mode.Pet);
                }
                else
                {
                    StaticManager.Sound.SetSFX();
                    GameManager.Instance.MakeToast(StaticManager.Langauge.Localize(47));
                }
            }
            else
            {
                StaticManager.Sound.SetSFX();
                GameManager.Instance.MakeToast(StaticManager.Langauge.Localize(45));
            }

        });
        
        petPurchaseButtons[2].onClick.AddListener(() =>
        {
            if (StaticManager.Backend.backendGameData.UserData.Level >= 30)
            {
                if (StaticManager.Backend.backendGameData.UserData.Diamond >= 400)
                {
                    StaticManager.Sound.SetSFX("Cash");
                    StaticManager.Backend.backendGameData.UserData.AddDiamond(-400);
                    StaticManager.UI.AlertUI.OpenUI(StaticManager.Langauge.Localize(142));
                    GameManager.Pet.SetPet(4);
                    GameManager.Instance.SaveAllData();
                    
                    ActiveMode(Mode.Pet);
                }
                else
                {
                    StaticManager.Sound.SetSFX();
                    GameManager.Instance.MakeToast(StaticManager.Langauge.Localize(47));
                }
            }
            else
            {
                StaticManager.Sound.SetSFX();
                GameManager.Instance.MakeToast(StaticManager.Langauge.Localize(45));
            }
        });

        for (int i = 0; i < 3; i++)
        {
            int num = i;
            petHelpButtons[num].onClick.AddListener(() =>
            {
                StaticManager.Sound.SetSFX();
                GameManager.Instance.MakeToast2(StaticManager.Langauge.Localize(109 + num), 3f);
            });
        }

        for (int i = 0; i < langaugeTexts.Length; i++)
        {
            langaugeTexts[i].text = StaticManager.Langauge.Localize(201 + i);
        }
    }

    public void ActiveMode(Mode mode = Mode.Diamond)
    {
        switch (mode)
        {
            case Mode.Diamond:
                
                diamondButtonGroup.transform.GetChild(0).gameObject.SetActive(false);
                diamondButtonGroup.transform.GetChild(1).gameObject.SetActive(true);
                
                goldButtonGroup.transform.GetChild(0).gameObject.SetActive(true);
                goldButtonGroup.transform.GetChild(1).gameObject.SetActive(false);
                
                petButtonGroup.transform.GetChild(0).gameObject.SetActive(true);
                petButtonGroup.transform.GetChild(1).gameObject.SetActive(false);

                diamondShopGroup.SetActive(true);
                goldShopGroup.SetActive(false);
                petShopGroup.SetActive(false);

                if(diamondShopGroup.GetComponentInChildren<Scrollbar>())
                    diamondShopGroup.GetComponentInChildren<Scrollbar>().value = 1f;
                break;
            
            case Mode.Gold:
                
                diamondButtonGroup.transform.GetChild(0).gameObject.SetActive(true);
                diamondButtonGroup.transform.GetChild(1).gameObject.SetActive(false);
                
                goldButtonGroup.transform.GetChild(0).gameObject.SetActive(false);
                goldButtonGroup.transform.GetChild(1).gameObject.SetActive(true);
                
                petButtonGroup.transform.GetChild(0).gameObject.SetActive(true);
                petButtonGroup.transform.GetChild(1).gameObject.SetActive(false);
                
                diamondShopGroup.SetActive(false);
                goldShopGroup.SetActive(true);
                petShopGroup.SetActive(false);
                
                if(goldShopGroup.GetComponentInChildren<Scrollbar>())
                    goldShopGroup.GetComponentInChildren<Scrollbar>().value = 1f;
                break;
            
            case Mode.Pet:
                
                diamondButtonGroup.transform.GetChild(0).gameObject.SetActive(true);
                diamondButtonGroup.transform.GetChild(1).gameObject.SetActive(false);
                
                goldButtonGroup.transform.GetChild(0).gameObject.SetActive(true);
                goldButtonGroup.transform.GetChild(1).gameObject.SetActive(false);
                
                petButtonGroup.transform.GetChild(0).gameObject.SetActive(false);
                petButtonGroup.transform.GetChild(1).gameObject.SetActive(true);
                
                diamondShopGroup.SetActive(false);
                goldShopGroup.SetActive(false);
                petShopGroup.SetActive(true);
                
                if(petShopGroup.GetComponentInChildren<Scrollbar>())
                    petShopGroup.GetComponentInChildren<Scrollbar>().value = 1f;


                bool isCheck = false;
                
                for (int i = 0; i < petShopGroup.transform.GetChild(0).GetChild(0).GetChild(0).childCount; i++)
                {
                    if (!StaticManager.Backend.backendGameData.PetData.Dictionary[i + 2])
                        isCheck = true;
                    
                    petShopGroup.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(i).gameObject.SetActive(!StaticManager.Backend.backendGameData.PetData.Dictionary[i + 2]);
                }
                
                if(!isCheck)
                    GameManager.Instance.MakeToast(StaticManager.Langauge.Localize(70));
                
                break;
        }
    }
    
    private void DestroyUI()
    {
        Destroy(this.gameObject);
    }
}
