using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestAnimation : MonoBehaviour
{
    public void isSprout()
    {
        GetComponent<Animator>().SetBool("isSeed", false);
    }

    public void isHarvest()
    {
        GetComponent<Animator>().SetBool("isHarvest", false);
    }
}
