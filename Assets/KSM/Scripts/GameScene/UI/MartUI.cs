using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MartUI : MonoBehaviour
{
    [SerializeField] private Button helpButton;
    void Start()
    {
        helpButton.onClick.AddListener(() =>
        {
            StaticManager.Sound.SetSFX();
            GameObject tutorialUI = StaticManager.UI.OpenUI("Prefabs/GameScene/TutorialUI", GameManager.Instance.UICanvas.transform);
            tutorialUI.GetComponent<TutorialUI>().Initialize("Mart");
        }); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
