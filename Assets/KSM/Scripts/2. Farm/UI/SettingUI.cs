using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class FarmUI : MonoBehaviour
{
    [Header("[ Setting Panel ]")]
    public GameObject settingPanel;
    public Toggle[] settingToggles;

    private void UpdateSetting()
    {
        //배경음
        settingToggles[0].isOn = PlayerPrefs.GetInt("Bgm_Mute") == 1 ? false : true;

        //효과음
        settingToggles[1].isOn = PlayerPrefs.GetInt("Effect_Mute") == 1 ? false : true;

        //진동
        settingToggles[2].isOn = PlayerPrefs.GetInt("Vibrate_Mute") == 1 ? false : true;
    }
}
