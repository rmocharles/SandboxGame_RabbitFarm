using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Include Backend
using BackEnd;
using static BackEnd.SendQueue;

//Include GPGS namespace
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using LitJson;
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using System.Text;
using System;

/*
 * Federation Login
 */

public class BackendServerManager : MonoBehaviour
{
    private static BackendServerManager instance;

    #region My Information

    public class MyInfo
    {
        public string nickName = string.Empty;
        public string indate = string.Empty;
        public string userIndate = string.Empty;
        public string gamerID = string.Empty;

        public int gold = 0;
        public int diamond = 0;

        public int fertilizer = 0;
        public int fertilizer2 = 0;

        public int[] seed = new int[9];
        public int[] harvest = new int[18];

        public int seed_Carrot = 0;
        public int seed_Potato = 0;
        public int seed_Tomato = 0;
        public int seed_Cucumber = 0;
        public int seed_Pumpkin = 0;
        public int seed_Apple = 0;
        public int seed_Peach = 0;
        public int seed_Pear = 0;
        public int seed_Cherry = 0;

        public int carrot = 0;
        public int potato = 0;
        public int tomato = 0;
        public int cucumber = 0;
        public int pumpkin = 0;
        public int apple = 0;
        public int peach = 0;
        public int pear = 0;
        public int cherry = 0;
    }
    public MyInfo myInfo = new MyInfo();

    private Action<bool, string> loginSuccessFunc = null;
    #endregion

    #region Error Code
    private const string SIGNUP_FAIL = "차단 당한 유저입니다.";
    private const string SIGNUP_FAIL_EN = "You are a blocked user";
    private const string SIGN_GUEST_FAIL = "출시 설정이 테스트인데 AU가 10을 초과하였습니다.\n(Backend Console Guest User Delete)";
    private const string SIGN_GUEST_FAIL_EN = "The release setting is test, but the AU exceeds 10.";
    private const string NICKNAME_EMPTY_FAIL = "닉네임을 입력해주세요.";
    private const string NICKNAME_EMPTY_FAIL_EN = "Please enter your nickname.";
    private const string NICKNAME_DUPLICATION_FAIL = "이미 사용되고 있는 닉네임입니다.";
    private const string NICKNAME_DUPLICATION_FAIL_EN = "This nickname is already in use.";
    private const string NICKNAME_BLANK_FAIL = "닉네임에 공백이 들어가 있습니다.";
    private const string NICKNAME_BLANK_FAIL_EN = "There is a space in the nickname.";

    private const string NETWORK_FAIL = "네트워크가 불안정합니다.\n다시 시도해주세요.";
    private const string NETWORK_FAIL_EN = "Network is unstable.\nPlease try again.";
    private const string SERVER_FAIL = "서버가 과부하상태이거나 불안정합니다.\n 다시 시도해주세요.";
    private const string SERVER_FAIL_EN = "Server is overloaded or unstable.\nPlease try again.";
    private const string TOKEN_FAIL = "다른 기기에서 로그인 되었습니다.\n다시 로그인해주세요.";
    private const string TOKEN_FAIL_EN = "You are logged in from another device.\nPlease log in again.";
    #endregion

    [Header("<<< BACKEND SHEET >>>")]
    public string langaugeSheetCode = "";
    public string fieldSheetCode = "";
    public string shopSheetCode = "";
    public string martSheetCode = "";
    public string guestSheetCode = "";
    public string harvestSheetCode = "";
    public string weatherSheetCode = "";

    [Header("< MONEY >")]
    [Tooltip("현재 소지하고 있는 골드")]
    public int myGold = 0;
    [Tooltip("현재 소지하고 있는 골드")]
    public int myDiamond = 0;

    [Header("< FIELD >")]
    public Dictionary<int, int> field = new Dictionary<int, int>();
    public Dictionary<int, int> fieldType = new Dictionary<int, int>();
    public Dictionary<int, string> fieldEndTime = new Dictionary<int, string>();

    [Header("< Mart >")]
    public Dictionary<int, int> TableType = new Dictionary<int, int>();
    public Dictionary<int, int> TableCount = new Dictionary<int, int>();

    [Header("< Weather >")]
    public string weather_Sun = string.Empty;
    public string weather_Cloud = string.Empty;

    [Header("< Represent Image >")]
    public string representImage = "0";

    private IAppleAuthManager appleAuthManager;


    void Start()
    {

        if (AppleAuthManager.IsCurrentPlatformSupported)
        {
            Debug.LogWarning("IOS CHECK");
            appleAuthManager = new AppleAuthManager(new PayloadDeserializer());
        }

        BackendInitialize();

        Backend.Notification.OnAuthorize = (bool Result, string Reason) =>
        {
            Debug.Log("실시간 서버 성공 여부 : " + Result);
            Debug.Log("실패 시 이유 : " + Reason);
        };

        Backend.Notification.OnDisConnect = (string Reason) =>
        {
            Debug.LogError("해제 이유 : " + Reason);
        };

        Backend.Notification.OnFriendConnected = (string inDate, string nickname) =>
        {
            Debug.Log(nickname + "님이 연결했습니다");

            if (FarmUI.GetInstance().friendsInfo[inDate] != null)
                FarmUI.GetInstance().friendsInfo[inDate].SetIsOnline(PlayerPrefs.GetString("Langauge") == "ko" ? "온라인" : "Online");
        };

        Backend.Notification.OnFriendDisconnected = (string inDate, string nickname) =>
        {
            Debug.Log(nickname + "님이 종료했습니다");

            if (FarmUI.GetInstance().friendsInfo[inDate] != null)
                FarmUI.GetInstance().friendsInfo[inDate].SetIsOnline(PlayerPrefs.GetString("Langauge") == "ko" ? "오프라인" : "Offline");
        };

        Backend.Notification.OnReceivedFriendRequest = () => {
            Debug.Log("친구 요청이 도착했습니다!");
            FarmUI.GetInstance().isRequestAlarm = true;
        };

        Backend.Notification.OnAcceptedFriendRequest = () =>
        {
            Debug.Log("친구 요청이 수락되었습니다!");
            FarmUI.GetInstance().isAcceptAlarm = true;
        };


        Backend.Notification.OnIsConnectUser = (bool isConnect, string nickName, string gamerIndate) =>
        {
            Debug.Log($"{nickName} / {gamerIndate} 접속 여부 확인 : " + isConnect);


            FarmUI.GetInstance().friendsInfo[gamerIndate].SetIsOnline(isConnect ? (PlayerPrefs.GetString("Langauge") == "ko" ? "온라인" : "Online") : (PlayerPrefs.GetString("Langauge") == "ko" ? "오프라인" : "Offline"));
        };
    }

    void Update()
    {
        //비동기 함수 폴링
        Backend.AsyncPoll();

        if (this.appleAuthManager != null)
        {
            this.appleAuthManager.Update();
        }

        
    }

    void Awake()
    {
        if (!instance) instance = this;
        DontDestroyOnLoad(gameObject);

        if (!PlayerPrefs.HasKey("Langauge"))
        {
            SystemLanguage lang = Application.systemLanguage;
            switch (lang)
            {
                case SystemLanguage.Korean:
                    PlayerPrefs.SetString("Langauge", "ko");
                    break;
                case SystemLanguage.English:
                    PlayerPrefs.SetString("Langauge", "en");
                    break;
            }
        }

    }

    public static BackendServerManager GetInstance()
    {
        if (instance == null)
            return null;
        return instance;
    }

    //====================================================================================================================

    private void BackendInitialize()
    {
#if UNITY_ANDROID
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration
            .Builder()
            .RequestServerAuthCode(false)
            .RequestEmail()
            .RequestIdToken()
            .Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;

        PlayGamesPlatform.Activate();
#endif
        var bro = Backend.Initialize(true);
        if (bro.IsSuccess())
        {
            LoginUI.GetInstance().AutoLogin();
        }
        //Error
        else
        {
            if (bro.GetStatusCode() == "403")
            {
                LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? SIGNUP_FAIL : SIGNUP_FAIL_EN, () => Application.Quit());
            }
            if (bro.IsClientRequestFailError())
            {
                LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? NETWORK_FAIL : NETWORK_FAIL_EN, () => Application.Quit());
            }
            else if (bro.IsServerError() || bro.IsMaintenanceError())
            {
                LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? SERVER_FAIL : SERVER_FAIL_EN, () => Application.Quit());
            }
            else if (bro.IsBadAccessTokenError())
            {
                LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? TOKEN_FAIL : TOKEN_FAIL_EN, () => Application.Quit());
            }
            else
                LoginUI.GetInstance().SetErrorObject(bro.ToString(), () => Application.Quit());
        }
    }

    public void BackendTokenLogin(Action<bool, string> func)
    {
        Enqueue(Backend.BMember.LoginWithTheBackendToken, callback =>
        {
            if (callback.IsSuccess())
            {
                Debug.LogWarning("토큰 로그인 성공");
                loginSuccessFunc = func;
                OnBackendAuthorized();
                return;
            }
            else
            {
                Debug.LogWarning("토큰 로그인 실패 : " + callback.ToString());

                if(callback.GetStatusCode() == "403")
                {
                    func(false, PlayerPrefs.GetString("Langauge") == "ko" ? SIGN_GUEST_FAIL : SIGN_GUEST_FAIL_EN);
                }
                if (callback.IsClientRequestFailError())
                {
                    func(false, PlayerPrefs.GetString("Langauge") == "ko" ? NETWORK_FAIL : NETWORK_FAIL_EN);
                }
                else if (callback.IsServerError() || callback.IsMaintenanceError())
                {
                    func(false, PlayerPrefs.GetString("Langauge") == "ko" ? SERVER_FAIL : SERVER_FAIL_EN);
                }
                else if (callback.IsBadAccessTokenError())
                {
                    func(false, PlayerPrefs.GetString("Langauge") == "ko" ? TOKEN_FAIL : TOKEN_FAIL_EN);
                }
                else
                    func(false, string.Empty);
                Backend.BMember.DeleteGuestInfo();
            }
        });
    }

    #region Google Login Logic
    private string GetFederationToken()
    {
#if UNITY_ANDROID
        if (!PlayGamesPlatform.Instance.localUser.authenticated)
        {
            Debug.LogError("GPGS에 접속되어 있지 않습니다.");
            return string.Empty;
        }

        string _IDtoken = PlayGamesPlatform.Instance.GetIdToken();
        return _IDtoken;
#endif
    }

    public void GoogleAuthorizeFederation(Action<bool, string> func)
    {
        if (Social.localUser.authenticated)
        {
            var token = GetFederationToken();
            if (token.Equals(string.Empty))
            {
                Debug.LogError("GPGS 토큰이 존재하지 않습니다.");
                func(false, "구글 로그인에 실패하였습니다.\n네트워크를 확인해주세요.");
                return;
            }

            Enqueue(Backend.BMember.AuthorizeFederation, token, FederationType.Google, "GPGS 인증", callback =>
            {
                if (callback.IsSuccess())
                {
                    Debug.LogWarning("GPGS 인증 성공");
                    loginSuccessFunc = func;

                    OnBackendAuthorized();
                    return;
                }

                if (callback.GetStatusCode() == "403")
                {
                    func(false, PlayerPrefs.GetString("Langauge") == "ko" ? SIGNUP_FAIL : SIGNUP_FAIL_EN);
                }
                if (callback.IsClientRequestFailError())
                {
                    func(false, PlayerPrefs.GetString("Langauge") == "ko" ? NETWORK_FAIL : NETWORK_FAIL_EN);
                }
                else if (callback.IsServerError() || callback.IsMaintenanceError())
                {
                    func(false, PlayerPrefs.GetString("Langauge") == "ko" ? SERVER_FAIL : SERVER_FAIL_EN);
                }
                else if (callback.IsBadAccessTokenError())
                {
                    func(false, PlayerPrefs.GetString("Langauge") == "ko" ? TOKEN_FAIL : TOKEN_FAIL_EN);
                }
                else
                    func(false, callback.ToString());
            });
        }

        else
        {
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    var token = GetFederationToken();
                    if (token.Equals(string.Empty))
                    {
                        Debug.LogError("GPGS 토큰이 존재하지 않습니다.");
                        func(false, "GPGS 인증을 실패하였습니다.\nGPGS 토큰이 존재하지 않습니다.");
                        return;
                    }

                    Enqueue(Backend.BMember.AuthorizeFederation, token, FederationType.Google, "GPGS 인증", callback =>
                    {
                        if (callback.IsSuccess())
                        {
                            Debug.LogWarning("GPGS 인증 성공");
                            loginSuccessFunc = func;

                            OnBackendAuthorized();
                            return;
                        }

                        Debug.LogError("GPGS 인증 실패\n" + callback.ToString());
                        if (callback.GetStatusCode() == "403")
                        {
                            func(false, PlayerPrefs.GetString("Langauge") == "ko" ? SIGNUP_FAIL : SIGNUP_FAIL_EN);
                        }

                        if (callback.IsClientRequestFailError())
                        {
                            func(false, PlayerPrefs.GetString("Langauge") == "ko" ? NETWORK_FAIL : NETWORK_FAIL_EN);
                        }
                        else if (callback.IsServerError() || callback.IsMaintenanceError())
                        {
                            func(false, PlayerPrefs.GetString("Langauge") == "ko" ? SERVER_FAIL : SERVER_FAIL_EN);
                        }
                        else if (callback.IsBadAccessTokenError())
                        {
                            func(false, PlayerPrefs.GetString("Langauge") == "ko" ? TOKEN_FAIL : TOKEN_FAIL_EN);
                        }
                        else
                            func(false, callback.ToString());
                    });
                }
                else
                {
                    Debug.LogError("GPGS 로그인 실패");
                    func(false, "GPGS 인증을 실패하였습니다. 예외처리 X");
                    return;
                }
            });
        }
    }

#endregion

    #region Apple Login Logic
    public void AppleAuthorizeFederation(Action<bool, string> func)
    {
        var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

        appleAuthManager.LoginWithAppleId(loginArgs, credential =>
        {
            var appleIdCredential = credential as IAppleIDCredential;
            if (appleIdCredential != null)
            {
                var userId = appleIdCredential.User;
                //PlayerPrefs.SetString(appleToken, userId);

                var email = appleIdCredential.Email;

                var fullName = appleIdCredential.FullName;

                var identityToken = Encoding.UTF8.GetString(
                            appleIdCredential.IdentityToken,
                            0,
                            appleIdCredential.IdentityToken.Length);

                var authorizationCode = Encoding.UTF8.GetString(
                            appleIdCredential.AuthorizationCode,
                            0,
                            appleIdCredential.AuthorizationCode.Length);
                Debug.LogError("Apple Login.....");

                Enqueue(Backend.BMember.AuthorizeFederation, identityToken, FederationType.Apple, "apple 인증", callback =>
                {
                    if (callback.IsSuccess())
                    {
                        loginSuccessFunc = func;
                        Debug.LogError("애플로그인 성공");
                        OnBackendAuthorized();
                        return;
                    }

                    Debug.LogError("Apple 로그인 에러\n" + callback.ToString());
                    if (callback.GetStatusCode() == "403")
                    {
                        func(false, PlayerPrefs.GetString("Langauge") == "ko" ? SIGNUP_FAIL : SIGNUP_FAIL_EN);
                    }

                    if (callback.IsClientRequestFailError())
                    {
                        func(false, PlayerPrefs.GetString("Langauge") == "ko" ? NETWORK_FAIL : NETWORK_FAIL_EN);
                    }
                    else if (callback.IsServerError() || callback.IsMaintenanceError())
                    {
                        func(false, PlayerPrefs.GetString("Langauge") == "ko" ? SERVER_FAIL : SERVER_FAIL_EN);
                    }
                    else if (callback.IsBadAccessTokenError())
                    {
                        func(false, PlayerPrefs.GetString("Langauge") == "ko" ? TOKEN_FAIL : TOKEN_FAIL_EN);
                    }
                    else
                        func(false, callback.ToString());
                });
            }
        },
        error =>
        {
            var authorizationErrorCode = error.GetAuthorizationErrorCode();
            Debug.LogError(authorizationErrorCode);

            func(false, authorizationErrorCode.ToString());
        });
    }
#endregion

    #region Guest Login Logic
    public void GuestLogin(Action<bool, string> func)
    {
        Enqueue(Backend.BMember.GuestLogin, callback =>
        {
            if (callback.IsSuccess())
            {
                Debug.LogWarning("게스트 로그인 성공");
                loginSuccessFunc = func;

                OnBackendAuthorized();
                return;
            }

            Debug.LogError("게스트 로그인 실패 : " + callback.ToString());
            if (callback.GetErrorCode().ToString() == "ForbiddenException")
            {
                func(false, PlayerPrefs.GetString("Langauge") == "ko" ? SIGN_GUEST_FAIL : SIGN_GUEST_FAIL_EN);
            }

            if (callback.IsClientRequestFailError())
            {
                func(false, PlayerPrefs.GetString("Langauge") == "ko" ? NETWORK_FAIL : NETWORK_FAIL_EN);
            }
            else if (callback.IsServerError() || callback.IsMaintenanceError())
            {
                func(false, PlayerPrefs.GetString("Langauge") == "ko" ? SERVER_FAIL : SERVER_FAIL_EN);
            }
            else if (callback.IsBadAccessTokenError())
            {
                func(false, PlayerPrefs.GetString("Langauge") == "ko" ? TOKEN_FAIL : TOKEN_FAIL_EN);
            }
            else
                func(false, callback.ToString());
        });
    }
#endregion

    #region Nickname Setting
    public void UpdateNickname(string nickname, Action<bool, string> func)
    {
        Enqueue(Backend.BMember.UpdateNickname, nickname, callback =>
        {
            if (!callback.IsSuccess())
            {
                if(callback.GetErrorCode().ToString() == "DuplicatedParameterException")
                {
                    Debug.LogError("중복");
                    func(false, PlayerPrefs.GetString("Langauge") == "ko" ? NICKNAME_DUPLICATION_FAIL : NICKNAME_DUPLICATION_FAIL_EN);
                }
                else if(callback.GetErrorCode().ToString() == "BadParameterException")
                {
                    Debug.LogError(NICKNAME_BLANK_FAIL);

                    func(false, PlayerPrefs.GetString("Langauge") == "ko" ? NICKNAME_BLANK_FAIL : NICKNAME_BLANK_FAIL_EN);
                }
                else
                {
                    Debug.LogError(NICKNAME_EMPTY_FAIL);
                    func(false, PlayerPrefs.GetString("Langauge") == "ko" ? NICKNAME_EMPTY_FAIL : NICKNAME_EMPTY_FAIL_EN);
                }

                return;
            }

            loginSuccessFunc = func;
            OnBackendAuthorized();
        });
    }
    #endregion

    #region 구글 로그아웃
    public void LogOutWithGoogle()
    {
        if (PlayGamesPlatform.Instance.IsAuthenticated())
        {
            PlayGamesPlatform.Instance.SignOut();
            PlayGamesPlatform.Activate();
        }
    }
    #endregion

    #region LogOut
    public void LogOut()
    {
        Backend.BMember.Logout();
    }
    #endregion

    #region User Information
    private void OnBackendAuthorized()
    {
        Enqueue(Backend.BMember.GetUserInfo, callback =>
        {
            if (!callback.IsSuccess())
            {
                Debug.LogError("유저 정보 불러오기 실패 : " + callback);
                loginSuccessFunc(false, callback.ToString());
                return;
            }

            var info = callback.GetReturnValuetoJSON()["row"];

            if(info["nickname"] == null)
            {
                LoginUI.GetInstance().ActiveNicknameObject();
                return;
            }


            myInfo.gamerID = info["gamerId"].ToString();
            myInfo.nickName = info["nickname"].ToString();
            myInfo.indate = info["inDate"].ToString();

            if(loginSuccessFunc != null)
            {
                Debug.LogWarning("씬 이동 !!!");
                LoginUI.GetInstance().SuccessLogin(loginSuccessFunc);

                Backend.Notification.Connect();

                InitializeCheck();
            }

            //Error
            else
            {
                if (callback.GetStatusCode() == "403")
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? SIGNUP_FAIL : SIGNUP_FAIL_EN);
                }
                if (callback.IsClientRequestFailError())
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? NETWORK_FAIL : NETWORK_FAIL_EN);
                }
                else if (callback.IsServerError() || callback.IsMaintenanceError())
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? SERVER_FAIL : SERVER_FAIL_EN);
                }
                else if (callback.IsBadAccessTokenError())
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? TOKEN_FAIL : TOKEN_FAIL_EN);
                }
                else
                    LoginUI.GetInstance().SetErrorObject(callback.ToString());
            }
        });
    }
    #endregion

    #region 신규 유저 체크
    public void InitializeCheck()
    {
        LoginUI.GetInstance().SetProgressObject("유저 데이터 확인중...");
        Enqueue(Backend.GameData.GetMyData, "User", new Where(), callback =>
        {
            if (callback.IsSuccess())
            {
                JsonData userData = callback.GetReturnValuetoJSON()["rows"];

                //신규 유저가 아닐 경우
                if(userData.Count != 0)
                {
                    Debug.LogWarning("기존 유저 확인");
                    LoginUI.GetInstance().SetProgressObject("기존 유저 확인");
                    myInfo.userIndate = userData[0]["inDate"]["S"].ToString();

                    //데이터 수



                    myInfo.gold = int.Parse(callback.GetReturnValuetoJSON()["rows"][0]["Cash_Gold"]["N"].ToString());
                    myInfo.diamond = int.Parse(callback.GetReturnValuetoJSON()["rows"][0]["Cash_Diamond"]["N"].ToString());

                    myInfo.fertilizer = int.Parse(callback.GetReturnValuetoJSON()["rows"][0]["Inventory_Fertilizer1"]["N"].ToString());
                    myInfo.fertilizer2 = int.Parse(callback.GetReturnValuetoJSON()["rows"][0]["Inventory_Fertilizer2"]["N"].ToString());

                    weather_Sun = callback.GetReturnValuetoJSON()["rows"][0]["Weather_Sun"]["S"].ToString();
                    weather_Cloud = callback.GetReturnValuetoJSON()["rows"][0]["Weather_Cloud"]["S"].ToString();

                    Debug.LogWarning("A : " + callback.GetReturnValuetoJSON()["rows"][0]["Weather_Sun"]["S"].ToString());


                    for (int j = 0; j < 18; j++)
                    {
                        myInfo.harvest[j] = int.Parse(callback.GetReturnValuetoJSON()["rows"][0]["Harvest_" + j]["N"].ToString());
                    }

                    for (int i = 0; i < 9; i++)
                    {
                        myInfo.seed[i] = int.Parse(callback.GetReturnValuetoJSON()["rows"][0]["Seed_" + i]["N"].ToString());

                        field.Add(i, int.Parse(callback.GetReturnValuetoJSON()["rows"][0]["Field_" + i]["N"].ToString()));

                        fieldType.Add(i, int.Parse(callback.GetReturnValuetoJSON()["rows"][0]["FieldType_" + i]["N"].ToString()));

                        fieldEndTime.Add(i, callback.GetReturnValuetoJSON()["rows"][0]["Field_" + i.ToString() + "_EndTime"]["S"].ToString());

                        TableType.Add(i, int.Parse(callback.GetReturnValuetoJSON()["rows"][0]["TableType_" + i]["N"].ToString()));
                        TableCount.Add(i, int.Parse(callback.GetReturnValuetoJSON()["rows"][0]["Table_" + i + "_Count"]["N"].ToString()));
                    }

                    representImage = callback.GetReturnValuetoJSON()["rows"][0]["RepresentImage"]["S"].ToString();

                    LanguageSheet();
                }

                //신규 유저인 경우
                else
                {
                    LoginUI.GetInstance().SetProgressObject("신규 유저 확인");
                    Debug.LogError("신규 유저 확인");
                    CreateInitialUserInfo();
                }
            }

            //Error
            else
            {
                if (callback.GetStatusCode() == "403")
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? SIGNUP_FAIL : SIGNUP_FAIL_EN);
                }
                if (callback.IsClientRequestFailError())
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? NETWORK_FAIL : NETWORK_FAIL_EN);
                }
                else if (callback.IsServerError() || callback.IsMaintenanceError())
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? SERVER_FAIL : SERVER_FAIL_EN);
                }
                else if (callback.IsBadAccessTokenError())
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? TOKEN_FAIL : TOKEN_FAIL_EN);
                }
                else
                    LoginUI.GetInstance().SetErrorObject(callback.ToString());
            }
        });
    }

    public void CreateInitialUserInfo()
    {

        Param param = new Param();

        //Money Check
        param.Add("Cash_Gold", 1000);
        param.Add("Cash_Diamond", 1000);

        //Tutorial Check
        param.Add("Tutorial_1", false);
        param.Add("Tutorial_2", false);
        param.Add("Tutorial_3", false);

        //Inventory Check

        param.Add("Inventory_Fertilizer1", 5);
        param.Add("Inventory_Fertilizer2", 5);

        param.Add("Weather_Sun", "");
        param.Add("Weather_Cloud", "");


        for (int i = 0; i < 9; i++)
        {
            //Field Check
            param.Add("Field_" + i, 0);

            param.Add("FieldType_" + i, 0);

            //Field End Time Check
            param.Add("Field_" + i + "_EndTime", "");


            //Mart Type
            param.Add("TableType_" + i, 1);
            param.Add("Table_" + i + "_Count", 0);

            param.Add("Seed_" + i, 5);
        }

        for(int i = 0; i < 18; i++)
        {
            param.Add("Harvest_" + i, 5);
        }

        //Represent Image Check
        param.Add("RepresentImage", "0");

        Enqueue(Backend.GameData.Insert, "User", param, callback =>
        {
            LoginUI.GetInstance().SetProgressObject("신규 데이터 생성중...");
            if (callback.IsSuccess())
            {
                Enqueue(Backend.GameData.Get, "User", new Where(), callback2 =>
                {
                    if (callback2.IsSuccess())
                    {

                        JsonData userData = callback2.GetReturnValuetoJSON()["rows"];
                        myInfo.userIndate = userData[0]["inDate"]["S"].ToString();

                        //데이터 수
                        myInfo.gold = int.Parse(callback2.GetReturnValuetoJSON()["rows"][0]["Cash_Gold"]["N"].ToString());
                        myInfo.diamond = int.Parse(callback2.GetReturnValuetoJSON()["rows"][0]["Cash_Diamond"]["N"].ToString());

                        myInfo.fertilizer = int.Parse(callback2.GetReturnValuetoJSON()["rows"][0]["Inventory_Fertilizer1"]["N"].ToString());
                        myInfo.fertilizer2 = int.Parse(callback2.GetReturnValuetoJSON()["rows"][0]["Inventory_Fertilizer2"]["N"].ToString());

                        weather_Sun = callback2.GetReturnValuetoJSON()["rows"][0]["Weather_Sun"]["S"].ToString();
                        weather_Cloud = callback2.GetReturnValuetoJSON()["rows"][0]["Weather_Cloud"]["S"].ToString();

                        for(int i = 0; i < 18; i++)
                        {
                            myInfo.harvest[i] = int.Parse(callback2.GetReturnValuetoJSON()["rows"][0]["Harvest_" + i]["N"].ToString());
                        }

                        for (int i = 0; i < 9; i++)
                        {
                            myInfo.seed[i] = int.Parse(callback2.GetReturnValuetoJSON()["rows"][0]["Seed_" + i]["N"].ToString());

                            fieldType.Add(i, int.Parse(callback2.GetReturnValuetoJSON()["rows"][0]["FieldType_" + i.ToString()]["N"].ToString()));
                            field.Add(i, int.Parse(callback2.GetReturnValuetoJSON()["rows"][0]["Field_" + i.ToString()]["N"].ToString()));

                            fieldEndTime.Add(i, callback2.GetReturnValuetoJSON()["rows"][0]["Field_" + i.ToString() + "_EndTime"]["S"].ToString());

                            TableType.Add(i, int.Parse(callback2.GetReturnValuetoJSON()["rows"][0]["TableType_" + i]["N"].ToString()));
                            TableCount.Add(i, int.Parse(callback2.GetReturnValuetoJSON()["rows"][0]["Table_" + i + "_Count"]["N"].ToString()));
                        }

                        representImage = callback2.GetReturnValuetoJSON()["rows"][0]["RepresentImage"]["S"].ToString();

                        LanguageSheet();
                    }
                    else
                    {
                        Debug.LogError(callback2.ToString());
                    }
                    
                });
            }

            //Error
            else
            {
                if (callback.GetStatusCode() == "403")
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? SIGNUP_FAIL : SIGNUP_FAIL_EN);
                }
                if (callback.IsClientRequestFailError())
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? NETWORK_FAIL : NETWORK_FAIL_EN);
                }
                else if (callback.IsServerError() || callback.IsMaintenanceError())
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? SERVER_FAIL : SERVER_FAIL_EN);
                }
                else if (callback.IsBadAccessTokenError())
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? TOKEN_FAIL : TOKEN_FAIL_EN);
                }
                else
                    LoginUI.GetInstance().SetErrorObject(callback.ToString());
            }
        });
    }
    #endregion

    #region Backend Sheet

    #region 언어 시트
    public class Langauge
    {
        public string ko;
        public string en;
    }
    public List<Langauge> langaugeSheet = new List<Langauge>();

    public void LanguageSheet()
    {
        Enqueue(Backend.Chart.GetChartContents, langaugeSheetCode, callback =>
        {
            if (callback.IsSuccess())
            {
                JsonData json = callback.FlattenRows();

                langaugeSheet = new List<Langauge>();
                for (int i = 0; i < json.Count; i++)
                {
                    Langauge lg = new Langauge();
                    lg.ko = json[i]["ko"].ToString();
                    lg.en = json[i]["en"].ToString();


                    langaugeSheet.Add(lg);
                }

                Debug.LogWarning("언어 시트 적용");

                FieldSheet();

            }

            //Error
            else
            {
                if (callback.GetStatusCode() == "403")
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? SIGNUP_FAIL : SIGNUP_FAIL_EN);
                }
                if (callback.IsClientRequestFailError())
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? NETWORK_FAIL : NETWORK_FAIL_EN);
                }
                else if (callback.IsServerError() || callback.IsMaintenanceError())
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? SERVER_FAIL : SERVER_FAIL_EN);
                }
                else if (callback.IsBadAccessTokenError())
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? TOKEN_FAIL : TOKEN_FAIL_EN);
                }
                else
                    LoginUI.GetInstance().SetErrorObject(callback.ToString());
            }
        });
    }
    #endregion

    #region 밭 시트
    public class Field
    {
        public bool isGold;
        public int price;
    }
    public List<Field> fieldSheet = new List<Field>();

    public void FieldSheet()
    {
        Enqueue(Backend.Chart.GetChartContents, fieldSheetCode, callback =>
        {
            if (callback.IsSuccess())
            {
                JsonData json = callback.FlattenRows();

                fieldSheet = new List<Field>();
                for (int i = 0; i < json.Count; i++)
                {
                    Field fl = new Field();
                    fl.isGold = json[i]["Type"].ToString() == "Gold" ? true : false;
                    fl.price = int.Parse(json[i]["Need"].ToString());

                    fieldSheet.Add(fl);
                }

                Debug.LogWarning("밭 가격 시트 적용");

                ShopSheet();
            }

            //Error
            else
            {
                if (callback.GetStatusCode() == "403")
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? SIGNUP_FAIL : SIGNUP_FAIL_EN);
                }
                if (callback.IsClientRequestFailError())
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? NETWORK_FAIL : NETWORK_FAIL_EN);
                }
                else if (callback.IsServerError() || callback.IsMaintenanceError())
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? SERVER_FAIL : SERVER_FAIL_EN);
                }
                else if (callback.IsBadAccessTokenError())
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? TOKEN_FAIL : TOKEN_FAIL_EN);
                }
                else
                    LoginUI.GetInstance().SetErrorObject(callback.ToString());
            }
        });
    }
    #endregion

    #region 상점 시트
    public class Shop
    {
        public int type;
        public string name;
        public string name_EN;
        public string cash;
        public int price;
        public int price_EN;
        public string compensate_Type;
        public int compensate_Price;
        public int bonus;
    }
    public List<Shop> shopSheet = new List<Shop>();

    public void ShopSheet()
    {
        Enqueue(Backend.Chart.GetChartContents, shopSheetCode, callback =>
        {
            if (callback.IsSuccess())
            {
                JsonData json = callback.FlattenRows();

                shopSheet = new List<Shop>();
                for (int i = 0; i < json.Count; i++)
                {
                    Shop sh = new Shop();
                    sh.type = int.Parse(json[i]["Type"].ToString());
                    sh.name = json[i]["Name"].ToString();
                    sh.name_EN = json[i]["Name_EN"].ToString();
                    sh.cash = json[i]["Cash"].ToString();
                    sh.price = int.Parse(json[i]["Price"].ToString());
                    sh.price_EN = int.Parse(json[i]["Price_EN"].ToString());
                    sh.compensate_Type = json[i]["Compensate_Type"].ToString();
                    sh.compensate_Price = int.Parse(json[i]["Compensate_Price"].ToString());
                    sh.bonus = int.Parse(json[i]["Bonus"].ToString());

                    shopSheet.Add(sh);
                }

                Debug.LogWarning("상점 가격 시트 적용");

                MartSheet();

            }

            //Error
            else
            {
                if (callback.GetStatusCode() == "403")
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? SIGNUP_FAIL : SIGNUP_FAIL_EN);
                }
                if (callback.IsClientRequestFailError())
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? NETWORK_FAIL : NETWORK_FAIL_EN);
                }
                else if (callback.IsServerError() || callback.IsMaintenanceError())
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? SERVER_FAIL : SERVER_FAIL_EN);
                }
                else if (callback.IsBadAccessTokenError())
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? TOKEN_FAIL : TOKEN_FAIL_EN);
                }
                else
                    LoginUI.GetInstance().SetErrorObject(callback.ToString());
            }
        });
    }
    #endregion

    #region 마트 시트
    public class Mart
    {
        public string name;
        public int count;
        public int cost;
    }
    public List<Mart> martSheet = new List<Mart>();

    public void MartSheet()
    {
        Enqueue(Backend.Chart.GetChartContents, martSheetCode, callback =>
        {
            if (callback.IsSuccess())
            {
                JsonData json = callback.FlattenRows();

                martSheet = new List<Mart>();
                for (int i = 0; i < json.Count; i++)
                {
                    Mart mt = new Mart();
                    mt.name = json[i]["Harvest"].ToString();
                    mt.count = int.Parse(json[i]["Count"].ToString());
                    mt.cost = int.Parse(json[i]["Cost"].ToString());

                    martSheet.Add(mt);
                }

                Debug.LogWarning("마트 가격 시트 적용");

                GuestSheet();

            }

            //Error
            else
            {
                if (callback.GetStatusCode() == "403")
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? SIGNUP_FAIL : SIGNUP_FAIL_EN);
                }
                if (callback.IsClientRequestFailError())
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? NETWORK_FAIL : NETWORK_FAIL_EN);
                }
                else if (callback.IsServerError() || callback.IsMaintenanceError())
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? SERVER_FAIL : SERVER_FAIL_EN);
                }
                else if (callback.IsBadAccessTokenError())
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? TOKEN_FAIL : TOKEN_FAIL_EN);
                }
                else
                    LoginUI.GetInstance().SetErrorObject(callback.ToString());
            }
        });
    }
    #endregion

    #region 게스트 설정 시트
    public class Guest
    {
        public int spawnCoolTime;
        public int percentEnterMart;
    }
    public List<Guest> guestSheet = new List<Guest>();

    public void GuestSheet()
    {
        Enqueue(Backend.Chart.GetChartContents, guestSheetCode, callback =>
        {
            if (callback.IsSuccess())
            {
                JsonData json = callback.FlattenRows();

                guestSheet = new List<Guest>();
                for (int i = 0; i < json.Count; i++)
                {
                    Guest gt = new Guest();

                    gt.spawnCoolTime = int.Parse(json[i]["SpawnCoolTime"].ToString());
                    gt.percentEnterMart = int.Parse(json[i]["EnterMart"].ToString());

                    guestSheet.Add(gt);
                }

                Debug.LogWarning("게스트 설정 시트 적용");

                HarvestSheet();

            }

            //Error
            else
            {
                if (callback.GetStatusCode() == "403")
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? SIGNUP_FAIL : SIGNUP_FAIL_EN);
                }
                if (callback.IsClientRequestFailError())
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? NETWORK_FAIL : NETWORK_FAIL_EN);
                }
                else if (callback.IsServerError() || callback.IsMaintenanceError())
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? SERVER_FAIL : SERVER_FAIL_EN);
                }
                else if (callback.IsBadAccessTokenError())
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? TOKEN_FAIL : TOKEN_FAIL_EN);
                }
                else
                    LoginUI.GetInstance().SetErrorObject(callback.ToString());
            }
        });
    }
    #endregion

    #region 농작물 설정 시트
    public class Harvest
    {
        public int coolTime;
    }
    public List<Harvest> harvestSheet = new List<Harvest>();

    public void HarvestSheet()
    {
        Enqueue(Backend.Chart.GetChartContents, harvestSheetCode, callback =>
        {
            if (callback.IsSuccess())
            {
                JsonData json = callback.FlattenRows();

                harvestSheet = new List<Harvest>();
                for (int i = 0; i < json.Count; i++)
                {
                    Harvest hs = new Harvest();

                    hs.coolTime = int.Parse(json[i]["CoolTime"].ToString());

                    harvestSheet.Add(hs);
                }

                Debug.LogWarning("수확물 쿨타임 설정 시트 적용");

                WeatherSheet();

            }

            //Error
            else
            {
                if (callback.GetStatusCode() == "403")
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? SIGNUP_FAIL : SIGNUP_FAIL_EN);
                }
                if (callback.IsClientRequestFailError())
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? NETWORK_FAIL : NETWORK_FAIL_EN);
                }
                else if (callback.IsServerError() || callback.IsMaintenanceError())
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? SERVER_FAIL : SERVER_FAIL_EN);
                }
                else if (callback.IsBadAccessTokenError())
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? TOKEN_FAIL : TOKEN_FAIL_EN);
                }
                else
                    LoginUI.GetInstance().SetErrorObject(callback.ToString());
            }
        });
    }
    #endregion

    #region 날씨 설정 시트
    public class Weather
    {
        public int coolTime;
    }
    public List<Weather> weatherSheet = new List<Weather>();

    public void WeatherSheet()
    {
        Enqueue(Backend.Chart.GetChartContents, weatherSheetCode, callback =>
        {
            if (callback.IsSuccess())
            {
                JsonData json = callback.FlattenRows();

                weatherSheet = new List<Weather>();
                for (int i = 0; i < json.Count; i++)
                {
                    Weather wt = new Weather();

                    wt.coolTime = int.Parse(json[i]["CoolTime"].ToString());

                    weatherSheet.Add(wt);
                }

                Debug.LogWarning("날씨 쿨타임 설정 시트 적용");

                Scene_Manager.GetInstance().ChangeState(Scene_Manager.GameState.Farm);

            }

            //Error
            else
            {
                if (callback.GetStatusCode() == "403")
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? SIGNUP_FAIL : SIGNUP_FAIL_EN);
                }
                if (callback.IsClientRequestFailError())
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? NETWORK_FAIL : NETWORK_FAIL_EN);
                }
                else if (callback.IsServerError() || callback.IsMaintenanceError())
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? SERVER_FAIL : SERVER_FAIL_EN);
                }
                else if (callback.IsBadAccessTokenError())
                {
                    LoginUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? TOKEN_FAIL : TOKEN_FAIL_EN);
                }
                else
                    LoginUI.GetInstance().SetErrorObject(callback.ToString());
            }
        });
    }
    #endregion

    #endregion

    #region 내 정보 저장
    public void SaveMyInfo(bool isAuto = false)
    {
        if(!isAuto)
            FarmUI.GetInstance().SetLoading();

        Param param = new Param();
        param.Add("Cash_Gold", myInfo.gold);
        param.Add("Cash_Diamond", myInfo.diamond);

        param.Add("Inventory_Fertilizer1", myInfo.fertilizer);
        param.Add("Inventory_Fertilizer2", myInfo.fertilizer2);

        param.Add("Weather_Sun", weather_Sun);
        param.Add("Weather_Cloud", weather_Cloud);

        for(int i = 0; i < 18; i++)
        {
            param.Add("Harvest_" + i, myInfo.harvest[i]);
        }

        for (int i = 0; i < 9; i++)
        {
            param.Add("Seed_" + i, myInfo.seed[i]);
            param.Add("FieldType_" + i, fieldType[i]);
            param.Add("Field_" + i, field[i]);

            param.Add("Field_" + i + "_EndTime", fieldEndTime[i]);

            param.Add("TableType_" + i, TableType[i]);
            param.Add("Table_" + i + "_Count", TableCount[i]);
        }

        param.Add("RepresentImage", FarmUI.GetInstance().nowImage);

        Enqueue(Backend.GameData.Update, "User", new Where(), param, callback =>
        {
            if(!isAuto)
                FarmUI.GetInstance().SetLoading(false);

            if (callback.IsSuccess())
            {
                Debug.LogError("Save Success");

                FarmUI.GetInstance().InitializeTable();
            }

            if (!callback.IsSuccess())
            {
                if (callback.IsClientRequestFailError())
                {
                    FarmUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? NETWORK_FAIL : NETWORK_FAIL_EN, () => Application.Quit());
                }
                else if (callback.IsServerError() || callback.IsMaintenanceError())
                {
                    FarmUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? SERVER_FAIL : SERVER_FAIL_EN, () => Application.Quit());
                }
                else if (callback.IsBadAccessTokenError())
                {
                    FarmUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? TOKEN_FAIL : TOKEN_FAIL_EN, () => Application.Quit());
                }
                else
                    FarmUI.GetInstance().SetErrorObject(callback.ToString());
            }

        });
    }
    #endregion

    #region 친구 정보
    public void GetFriendList(Action<bool, List<FriendList>> func)
    {
        Enqueue(Backend.Friend.GetFriendList, callback =>
        {
            if (callback.IsSuccess())
            {

                var friendList = new List<FriendList>();

                foreach (LitJson.JsonData tmp in callback.Rows())
                {
                    if (!tmp.Keys.Contains("nickname")) continue;

                    FriendList friend = new FriendList();
                    friend.nickname = tmp["nickname"]["S"].ToString();
                    friend.ownerIndate = tmp["inDate"]["S"].ToString();

                    Where where = new Where();
                    where.Equal("owner_inDate", friend.ownerIndate);

                    var callback2 = Backend.GameData.Get("User", where);

                    if (callback2.IsSuccess())
                    {
                        Debug.LogError("다른 유저 정보 불러오기 성공!");

                        Debug.LogError(callback2.FlattenRows()[0]["updatedAt"].ToString());
                        DateTime parsedDate = DateTime.Parse(callback2.FlattenRows()[0]["updatedAt"].ToString());
                        TimeSpan span = (DateTime.Now - parsedDate);

                        if (span.TotalMinutes < 60)
                        {
                            friend.lastLogin = Mathf.Floor((float)span.TotalMinutes).ToString() + "분 전";
                        }
                        else if (span.TotalMinutes > 60 && span.TotalMinutes < 1440)
                        {
                            friend.lastLogin = Mathf.Floor((float)span.TotalHours).ToString() + "시간 전";
                        }
                        else
                        {
                            friend.lastLogin = Mathf.Floor((float)span.TotalDays).ToString() + "일 전";
                        }

                        friend.rowIndate = callback2.FlattenRows()[0]["inDate"].ToString();
                        friend.representImage = callback2.FlattenRows()[0]["RepresentImage"].ToString();
                        friendList.Add(friend);
                    }
                }

                func(true, friendList);

            }

            else
            {
                if (callback.IsClientRequestFailError())
                {
                    FarmUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? NETWORK_FAIL : NETWORK_FAIL_EN, () => Application.Quit());
                }
                else if (callback.IsServerError() || callback.IsMaintenanceError())
                {
                    FarmUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? SERVER_FAIL : SERVER_FAIL_EN, () => Application.Quit());
                }
                else if (callback.IsBadAccessTokenError())
                {
                    FarmUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? TOKEN_FAIL : TOKEN_FAIL_EN, () => Application.Quit());
                }
                else
                    FarmUI.GetInstance().SetErrorObject(callback.ToString());

                func(false, null);
            }
        });
    }


    public void GetReceivedRequestFriendList(Action<bool, List<FriendList>> func)
    {
        Enqueue(Backend.Friend.GetReceivedRequestList, callback =>
        {
            if (callback.IsSuccess())
            {
                var friendList = new List<FriendList>();

                foreach (LitJson.JsonData tmp in callback.Rows())
                {
                    if (!tmp.Keys.Contains("nickname")) continue;

                    FriendList friend = new FriendList();
                    friend.nickname = tmp["nickname"]["S"].ToString();
                    friend.ownerIndate = tmp["inDate"]["S"].ToString();

                    Where where = new Where();
                    where.Equal("owner_inDate", friend.ownerIndate);

                    var callback2 = Backend.GameData.Get("User", where);

                    if (callback2.IsSuccess())
                    {
                        Debug.LogError("다른 유저 정보 불러오기 성공!");
                        friend.rowIndate = callback2.FlattenRows()[0]["inDate"].ToString();
                        friend.representImage = callback2.FlattenRows()[0]["RepresentImage"].ToString();
                        friendList.Add(friend);
                    }
                    else
                        Debug.LogError(callback2);
                }



                func(true, friendList);
            }

            else
            {
                if (callback.IsClientRequestFailError())
                {
                    FarmUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? NETWORK_FAIL : NETWORK_FAIL_EN, () => Application.Quit());
                }
                else if (callback.IsServerError() || callback.IsMaintenanceError())
                {
                    FarmUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? SERVER_FAIL : SERVER_FAIL_EN, () => Application.Quit());
                }
                else if (callback.IsBadAccessTokenError())
                {
                    FarmUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? TOKEN_FAIL : TOKEN_FAIL_EN, () => Application.Quit());
                }
                else
                    FarmUI.GetInstance().SetErrorObject(callback.ToString());

                func(false, null);
            }
        });
    }

    public void GetRecommendFriendList(Action<bool, List<FriendList>> func)
    {
        Enqueue(Backend.Social.GetRandomUserInfo, "User", "Gold", 0, 10000, 25, callback =>
        {
            if (callback.IsSuccess())
            {
                print(callback);
                var friendList = new List<FriendList>();

                foreach (JsonData tmp in callback.Rows())
                {
                    if (!tmp.Keys.Contains("nickname")) continue;

                    FriendList friend = new FriendList();
                    friend.nickname = tmp["nickname"].ToString();
                    friend.ownerIndate = tmp["inDate"].ToString();

                    Where where = new Where();
                    where.Equal("owner_inDate", friend.ownerIndate);

                    var callback2 = Backend.GameData.Get("User", where);

                    if (callback2.IsSuccess())
                    {
                        Debug.LogError("다른 유저 정보 불러오기 성공!");
                        friend.rowIndate = callback2.FlattenRows()[0]["inDate"].ToString();
                        friend.representImage = callback2.FlattenRows()[0]["RepresentImage"].ToString();
                        friendList.Add(friend);
                    }
                    else
                        Debug.LogError(callback2);
                }

                func(true, friendList);
            }

            else
            {
                if (callback.IsClientRequestFailError())
                {
                    FarmUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? NETWORK_FAIL : NETWORK_FAIL_EN, () => Application.Quit());
                }
                else if (callback.IsServerError() || callback.IsMaintenanceError())
                {
                    FarmUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? SERVER_FAIL : SERVER_FAIL_EN, () => Application.Quit());
                }
                else if (callback.IsBadAccessTokenError())
                {
                    FarmUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? TOKEN_FAIL : TOKEN_FAIL_EN, () => Application.Quit());
                }
                else
                    FarmUI.GetInstance().SetErrorObject(callback.ToString());

                func(false, null);
            }
        });
    }


    public void RequestFirend(string nickName, Action<bool, string> func)
    {
        Enqueue(Backend.Social.GetUserInfoByNickName, nickName, callback =>
        {
            Debug.Log(callback);
            if (!callback.IsSuccess())
            {
                switch (callback.GetStatusCode())
                {
                    //존재하지 않는 닉네임
                    case "404":
                        func(false, "존재하지 않는 유저입니다.");
                        break;

                    //이미 요청한 닉네임
                    case "409":
                        func(false, "이미 요청을 보낸 유저입니다.");
                        break;
                }

                if (callback.IsClientRequestFailError())
                {
                    FarmUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? NETWORK_FAIL : NETWORK_FAIL_EN, () => Application.Quit());
                }
                else if (callback.IsServerError() || callback.IsMaintenanceError())
                {
                    FarmUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? SERVER_FAIL : SERVER_FAIL_EN, () => Application.Quit());
                }
                else if (callback.IsBadAccessTokenError())
                {
                    FarmUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? TOKEN_FAIL : TOKEN_FAIL_EN, () => Application.Quit());
                }
                else
                    FarmUI.GetInstance().SetErrorObject(callback.ToString());
                return;
            }

            string inDate = callback.GetReturnValuetoJSON()["row"]["inDate"].ToString();
            Enqueue(Backend.Friend.RequestFriend, inDate, callback2 =>
            {
                Debug.Log(callback2);
                if (!callback2.IsSuccess())
                {
                    switch (callback2.GetStatusCode())
                    {
                        //존재하지 않는 닉네임
                        case "404":
                            func(false, "존재하지 않는 유저입니다.");
                            break;

                        //이미 요청한 닉네임
                        case "409":
                            func(false, "이미 요청을 보낸 유저입니다.");
                            break;
                    }

                    if (callback.IsClientRequestFailError())
                    {
                        FarmUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? NETWORK_FAIL : NETWORK_FAIL_EN, () => Application.Quit());
                    }
                    else if (callback.IsServerError() || callback.IsMaintenanceError())
                    {
                        FarmUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? SERVER_FAIL : SERVER_FAIL_EN, () => Application.Quit());
                    }
                    else if (callback.IsBadAccessTokenError())
                    {
                        FarmUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? TOKEN_FAIL : TOKEN_FAIL_EN, () => Application.Quit());
                    }
                    else
                        FarmUI.GetInstance().SetErrorObject(callback.ToString());

                    return;
                }

                func(true, string.Empty);
            });
        });
    }

    public void AcceptFriend(string inDate, Action<bool, string> func)
    {
        Enqueue(Backend.Friend.AcceptFriend, inDate, callback =>
        {
            if (callback.IsSuccess() == false)
            {
                if (callback.IsClientRequestFailError())
                {
                    FarmUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? NETWORK_FAIL : NETWORK_FAIL_EN, () => Application.Quit());
                }
                else if (callback.IsServerError() || callback.IsMaintenanceError())
                {
                    FarmUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? SERVER_FAIL : SERVER_FAIL_EN, () => Application.Quit());
                }
                else if (callback.IsBadAccessTokenError())
                {
                    FarmUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? TOKEN_FAIL : TOKEN_FAIL_EN, () => Application.Quit());
                }
                else
                    FarmUI.GetInstance().SetErrorObject(callback.ToString());

                func(false, callback.ToString());
                return;
            }

            func(true, string.Empty);
        });
    }

    public void RejectFriend(string inDate, Action<bool, string> func)
    {
        Enqueue(Backend.Friend.RejectFriend, inDate, callback =>
        {
            if (callback.IsSuccess() == false)
            {
                if (callback.IsClientRequestFailError())
                {
                    FarmUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? NETWORK_FAIL : NETWORK_FAIL_EN, () => Application.Quit());
                }
                else if (callback.IsServerError() || callback.IsMaintenanceError())
                {
                    FarmUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? SERVER_FAIL : SERVER_FAIL_EN, () => Application.Quit());
                }
                else if (callback.IsBadAccessTokenError())
                {
                    FarmUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? TOKEN_FAIL : TOKEN_FAIL_EN, () => Application.Quit());
                }
                else
                    FarmUI.GetInstance().SetErrorObject(callback.ToString());

                func(false, callback.ToString());
                return;
            }

            func(true, string.Empty);
        });
    }

    public void BreakFriend(string inDate, Action<bool, string> func)
    {
        Enqueue(Backend.Friend.BreakFriend, inDate, callback =>
        {
            if (callback.IsSuccess() == false)
            {
                if (callback.IsClientRequestFailError())
                {
                    FarmUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? NETWORK_FAIL : NETWORK_FAIL_EN, () => Application.Quit());
                }
                else if (callback.IsServerError() || callback.IsMaintenanceError())
                {
                    FarmUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? SERVER_FAIL : SERVER_FAIL_EN, () => Application.Quit());
                }
                else if (callback.IsBadAccessTokenError())
                {
                    FarmUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? TOKEN_FAIL : TOKEN_FAIL_EN, () => Application.Quit());
                }
                else
                    FarmUI.GetInstance().SetErrorObject(callback.ToString());

                func(false, callback.ToString());
                return;
            }

            func(true, string.Empty);
        });
    }
    #endregion

    #region 서버 시간
    public DateTime nowTime()
    {
        BackendReturnObject servertime = Backend.Utils.GetServerTime();

        string time = servertime.GetReturnValuetoJSON()["utcTime"].ToString();
        DateTime parsedDate = DateTime.Parse(time);

        return parsedDate;
    }
    #endregion
}
