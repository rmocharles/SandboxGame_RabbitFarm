using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject backgroundGroup;
    [SerializeField] private Image representImage;
    [SerializeField] private GameObject profileGroup;
    [SerializeField] private TMP_Text infoText;

    void Start()
    {
        closeButton.onClick.AddListener(() =>
        {
            StaticManager.Sound.SetSFX();
            backgroundGroup.GetComponent<RectTransform>().DOScale(Vector3.zero, 0.1f);
            Invoke("DestroyUI", 0.1f);
        });

        //대표 이미지
        representImage.sprite = Resources.Load<Sprite>("Sprites/Profile/profile_" + (StaticManager.Backend.backendGameData.ProfileData.Represent + 1).ToString("D2"));

        for (int i = 0; i < 12; i++)
        {
            int num = i;
            profileGroup.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(() =>
            {
                StaticManager.Sound.SetSFX();
                ChangeRepresentImage(num);
            });
        }

        infoText.text = StaticManager.Langauge.Localize(31);
    }

    void Update()
    {
        //선택 체크
        for (int i = 0; i < 12; i++)
        {
                profileGroup.transform.GetChild(i).GetChild(0).gameObject.SetActive(i == StaticManager.Backend.backendGameData.ProfileData.Represent);
                profileGroup.transform.GetChild(i).GetChild(1).gameObject.SetActive(!StaticManager.Backend.backendGameData.ProfileData.Dictionary[i]);
        }
    }

    private void ChangeRepresentImage(int code)
    {
        if (!StaticManager.Backend.backendGameData.ProfileData.Dictionary[code])
        {
            //광고 관련
            StaticManager.AD.ShowRewardAD(() =>
            {
                representImage.sprite = Resources.Load<Sprite>("Sprites/Profile/profile_" + (code + 1).ToString("D2"));
                StaticManager.Backend.backendGameData.ProfileData.AddRepresentImage(code);

                GameManager.Instance.SaveAllData();
            });
        }
        else
        {
            representImage.sprite = Resources.Load<Sprite>("Sprites/Profile/profile_" + (code + 1).ToString("D2"));
            StaticManager.Backend.backendGameData.ProfileData.SetRerpesentImage(code);

            GameManager.Instance.SaveAllData();
        }
    }
    private void DestroyUI()
    {
        Destroy(this.gameObject);
    }
}
