using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//0 - 게임 시작, 1 - 첫 튜토리얼 본 상태, 2 - 튜토리얼 팝업 종료, 3 - 첫 수확, 4 - 마트 튜토리얼
public class TutorialManager : MonoBehaviour
{
    [SerializeField] private string[] momText;
    [SerializeField] private string[] dadText;
    [SerializeField] private string[] daughterText;
    [SerializeField] private string[] sonText;

    [SerializeField] private GameObject bubbleObject;

    public GameObject clickObject;

    public bool isTutorial = false;
    private int index = 0, index2 = 33, index3 = 48;

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
        if(GameManager.Camera.IsPad())
            bubbleObject.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1200, 280);
        
        //튜토리얼이 진행상태라면
        if (StaticManager.Backend.backendGameData.UserData.Tutorial == 0)
        {
            SetTutorial(0);
        }
        if (StaticManager.Backend.backendGameData.UserData.Tutorial == 1)
        {
            GameObject tutorialUI = StaticManager.UI.OpenUI("Prefabs/GameScene/TutorialUI", GameManager.Instance.UICanvas.transform);
            tutorialUI.GetComponent<TutorialUI>().Initialize("Farm", true);
        }

        bubbleObject.GetComponentInChildren<Button>().onClick.AddListener(() =>
        {
            StaticManager.Sound.SetSFX();
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
                        tutorialUI.GetComponent<TutorialUI>().Initialize("Farm", true);
                        
                        GameManager.Instance.SaveAllData();
                        return;
                }
                
                int[] characterIndex = { 2, 0, 0, 0, 1, 3, 1, 2, 3, 0, 4, 0, 2, 3, 0, 1, 4, 0 };
                SetBubble(characterIndex[index], index + 1);
                
                index++;
            }
            
            else if (StaticManager.Backend.backendGameData.UserData.Tutorial == 3)
            {
                int[] characterIndex = { 2, 3, 1, 0, 4, 0, 3, 3, 2, 0, 0, 0, 4, 1 };
                SetBubble(characterIndex[index2 - 33], index2 + 1);

                if (index2 == 46)
                {
                    bubbleObject.SetActive(false);
                    GameManager.Bunny.bunnies[1].textBubbleObject.SetActive(false);
                    for (int i = 0; i < 4; i++)
                    {
                        GameManager.Bunny.ChangeBunnyState(i, BunnyController.State.Idle);
                        GameManager.Bunny.MoveBunny(i, GameManager.Bunny.bunnies[i].originPos);
                    }

                    Camera.main.transform.position = new Vector3(0, 0, -10);

                    isTutorial = false;

                    StaticManager.Backend.backendGameData.UserData.SetTutorial(4);
                        
                    GameManager.Instance.SaveAllData();
                }

                index2++;
            }
            
            else if (StaticManager.Backend.backendGameData.UserData.Tutorial == 4)
            {
                index3++;

                int[] characterIndex = { 0, 1, 0, 0, 1, 0, 0, 1, 1 };
                
                Debug.LogError(index3);
                if (index3 == 58)
                {
                    bubbleObject.SetActive(false);
                    GameManager.Bunny.bunnies[1].textBubbleObject.SetActive(false);
                    for (int i = 0; i < 4; i++)
                    {
                        GameManager.Bunny.ChangeBunnyState(i, BunnyController.State.Idle);
                        GameManager.Bunny.MoveBunny(i, GameManager.Bunny.bunnies[i].originPos);
                    }

                    isTutorial = false;

                    StaticManager.Backend.backendGameData.UserData.SetTutorial(5);
                    clickObject.SetActive(true);
                    
                    GameObject tutorialUI = StaticManager.UI.OpenUI("Prefabs/GameScene/TutorialUI", GameManager.Instance.UICanvas.transform);
                    tutorialUI.GetComponent<TutorialUI>().Initialize("Mart");
                    
                    GameManager.Mart.Guest.Initialize();

                    GameManager.Instance.SaveAllData();

                    return;
                }

                SetBubble(characterIndex[index3 - 49], index3);

            }
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
                
                Camera.main.transform.DOMove(new Vector3(4, 5, -10), 0.1f);
                SetBubble(1, 33);
                
                GameManager.Bunny.MoveBunny(0, new Vector3(5.2f, 3.5f, 0));
                GameManager.Bunny.MoveBunny(1, new Vector3(8.5f, 1.5f, 0));
                GameManager.Bunny.MoveBunny(2, new Vector3(7, 3, 0));
                GameManager.Bunny.MoveBunny(3, new Vector3(3.5f, 2.5f, 0));

                GameManager.Bunny.bunnies[0].direction = 1;
                GameManager.Bunny.bunnies[1].direction = 2;
                GameManager.Bunny.bunnies[2].direction = 2;
                GameManager.Bunny.bunnies[3].direction = 1;
                break;
            
            case 2:
                GameManager.Bunny.ChangeBunnyState(0, BunnyController.State.Idle);
                GameManager.Bunny.ChangeBunnyState(1, BunnyController.State.Idle);
                SetBubble(1, 48);
                GameManager.Bunny.MoveBunny(0, new Vector3(50f, -5.5f, 0));
                GameManager.Bunny.MoveBunny(1, GameManager.Mart.Guest.workPoint.position);

                GameManager.Bunny.bunnies[0].direction = 2;
                GameManager.Bunny.bunnies[1].direction = 1;
                break;
        }
    }

    public void SetBubble(int nameIndex, int contentIndex)
    {
        for(int i = 0; i < 4; i++)
            GameManager.Bunny.bunnies[i].textBubbleObject.SetActive(i == nameIndex);

        if (nameIndex == 4)
        {
            GameManager.Bunny.bunnies[2].textBubbleObject.SetActive(true);
            GameManager.Bunny.bunnies[3].textBubbleObject.SetActive(true);
        }
        
        nameText.text = StaticManager.Langauge.Localize(75 + nameIndex);
        contentText.text = StaticManager.Langauge.Localize(80 + contentIndex);
    }
}
