using System.Collections;
using System.Collections.Generic;
using System.Text;
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using BackEnd;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
 * LoginSceneManager에 들어갈 기능
 *
 * 1. 신규 유저 확인
 * 2. 닉네임 확인
 * 3. 유저 정보 확인
 */
public partial class LoginSceneManager : Singleton<LoginSceneManager>
{
    [SerializeField] private GameObject titleSpine;
    [SerializeField] private Canvas loginUICanvas;
    [SerializeField] private GameObject loginButtonGroup;   //로그인 버튼 그룹
    [SerializeField] private GameObject copyRightObject;
    [SerializeField] private GameObject versionObject;
    
    private IAppleAuthManager appleAuthManager;
    
    public string appleToken = "";


    void Awake()
    {
        //StaticManager가 없을 경우 새로 생성
        if (FindObjectOfType(typeof(StaticManager)) == null)
        {
            var obj = Resources.Load<GameObject>("Prefabs/StaticManager");
            Instantiate(obj);
        }
        
#if UNITY_ANDROID || UNITY_IOS
        Application.targetFrameRate = 120;
#endif
    }
    void Start()
    {
        titleSpine.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "play", false);
        titleSpine.GetComponent<SkeletonGraphic>().AnimationState.AddAnimation(0, "idle", true, 0);
        
        #region GPGS 플러그인 설정

#if UNITY_ANDROID
        // GPGS 플러그인 설정
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration
                .Builder()
            .RequestServerAuthCode(false)
            .RequestEmail()
            .RequestIdToken()
            .Build();
        //커스텀 된 정보로 GPGS 초기화
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true; // 디버그 로그를 보고 싶지 않다면 false
        PlayGamesPlatform.Activate();
#endif

        #endregion

        #region IOS 플러그인

#if UNITY_IOS
        if (AppleAuthManager.IsCurrentPlatformSupported)
            {
                Debug.LogError("IOS CHECK");
                var deserializer = new PayloadDeserializer();
                appleAuthManager = new AppleAuthManager(deserializer);
            }
#endif

        #endregion

        #region 오브젝트 초기화

        loginButtonGroup.SetActive(false);
        loadingText.gameObject.SetActive(false);
        copyRightObject.SetActive(false);
        versionObject.GetComponentInChildren<TMP_Text>().text = Application.version;
        versionObject.SetActive(false);

        #endregion
        
        if (Application.internetReachability == NetworkReachability.NotReachable)
            StaticManager.UI.AlertUI.OpenUI("네트워크에 연결이 되어 있지 않습니다.\n앱을 다시 실행해주세요.", () =>
            {
                Application.Quit();
            }); 

        StartCoroutine(Initialize());
    }

    //StaticManager 언어 불러온 후 실행
    public IEnumerator Initialize()
    {
        StaticManager.Sound.SetBGM("LoginBGM");
        yield return new WaitForSeconds(3f);

        //로그인 버튼 기능 삽입
        Button[] loginButtons = loginButtonGroup.GetComponentsInChildren<Button>();
        loginButtons[0].onClick.AddListener(() => BackendLogin("Google"));
        loginButtons[1].onClick.AddListener(() => BackendLogin("Apple"));
        loginButtons[2].onClick.AddListener(() => BackendLogin("Guest"));
        loginButtons[3].onClick.AddListener(() => BackendLogin("Test"));

        copyRightObject.SetActive(true);
        versionObject.SetActive(true);
        
#if UNITY_ANDROID
        loginButtons[1].gameObject.SetActive(false);
#elif UNITY_IOS
        loginButtons[0].gameObject.SetActive(false);
#endif
        
        //테스트 종료 시 해당 주석 지울 것
        //loginButtons[3].gameObject.SetActive(false);

        AutoLoginWithBackend();
    }
    
    //PolicyUI => SetLoginButton
    public void SetLoginButton()
    {
        copyRightObject.SetActive(true);
        
        if (loginButtonGroup.activeSelf) return;
        loginButtonGroup.SetActive(true);
    }
    
    /* 자동 로그인 함수 호출
     * 신규 유저 : 회원가입 버튼 활성화
     * 기존 유저 : 자동 로그인
     */
    private void AutoLoginWithBackend()
    {
        SendQueue.Enqueue(Backend.BMember.LoginWithTheBackendToken, callback =>
        {
            Debug.LogWarning($"Backend.BMember.LoginWithTheBackendToken : {callback}");

            //로그인 성공했을 경우(토큰 O)
            if (callback.IsSuccess())
            {

                //닉네임이 없을 경우
                if (string.IsNullOrEmpty(Backend.UserNickName))
                {
                    //정책 UI 활성화
                    copyRightObject.SetActive(false);
                    loginButtonGroup.SetActive(false);
                    GetPolicy();
                }

                //닉네임이 있을 경우
                else
                {
                    InitializeLoading();
                }
            }

            //최초 로그인일 경우
            else
            {
                loginButtonGroup.SetActive(true);
            }
        });
    }
    
    /* 뒤끝 로그인 기능. 이후 처리는 AuthorizeProcess 참고 */
    private void BackendLogin(string platform)
    {
        StaticManager.UI.SetLoading(true);

        switch (platform)
        {
            case "Google":
                if (Social.localUser.authenticated)
                {
                    var token = GetFederationToken();
                    if (token.Equals(string.Empty))
                    {
                        StaticManager.UI.SetLoading(false);

                        Debug.LogError("GPGS 토큰이 존재하지 않습니다.");
                        StaticManager.UI.AlertUI.OpenUI("GPGS 토큰이 존재하지 않습니다.");
                        return;
                    }

                    SendQueue.Enqueue(Backend.BMember.AuthorizeFederation, token, FederationType.Google, AuthorizeProcess);
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
                                StaticManager.UI.SetLoading(false);

                                Debug.LogError("GPGS 토큰이 존재하지 않습니다.");
                                StaticManager.UI.AlertUI.OpenUI("GPGS 토큰이 존재하지 않습니다.");
                                return;
                            }

                            SendQueue.Enqueue(Backend.BMember.AuthorizeFederation, token, FederationType.Google, AuthorizeProcess);
                        }
                        else
                        {
                            StaticManager.UI.SetLoading(false);
                            Debug.LogError("GPGS 토큰이 존재하지 않습니다2.");
                            StaticManager.UI.AlertUI.OpenUI("GPGS 토큰이 존재하지 않습니다2.\n" + success.ToString());
                        }
                    });
                }
                break;
            
            case "Apple":
                
                var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

        this.appleAuthManager.LoginWithAppleId(
            loginArgs,
            credential =>
            {
                Debug.LogError("함수 실!!");

                // Obtained credential, cast it to IAppleIDCredential
                var appleIdCredential = credential as IAppleIDCredential;
                if (appleIdCredential != null)
                {
                    // Apple User ID
                    // You should save the user ID somewhere in the device
                    var userId = appleIdCredential.User;
                    PlayerPrefs.SetString(appleToken, userId);

                    // Email (Received ONLY in the first login)
                    var email = appleIdCredential.Email;

                    // Full name (Received ONLY in the first login)
                    var fullName = appleIdCredential.FullName;

                    // Identity token
                    var identityToken = Encoding.UTF8.GetString(
                                appleIdCredential.IdentityToken,
                                0,
                                appleIdCredential.IdentityToken.Length);

                    // Authorization code
                    var authorizationCode = Encoding.UTF8.GetString(
                                appleIdCredential.AuthorizationCode,
                                0,
                                appleIdCredential.AuthorizationCode.Length);
                    Debug.LogError("Apple Login.....");

                    // And now you have all the information to create/login a user in your system
                    SendQueue.Enqueue(Backend.BMember.AuthorizeFederation, identityToken, FederationType.Apple, "apple 인증", callback =>
                    {
                        if (callback.IsSuccess())
                        {
                            Debug.LogError("애플로그인 성공");
                            StaticManager.UI.SetLoading(false);
                            SendQueue.Enqueue(Backend.BMember.AuthorizeFederation, identityToken, FederationType.Apple, AuthorizeProcess);
                            return;
                        }
                        StaticManager.UI.SetLoading(false);
                        Debug.LogError("Apple 로그인 에러\n" + callback.ToString());
                    });
                }
            },
            error =>
            {
                // Something went wrong
                StaticManager.UI.SetLoading(false);
                var authorizationErrorCode = error.GetAuthorizationErrorCode();
                Debug.LogError(authorizationErrorCode);
            });
                
                break;
            
            case "Guest":
                SendQueue.Enqueue(Backend.BMember.GuestLogin, AuthorizeProcess);
                break;
            
            case "Test":
                SendQueue.Enqueue(Backend.BMember.CustomSignUp, "SandboxTest_" + Random.Range(0, 10000), "1234", AuthorizeProcess);
                break;
        }
    }

    #region 구글 토큰

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
        
        return null;
    }

    #endregion
    
    /*
     * 로그인 함수 호출 후 처리하는 함수
     */
    private void AuthorizeProcess(BackendReturnObject callback)
    {
        StaticManager.UI.SetLoading(false);
        Debug.LogWarning($"Backend.BMember.AuthroizeProcess : {callback}");
        
        if (!callback.IsSuccess())
        {
            if(callback.GetStatusCode() == "401")
            {
                string id = Backend.BMember.GetGuestID();

                if (!string.IsNullOrEmpty(id))
                {
                    Debug.Log("로컬 기기에 저장된 아이디 :" + id);
                    Backend.BMember.DeleteGuestInfo();
                    SendQueue.Enqueue(Backend.BMember.GuestLogin, AuthorizeProcess);
                }
            }
            else
            {
                loginButtonGroup.SetActive(true);
            }
            return;
        }

        //닉네임이 없을 경우
        if (string.IsNullOrEmpty(Backend.UserNickName))
        {
            copyRightObject.SetActive(false);
            loginButtonGroup.SetActive(false);
            GetPolicy();
            return;
        }

        //새로 가입인 경우에는 StatusCode가 201, 기존 로그인일 경우에는 200이 리턴
        if (callback.GetStatusCode() == "201")
        {
            copyRightObject.SetActive(false);
            loginButtonGroup.SetActive(false);
            
            GetPolicy();
        }
        else
        {
            //로딩 불러오기
            InitializeLoading();
        }
    }

    private void GetPolicy()
    {
        StaticManager.UI.OpenUI("Prefabs/LoginScene/PolicyUI", loginUICanvas.transform);
    }
}
