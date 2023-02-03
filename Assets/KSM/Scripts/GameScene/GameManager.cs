using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/*
 * GameManager
 *
 * 1. 캐릭터 기능
 */
public class GameManager : Singleton<GameManager>
{
    public Canvas harvestUICanvas;
    public Canvas UICanvas;
    public Canvas worldCanvas;
    public Button bagButton;

    public Button settingButton;

    public TMP_Text goldText;
    public TMP_Text diamondText;

    public TMP_Text levelText;
    public TMP_Text nicknameText;
    public TMP_Text expBarText;
    public Image expBarImage;

    public Button profileButton;
    public GameObject profileGroup;
    public GameObject cashGroup;
    public Button inventoryButton;
    public Button shopButton;
    public Button goldGroupButton;
    public Button goldPlusButton;
    public Button diamondGroupButton;
    public Button diamondPlusButton;
    public Button moveButton;
    public Button questButton;
    public GameObject questActiveObject;

    public GameObject[] allObject;

    public DateTime nowTime = new DateTime(2000, 1, 2);
    public DateTime m_AppQuitTime = new DateTime(2000, 1, 2);


    public Vector3 originQuestPos, originBagPos, originShopPos, originMovePos;
    public static FieldManager Field { get; private set; }
    public static BunnyManager Bunny { get; private set; }
    
    public static CameraManager Camera { get; private set; }
    
    public static MartManager Mart { get; private set; }
    
    public static AnimalManager Animal { get; private set; }
    
    public static WeatherManager Weather { get; private set; }
    
    public static PetManager Pet { get; private set; }
    
    public static PartTimeManager PartTime { get; private set; }
    
    public static TutorialManager Tutorial { get; private set; }

    public enum Mode
    {
        Farm,
        Mart
    }

    public Mode nowMode = Mode.Farm;

    void Awake()
    {
        originQuestPos = questButton.transform.position;
        originBagPos = bagButton.transform.position;
        originShopPos = shopButton.transform.position;
        originMovePos = moveButton.transform.position;

        if (!Backend.IsInitialized)
            SceneManager.LoadScene("1. Login");
    }
    
    void Start()
    {
        SendQueue.Enqueue(Backend.Utils.GetServerTime, ( callback ) => 
        {
            string time = callback.GetReturnValuetoJSON()["utcTime"].ToString();
            DateTime parsedDate = DateTime.Parse(time);

            nowTime = parsedDate;
            StartCoroutine(NowTime());
        });

        StaticManager.Sound.SetBGM("FarmBGM");

        StaticManager.UI.FadeUI.FadeStart(FadeUI.FadeType.ChangeToTransparent, null, 1f);

        Bunny = GetComponentInChildren<BunnyManager>();
        Field = GetComponentInChildren<FieldManager>();
        Camera = GetComponentInChildren<CameraManager>();
        Mart = GetComponentInChildren<MartManager>();
        Animal = GetComponentInChildren<AnimalManager>();
        Weather = GetComponentInChildren<WeatherManager>();
        Pet = GetComponentInChildren<PetManager>();
        PartTime = GetComponentInChildren<PartTimeManager>();
        Tutorial = GetComponentInChildren<TutorialManager>();

        Bunny.Initialize();
        Field.Initialize();
        Camera.Initialize();
        Mart.Initialize();
        Animal.Initialize();
        Weather.Initialize();
        Tutorial.Initialize();
        
        //프로필 변경
        profileButton.GetComponent<Image>().sprite =
            Resources.Load<Sprite>("Sprites/Profile/profile_" + (StaticManager.Backend.backendGameData.ProfileData.Represent + 1).ToString("D2"));
        profileButton.onClick.AddListener(() =>
        {
            Field.CloseUI();
            StaticManager.Sound.SetSFX();
            GameObject profileUI = StaticManager.UI.OpenUI("Prefabs/GameScene/ProfileUI", UICanvas.transform);
            
            if(Camera.IsPad())
                profileUI.transform.GetChild(0).GetComponent<RectTransform>().localScale = Vector3.one * 1.2f;
        });
        
        settingButton.onClick.AddListener(() =>
        {
            Field.CloseUI();
            StaticManager.Sound.SetSFX();
            GameObject settingUI = StaticManager.UI.OpenUI("Prefabs/GameScene/SettingUI", UICanvas.transform);
            if(Camera.IsPad())
                settingUI.transform.GetChild(0).GetComponent<RectTransform>().localScale = Vector3.one * 1.2f;
        });
        
        inventoryButton.onClick.AddListener(() =>
        {
            Field.CloseUI();
            StaticManager.Sound.SetSFX();
            StaticManager.UI.OpenUI("Prefabs/GameScene/InventoryUI", UICanvas.transform);
        });
        
        shopButton.onClick.AddListener(() =>
        {
            Field.CloseUI();
            StaticManager.Sound.SetSFX();
            GameObject shopUI = StaticManager.UI.OpenUI("Prefabs/GameScene/ShopUI", UICanvas.transform);
            shopUI.GetComponent<ShopUI>().ActiveMode(ShopUI.Mode.Diamond);
            
            if(Camera.IsPad())
                shopUI.transform.GetChild(0).GetComponent<RectTransform>().localScale = Vector3.one * .9f;
        });
        
        goldGroupButton.onClick.AddListener(() =>
        {
            Field.CloseUI();
            StaticManager.Sound.SetSFX();
            GameObject shopUI = StaticManager.UI.OpenUI("Prefabs/GameScene/ShopUI", UICanvas.transform);
            shopUI.GetComponent<ShopUI>().ActiveMode(ShopUI.Mode.Gold);
            
            if(Camera.IsPad())
                shopUI.transform.GetChild(0).GetComponent<RectTransform>().localScale = Vector3.one * .9f;
        });
        
        goldPlusButton.onClick.AddListener(() =>
        {
            Field.CloseUI();
            StaticManager.Sound.SetSFX();
            GameObject shopUI = StaticManager.UI.OpenUI("Prefabs/GameScene/ShopUI", UICanvas.transform);
            shopUI.GetComponent<ShopUI>().ActiveMode(ShopUI.Mode.Gold);
            
            if(Camera.IsPad())
                shopUI.transform.GetChild(0).GetComponent<RectTransform>().localScale = Vector3.one * .9f;
        });
        
        diamondGroupButton.onClick.AddListener(() =>
        {
            Field.CloseUI();
            StaticManager.Sound.SetSFX();
            GameObject shopUI = StaticManager.UI.OpenUI("Prefabs/GameScene/ShopUI", UICanvas.transform);
            shopUI.GetComponent<ShopUI>().ActiveMode(ShopUI.Mode.Diamond);
            
            if(Camera.IsPad())
                shopUI.transform.GetChild(0).GetComponent<RectTransform>().localScale = Vector3.one * .9f;
        });
        
        diamondPlusButton.onClick.AddListener(() =>
        {
            Field.CloseUI();
            StaticManager.Sound.SetSFX();
            GameObject shopUI = StaticManager.UI.OpenUI("Prefabs/GameScene/ShopUI", UICanvas.transform);
            shopUI.GetComponent<ShopUI>().ActiveMode(ShopUI.Mode.Diamond);
            
            if(Camera.IsPad())
                shopUI.transform.GetChild(0).GetComponent<RectTransform>().localScale = Vector3.one * .9f;
        });
        
        moveButton.onClick.AddListener(() =>
        {
            if (nowMode == Mode.Farm)
            {
                if (StaticManager.Backend.backendGameData.UserData.Level >= 3)
                {
                    StaticManager.Sound.SetBGM("MartBGM");
                    moveButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Button/btn_farmgo");
                    nowMode = Mode.Mart;
                    UnityEngine.Camera.main.orthographicSize = GameManager.Camera.IsPad() ? 11 : 8;
                    UnityEngine.Camera.main.transform.position = new Vector3(49, -1, -10);

                    if (StaticManager.Backend.backendGameData.UserData.Tutorial == 4)
                    {
                        Tutorial.SetTutorial(2);
                    }
                }
                else
                {
                    MakeToast(StaticManager.Langauge.Localize(51));
                }
            }
            else if (nowMode == Mode.Mart)
            {
                StaticManager.Sound.SetBGM("FarmBGM");
                moveButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Button/btn_martgo");
                nowMode = Mode.Farm;
                UnityEngine.Camera.main.orthographicSize = GameManager.Camera.IsPad() ? 10 : 7;
                UnityEngine.Camera.main.transform.position = new Vector3(0, 0, -10);
            }
        });
        
        questButton.onClick.AddListener(() =>
        {
            StaticManager.Sound.SetSFX();
            //MakeToast(StaticManager.Langauge.Localize(38));
            GameObject questUI = StaticManager.UI.OpenUI("Prefabs/GameScene/QuestUI", UICanvas.transform);
        });

        ActiveQuestIcon();
        
        #region 해상도 대응

        if (Camera.IsPad())
        {
            profileGroup.GetComponent<RectTransform>().anchoredPosition = new Vector2(25, 0);
            profileGroup.GetComponent<RectTransform>().localScale = Vector3.one * 0.9f;

            cashGroup.GetComponent<RectTransform>().anchoredPosition = new Vector2(-250, -75);
            cashGroup.GetComponent<RectTransform>().localScale = Vector3.one * 1.5f;
        }

        #endregion
    }

    IEnumerator NowTime()
    {
        while (true)
        {
            DateTime addTime = nowTime.AddSeconds(1);
            nowTime = addTime;
            yield return new WaitForSeconds(1f);
        }
    }

    void Update()
    {
        for (int i = 0; i < allObject.Length; i++)
        {
            allObject[i].SetActive(!Tutorial.isTutorial);
        }

        //재화 요소
        goldText.text = StaticManager.Backend.backendGameData.UserData.Gold > 0 ? string.Format("{0:#,###}", StaticManager.Backend.backendGameData.UserData.Gold) : 0.ToString();
        diamondText.text = StaticManager.Backend.backendGameData.UserData.Diamond > 0 ? string.Format("{0:#,###}", StaticManager.Backend.backendGameData.UserData.Diamond) : 0.ToString();
        
        //닉네임
        nicknameText.text = Backend.UserNickName;
        
        //레벨 및 경험치
        levelText.text = "LV" + StaticManager.Backend.backendGameData.UserData.Level;
        
        //레벨이 12, 15 일경우 닭, 소 오픈
        if (StaticManager.Backend.backendGameData.UserData.Level >= StaticManager.Backend.backendChart.Harvest.harvestSheet[10].RequireLevel && StaticManager.Backend.backendGameData.AnimalData.Dictionary["Chicken"].Upgrade == -1)
        {
            StaticManager.Backend.backendGameData.AnimalData.SetAnimal("Chicken", 1);
            SaveAllData();
        }
        
        if(StaticManager.Backend.backendGameData.UserData.Level >= StaticManager.Backend.backendChart.Harvest.harvestSheet[9].RequireLevel && StaticManager.Backend.backendGameData.AnimalData.Dictionary["Cow"].Upgrade == -1)
        {
            StaticManager.Backend.backendGameData.AnimalData.SetAnimal("Cow", 1);
            SaveAllData();
        }
        
        expBarImage.fillAmount = StaticManager.Backend.backendGameData.UserData.Exp / (float)StaticManager.Backend.backendChart.Balance.balanceSheet[StaticManager.Backend.backendGameData.UserData.Level - 1].Exp;
        if (expBarImage.fillAmount >= 0.4f)
            expBarText.text = Mathf.Min((float)(Math.Truncate(expBarImage.fillAmount * 10000) / 100), 100) + "%";
        else
            expBarText.text = string.Empty;
        
        //레벨업 관련
        if (expBarImage.fillAmount >= 1 && StaticManager.Backend.backendGameData.UserData.Level < StaticManager.Backend.backendChart.Balance.MaxLevel)
        {
            GameObject LevelUpUI = StaticManager.UI.OpenUI("Prefabs/GameScene/LevelUpUI", profileGroup.transform);
            
            StaticManager.Backend.backendGameData.UserData.AddExp(-StaticManager.Backend.backendChart.Balance.balanceSheet[StaticManager.Backend.backendGameData.UserData.Level - 1].Exp);
            StaticManager.Backend.backendGameData.UserData.AddLevel(1);
            
            MakeToast(PlayerPrefs.GetInt("LangIndex") == 0 ? (StaticManager.Backend.backendGameData.UserData.Level) + "레벨이 되었습니다." : "It is Level " + StaticManager.Backend.backendGameData.UserData.Level);

            if (StaticManager.Backend.backendGameData.UserData.Level == 3)
            {
                MakeToast(StaticManager.Langauge.Localize(112));
            }
        }

        if (!StaticManager.Backend.backendGameData.PetData.Dictionary[1] && StaticManager.Backend.backendGameData.UserData.Level >= 5)
        {
            MakeToast(StaticManager.Langauge.Localize(194));
            StaticManager.Backend.backendGameData.PetData.SetPet(1, true);
            Pet.SetPet(1);
            SaveAllData();
        }
        
        //마트 활성화 버튼
        if (StaticManager.Backend.backendGameData.UserData.Level >= 3)
        {
            //MakeToast(StaticManager.Langauge.Localize(112));
            moveButton.transform.GetChild(0).gameObject.SetActive(StaticManager.Backend.backendGameData.UserData.Tutorial == 4);
        }
        else
        {
            moveButton.transform.GetChild(0).gameObject.SetActive(false);
        }
        
        //게임 종료
        if (Input.GetKeyDown(KeyCode.Escape))
            StaticManager.UI.OpenUI("Prefabs/GameScene/CloseUI", UICanvas.transform);
    }

    private void OnApplicationFocus(bool isFocus)
    {
        if (isFocus)
        {
            SendQueue.Enqueue(Backend.Utils.GetServerTime, (callback) =>
            {
                string time = callback.GetReturnValuetoJSON()["utcTime"].ToString();
                DateTime parsedDate = DateTime.Parse(time);

                nowTime = parsedDate;
            });
            SaveAllData();
            
            if (!Backend.IsInitialized)
                SceneManager.LoadScene("1. Login");
        }
        else
        {
        }
    }

    void OnApplicationPause(bool isPaused)
    {
        IronSource.Agent.onApplicationPause(isPaused);
    }
    private void OnApplicationQuit()
    {
        SaveAllData();
    }

    public void SaveAllData()
    {
        SendQueue.Enqueue(Backend.BMember.IsAccessTokenAlive, callback =>
        {
            if (callback.IsSuccess())
            {
                StaticManager.Backend.UpdateAllGameData(callback =>
                {
                    if (!callback.IsSuccess())
                    {
                        StaticManager.UI.AlertUI.OpenUI(StaticManager.Langauge.Localize(27), () => Application.Quit());
                    }
                    else
                    {
                        Debug.LogWarning("저장 성공");
                    }
                });
            }
            else
            {
                StaticManager.UI.AlertUI.OpenUI(StaticManager.Langauge.Localize(143), () => Application.Quit());
            }
        });
        
        
        ActiveQuestIcon();
    }

    public void ActiveQuestIcon()
    {
        bool isActive = false;
        
        for (int i = 0; i < 8; i++)
        {
            if (StaticManager.Backend.backendGameData.QuestData.Dictionary[i].Count >= StaticManager.Backend.backendChart.Quest.questSheet[i].Upgrade *
                StaticManager.Backend.backendGameData.QuestData.Dictionary[i].Level)
            {
                isActive = true;
            }
        }
       
        questActiveObject.SetActive(isActive);
    }

    public void MakeToast(string text, float delay = 1.5f)
    {
        for (int i = 0; i < UICanvas.transform.childCount; i++)
        {
            if(Instance.UICanvas.transform.GetChild(i).GetComponent<ToastUI>())
                Destroy(Instance.UICanvas.transform.GetChild(i).gameObject);
        }
        
        GameObject toastUI = StaticManager.UI.OpenUI("Prefabs/GameScene/ToastUI", UICanvas.transform);
        toastUI.GetComponent<ToastUI>().Initialize(text, delay);
    }
    
    public void MakeToast2(string text, float delay = 1.5f)
    {
        for (int i = 0; i < UICanvas.transform.childCount; i++)
        {
            if(Instance.UICanvas.transform.GetChild(i).GetComponent<ToastUI>())
                Destroy(Instance.UICanvas.transform.GetChild(i).gameObject);
        }
        
        GameObject toastUI = StaticManager.UI.OpenUI("Prefabs/GameScene/Toast2UI", UICanvas.transform);
        toastUI.GetComponent<ToastUI>().Initialize(text, delay);
    }

    public GameObject GoldUI(string text)
    {
        for (int i = 0; i < GameManager.Instance.goldText.transform.childCount; i++)
            Destroy(GameManager.Instance.goldText.transform.GetChild(i).gameObject);
        
        GameObject obj = StaticManager.UI.OpenUI("Prefabs/GameScene/GoldUI", goldText.transform);
        obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(50, -70);
        obj.GetComponent<MiniBubbleUI>().Initialize(text);
        return obj;
    }
    public GameObject DiamondUI(string text)
    {
        for (int i = 0; i < GameManager.Instance.diamondText.transform.childCount; i++)
            Destroy(GameManager.Instance.diamondText.transform.GetChild(i).gameObject);
        
        GameObject obj = StaticManager.UI.OpenUI("Prefabs/GameScene/DiamondUI", diamondText.transform);
        obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(50, -70);
        obj.GetComponent<MiniBubbleUI>().Initialize(text);
        return obj;
    }
}
