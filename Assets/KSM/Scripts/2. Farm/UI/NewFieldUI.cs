using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Random = UnityEngine.Random;
/*
* 밭 오픈
* 작물 심기
* 작물 기능
*/

public partial class FarmUI : MonoBehaviour
{
    [Header("< Field UI >")]
    [Tooltip("밭 기능 관련")]
    public GameObject fieldCanvas;

    [Tooltip("작물 선택 관련")]
    public GameObject selectCanvas;

    [Tooltip("작물 상태 관련")]
    public GameObject harvestCanvasBundle;
    public GameObject selectPanelPrefab;

    [Tooltip("작물 타이머 관련")]
    public GameObject timerCanvas;

    [Tooltip("스킵 기능 관련")]
    public GameObject skipPanel;

    public GameObject aiObject;

    public GameObject harvestSuccessPrefab;

    [HideInInspector]
    private Vector3[] wayPoints = new Vector3[1];
    private bool randomLoop;
    private int randomHarvest = 0;
    private Vector3 previousPosition = Vector3.zero;
    private float harvestAnimationTime = 0;
    private bool isHarvesting = false;

    #region Update용 함수
    private void UpdateFieldUI()
    {
        UpdateFieldData();
        UpdateHarvestAnimation();
        UpdateSkipFieldData();
    }
    #endregion

    #region Update() -> 밭 상태 체크
    private void UpdateFieldData()
    {
        for(int i = 0; i < BackendServerManager.GetInstance().fieldType.Count; i++)
        {
            switch (BackendServerManager.GetInstance().fieldType[i])
            {
                case -1:    //미오픈
                    fieldCanvas.transform.GetChild(i).GetComponent<Image>().sprite = fieldImage[0];
                    fieldCanvas.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
                    fieldCanvas.transform.GetChild(i).GetChild(1).gameObject.SetActive(true);
                    break;

                case int n when (0 <= n && n <= 2): //건조, 촉촉, 비옥한 밭
                    fieldCanvas.transform.GetChild(i).GetComponent<Image>().sprite = fieldImage[n];
                    fieldCanvas.transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
                    fieldCanvas.transform.GetChild(i).GetChild(1).gameObject.SetActive(false);

                    if(BackendServerManager.GetInstance().field[i] >= 10)
                    {
                        fieldCanvas.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
                        fieldCanvas.transform.GetChild(i).GetChild(1).gameObject.SetActive(false);

                        UpdateHarvestData(i);
                    }

                    break;
            }
        }
    }
    #endregion

    #region 농작물 상태 체크
    private void UpdateHarvestData(int fieldNumber)
    {
        TimeSpan remainTime = DateTime.Parse(BackendServerManager.GetInstance().fieldEndTime[fieldNumber]) - DateTime.UtcNow;
        if (remainTime.TotalSeconds > 0)
        {
            //씨앗 상태일 경우
            if ((1 - (float)remainTime.TotalSeconds / BackendServerManager.GetInstance().harvestSheet[BackendServerManager.GetInstance().field[fieldNumber] - 10].coolTime) <= 0.5f)
            {
                harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(0).gameObject.SetActive(true);
                harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(1).gameObject.SetActive(false);
            }

            //새싹 상태일 경우
            else
            {
                harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(0).gameObject.SetActive(false);
                harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(1).gameObject.SetActive(true);

                harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(1).GetComponent<HarvestAnimation>().sproutStart = true;
            }

            timerCanvas.transform.GetChild(fieldNumber).gameObject.SetActive(true);

            timerCanvas.transform.GetChild(fieldNumber).GetChild(0).GetComponent<Image>().fillAmount = 1 - (float)remainTime.TotalSeconds / BackendServerManager.GetInstance().harvestSheet[BackendServerManager.GetInstance().field[fieldNumber] - 10].coolTime;

            if (remainTime.TotalSeconds > 60)
                timerCanvas.transform.GetChild(fieldNumber).GetChild(2).GetComponent<Text>().text = Mathf.FloorToInt(((int)remainTime.TotalSeconds)) >= 0 ? (Mathf.FloorToInt((int)remainTime.TotalSeconds) / 60).ToString() + "m " + (Mathf.FloorToInt((int)remainTime.TotalSeconds) % 60).ToString() + "s" : "0" + "s";
            else
                timerCanvas.transform.GetChild(fieldNumber).GetChild(2).GetComponent<Text>().text = Mathf.FloorToInt(((int)remainTime.TotalSeconds)) >= 0 ? Mathf.FloorToInt((int)remainTime.TotalSeconds).ToString() + "s" : "0" + "s";
        }

        //완성 되었을 때
        else
        {
            if (harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(BackendServerManager.GetInstance().field[fieldNumber] - 8).gameObject.activeSelf) return;
            harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(0).gameObject.SetActive(false);
            harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(1).gameObject.SetActive(false);

            timerCanvas.transform.GetChild(fieldNumber).gameObject.SetActive(false);

            harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(BackendServerManager.GetInstance().field[fieldNumber] - 8).gameObject.SetActive(true);

            harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(BackendServerManager.GetInstance().field[fieldNumber] - 8).GetComponent<HarvestAnimation>().harvestStart = true;

            harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(BackendServerManager.GetInstance().field[fieldNumber] - 8).GetChild(harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(BackendServerManager.GetInstance().field[fieldNumber] - 8).childCount - 1).GetComponent<Button>().onClick.RemoveAllListeners();
            harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(BackendServerManager.GetInstance().field[fieldNumber] - 8).GetChild(harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(BackendServerManager.GetInstance().field[fieldNumber] - 8).childCount - 1).GetComponent<Button>().onClick.AddListener(() =>
            {

                if (!isHarvesting)
                {
                    randomHarvest = Random.Range(0, 3);
                    previousPosition = characterList[randomHarvest].transform.position;
                }

                isHarvesting = true;
                harvestAnimationTime = 1f;
                characterList[randomHarvest].GetComponent<SortingGroup>().sortingLayerName = "UI";
                characterList[randomHarvest].GetComponent<SortingGroup>().sortingOrder = -1;
                characterList[randomHarvest].GetComponent<NavMeshAgent>().enabled = false;
                characterList[randomHarvest].GetComponent<FarmCharacter>().isHarvest = true;
                characterList[randomHarvest].transform.position = fieldCanvas.transform.GetChild(fieldNumber).position + (characterList[randomHarvest].GetComponent<SkeletonAnimation>().skeleton.ScaleX > 0 ? new Vector3(1, -1, 0) : new Vector3(-1, -1, 0));
                characterList[randomHarvest].GetComponent<SkeletonAnimation>().state.SetAnimation(0, "harvest_front_l", false);

                StartCoroutine(GetHarvest(fieldNumber));
            });
        }
    }
    #endregion

    #region 스킵
    private void UpdateSkipFieldData()
    {
        if (!skipPanel.activeSelf) return;
        skipPanel.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = PlayerPrefs.GetString("Langauge") == "ko" ? BackendServerManager.GetInstance().langaugeSheet[BackendServerManager.GetInstance().field[skipField] - 3].ko : BackendServerManager.GetInstance().langaugeSheet[BackendServerManager.GetInstance().field[skipField] - 3].en;
        skipPanel.transform.GetChild(4).GetComponentInChildren<TextMeshProUGUI>().text = PlayerPrefs.GetString("Langauge") == "ko" ? BackendServerManager.GetInstance().langaugeSheet[1].ko : BackendServerManager.GetInstance().langaugeSheet[1].en;
        skipPanel.transform.GetChild(2).GetComponent<Image>().sprite = inventoryHarvestImage[BackendServerManager.GetInstance().field[skipField] - 10];

        TimeSpan ts = DateTime.Parse(BackendServerManager.GetInstance().fieldEndTime[skipField]) - DateTime.UtcNow;
        if (ts.TotalSeconds > 60)
        {
            skipPanel.transform.GetChild(3).GetComponentInChildren<TextMeshProUGUI>().text = Mathf.FloorToInt(((int)ts.TotalSeconds)) >= 0 ? (Mathf.FloorToInt((int)ts.TotalSeconds) / 60).ToString() + "m " + (Mathf.FloorToInt((int)ts.TotalSeconds) % 60).ToString() + "s" : "0s";
        }
        else if (ts.TotalSeconds < 60 && ts.TotalSeconds > 0)
        {
            skipPanel.transform.GetChild(3).GetComponentInChildren<TextMeshProUGUI>().text = Mathf.FloorToInt(((int)ts.TotalSeconds)) >= 0 ? Mathf.FloorToInt((int)ts.TotalSeconds).ToString() + "s" : "0" + "s";
        }
        else
        {
            skipPanel.SetActive(false);
        }

        skipPanel.transform.GetChild(5).GetComponentInChildren<Text>().text = (Mathf.FloorToInt(((int)ts.TotalSeconds)) / 60 + 1).ToString();
    }

    private int skipField = 0;
    public void SkipHarvest(int fieldNumber)
    {
        skipField = fieldNumber;

        skipPanel.SetActive(true);

        skipPanel.transform.GetChild(5).GetComponent<Button>().onClick.RemoveAllListeners();
        skipPanel.transform.GetChild(5).GetComponent<Button>().onClick.AddListener(() =>
        {
            //현재 스킵할 수 있는 다이아량을 소지하고 있을 때
            if (BackendServerManager.GetInstance().myInfo.diamond >= int.Parse(skipPanel.transform.GetChild(5).GetComponentInChildren<Text>().text))
            {
                skipPanel.SetActive(false);

                BackendServerManager.GetInstance().myInfo.diamond -= int.Parse(skipPanel.transform.GetChild(5).GetComponentInChildren<Text>().text);
                BackendServerManager.GetInstance().fieldEndTime[fieldNumber] = DateTime.UtcNow.ToString();

                BackendServerManager.GetInstance().SaveMyInfo(true);

            }

            //돈이 부족할 경우
            else
            {
                SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? "다이아가 부족합니다.\n상점으로 이동하겠습니까?" : "Not enough diamonds.\nWould you like to go to the store?", () =>
                {
                    ChangeShopCategory(0);
                });
            }
        });
    }
    #endregion

    #region Harvest Animation
    private void UpdateHarvestAnimation()
    {
        if (isHarvesting)
        {
            if(harvestAnimationTime > 0)
            {
                harvestAnimationTime -= Time.deltaTime;
            }
            else
            {
                isHarvesting = false;
                characterList[randomHarvest].GetComponent<FarmCharacter>().isHarvest = false;
                characterList[randomHarvest].transform.position = previousPosition;
                characterList[randomHarvest].GetComponent<NavMeshAgent>().enabled = true;
                characterList[randomHarvest].GetComponent<SortingGroup>().sortingLayerName = "ForeGround";
                characterList[randomHarvest].GetComponent<SortingGroup>().sortingOrder = 0;
            }
        }
    }
    #endregion

    #region 수확 애니메이션
    private IEnumerator GetHarvest(int fieldNumber)
    {
        harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(BackendServerManager.GetInstance().field[fieldNumber] - 8).GetChild(harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(BackendServerManager.GetInstance().field[fieldNumber] - 8).childCount - 1).gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);

        GameObject harvestPrefab = Instantiate(harvestSuccessPrefab, inventoryObject.transform);
        harvestPrefab.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);


        harvestPrefab.transform.GetChild(0).GetComponent<Image>().sprite = inventoryHarvestImage[BackendServerManager.GetInstance().field[fieldNumber] - 10];
        harvestPrefab.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(inventoryHarvestImage[BackendServerManager.GetInstance().field[fieldNumber] - 10].textureRect.width, inventoryHarvestImage[BackendServerManager.GetInstance().field[fieldNumber] - 10].textureRect.height);

        harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(BackendServerManager.GetInstance().field[fieldNumber] - 8).gameObject.SetActive(false);
        harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(BackendServerManager.GetInstance().field[fieldNumber] - 8).GetChild(harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(BackendServerManager.GetInstance().field[fieldNumber] - 8).childCount - 1).gameObject.SetActive(true);
        BackendServerManager.GetInstance().myInfo.harvest[BackendServerManager.GetInstance().field[fieldNumber] - 10]++;
        BackendServerManager.GetInstance().field[fieldNumber] = 0;
        BackendServerManager.GetInstance().fieldType[fieldNumber] = 0;
        BackendServerManager.GetInstance().SaveMyInfo(true);

    }
    #endregion

    #region 밭을 눌렀을 때 뜨는 팝업창
    public void FieldClick(int fieldNumber)
    {
        ButtonClick();

        //기존에 켜져있는 팝업창 삭제
        if (selectCanvas.transform.childCount > 0)
            for (int i = 0; i < selectCanvas.transform.childCount; i++)
                Destroy(selectCanvas.transform.GetChild(i).gameObject);

        GameObject selectPrefab = Instantiate(selectPanelPrefab, selectCanvas.transform);
        selectPrefab.transform.position = fieldCanvas.transform.GetChild(fieldNumber).position;

        //이미지 투명한 곳 터치 방지
        for(int i = 0; i < 3; i++)
        {
            selectPrefab.transform.GetChild(0).GetChild(i).GetComponentsInChildren<Image>()[0].alphaHitTestMinimumThreshold = 0.1f;
            selectPrefab.transform.GetChild(0).GetChild(i).GetComponentsInChildren<Image>()[1].alphaHitTestMinimumThreshold = 0.1f;
        }

        //비료 보유량 개수
        selectPrefab.transform.GetChild(0).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().color = BackendServerManager.GetInstance().myInfo.fertilizer > 0 ? new Color(1, 0.65f, 0) : new Color(1, 0, 0);
        selectPrefab.transform.GetChild(0).GetChild(2).GetComponentInChildren<TextMeshProUGUI>().color = BackendServerManager.GetInstance().myInfo.fertilizer2 > 0 ? new Color(1, 0.65f, 0) : new Color(1, 0, 0);
        selectPrefab.transform.GetChild(0).GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = BackendServerManager.GetInstance().myInfo.fertilizer2.ToString();
        selectPrefab.transform.GetChild(0).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = BackendServerManager.GetInstance().myInfo.fertilizer.ToString();

        //비료 버튼 활성화
        selectPrefab.transform.GetChild(0).GetChild(2).GetComponent<Toggle>().interactable = BackendServerManager.GetInstance().myInfo.fertilizer2 > 0;
        selectPrefab.transform.GetChild(0).GetChild(1).GetComponent<Toggle>().interactable = BackendServerManager.GetInstance().myInfo.fertilizer > 0;

        //최초 실행했을 때(비료 X)
        for (int i = 0; i < 9; i++)
        {
            if (i < 3)
            {
                selectPrefab.transform.GetChild(i + 1).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                selectPrefab.transform.GetChild(i + 1).GetChild(0).gameObject.SetActive(true);
                selectPrefab.transform.GetChild(i + 1).GetComponent<MultiImageButton>().interactable = true;
                selectPrefab.transform.GetChild(i + 1).GetComponentInChildren<TextMeshProUGUI>().text = BackendServerManager.GetInstance().myInfo.seed[i] > 99 ? "99+" : BackendServerManager.GetInstance().myInfo.seed[i].ToString();
                selectPrefab.transform.GetChild(i + 1).GetComponentInChildren<TextMeshProUGUI>().color = BackendServerManager.GetInstance().myInfo.seed[i] > 0 ? new Color(0, 0, 0, 1) : new Color(1, 0, 0, 1);
            }
            else
            {
                selectPrefab.transform.GetChild(i + 1).GetComponent<Image>().color = new Color(0, 0, 0, 1);
                selectPrefab.transform.GetChild(i + 1).GetChild(0).gameObject.SetActive(false);
                selectPrefab.transform.GetChild(i + 1).GetComponent<MultiImageButton>().interactable = false;
            }
        }

        //비료 X
        selectPrefab.transform.GetChild(0).GetChild(0).GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
        selectPrefab.transform.GetChild(0).GetChild(0).GetComponent<Toggle>().onValueChanged.AddListener(delegate
        {
            if (selectPrefab.transform.GetChild(0).GetChild(0).GetComponent<Toggle>().isOn)
            {
                ButtonClick();

                for (int i = 0; i < 9; i++)
                {
                    if (i < 3)
                    {
                        selectPrefab.transform.GetChild(i + 1).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                        selectPrefab.transform.GetChild(i + 1).GetChild(0).gameObject.SetActive(true);
                        selectPrefab.transform.GetChild(i + 1).GetComponent<MultiImageButton>().interactable = true;
                        selectPrefab.transform.GetChild(i + 1).GetComponentInChildren<TextMeshProUGUI>().text = BackendServerManager.GetInstance().myInfo.seed[i] > 99 ? "99+" : BackendServerManager.GetInstance().myInfo.seed[i].ToString();
                        selectPrefab.transform.GetChild(i + 1).GetComponentInChildren<TextMeshProUGUI>().color = BackendServerManager.GetInstance().myInfo.seed[i] > 0 ? new Color(0, 0, 0, 1) : new Color(1, 0, 0, 1);
                    }
                    else
                    {
                        selectPrefab.transform.GetChild(i + 1).GetComponent<Image>().color = new Color(0, 0, 0, 1);
                        selectPrefab.transform.GetChild(i + 1).GetChild(0).gameObject.SetActive(false);
                        selectPrefab.transform.GetChild(i + 1).GetComponent<MultiImageButton>().interactable = false;
                    }
                }
            }
        });

        //일반 비료
        selectPrefab.transform.GetChild(0).GetChild(1).GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
        selectPrefab.transform.GetChild(0).GetChild(1).GetComponent<Toggle>().onValueChanged.AddListener(delegate
        {
            if (selectPrefab.transform.GetChild(0).GetChild(1).GetComponent<Toggle>().isOn)
            {
                ButtonClick();

                for (int i = 0; i < 9; i++)
                {
                    if (i < 6)
                    {
                        selectPrefab.transform.GetChild(i + 1).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                        selectPrefab.transform.GetChild(i + 1).GetChild(0).gameObject.SetActive(true);
                        selectPrefab.transform.GetChild(i + 1).GetComponent<MultiImageButton>().interactable = true;
                        selectPrefab.transform.GetChild(i + 1).GetComponentInChildren<TextMeshProUGUI>().text = BackendServerManager.GetInstance().myInfo.seed[i] > 99 ? "99+" : BackendServerManager.GetInstance().myInfo.seed[i].ToString();
                        selectPrefab.transform.GetChild(i + 1).GetComponentInChildren<TextMeshProUGUI>().color = BackendServerManager.GetInstance().myInfo.seed[i] > 0 ? new Color(0, 0, 0, 1) : new Color(1, 0, 0, 1);

                    }
                    else
                    {
                        selectPrefab.transform.GetChild(i + 1).GetComponent<Image>().color = new Color(0, 0, 0, 1);
                        selectPrefab.transform.GetChild(i + 1).GetChild(0).gameObject.SetActive(false);
                        selectPrefab.transform.GetChild(i + 1).GetComponent<MultiImageButton>().interactable = false;
                    }
                }
            }
        });

        //천연 비료
        selectPrefab.transform.GetChild(0).GetChild(2).GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
        selectPrefab.transform.GetChild(0).GetChild(2).GetComponent<Toggle>().onValueChanged.AddListener(delegate
        {
            if (selectPrefab.transform.GetChild(0).GetChild(2).GetComponent<Toggle>().isOn)
            {
                ButtonClick();

                for (int i = 0; i < 9; i++)
                {
                    selectPrefab.transform.GetChild(i + 1).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    selectPrefab.transform.GetChild(i + 1).GetChild(0).gameObject.SetActive(true);
                    selectPrefab.transform.GetChild(i + 1).GetComponent<MultiImageButton>().interactable = true;
                    selectPrefab.transform.GetChild(i + 1).GetComponentInChildren<TextMeshProUGUI>().text = BackendServerManager.GetInstance().myInfo.seed[i] > 99 ? "99+" : BackendServerManager.GetInstance().myInfo.seed[i].ToString();
                    selectPrefab.transform.GetChild(i + 1).GetComponentInChildren<TextMeshProUGUI>().color = BackendServerManager.GetInstance().myInfo.seed[i] > 0 ? new Color(0, 0, 0, 1) : new Color(1, 0, 0, 1);

                }
            }
        });

        //씨앗 선택
        for(int i = 0; i < 9; i++)
        {
            int n = i;

            selectPrefab.transform.GetChild(i + 1).GetComponent<Button>().onClick.RemoveAllListeners();
            selectPrefab.transform.GetChild(i + 1).GetComponent<Button>().onClick.AddListener(() =>
            {
                ButtonClick();

                if(BackendServerManager.GetInstance().myInfo.seed[n] > 0)
                {
                    ButtonClick(2);

                    BackendServerManager.GetInstance().myInfo.seed[n]--;

                    //비료의 상태에 따라
                    for(int i = 0; i < 3; i++)
                    {
                        if (selectPrefab.transform.GetChild(0).GetChild(i).GetComponent<Toggle>().isOn)
                        {
                            switch (i)
                            {
                                case 1:
                                    BackendServerManager.GetInstance().myInfo.fertilizer--;
                                    BackendServerManager.GetInstance().fieldType[fieldNumber] = 1;
                                    break;

                                case 2:
                                    BackendServerManager.GetInstance().myInfo.fertilizer2--;
                                    BackendServerManager.GetInstance().fieldType[fieldNumber] = 2;
                                    break;
                            }
                        }
                    }

                    BackendServerManager.GetInstance().field[fieldNumber] = 10 + n;
                    BackendServerManager.GetInstance().fieldEndTime[fieldNumber] = DateTime.UtcNow.AddSeconds(BackendServerManager.GetInstance().harvestSheet[n].coolTime).ToString();

                    harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(0).GetComponent<HarvestAnimation>().seedStart = true;

                    BackendServerManager.GetInstance().SaveMyInfo(true);

                    Destroy(selectPrefab);
                }

                //씨앗이 부족할 경우
                else
                {
                    SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? "씨앗이 부족합니다.\n상점으로 이동하겠습니까?" : "There are not enough seeds.\nWould you like to move to the store?", () =>
                    {
                        ChangeShopCategory(1);

                        Destroy(selectPrefab);
                    });
                }

            });
        }

    }
    #endregion

    //==================================================================

}
