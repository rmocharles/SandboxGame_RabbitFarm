using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalManager : MonoBehaviour
{
    [SerializeField] private GameObject cowObject;
    [SerializeField] private GameObject chickenObject;

    public void Initialize()
    {
        cowObject.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
        cowObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            StaticManager.Sound.SetSFX();
            GameObject upgradeUI = StaticManager.UI.OpenUI("Prefabs/GameScene/AnimalUpgradeUI", GameManager.Instance.UICanvas.transform);
            upgradeUI.GetComponent<AnimalUpgradeUI>().Initialize(AnimalUpgradeUI.Animal.Cow);
            
        });
        chickenObject.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
        chickenObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            StaticManager.Sound.SetSFX();
            GameObject upgradeUI = StaticManager.UI.OpenUI("Prefabs/GameScene/AnimalUpgradeUI", GameManager.Instance.UICanvas.transform);
            upgradeUI.GetComponent<AnimalUpgradeUI>().Initialize(AnimalUpgradeUI.Animal.Chicken);
        });
        
        InitializeAnimal(AnimalUpgradeUI.Animal.Chicken);
        InitializeAnimal(AnimalUpgradeUI.Animal.Cow);
    }

    public void InitializeAnimal(AnimalUpgradeUI.Animal animal)
    {
        switch (animal)
        {
            case AnimalUpgradeUI.Animal.Chicken:
                chickenObject.GetComponent<AnimalInfo>().Initialize(animal);
                break;
            
            case AnimalUpgradeUI.Animal.Cow:
                cowObject.GetComponent<AnimalInfo>().Initialize(animal);
                break;
        }
    }

    public void SetAnimal(AnimalUpgradeUI.Animal animal, string remainTimer)
    {
        switch (animal)
        {
            case AnimalUpgradeUI.Animal.Chicken:
                StaticManager.Backend.backendGameData.AnimalData.SetAnimal("Chicken", remainTimer);
                break;
            
            case AnimalUpgradeUI.Animal.Cow:
                StaticManager.Backend.backendGameData.AnimalData.SetAnimal("Cow", remainTimer);
                break;
        }
        
        GameManager.Instance.SaveAllData();
    }

}
