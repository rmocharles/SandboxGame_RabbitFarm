using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BackEnd;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
 * Policy UI
 *
 * 닉네임 (6~8글자, 특수문자 제외)
 * 개인정보 및 서비스 동의
 */

public class PolicyUI : MonoBehaviour
{
    [SerializeField] private TMP_Text welcomeText;
    [SerializeField] private TMP_Text warningText;
    [SerializeField] private TMP_InputField nicknameInputField;
    [SerializeField] private TMP_Text errorText;
    [SerializeField] private Button createButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Toggle termToggle;
    [SerializeField] private Button termButton;
    [SerializeField] private Toggle serviceToggle;
    [SerializeField] private Button serviceButton;
    [SerializeField] private Button termReadButton;
    [SerializeField] private Button serviceReadButton;
    
    void Start()
    {
        createButton.onClick.AddListener(CreateNickname);
        cancelButton.onClick.AddListener(CancelNickname);
        
        termButton.onClick.AddListener(() => ChangeAgree("Term"));
        serviceButton.onClick.AddListener(() => ChangeAgree("Service"));
        
        termReadButton.onClick.AddListener(() => GoToURL("https://sandboxnetwork.zendesk.com/hc/ko/articles/9193451596313-%EC%84%9C%EB%B9%84%EC%8A%A4-%EC%9D%B4%EC%9A%A9%EC%95%BD%EA%B4%80"));
        serviceReadButton.onClick.AddListener(() => GoToURL("https://sandboxnetwork.zendesk.com/hc/ko/articles/9275451667993-%EA%B0%9C%EC%9D%B8%EC%A0%95%EB%B3%B4-%EC%B2%98%EB%A6%AC%EB%B0%A9%EC%B9%A8"));
    }

    void Update()
    {
        //모든 약관에 동의해야 생성 버튼 활성화
        createButton.interactable = termToggle.isOn && serviceToggle.isOn;

        welcomeText.text = StaticManager.Langauge.Localize(0);
        warningText.text = StaticManager.Langauge.Localize(1);
        nicknameInputField.GetComponentInChildren<TMP_Text>().text = StaticManager.Langauge.Localize(2);
        serviceButton.GetComponentInChildren<TMP_Text>().text = StaticManager.Langauge.Localize(7);
        serviceReadButton.GetComponentInChildren<TMP_Text>().text = StaticManager.Langauge.Localize(9);
        termButton.GetComponentInChildren<TMP_Text>().text = StaticManager.Langauge.Localize(8);
        termReadButton.GetComponentInChildren<TMP_Text>().text = StaticManager.Langauge.Localize(9);
    }
    
    //============================================================
    //닉네임 생성
    //============================================================
    private void CreateNickname()
    {
        StaticManager.UI.SetLoading(true);
        //효과음
        StaticManager.Sound.SetSFX();

        string nickname = nicknameInputField.text;
        
        if (string.IsNullOrEmpty(nickname))
        {
            StaticManager.UI.SetLoading(false);
            errorText.text = StaticManager.Langauge.Localize(3);
            errorText.gameObject.GetComponent<DOTweenAnimation>().DORestart();
            return;
        }

        string idChecker = Regex.Replace(nickname, @"[^a-zA-Z0-9가-힣\.*,]", "", RegexOptions.Singleline);
        if (!nickname.Equals(idChecker))
        {
            StaticManager.UI.SetLoading(false);
            errorText.text = StaticManager.Langauge.Localize(4);
            errorText.gameObject.GetComponent<DOTweenAnimation>().DORestart();
            return;
        }

        if (nickname.IndexOf("시발") != -1 || nickname.IndexOf("씨발") != -1 || nickname.IndexOf("병신") != -1 || nickname.IndexOf("새끼") != -1 || nickname.IndexOf("애미") != -1 || nickname.IndexOf("애비") != -1 || nickname.IndexOf("fuck") != -1)
        {
            StaticManager.UI.SetLoading(false);
            errorText.text = StaticManager.Langauge.Localize(5);
            errorText.gameObject.GetComponent<DOTweenAnimation>().DORestart();
            return;
        }

        //[뒤끝] 닉네임 업데이트 함수
        SendQueue.Enqueue(Backend.BMember.UpdateNickname, nickname, callback =>
        {
            try
            {
                StaticManager.UI.SetLoading(false);
                if (!callback.IsSuccess())
                {
                    if (callback.GetStatusCode() == "400")
                    {
                        if (callback.GetMessage().Contains("undefined nickname"))
                        {
                            errorText.text = StaticManager.Langauge.Localize(3);
                        }
                        else if (callback.GetMessage().Contains("bad beginning or end"))
                        {
                            errorText.text = StaticManager.Langauge.Localize(4);
                        }
                        else
                        {
                            errorText.text = "알 수 없는 에러입니다.";
                        }
                    }
                    else if (callback.GetStatusCode() == "409")
                    {
                        errorText.text = StaticManager.Langauge.Localize(6);
                    }
                    else
                    {
                        StaticManager.UI.AlertUI.OpenUI(callback.ToString());
                    }

                    errorText.gameObject.GetComponent<DOTweenAnimation>().DORestart();
                }
                else
                {
                    Destroy(this.gameObject);
                    //로딩 불러오기
                    LoginSceneManager.Instance.InitializeLoading();
                }
            }
            catch (Exception e)
            {
                StaticManager.UI.AlertUI.OpenUI(e.ToString());
            }
        });
    }

    private void CancelNickname()
    {
        StaticManager.Sound.SetSFX();
        
        Destroy(this.gameObject);
        
        LoginSceneManager.Instance.SetLoginButton();
    }

    private void ChangeAgree(string type)
    {
        switch (type)
        {
            case "Term":
                termToggle.isOn =
                    !termToggle.isOn;
                break;
            case "Service":
                serviceToggle.isOn =
                    !serviceToggle.isOn;
                break;
        }
    }

    private void GoToURL(string url)
    {
        StaticManager.Sound.SetSFX();
        
        Application.OpenURL(url);
    }
}
