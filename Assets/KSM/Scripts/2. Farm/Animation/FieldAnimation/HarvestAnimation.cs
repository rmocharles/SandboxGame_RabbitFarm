using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HarvestAnimation : MonoBehaviour
{
    public int fieldNumber = 0;

    public bool seedStart, sproutStart, harvestStart;

    void Update()
    {
        if (seedStart)
        {
            seedStart = false;
            GetComponent<Animator>().SetBool("isStart", true);
        }

        if (sproutStart)
        {
            sproutStart = false;
            GetComponent<Animator>().SetBool("isSeed", true);
        }

        if (harvestStart)
        {
            harvestStart = false;
            GetComponent<Animator>().SetBool("isHarvest", true);
        }
    }

    public void isSprout()
    {
        GetComponent<Animator>().SetBool("isSeed", false);
    }

    public void isHarvest()
    {
        GetComponent<Animator>().SetBool("isHarvest", false);
    }
}
