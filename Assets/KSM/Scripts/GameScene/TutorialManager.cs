using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private string[] momText;
    [SerializeField] private string[] dadText;
    [SerializeField] private string[] daughterText;
    [SerializeField] private string[] sonText;

    void Start()
    {
        //튜토리얼이 진행상태라면
        if (StaticManager.Backend.backendGameData.UserData.Tutorial == 0)
        {
            
        }
    }

    private IEnumerator Typing(GameObject textObject, string content)
    {
        textObject.SetActive(true);
        for (int i = 0; i < content.Length; i++)
        {
            textObject.GetComponentInChildren<TMP_Text>().text = content.Substring(0, i);

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void SetTutorial(int index)
    {
        var mom = GameManager.Bunny.bunnies[0].textBubbleObject;
        var dad = GameManager.Bunny.bunnies[3].textBubbleObject;
        var son = GameManager.Bunny.bunnies[1].textBubbleObject;
        var daughter = GameManager.Bunny.bunnies[2].textBubbleObject;
        
        //카메라 이동
        Camera.main.transform.position = new Vector3(4, 4.5f, -10);
        
        //나린이 대화
        //GameManager.Bunny.bunnies[3].textBubbleObject.GetComponentInChildren<TMP_Text>()
    }
}
