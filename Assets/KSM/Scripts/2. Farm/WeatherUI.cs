using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public partial class FarmUI : MonoBehaviour
{
    [Header("[ Weather UI ]")]
    public GameObject cloudButton;
    public GameObject sunButton;
    public GameObject weatherObject;
    public GameObject useCloudObject;
    public GameObject useSunObject;

    public GameObject rainObject;
    public GameObject rainEffectPrefab;

    public GameObject sunObject;

    public Text weatherCoolTimeText;

    private float rainCoolTime = 0;
    private bool isUseCloud = false, isUseSun = false;
    private int nowWeatherMode = 0; 

    private float weatherCoolTime = 3f, waitCoolTime = 0;

    void UpdateWeather()
    {
        ActiveWeatherObject();

        IsActiveCloud();

        IsActiveSun();
    }

    private void ActiveWeatherObject()
    {
        if (isScreenShotMode)
        {
            cloudButton.SetActive(false);
            sunButton.SetActive(false);
        }

        if(!isUseCloud && !isUseSun && !isMart)
        {
            if (weatherCoolTime > 0 && waitCoolTime <= 0)
                weatherCoolTime -= Time.deltaTime;

            if (waitCoolTime > 0)
                waitCoolTime -= Time.deltaTime;
            else
            {
                waitCoolTime = 0;

                if (cloudButton.activeSelf)
                    cloudButton.GetComponent<Animator>().SetBool("isAppear", false);

                if (sunButton.activeSelf)
                    sunButton.GetComponent<Animator>().SetBool("isAppear", false);
            }

            if (weatherCoolTime <= 0 && waitCoolTime <= 0)
            {
                weatherCoolTime = Random.Range(2, 3);
                if (!isScreenShotMode)
                {
                    switch (nowWeatherMode)
                    {
                        case 0: //처음
                            int randomNum = Random.Range(0, 2);
                            if (randomNum == 0)  //태양
                            {
                                nowWeatherMode = 1;
                                waitCoolTime = 10f;

                                sunButton.SetActive(true);
                            }
                            else
                            {
                                nowWeatherMode = 2;
                                waitCoolTime = 10f;

                                cloudButton.SetActive(true);
                            }
                            break;

                        case 1: //태양
                            nowWeatherMode = 2;
                            waitCoolTime = 10f;

                            cloudButton.SetActive(true);
                            break;

                        case 2: //구름
                            nowWeatherMode = 1;
                            waitCoolTime = 10f;

                            sunButton.SetActive(true);
                            break;
                    }
                }
            }
        }
    }

    private void IsActiveSun()
    {
        if (string.IsNullOrEmpty(BackendServerManager.GetInstance().weather_Sun)) return;

        TimeSpan ts = DateTime.Parse(BackendServerManager.GetInstance().weather_Sun) - DateTime.UtcNow;

        if (ts.TotalSeconds > 0)
        {
            sunButton.SetActive(false);
            cloudButton.SetActive(false);

            useSunObject.SetActive(true);
            weatherCoolTimeText.gameObject.SetActive(true);

            isUseSun = true;

            sunObject.SetActive(true);

            if (ts.TotalSeconds >= 3600)
                weatherCoolTimeText.text = (Mathf.FloorToInt((int)ts.TotalSeconds) / 3600).ToString() + "h " + (Mathf.FloorToInt((int)ts.TotalSeconds) / 3600 / 60).ToString() + "m " + (Mathf.FloorToInt((int)ts.TotalSeconds) / 3600 % 60).ToString() + "s";
            else if (ts.TotalSeconds >= 60 && ts.TotalSeconds < 3600)
                weatherCoolTimeText.text = (Mathf.FloorToInt((int)ts.TotalSeconds) / 60).ToString() + "m " + (Mathf.FloorToInt((int)ts.TotalSeconds) % 60).ToString() + "s";
            else
                weatherCoolTimeText.text = (Mathf.FloorToInt((int)ts.TotalSeconds)).ToString() + "s";
        }
        else
        {
            if(!isUseCloud && !isUseSun)
                weatherCoolTimeText.gameObject.SetActive(false);
            useSunObject.SetActive(false);

            isUseSun = false;

            if (!sunObject.GetComponent<Animator>().GetBool("isEnd"))
            {
                sunObject.GetComponent<Animator>().SetBool("isEnd", true);
                sunObject.GetComponent<Animator>().SetBool("isDown", false);
            }
        }
    }

    private void IsActiveCloud()
    {
        if (string.IsNullOrEmpty(BackendServerManager.GetInstance().weather_Cloud)) return;

        TimeSpan ts = DateTime.Parse(BackendServerManager.GetInstance().weather_Cloud) - DateTime.UtcNow;

        if (ts.TotalSeconds > 0)
        {
            sunButton.SetActive(false);
            cloudButton.SetActive(false);

            rainObject.SetActive(true);

            useCloudObject.SetActive(true);
            weatherCoolTimeText.gameObject.SetActive(true);

            isUseCloud = true;

            if (rainCoolTime > 0)
                rainCoolTime -= Time.deltaTime;
            else
            {
                GameObject rainPrefab = Instantiate(rainEffectPrefab, rainObject.transform.parent);
                rainPrefab.transform.position = new Vector3(Random.Range(-15f, 15f), Random.Range(-10f, 3f), 0);

                rainCoolTime = 0.1f;
            }

            if (ts.TotalSeconds >= 3600)
                weatherCoolTimeText.text = (Mathf.FloorToInt((int)ts.TotalSeconds) / 3600).ToString() + "h " + (Mathf.FloorToInt((int)ts.TotalSeconds) / 3600 / 60).ToString() + "m " + (Mathf.FloorToInt((int)ts.TotalSeconds) / 3600 % 60).ToString() + "s";
            else if (ts.TotalSeconds >= 60 && ts.TotalSeconds < 3600)
                weatherCoolTimeText.text = (Mathf.FloorToInt((int)ts.TotalSeconds) / 60).ToString() + "m " + (Mathf.FloorToInt((int)ts.TotalSeconds) % 60).ToString() + "s";
            else
                weatherCoolTimeText.text = (Mathf.FloorToInt((int)ts.TotalSeconds)).ToString() + "s";
        }
        else
        {
            rainObject.SetActive(false);
            if (!isUseCloud && !isUseSun)
                weatherCoolTimeText.gameObject.SetActive(false);
            useCloudObject.SetActive(false);

            isUseCloud = false;
        }
    }

    public void WeatherObject(int mode)
    {
        weatherObject.transform.GetChild(0).GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();

        switch (mode)
        {
            case 0: //해
                weatherObject.SetActive(true);
                weatherObject.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                weatherObject.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
                weatherObject.transform.GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("Langauge") == "ko" ? BackendServerManager.GetInstance().langaugeSheet[36].ko : BackendServerManager.GetInstance().langaugeSheet[36].en;
                weatherObject.transform.GetChild(0).GetChild(4).GetComponent<Button>().onClick.AddListener(() =>
                {
                    //ADManager.GetInstance().ShowRewardAD(0);
                });
                break;

            case 1: //구름
                weatherObject.SetActive(true);
                weatherObject.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                weatherObject.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                weatherObject.transform.GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("Langauge") == "ko" ? BackendServerManager.GetInstance().langaugeSheet[37].ko : BackendServerManager.GetInstance().langaugeSheet[37].en;
                weatherObject.transform.GetChild(0).GetChild(4).GetComponent<Button>().onClick.AddListener(() =>
                {
                    //ADManager.GetInstance().ShowRewardAD(1);
                });
                break;

            case 2: //창 닫기
                weatherObject.GetComponent<Animator>().SetBool("isClose", true);
                break;
        }

    }

    public void RewardAD(int mode)
    {
        switch (mode)
        {
            case 0: //해
                Debug.LogError("ANG");
                BackendServerManager.GetInstance().weather_Sun = DateTime.UtcNow.AddSeconds(BackendServerManager.GetInstance().weatherSheet[0].coolTime).ToString();
                BackendServerManager.GetInstance().SaveMyInfo(true);
                weatherObject.GetComponent<Animator>().SetBool("isClose", true);
                break;

            case 1: //구름
                Debug.LogError("ANG2");
                BackendServerManager.GetInstance().weather_Cloud = DateTime.UtcNow.AddSeconds(BackendServerManager.GetInstance().weatherSheet[1].coolTime).ToString();
                BackendServerManager.GetInstance().SaveMyInfo(true);
                weatherObject.GetComponent<Animator>().SetBool("isClose", true);
                break;
        }
    }
}
