using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeatherUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject backgroundGroup;

    public GameObject sunObject;
    public GameObject rainObject;
    public TMP_Text infoText;
    public Button adButton;

    public void Initialize(WeatherManager.Weather weather)
    {
        sunObject.SetActive(weather == WeatherManager.Weather.Sun);
        rainObject.SetActive(weather == WeatherManager.Weather.Rain);
        
        closeButton.onClick.AddListener(() =>
        {
            StaticManager.Sound.SetSFX();
            backgroundGroup.GetComponent<RectTransform>().DOScale(Vector3.zero, 0.1f);
            Invoke(nameof(DestroyUI), 0.1f);
        });
        
        adButton.onClick.AddListener(() =>
        {
            // StaticManager.Sound.SetSFX(weather == WeatherManager.Weather.Rain ? "Rain" : "Sun");
            // GameManager.Weather.ActiveWeather(weather == WeatherManager.Weather.Rain ? 2 : 1);
            
            backgroundGroup.GetComponent<RectTransform>().DOScale(Vector3.zero, 0.1f);
            Invoke(nameof(DestroyUI), 0.1f);
            //광고 후
            StaticManager.AD.ShowRewardAD(() =>
            {
                StaticManager.Sound.SetSFX(weather == WeatherManager.Weather.Rain ? "Rain" : "Sun");
                GameManager.Weather.ActiveWeather(weather == WeatherManager.Weather.Rain ? 2 : 1);
            
                backgroundGroup.GetComponent<RectTransform>().DOScale(Vector3.zero, 0.1f);
                Invoke(nameof(DestroyUI), 0.1f);
            });
            
            //GameManager.Weather.ActiveWeather(weather);
            
            backgroundGroup.GetComponent<RectTransform>().DOScale(Vector3.zero, 0.1f);
            Invoke(nameof(DestroyUI), 0.1f);
        });

        infoText.text = weather == WeatherManager.Weather.Rain ? StaticManager.Langauge.Localize(43) : StaticManager.Langauge.Localize(42);
    }
    
    private void DestroyUI()
    {
        Destroy(this.gameObject);
    }
}
