using System.Collections;
using System.Collections.Generic;
using BackEnd;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeleteUI : MonoBehaviour
{
    [SerializeField] private TMP_Text alertInfoText;
    [SerializeField] private TMP_Text alertInfoText2;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    void Start()
    {
        alertInfoText.text = StaticManager.Langauge.Localize(199);
        alertInfoText2.text = StaticManager.Langauge.Localize(200);
        
        this.confirmButton.onClick.RemoveAllListeners();
        this.confirmButton.onClick.AddListener(() =>
        {
            StaticManager.Sound.SetSFX();
            SendQueue.Enqueue(Backend.BMember.WithdrawAccount, callback => {
                if (callback.IsSuccess())
                {
                    SceneManager.LoadScene("0. Logo");
                    StaticManager.Sound.SetBGM("FarmBGM", false);
                }
            });
        });
        
        this.cancelButton.onClick.RemoveAllListeners();
        this.cancelButton.onClick.AddListener(() =>
        {
            StaticManager.Sound.SetSFX();
            CloseUI();
        });
    }

    public void CloseUI() => Destroy(this.gameObject);
}