using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Playables;
using Cinemachine;
using Random = UnityEngine.Random;

/*
 * 메인 화면 UI
 */

public partial class FarmUI : MonoBehaviour
{
    private static FarmUI instance;

    [Header("[ Loading Object ]")]
    public GameObject loadingObject;

    [Header("[ Error Object ]")]
    public GameObject errorObject;

    [Header("[ App Quit Object ]")]
    public GameObject appQuitObject;

    [Header("[ Inventory Object ]")]
    public GameObject inventoryObject;

    [Header("[ Friend Object ]")]
    public GameObject friendObject;

    [Header("[ Shop Object ]")]
    public GameObject shopObject;

    [Header("[ Mart Object ]")]
    public GameObject martObject;

    [Header("[ Farm Object ]")]
    public GameObject farmObject;

    [Header("[ Hide Button for ScreenShot ]")]
    public GameObject[] hideObject;

    [Header("[ Money ]")]
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI diamondText;

    [Header("[ Pad Resoloution ]")]
    public RectTransform[] padResoloution;

    [Header("[ Guest Player ]")]
    public GameObject guestSpawn;
    public GameObject guestPlayerPrefab;

    [HideInInspector]
    public bool isMart = false;


    [Space(25f)]
    [Header("< My Info >")]
    public string nowImage;

    public int characterRandom;

    public int randomFamily = 0;

    public List<GameObject> characterList = new List<GameObject>();

    public static FarmUI GetInstance()
    {
        if (instance == null)
            return null;
        return instance;
    }

    void Awake()
    {
        if (!instance) instance = this;

        //isSprout = new bool[9];
        //isSeed = new bool[9];
        //isHarvest = new bool[9];
        //isComplete = new bool[9];
    }

    void Start()
    {
        randomFamily = Random.Range(0, 4);

        for (int i = 0; i < 4; i++)
        {
            if (i != randomFamily)
                characterList.Add(aiObject.transform.GetChild(i + 1).gameObject);
            else
                aiObject.transform.GetChild(randomFamily + 1).gameObject.SetActive(false);
        }

        characterRandom = Random.Range(0, 4);


        familyPartTimerObject.transform.GetChild(randomFamily).gameObject.SetActive(true);

#if UNITY_ANDROID || UNITY_IOS
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
        //Application.targetFrameRate = 120;
#endif
        settingPanel.SetActive(false);
        profilePanel.SetActive(false);
        friendProfilePanel.SetActive(false);
        friendPanel.SetActive(false);
        addFriendPanel.SetActive(false);
        inventoryPanel.SetActive(false);
        shopPanel.SetActive(false);
        weatherObject.SetActive(false);
        appQuitObject.SetActive(false);
        //skipObject.SetActive(false);

        //for(int i = 0; i < BackendServerManager.GetInstance().field.Count; i++)
        //{
        //    selectPanels[i].SetActive(false);
        //    selectSeedPanels[i].SetActive(false);
        //}

        loadingObject.SetActive(false);

        //for(int i = 0; i < fieldObject.Length; i++)
        //{
        //    fieldObject[i].transform.GetChild(0).GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
        //    tableSelectCanvas.transform.GetChild(i).GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
        //    selectPanels[i].GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
        //    selectSeedPanels[i].GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
        //}

        ImportingPlayerData();

        StartCoroutine(InstantiateGuestPlayer());

        SoundManager.GetInstance().SetBgm(0);

        for (int i = 0; i < 9; i++)
            tableObject[i].GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
    }

    void Update()
    {
        //마트 버튼 활성화
        if (!isScreenShotMode)
        {
            if (isMart)
            {
                martObject.SetActive(false);
                farmObject.SetActive(true);
            }
            else
            {
                martObject.SetActive(true);
                farmObject.SetActive(false);
            }
        }
        

        //태블릿 해상도
        if ((float)Screen.width / (float)Screen.height < 1.5f)
        {
            //for (int i = 0; i < padResoloution.Length; i++)
            //{
            //    if(i > 1)
            //    {
            //        padResoloution[i].localScale = new Vector3(padResoloution[i].localScale.x, 2, padResoloution[i].localScale.z);
            //    }
            //    else
            //    {
            //        padResoloution[i].localScale = new Vector3(1.8f, 1.8f, 1.8f);

            //    }
            //}
        }
        else
        {
            if(selectCanvas.transform.childCount > 0)
            {
                selectCanvas.transform.GetChild(0).localScale = new Vector3(0.005f + 0.0005f * (Camera.main.orthographicSize - 7), 0.005f + 0.0005f * (Camera.main.orthographicSize - 7), 1);
            }
            //for (int i = 0; i < padResoloution.Length; i++)
            //{
            //    if (i > 1)
            //    {
            //        padResoloution[i].localScale = new Vector3(padResoloution[i].localScale.x, 1.6f, padResoloution[i].localScale.z);
            //    }
            //    else
            //    {
            //        padResoloution[i].localScale = new Vector3(1.25f, 1.25f, 1.25f);

            //    }
            //}
        }


        #region 친구 요청 알림 (실시간)
        if (isRequestAlarm)
        {
            isRequestAlarm = false;

            if (friendRequestListButton.transform.GetChild(0).gameObject.activeSelf)
                ChangeMode(true);
        }
        if (isAcceptAlarm)
        {
            isAcceptAlarm = false;

            if (friendListButton.transform.GetChild(0).gameObject.activeSelf)
                ChangeMode(false);
        }
        #endregion

        #region App Quit
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (addFriendPanel.activeSelf) addFriendPanel.SetActive(false);
            else if (profilePanel.activeSelf) profilePanel.SetActive(false);
            else if (friendPanel.activeSelf) friendPanel.SetActive(false);
            else if (settingPanel.activeSelf) settingPanel.SetActive(false);
            //else if (skipObject.activeSelf) CloseSkipHarvestCoolTime();
            else if (shopPanel.activeSelf) shopPanel.SetActive(false);
            else if (inventoryPanel.activeSelf) inventoryPanel.SetActive(false);
            else if (weatherObject.activeSelf) weatherObject.SetActive(false);
            else if (errorObject.activeSelf) return;
            else AppQuit();
        }
        #endregion

        SetLangauge();

        SetInfoMoney();

        UpdateFieldUI();

        //UpdateSkipFieldData();

        //UpdateHarvestAnimation();   //캐릭터 수확 쿨타임

        UpdateProfile();

        UpdateSetting();

        UpdateWeather();
    }

    #region 사용자 이벤트
    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            BackendServerManager.GetInstance().SaveMyInfo();
        }

        else
        {
        }
    }

    private void OnApplicationQuit()
    {
        BackendServerManager.GetInstance().SaveMyInfo();
        BackEnd.Backend.Notification.DisConnect();

    }
    #endregion




    #region 언어 변경 선택시
    public void ChangeLangauge()
    {
        switch (PlayerPrefs.GetString("Langauge"))
        {
            case "ko":
                PlayerPrefs.SetString("Langauge", "en");
                break;

            case "en":
                PlayerPrefs.SetString("Langauge", "ko");
                break;
        }
    }
    #endregion

    #region 앱 종료시 뜨는 팝업
    public void AppQuit()
    {
        if (appQuitObject.activeSelf) return;

        BackendServerManager.GetInstance().SaveMyInfo();

        appQuitObject.SetActive(true);

        appQuitObject.GetComponentInChildren<TextMeshProUGUI>().text = PlayerPrefs.GetString("Langauge") == "ko" ? BackendServerManager.GetInstance().langaugeSheet[0].ko : BackendServerManager.GetInstance().langaugeSheet[0].en;
        appQuitObject.GetComponentsInChildren<Button>()[1].onClick.AddListener(() =>
        {
            appQuitObject.GetComponentInChildren<Button>().onClick.RemoveAllListeners();

            //실시간 알림 서버 접속을 해제한다
            BackEnd.Backend.Notification.DisConnect();

            Debug.LogError("앱 종료");

            Application.Quit();


        });
        
    }
    #endregion

    #region 사용자 편의 함수

    public void SetErrorObject(string error)
    {
        errorObject.SetActive(true);
        errorObject.GetComponentInChildren<TextMeshProUGUI>().text = error;
        errorObject.transform.GetChild(0).GetComponentInChildren<Button>().onClick.RemoveAllListeners();
    }

    public void SetErrorObject(string error, Action func = null)
    {
        errorObject.SetActive(true);
        errorObject.GetComponentInChildren<TextMeshProUGUI>().text = error;

        if (func != null)
        {
            errorObject.transform.GetChild(0).GetComponentInChildren<Button>().onClick.RemoveAllListeners();
            errorObject.transform.GetChild(0).GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                func();
            });
        }
    }

    public void DestroySelectPrefab()
    {
        if (selectCanvas.transform.childCount > 0)
        {
            for (int i = 0; i < selectCanvas.transform.childCount; i++)
                Destroy(selectCanvas.transform.GetChild(i).gameObject);
        }
    }

    public void ButtonClick(int num = 0)
    {
        SoundManager.GetInstance().SetEffect(num);
        SoundManager.GetInstance().Vibrate();
    }

    public void SetLoading(bool isActive = true)
    {
        loadingObject.SetActive(isActive);
    }

    public bool IsActiveObject()
    {
        return errorObject.activeSelf || loadingObject.activeSelf || addFriendPanel.activeSelf || friendPanel.activeSelf || profilePanel.activeSelf || settingPanel.activeSelf || shopPanel.activeSelf || inventoryPanel.activeSelf;
    }
    #endregion

    #region 언어 변경시 즉시 수정되어야 할 부분들

    public void SetLangauge()
    {
        //설정창 설정
        settingPanel.GetComponentInChildren<TextMeshProUGUI>().text = BackendServerManager.GetInstance().myInfo.gamerID;
        settingPanel.GetComponentsInChildren<TextMeshProUGUI>()[1].text = PlayerPrefs.GetString("Langauge") == "ko" ? "한국어" : "English";

        //프로필 설정
        profilePanel.transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("Langauge") == "ko" ? "랭크" : "Rank";
        profilePanel.transform.GetChild(1).GetChild(4).GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("Langauge") == "ko" ? BackendServerManager.GetInstance().langaugeSheet[2].ko : BackendServerManager.GetInstance().langaugeSheet[2].en;

        //친구창 설정
        friendListButton.transform.GetChild(0).GetComponentInChildren<Text>().text = PlayerPrefs.GetString("Langauge") == "ko" ? BackendServerManager.GetInstance().langaugeSheet[3].ko : BackendServerManager.GetInstance().langaugeSheet[3].en;
        friendListButton.transform.GetChild(1).GetComponentInChildren<Text>().text = PlayerPrefs.GetString("Langauge") == "ko" ? BackendServerManager.GetInstance().langaugeSheet[3].ko : BackendServerManager.GetInstance().langaugeSheet[3].en;

        friendRequestListButton.transform.GetChild(0).GetComponentInChildren<Text>().text = PlayerPrefs.GetString("Langauge") == "ko" ? BackendServerManager.GetInstance().langaugeSheet[4].ko : BackendServerManager.GetInstance().langaugeSheet[4].en;
        friendRequestListButton.transform.GetChild(1).GetComponentInChildren<Text>().text = PlayerPrefs.GetString("Langauge") == "ko" ? BackendServerManager.GetInstance().langaugeSheet[4].ko : BackendServerManager.GetInstance().langaugeSheet[4].en;

        friendListButton.transform.parent.GetChild(4).GetComponentInChildren<Text>().text = PlayerPrefs.GetString("Langauge") == "ko" ? BackendServerManager.GetInstance().langaugeSheet[5].ko : BackendServerManager.GetInstance().langaugeSheet[5].en;

        addFriendPanel.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("Langauge") == "ko" ? BackendServerManager.GetInstance().langaugeSheet[6].ko : BackendServerManager.GetInstance().langaugeSheet[6].en;
    }
    #endregion

    #region 로그아웃
    public void LogOut()
    {
        BackendServerManager.GetInstance().LogOut();
        BackendServerManager.GetInstance().LogOutWithGoogle();

        Scene_Manager.GetInstance().ChangeState(Scene_Manager.GameState.Login);
    }
    #endregion

    #region 재화 요소

    public void SetInfoMoney()
    {
        goldText.text = BackendServerManager.GetInstance().myInfo.gold <= 0 ? 0.ToString() : string.Format("{0:#,###}", BackendServerManager.GetInstance().myInfo.gold);
        diamondText.text = BackendServerManager.GetInstance().myInfo.diamond <= 0 ? 0.ToString() : string.Format("{0:#,###}", BackendServerManager.GetInstance().myInfo.diamond);
    }
    #endregion

    public void ImportingPlayerData()
    {
        //플레이어 닉네임
        nicknameText.text = BackendServerManager.GetInstance().myInfo.nickName;

        //플레이어 아이디 정보
        gamerIDText.text = BackendServerManager.GetInstance().myInfo.gamerID;

        //플레이어 대표 이미지
        nowImage = BackendServerManager.GetInstance().representImage;

        //대표 이미지 정보
        SelectImage(nowImage);

        InitializeTable();
    }

    float previousOrthographicSize = 0;
    public void MoveMart(bool isMove)
    {
        DestroySelectPrefab();
        if (isMove)
        {
            cloudButton.SetActive(false);
            sunButton.SetActive(false);

            SoundManager.GetInstance().SetBgm(1);
            previousOrthographicSize = Camera.main.orthographicSize;
            Camera.main.transform.position = new Vector3(0, -3, -10);
        }
        else
        {

            SoundManager.GetInstance().SetBgm(0);
            Camera.main.orthographicSize = 100;
            Camera.main.transform.position = new Vector3(10, -2.5f, -10);
            //mainCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = previousOrthographicSize;
        }

        isMart = isMove;
    }


    public bool isScreenShotMode = false;
    public void ScreenShotMode(bool isActive)
    {
        DestroySelectPrefab();
        isScreenShotMode = isActive;
        for (int i = 0; i < hideObject.Length; i++)
            hideObject[i].SetActive(!isActive);
    }

    public void YoutubeURL()
    {
        Application.OpenURL("https://youtube.com/c/yoonmi0712");
    }

    IEnumerator InstantiateGuestPlayer()
    {
        while (true)
        {
            GameObject guestPlayer = Instantiate(guestPlayerPrefab, guestSpawn.transform);

            yield return new WaitForSeconds(BackendServerManager.GetInstance().guestSheet[0].spawnCoolTime);
        }
    }
}
