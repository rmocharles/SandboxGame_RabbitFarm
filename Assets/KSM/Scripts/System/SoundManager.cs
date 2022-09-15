using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundManager : MonoBehaviour
{
    public static class Vibration
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        public static AndroidJavaClass AndroidPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        public static AndroidJavaObject AndroidcurrentActivity = AndroidPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        public static AndroidJavaObject AndroidVibrator = AndroidcurrentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#endif
        public static void Vibrate()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidVibrator.Call("vibrate");
#else
            Handheld.Vibrate();
#endif
        }

        public static void Vibrate(long milliseconds)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidVibrator.Call("vibrate", milliseconds);
#else
            Handheld.Vibrate();
#endif
        }

        public static void Vibrate(long[] pattern, int repeat)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidVibrator.Call("vibrate", pattern, repeat);
#else
            Handheld.Vibrate();
#endif
        }

        public static void Cancel()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidVibrator.Call("cancel");
#endif
        }
    }



    public AudioSource bgmSource;
    public AudioSource effectSource;

    public AudioClip[] bgmSounds;
    public AudioClip[] effectSounds;

    bool isVive;

    private static SoundManager instance;

    public static SoundManager GetInstance()
    {
        if (instance == null)
            return null;
        return instance;
    }

    void Awake()
    {
        if (!instance) instance = this;
    }

    void Start()
    {
        if (!PlayerPrefs.HasKey("Bgm_Mute"))
        {
            PlayerPrefs.SetInt("Bgm_Mute", 0);
            PlayerPrefs.SetInt("Effect_Mute", 0);
            PlayerPrefs.SetInt("Vibrate_Mute", 1);
        }
    }

    void Update()
    {
        bgmSource.mute = PlayerPrefs.GetInt("Bgm_Mute") == 1 ? true : false;
        effectSource.mute = PlayerPrefs.GetInt("Effect_Mute") == 1 ? true : false;
        isVive = PlayerPrefs.GetInt("Vibrate_Mute") == 0 ? true : false;
    }

    public void Vibrate(int time = 100)
    {
        if (isVive)
            Vibration.Vibrate(time);
    }

    public void IsMuteBgm(bool isCheck)
    {
        PlayerPrefs.SetInt("Bgm_Mute", !isCheck ? 1 : 0);
    }

    public void IsMuteEffect(bool isCheck)
    {
        PlayerPrefs.SetInt("Effect_Mute", !isCheck ? 1 : 0);
    }

    public void IsMuteVibrate(bool isCheck)
    {
        PlayerPrefs.SetInt("Vibrate_Mute", !isCheck ? 1 : 0);
    }

    public void SetBgm(int num)
    {
        bgmSource.clip = bgmSounds[num];
        bgmSource.Play();
    }

    public void SetEffect(int num = 0)
    {
        effectSource.clip = effectSounds[num];
        effectSource.Play();
    }

    public void IsPlayBgm(bool isPlay)
    {
        if (isPlay)
            bgmSource.Play();
        else
            bgmSource.Pause();
    }

    public void IsPlayEffect(bool isPlay)
    {
        if (isPlay)
            effectSource.Play();
        else
            effectSource.Pause();
    }
}
