using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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



    [Serializable]
    private class SoundSource
    {
        public string name;
        public AudioClip audioClip;
    }
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource effectSource;
    [SerializeField] private SoundSource[] soundSource;
    
    void Start()
    {
        //최초로 실행할 시 소리 모두 On 상태
        if (!PlayerPrefs.HasKey("isNew"))
        {
            PlayerPrefs.SetInt("isNew", 1);
            
            PlayerPrefs.SetInt("bgm", 1);
            PlayerPrefs.SetInt("sfx", 1);
            PlayerPrefs.SetInt("vibrate", 1);
        }
    }

    void Update()
    {
        bgmSource.mute = PlayerPrefs.GetInt("bgm") == 0;
        effectSource.mute = PlayerPrefs.GetInt("sfx") == 0;
    }

    public void SwitchBackgroundSound(int num)
    {
        SetSFX();
        PlayerPrefs.SetInt("bgm", num);
    }

    public void SwitchEffectSound(int num)
    {
        SetSFX();
        PlayerPrefs.SetInt("sfx", num);
    }
    
    //진동 효과
    public void Vibrate(int time = 100)
    {
        if (PlayerPrefs.GetInt("vibrate") == 1)
            Vibration.Vibrate(time);
    }
    
    //배경음 설정
    public void SetBGM(string name, bool isPlay = true)
    {
        for (int i = 0; i < soundSource.Length; i++)
        {
            if (soundSource[i].name == name)
            {
                bgmSource.clip = soundSource[i].audioClip;
                if(isPlay)
                    bgmSource.Play();
            }
        }
    }
    
    //배경음 소리 설정
    public void SetVolume(float value)
    {
        if (value > 0.5f)
            value = 0.5f;

        if (value < 0.1f)
            value = 0.1f;

        bgmSource.volume = value;
    }
    
    public void SetSFX(string name = "ButtonSFX")
    {
        for (int i = 0; i < soundSource.Length; i++)
        {
            if (soundSource[i].name == name)
            {
                effectSource.clip = soundSource[i].audioClip;
                effectSource.Play();
            }
        }
    }

    public AudioClip SearchClip(string name)
    {
        for (int i = 0; i < soundSource.Length; i++)
        {
            if (soundSource[i].name == name)
            {
                return soundSource[i].audioClip;
            }
        }

        Debug.LogError("없음");
        return null;
    }
}
