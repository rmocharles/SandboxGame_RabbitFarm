using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private string[] momText;
    [SerializeField] private string[] dadText;
    [SerializeField] private string[] daughterText;
    [SerializeField] private string[] sonText;

    [SerializeField] private GameObject bubbleObject;

    public bool isTutorial = false;
    public int index = 0;

    private TMP_Text nameText;
    private TMP_Text contentText;

    void Awake()
    {
        nameText = bubbleObject.GetComponentsInChildren<TMP_Text>()[0];
        Debug.LogError((nameText.name));
        contentText = bubbleObject.GetComponentsInChildren<TMP_Text>()[1];
        
        bubbleObject.SetActive(false);
    }

    public void Initialize()
    {
        //튜토리얼이 진행상태라면
        if (StaticManager.Backend.backendGameData.UserData.Tutorial == 0)
        {
            SetTutorial(0);
        }
        if (StaticManager.Backend.backendGameData.UserData.Tutorial == 1)
        {
            GameObject tutorialUI = StaticManager.UI.OpenUI("Prefabs/GameScene/TutorialUI", GameManager.Instance.UICanvas.transform);
        }

        bubbleObject.GetComponentInChildren<Button>().onClick.AddListener(() =>
        {
            if (StaticManager.Backend.backendGameData.UserData.Tutorial == 0)
            {
                switch (index)
                {
                    case 0:
                        break;
                    
                    case 1:
                        break;
                    
                    case 2:
                        break;
                    
                    case 18:
                        bubbleObject.SetActive(false);
                        GameManager.Bunny.bunnies[0].textBubbleObject.SetActive(false);
                        for (int i = 0; i < 4; i++)
                        {
                            GameManager.Bunny.ChangeBunnyState(i, BunnyController.State.Idle);
                            GameManager.Bunny.MoveBunny(i, GameManager.Bunny.bunnies[i].originPos);
                        }

                        Camera.main.transform.position = new Vector3(0, 0, -10);

                        isTutorial = false;
                        
                        StaticManager.Backend.backendGameData.UserData.SetTutorial(1);
                        GameObject tutorialUI = StaticManager.UI.OpenUI("Prefabs/GameScene/TutorialUI", GameManager.Instance.UICanvas.transform);
                        
                        GameManager.Instance.SaveAllData();
                        return;
                }
                
                int[] characterIndex = { 2, 0, 0, 0, 1, 3, 1, 2, 3, 0, 4, 0, 2, 3, 0, 1, 4, 0 };
                SetBubble(characterIndex[index], index + 1);
            }
            
            index++;
        });
    }

    private IEnumerator Typing(TMP_Text textObject, string content)
    {
        for (int i = 0; i < content.Length; i++)
        {
            textObject.text = content.Substring(0, i);

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void SetTutorial(int index)
    {
        isTutorial = true;
        bubbleObject.SetActive(true);

        for (int i = 0; i < 4; i++)
        {
            GameManager.Bunny.bunnies[i].ChangeState(BunnyController.State.Tutorial);
        }
        
        switch (index)
        {
            //처음
            case 0:
                Camera.main.transform.DOMove(new Vector3(4, 5, -10), 0.1f);
                SetBubble(3, 0);
                
                GameManager.Bunny.MoveBunny(0, new Vector3(5.2f, 3.5f, 0));
                GameManager.Bunny.MoveBunny(1, new Vector3(8.5f, 1.5f, 0));
                GameManager.Bunny.MoveBunny(2, new Vector3(7, 3, 0));
                GameManager.Bunny.MoveBunny(3, new Vector3(3.5f, 2.5f, 0));

                GameManager.Bunny.bunnies[0].direction = 1;
                GameManager.Bunny.bunnies[1].direction = 2;
                GameManager.Bunny.bunnies[2].direction = 2;
                GameManager.Bunny.bunnies[3].direction = 1;
                break;
            case 1:
                break;
            
            case 2:
                break;
        }
    }

    public void SetBubble(int nameIndex, int contentIndex)
    {
        for(int i = 0; i < 4; i++)
            GameManager.Bunny.bunnies[i].textBubbleObject.SetActive(i == nameIndex);
        
        nameText.text = StaticManager.Langauge.Localize(75 + nameIndex);
        contentText.text = StaticManager.Langauge.Localize(80 + contentIndex);
    }

    // public void SetTutorial(int index)
    // {
    //     var mom = GameManager.Bunny.bunnies[0].textBubbleObject;
    //     var dad = GameManager.Bunny.bunnies[3].textBubbleObject;
    //     var son = GameManager.Bunny.bunnies[1].textBubbleObject;
    //     var daughter = GameManager.Bunny.bunnies[2].textBubbleObject;
    //     
    //     //카메라 이동
    //     Camera.main.transform.position = new Vector3(4, 4.5f, -10);
    //     
    //     //나린이 대화
    //     //GameManager.Bunny.bunnies[3].textBubbleObject.GetComponentInChildren<TMP_Text>()
    // }
}
