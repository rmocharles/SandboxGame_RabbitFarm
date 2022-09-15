using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherAnimator : MonoBehaviour
{
    public void StartIdle()
    {
        GetComponent<Animator>().SetBool("isAppear", true);
    }

    public void StartDisappear()
    {
        gameObject.SetActive(false);
    }
}
