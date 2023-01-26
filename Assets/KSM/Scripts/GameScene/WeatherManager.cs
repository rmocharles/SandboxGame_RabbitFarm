using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/*
 * Sun - 비 종료 후, 1분 후 생성, 20초동안 활성화
 * Rain - 해 종료 후, 1분 후 생성, 20초동안 활성화
 */
public class WeatherManager : MonoBehaviour
{
    private float sunCoolTime;
    private float sunActiveTime;
    public bool isSun = false;
    public float sunAbilityTime = 15;

    private float rainCoolTime;
    private float rainActiveTime;
    public bool isRain = false;
    public float rainAbilityTime = 15;

    public GameObject sunObject;
    public GameObject rainObject;
    public TMP_Text coolTimeText;

    public GameObject sunParticleObject;
    public GameObject rainParticleObject;

    public string appQuitTime = string.Empty;

    public enum Weather
    {
        None, Sun, Rain
    }

    public Weather nowWeather;

    public void Initialize()
    {
        sunObject.SetActive(false);
        rainObject.SetActive(false);

        if (StaticManager.Backend.backendGameData.WeatherData.Type == 0)
        {
            //처음, 해 or 구름 선택
            SetWeather((Weather)Random.Range(1, 3));
        }
        else
        {
            //데이터 불러오기
            
        }
        
        
        sunObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            StaticManager.Sound.SetSFX();
            if (StaticManager.Backend.backendGameData.WeatherData.Type == 0)
            {
                GameObject weatherUI = StaticManager.UI.OpenUI("prefabs/GameScene/WeatherUI", GameManager.Instance.UICanvas.transform);
                weatherUI.GetComponent<WeatherUI>().Initialize(Weather.Sun);
            }
            else
            {
                GameManager.Instance.MakeToast(StaticManager.Langauge.Localize(44));
            }
        });
        
        rainObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            StaticManager.Sound.SetSFX();
            if (StaticManager.Backend.backendGameData.WeatherData.Type == 0)
            {
                GameObject weatherUI = StaticManager.UI.OpenUI("prefabs/GameScene/WeatherUI", GameManager.Instance.UICanvas.transform);
                weatherUI.GetComponent<WeatherUI>().Initialize(Weather.Rain);
            }
            else
            {
                GameManager.Instance.MakeToast(StaticManager.Langauge.Localize(44));
            }
        });
    }

    public void SetWeather(Weather weather)
    {
        nowWeather = weather;
        switch (weather)
        {
            case Weather.Rain:
                rainCoolTime = 10;
                rainActiveTime = 5;
                break;
            
            case Weather.Sun:
                sunCoolTime = 10;
                sunActiveTime = 5;
                break;
        }
    }

    public void ActiveWeather(int weather)
    {
        Debug.LogError(weather);
        StaticManager.Backend.backendGameData.WeatherData.SetType(weather);
        GameManager.Instance.SaveAllData();
    }

    void Update()
    {
        //아무것도 아닐 때
        if (StaticManager.Backend.backendGameData.WeatherData.Type == 0)
        {
            sunParticleObject.SetActive(false);
            rainParticleObject.SetActive(false);
            coolTimeText.transform.parent.gameObject.SetActive(false);
            switch (nowWeather)
                    {
                        case Weather.Rain:
                            sunObject.SetActive(false);
                            
                            if (rainCoolTime > 0)
                            {
                                rainObject.SetActive(false);
                                rainCoolTime -= Time.deltaTime;
                            }
                            
                            else if (rainCoolTime <= 0)
                            {
                                rainObject.SetActive(rainActiveTime > 0);
                                if (rainActiveTime > 0)
                                {
                                    rainActiveTime -= Time.deltaTime;
                                }
                                else
                                {
                                    SetWeather(Weather.Sun);
                                }
                                
                            }
                            break;
                        
                        case Weather.Sun:
                            rainObject.SetActive(false);
                            
                            if (sunCoolTime > 0)
                            {
                                sunObject.SetActive(false);
                                sunCoolTime -= Time.deltaTime;
                            }
                            
                            else if (sunCoolTime <= 0)
                            {
                                sunObject.SetActive(sunActiveTime > 0);
                                if (sunActiveTime > 0)
                                {
                                    sunActiveTime -= Time.deltaTime;
                                }
                                else
                                {
                                    SetWeather(Weather.Rain);
                                }
                                
                    }
                break;
            }
        }

        if (!string.IsNullOrEmpty(StaticManager.Backend.backendGameData.WeatherData.RemainTime))
        {
            TimeSpan remainTime = DateTime.Parse(StaticManager.Backend.backendGameData.WeatherData.RemainTime) - DateTime.UtcNow;
            //태양
            if (StaticManager.Backend.backendGameData.WeatherData.Type == 1)
            {
                sunObject.SetActive(true);
                rainObject.SetActive(false);
                coolTimeText.transform.parent.gameObject.SetActive(true);
                sunParticleObject.SetActive(true);
                rainParticleObject.SetActive(false);

                if (remainTime.TotalSeconds > 0)
                {
                    coolTimeText.text = (remainTime.TotalSeconds / 60 - 1).ToString("00") + ":" + (remainTime.TotalSeconds % 60).ToString("00");
                }
                else
                {
                    coolTimeText.text = string.Empty;
                    StaticManager.Backend.backendGameData.WeatherData.SetType(0);
                    SetWeather(Weather.Rain);
                    GameManager.Instance.SaveAllData();
                }
            }
        
            //비
            if (StaticManager.Backend.backendGameData.WeatherData.Type == 2)
            {
                sunObject.SetActive(false);
                rainObject.SetActive(true);
                coolTimeText.transform.parent.gameObject.SetActive(true);
                sunParticleObject.SetActive(false);
                rainParticleObject.SetActive(true);

                if (remainTime.TotalSeconds > 0)
                {
                    coolTimeText.text = (remainTime.TotalSeconds / 60 - 1).ToString("00") + ":" + (remainTime.TotalSeconds % 60).ToString("00");
                }
                else
                {
                    coolTimeText.text = string.Empty;
                    StaticManager.Backend.backendGameData.WeatherData.SetType(0);
                    SetWeather(Weather.Sun);
                    GameManager.Instance.SaveAllData();
                }
            }
        }
        
        // if (isSun)
        // {
        //     sunObject.SetActive(true);
        //     rainObject.SetActive(false);
        //     coolTimeText.transform.parent.gameObject.SetActive(true);
        //     sunParticleObject.SetActive(true);
        //     
        //     if (sunAbilityTime > 0)
        //     {
        //         sunAbilityTime -= Time.deltaTime;
        //         coolTimeText.text = (sunAbilityTime / 60).ToString("00") + ":" + (sunAbilityTime % 60).ToString("00");
        //     }
        //     else
        //     {
        //         isSun = false;
        //         SetWeather(Weather.Rain);
        //         coolTimeText.text = string.Empty;
        //     }
        // }

        // if (isRain)
        // {
        //     sunObject.SetActive(false);
        //     rainObject.SetActive(true);
        //     coolTimeText.transform.parent.gameObject.SetActive(true);
        //     rainParticleObject.SetActive(true);
        //     
        //     if (rainAbilityTime > 0)
        //     {
        //         rainAbilityTime -= Time.deltaTime;
        //         
        //         coolTimeText.text = (rainAbilityTime / 60).ToString("00") + ":" + (rainAbilityTime % 60).ToString("00");
        //     }
        //     else
        //     {
        //         isRain = false;
        //         SetWeather(Weather.Sun);
        //         coolTimeText.text = string.Empty;
        //     }
        // }
        
        //능력이 발동되지 않았을 경우 싸이클
        // if (!isSun && !isRain)
        // {
        //     sunParticleObject.SetActive(false);
        //     rainParticleObject.SetActive(false);
        //     coolTimeText.transform.parent.gameObject.SetActive(false);
        //     switch (nowWeather)
        //             {
        //                 case Weather.Rain:
        //                     sunObject.SetActive(false);
        //                     
        //                     if (rainCoolTime > 0)
        //                     {
        //                         rainObject.SetActive(false);
        //                         rainCoolTime -= Time.deltaTime;
        //                     }
        //                     
        //                     else if (rainCoolTime <= 0)
        //                     {
        //                         rainObject.SetActive(rainActiveTime > 0);
        //                         if (rainActiveTime > 0)
        //                         {
        //                             rainActiveTime -= Time.deltaTime;
        //                         }
        //                         else
        //                         {
        //                             SetWeather(Weather.Sun);
        //                         }
        //                         
        //                     }
        //                     break;
        //                 
        //                 case Weather.Sun:
        //                     rainObject.SetActive(false);
        //                     
        //                     if (sunCoolTime > 0)
        //                     {
        //                         sunObject.SetActive(false);
        //                         sunCoolTime -= Time.deltaTime;
        //                     }
        //                     
        //                     else if (sunCoolTime <= 0)
        //                     {
        //                         sunObject.SetActive(sunActiveTime > 0);
        //                         if (sunActiveTime > 0)
        //                         {
        //                             sunActiveTime -= Time.deltaTime;
        //                         }
        //                         else
        //                         {
        //                             SetWeather(Weather.Rain);
        //                         }
        //                         
        //             }
        //         break;
        //     }
        // }
        
        //마트에 있을 경우 비활성화
        if (GameManager.Instance.nowMode == GameManager.Mode.Mart)
        {
            sunParticleObject.SetActive(false);
            rainParticleObject.SetActive(false);
            sunObject.SetActive(false);
            rainObject.SetActive(false);
            coolTimeText.transform.parent.gameObject.SetActive(false);
        }
    }
}
