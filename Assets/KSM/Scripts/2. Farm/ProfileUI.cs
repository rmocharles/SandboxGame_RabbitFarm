using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class FarmUI : MonoBehaviour
{

    [Header("[ Profile Object ]")]
    public GameObject profileObject;

    [Header("[ Profile Panel ]")]
    public GameObject profilePanel;
    public TextMeshProUGUI nicknameText;
    public TextMeshProUGUI gamerIDText;
    public Image representImage;
    public Sprite[] characterImage;
    public Sprite[] backgroundCharacterImage;
    public GameObject[] characterSelectButton;

    #region 프로필 활성화
    public void ActiveSelectImage()
    {
        profilePanel.transform.GetChild(2).gameObject.SetActive(!profilePanel.transform.GetChild(2).gameObject.activeSelf);
        SelectImage();
        DestroySelectPrefab();
    }
    #endregion

    public void UpdateProfile()
    {
        profileObject.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = backgroundCharacterImage[int.Parse(nowImage)];

        profileObject.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = BackendServerManager.GetInstance().myInfo.nickName;
    }


    public void SelectImage(string num = "")
    {
        if (num.Equals(string.Empty))
        {
            for (int i = 0; i < characterSelectButton.Length; i++)
            {
                if (i == int.Parse(nowImage))
                {
                    representImage.sprite = characterImage[i];
                    characterSelectButton[i].transform.GetChild(0).gameObject.SetActive(true);
                }
                else
                {
                    characterSelectButton[i].transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }
        else
        {
            for (int i = 0; i < characterSelectButton.Length; i++)
            {
                if (i == int.Parse(num))
                {
                    representImage.sprite = characterImage[i];
                    characterSelectButton[i].transform.GetChild(0).gameObject.SetActive(true);
                    nowImage = i.ToString();

                    BackendServerManager.GetInstance().SaveMyInfo();
                }
                else
                {
                    characterSelectButton[i].transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }
    }
}
