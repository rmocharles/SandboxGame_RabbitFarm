using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToastUI : MonoBehaviour
{
    [SerializeField] private TMP_Text infoText;

    public void Initialize(string infoText)
    {
        this.infoText.text = infoText;
        
        Invoke("DestroyUI", 1.5f);
    }

    private void DestroyUI()
    {
        Destroy(this.gameObject);
    }
}
