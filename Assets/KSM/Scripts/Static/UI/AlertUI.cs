using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlertUI : MonoBehaviour
{
    [SerializeField] private TMP_Text alertInfoText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    public delegate void ConfirmButton();

    public delegate void CancelButton();

    public void OpenUI(string infoText, ConfirmButton confirmButton = null, CancelButton cancelButton = null)
    {
        gameObject.SetActive(true);

        alertInfoText.text = infoText;
        
        this.confirmButton.onClick.RemoveAllListeners();
        this.confirmButton.onClick.AddListener(() =>
        {
            StaticManager.Sound.SetSFX();
            CloseUI();
            confirmButton?.Invoke();
        });
        
        this.cancelButton.onClick.RemoveAllListeners();
        this.cancelButton.gameObject.SetActive(cancelButton != null);
        this.cancelButton.onClick.AddListener(() =>
        {
            StaticManager.Sound.SetSFX();
            CloseUI();
            cancelButton?.Invoke();
        });
    }

    public void CloseUI() => gameObject.SetActive(false);
}
