using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    //Weather PopUp
    public void CloseWeatherObject()
    {
        FarmUI.GetInstance().weatherObject.SetActive(false);
    }

    public void DestroyRainEffect()
    {
        Destroy(this.gameObject);
    }

    public void MoveSun()
    {
        GetComponent<Animator>().SetBool("isDown", true);
    }

    public void DestroySun()
    {
        this.gameObject.SetActive(false);
        GetComponent<Animator>().SetBool("isEnd", false);
    }

    public void DestroyCoin()
    {
        Destroy(this.gameObject);
    }

    public void InActiveReward()
    {
        gameObject.SetActive(false);
    }
}
