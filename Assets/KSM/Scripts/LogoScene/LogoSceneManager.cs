using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogoSceneManager : MonoBehaviour
{
    [SerializeField] private Image logoImage;
    void Start()
    {
        StartCoroutine(SplashLogo());
    }

    private IEnumerator SplashLogo()
    {
        List<Sprite> logoSprites = new List<Sprite>();
        
        for(int i = 0; i < 42; i++)
            logoSprites.Add(Resources.Load<Sprite>("Sprites/Splash/" + i.ToString("00")));

        yield return new WaitForSeconds(.1f);
        
        logoImage.GetComponent<AudioSource>().Play();

        int seq = 0;
        while (seq < logoSprites.Count)
        {
            logoImage.sprite = logoSprites[seq++];
            yield return new WaitForSeconds(.05f);
        }

        yield return new WaitForSeconds(.5f);
        SceneManager.LoadScene("1. Login");
    }
}
