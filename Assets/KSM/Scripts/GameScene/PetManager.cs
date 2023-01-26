using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PetManager : MonoBehaviour
{
    [SerializeField] private GameObject[] pets;

    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            pets[i].SetActive(StaticManager.Backend.backendGameData.PetData.Dictionary[i]);
        }
    }

    public void SetPet(int index)
    {
        StaticManager.Backend.backendGameData.PetData.SetPet(index, true);
        pets[index].SetActive(true);
    }
}
