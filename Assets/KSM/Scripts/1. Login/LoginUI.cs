using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battlehub.Dispatcher;
using UnityEngine.UI;
using System;
using TMPro;
using System.Text.RegularExpressions;

public class LoginUI : MonoBehaviour
{
    private static LoginUI instance;

    [Header("[ Login Object ]")]
    public GameObject loginObject;

    [Header("[ CopyRight Object ]")]
    public GameObject copyRightObject;

    [Header("[ Nickname Object ]")]
    public GameObject nicknameObject;
    public TextMeshProUGUI warningText;

    [Header("[ Error Object ]")]
    public GameObject errorObject;

    [Header("[ Loading Object ]")]
    public GameObject loadingObject;

    [Header("[ Progress Object ]")]
    public GameObject progressObject;

    #region ErrorCode
    private const string BAD_NICKNAME = "부적절하거나 남을 비방하는 닉네임은 제외될 수 있습니다."; 
    private const string BAD_NICKNAME_EN = "Inappropriate or libelous nicknames may be excluded.";
    private const string INPUT_NICKNAME = "닉네임을 입력해주세요. (특수문자 제외, 최대 8글자)";
    private const string INPUT_NICKNAME_EN = "Please enter your nickname.\n(Excluding special characters, up to 8 characters)";
    private const string PROFANITY_FAIL = "닉네임에 비속어를 사용할 수 없습니다.";
    private const string PROFANITY_FAIL_EN = "You cannot use profanity in your nickname.";
    private const string SPECIAL_CHARACTER_FAIL = "닉네임에 공백 또는 특수문자를 사용할 수 없습니다.";
    private const string SPECIAL_CHARACTER_FAIL_EN = "You cannot use spaces or special characters in your nickname.";
    #endregion

    private bool checkNickname = false;



    void Awake()
    {
        if (!instance) instance = this;
    }
    void Start()
    {
#if UNITY_ANDROID
        loginObject.transform.GetChild(0).gameObject.SetActive(true);
        loginObject.transform.GetChild(1).gameObject.SetActive(false);
#elif UNITY_IOS
        loginObject.transform.GetChild(0).gameObject.SetActive(false);
        loginObject.transform.GetChild(1).gameObject.SetActive(true);
#endif

        loginObject.SetActive(false);
        nicknameObject.SetActive(false);
        progressObject.SetActive(false);
    }

    void Update()
    {
        nicknameObject.GetComponentsInChildren<TextMeshProUGUI>()[0].text = PlayerPrefs.GetString("Langauge") == "ko" ? INPUT_NICKNAME : INPUT_NICKNAME_EN;
        nicknameObject.GetComponentsInChildren<Text>()[0].text = PlayerPrefs.GetString("Langauge") == "ko" ? BAD_NICKNAME : BAD_NICKNAME_EN;
    }

    public static LoginUI GetInstance()
    {
        if (instance == null)
            return null;
        return instance;
    }
    //=============================================================

    public void AutoLogin()
    {
        loadingObject.SetActive(true);
        BackendServerManager.GetInstance().BackendTokenLogin((bool result, string error) =>
        {
            Dispatcher.Current.BeginInvoke(() =>
            {
                if (result) return;

                loadingObject.SetActive(false);
                if (!error.Equals(string.Empty))
                {
                    SetErrorObject(error, () =>
                    {
                        Application.Quit();
                    });
                }

                loginObject.SetActive(true);
                copyRightObject.SetActive(true);
            });
        });
    }

    #region Google Login
    public void GoogleLogin()
    {
        if (errorObject.activeSelf) return;

        loadingObject.SetActive(true);
        BackendServerManager.GetInstance().GoogleAuthorizeFederation((bool result, string error) =>
        {
            if (!result)
            {
                loadingObject.SetActive(false);
                SetErrorObject(error, () => { Application.Quit(); });

                return;
            }
        });
    }
    #endregion

    #region Apple Login
    public void AppleLogin()
    {
        if (errorObject.activeSelf) return;

        loadingObject.SetActive(true);
        BackendServerManager.GetInstance().AppleAuthorizeFederation((bool result, string error) =>
        {
            if (!result)
            {
                loadingObject.SetActive(false);
                SetErrorObject(error, () => { Application.Quit(); });

                return;
            }
        });
    }
    #endregion

    #region Guest Login
    public void GuestLogin()
    {
        if (errorObject.activeSelf) return;

        loadingObject.SetActive(true);
        BackendServerManager.GetInstance().GuestLogin((bool result, string error) =>
        {
            if (!result)
            {
                loadingObject.SetActive(false);
                SetErrorObject(error, () => { Application.Quit(); });

                return;
            }
        });
    }
    #endregion

    #region Active Nickname Setting
    public void ActiveNicknameObject()
    {
        Dispatcher.Current.BeginInvoke(() =>
        {
            loadingObject.SetActive(false);
            loginObject.SetActive(false);
            nicknameObject.SetActive(true);
            nicknameObject.GetComponentInChildren<TMP_InputField>().text = "";
        });
    }
    #endregion

    #region Decide Nickname
    public void UpdateNickname()
    {
        if (errorObject.activeSelf) return;


        string nickname = nicknameObject.GetComponentInChildren<TMP_InputField>().text;
        string idChecker = Regex.Replace(nickname, @"[^a-zA-Z0-9가-힣\.*,]", "", RegexOptions.Singleline);
        if (!nickname.Equals(idChecker))
        {
            checkNickname = true;
            warningText.text = PlayerPrefs.GetString("Langauge") == "ko" ? SPECIAL_CHARACTER_FAIL : SPECIAL_CHARACTER_FAIL_EN;
            return;
        }

        if (nickname.IndexOf("시발") != -1 || nickname.IndexOf("씨발") != -1 || nickname.IndexOf("병신") != -1 || nickname.IndexOf("새끼") != -1 || nickname.IndexOf("애미") != -1 || nickname.IndexOf("애비") != -1 || nickname.IndexOf("fuck") != -1)
        {
            checkNickname = true;
            warningText.text = PlayerPrefs.GetString("Langauge") == "ko" ? PROFANITY_FAIL : PROFANITY_FAIL_EN;
            return;
        }
        loadingObject.SetActive(true);

        BackendServerManager.GetInstance().UpdateNickname(nickname, (bool result, string error) =>
        {
            Dispatcher.Current.BeginInvoke(() =>
            {
                loadingObject.SetActive(false);
                if (!result)
                {
                    checkNickname = true;
                    warningText.text = error;
                    return;
                }
            });
        });
    }
    #endregion

    #region Cancel Nickname
    public void CancelNickname()
    {
        if(!BackEnd.Backend.BMember.GetGuestID().Equals(string.Empty))
        {
            BackEnd.Backend.BMember.DeleteGuestInfo();
        }

        loadingObject.SetActive(false);

        nicknameObject.SetActive(false);
        loginObject.SetActive(true);
    }
    #endregion

    #region CheckChangeNickname
    public void CheckChangeNickname()
    {

        if (checkNickname)
        {
            warningText.text = "";
            checkNickname = false;
        }
    }
    #endregion

    #region Success Login
    public void SuccessLogin(Action<bool, string> func)
    {
        Dispatcher.Current.BeginInvoke(() =>
        {
            Debug.LogWarning("Login Success");
            loadingObject.SetActive(false);
            loginObject.SetActive(false);
            nicknameObject.SetActive(false);

            func(true, string.Empty);
        });
    }
    #endregion

    public void SetErrorObject(string error, Action func = null)
    {
        errorObject.SetActive(true);
        errorObject.GetComponentInChildren<TextMeshProUGUI>().text = error;

        if(func != null)
        {
            errorObject.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                func();
                errorObject.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
            });
        }
    }

    public void SetProgressObject(string msg, bool isActive = true)
    {
        progressObject.SetActive(isActive);

        progressObject.GetComponentInChildren<TextMeshProUGUI>().text = msg;
    }
}
