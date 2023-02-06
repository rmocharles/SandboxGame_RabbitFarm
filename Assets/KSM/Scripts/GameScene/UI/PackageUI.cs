using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class PackageUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;

    [SerializeField] private TMP_Text diamondText;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text fertilizerText;

    [SerializeField] private IAPButton purchaseButton;
    // Start is called before the first frame update
    public void Initialize(int index)
    {
        //index - 0 : beginner, 1 : happy
        closeButton.onClick.AddListener(() =>
        {
            StaticManager.Sound.SetSFX();
            Destroy(this.gameObject);
        });

        if (index == 0)
        {
            diamondText.text = StaticManager.Langauge.Localize(219);
            goldText.text = StaticManager.Langauge.Localize(220);
            fertilizerText.text = StaticManager.Langauge.Localize(221);
        }
        else
        {
            diamondText.text = StaticManager.Langauge.Localize(222);
            goldText.text = StaticManager.Langauge.Localize(223);
            fertilizerText.text = StaticManager.Langauge.Localize(224);
        }

        purchaseButton.productId = index == 0 ? "beginner_package" : "newyear_package";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
