using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;
using Cinemachine;
using System;
using UnityEngine.Events;
using TMPro;
using Spine.Unity;
using UnityEngine.Rendering;

public partial class FarmUI : MonoBehaviour
{
    [Header("[ Field Object ]")]
    public GameObject[] fieldObject;

    [Header("[ Field Panel ]")]
    public GameObject[] fieldUIObject;
    public GameObject[] selectPanels;
    public GameObject[] selectSeedPanels;
    public Sprite[] fieldImage;

    [Header("[ Camera ] ")]
    public GameObject mainCamera;
    public GameObject zoomCamera;
    public PlayableDirector playableDirector;
    public TimelineAsset zoomInTimeline;

    [Header("[ Skip Object ]")]
    public GameObject skipObject;
    private int skipField = 0;
    private bool isSkipObjectActive = false;

    [Header("< New Field UI >")]
    public GameObject fieldCanvas;
    public GameObject selectCanvas;
    public GameObject selectPanelPrefab;
    public GameObject harvestCanvasBundle;
    public GameObject timerCanvas;
    public GameObject harvestDragCanvas;

    public GameObject[] aiObject;

    /*
     * 애니메이션 관련
     * 
     */

    [HideInInspector]
    public bool[] isSprout, isSeed, isHarvest, isComplete;

    private float harvestAnimationTime = 0f;
    private Vector3 previousAnimationPosition = Vector3.zero;
    private bool isPrevious = false;


    //애니메이션 체크

    public Coroutine[] fieldCoroutine = new Coroutine[9]; 

    #region 밭 상태 변화 체크
    public void ReloadFieldData()
    {
        //총 9번 실행
        for (int i = 0; i < BackendServerManager.GetInstance().fieldType.Count; i++)
        {
            switch (BackendServerManager.GetInstance().fieldType[i])
            {
                case -1:    //미오픈
                    fieldCanvas.transform.GetChild(i).GetComponent<Image>().sprite = fieldImage[0];
                    fieldCanvas.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
                    fieldCanvas.transform.GetChild(i).GetChild(1).gameObject.SetActive(true);
                    break;
                case int n when (0 <= n && n <= 2): //건조한 밭, 촉촉한 밭, 비옥한 밭
                    fieldCanvas.transform.GetChild(i).GetComponent<Image>().sprite = fieldImage[n];
                    fieldCanvas.transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
                    fieldCanvas.transform.GetChild(i).GetChild(1).gameObject.SetActive(false);

                    if (BackendServerManager.GetInstance().field[i] >= 10)
                    {
                        fieldCanvas.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
                        fieldCanvas.transform.GetChild(i).GetChild(1).gameObject.SetActive(false);
                        UpdateFieldState(i);
                    }
                    break;
            }
        }
    }
    #endregion

    private void UpdateFieldState(int fieldNumber)
    {
        TimeSpan ts = DateTime.Parse(BackendServerManager.GetInstance().fieldEndTime[fieldNumber]) - DateTime.UtcNow;

        if (ts.TotalSeconds < 0)
        {
            harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(0).gameObject.SetActive(false);    //씨앗 해제
            harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(1).gameObject.SetActive(false);    //새싹 해제

            timerCanvas.transform.GetChild(fieldNumber).gameObject.SetActive(false);    //타이머 해제

            harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(BackendServerManager.GetInstance().field[fieldNumber] - 8).gameObject.SetActive(true);

            //접속 중에 새싹에서 완성이 되었을 때
            if (isHarvest[fieldNumber])
            {
                harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(BackendServerManager.GetInstance().field[fieldNumber] - 8).GetComponent<Animator>().SetBool("isHarvest", true); //새싹에서 완성으로 가는 애니메이션
                isHarvest[fieldNumber] = false;     //수확 애니메이션 초기화
            }

            if (!isComplete[fieldNumber])
            {

                harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(BackendServerManager.GetInstance().field[fieldNumber] - 8).GetChild(harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(BackendServerManager.GetInstance().field[fieldNumber] - 8).childCount - 1).GetComponent<Button>().onClick.RemoveAllListeners();
                harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(BackendServerManager.GetInstance().field[fieldNumber] - 8).GetChild(harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(BackendServerManager.GetInstance().field[fieldNumber] - 8).childCount - 1).GetComponent<Button>().onClick.AddListener(() =>
                {
                    ButtonClick();

                    harvestAnimationTime = 1f;  //돌아갈 타이

                    if (!isPrevious)
                    {
                        isPrevious = true;
                        previousAnimationPosition = aiObject[0].transform.position;
                    }

                    aiObject[0].GetComponent<SortingGroup>().sortingOrder = 99;
                    aiObject[0].GetComponent<AIMovement>().isHarvest = true;
                    aiObject[0].transform.position = harvestCanvasBundle.transform.GetChild(fieldNumber).position + new Vector3(2, 0, 0);
                    aiObject[0].GetComponent<SkeletonAnimation>().Initialize(true);
                    aiObject[0].GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "harvest_front_l", true);

                    BackendServerManager.GetInstance().myInfo.harvest[BackendServerManager.GetInstance().field[fieldNumber] - 10]++;

                    harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(BackendServerManager.GetInstance().field[fieldNumber] - 8).gameObject.SetActive(false);

                    BackendServerManager.GetInstance().field[fieldNumber] = 0;
                    BackendServerManager.GetInstance().fieldType[fieldNumber] = 0;

                    isComplete[fieldNumber] = false;

                    BackendServerManager.GetInstance().SaveMyInfo(true);
                });

                isComplete[fieldNumber] = true;
            }
        }

        else
        {

            //씨앗일 경우
            if ((1 - (float)ts.TotalSeconds / BackendServerManager.GetInstance().harvestSheet[BackendServerManager.GetInstance().field[fieldNumber] - 10].coolTime) < 0.5f)
            {
                if (!harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(0).gameObject.activeSelf)
                    harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(0).gameObject.SetActive(true);
                harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(1).gameObject.SetActive(false);

                //씨앗 튀어나오는 애니메이션
                if (isSeed[fieldNumber])
                {
                    harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(0).GetComponent<Animator>().SetBool("isStart", true);

                    isSeed[fieldNumber] = false;        //씨앗 애니메이션 초기화
                }

                //씨앗 상태일 경우 새싹 애니메이션 활성화
                isSprout[fieldNumber] = true;
            }

            else
            {
                harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(0).gameObject.SetActive(false);
                if(!harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(1).gameObject.activeSelf)
                    harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(1).gameObject.SetActive(true);

                if (isSprout[fieldNumber])
                {
                    harvestCanvasBundle.transform.GetChild(fieldNumber).GetChild(1).GetComponent<Animator>().SetBool("isSeed", true);

                    isSprout[fieldNumber] = false;
                }
            }

            isHarvest[fieldNumber] = true;

            //타이머 활성화
            timerCanvas.transform.GetChild(fieldNumber).gameObject.SetActive(true);

            timerCanvas.transform.GetChild(fieldNumber).GetChild(0).GetComponent<Image>().fillAmount = 1 - (float)ts.TotalSeconds / BackendServerManager.GetInstance().harvestSheet[BackendServerManager.GetInstance().field[fieldNumber] - 10].coolTime;

            if (ts.TotalSeconds > 60)
            {
                timerCanvas.transform.GetChild(fieldNumber).GetChild(2).GetComponent<Text>().text = Mathf.FloorToInt(((int)ts.TotalSeconds)) >= 0 ? (Mathf.FloorToInt((int)ts.TotalSeconds) / 60).ToString() + "m " + (Mathf.FloorToInt((int)ts.TotalSeconds) % 60).ToString() + "s" : "0" + "s";
            }
            else
            {
                timerCanvas.transform.GetChild(fieldNumber).GetChild(2).GetComponent<Text>().text = Mathf.FloorToInt(((int)ts.TotalSeconds)) >= 0 ? Mathf.FloorToInt((int)ts.TotalSeconds).ToString() + "s" : "0" + "s";
            }
        }

    }
    private void UpdateHarvestAnimation()
    {
        if(harvestAnimationTime > 0)
        {
            harvestAnimationTime -= Time.deltaTime;
        }
        if(isPrevious && harvestAnimationTime <= 0)
        {
            harvestAnimationTime = 0;
            isPrevious = false;
            aiObject[0].GetComponent<AIMovement>().isHarvest = false;
            aiObject[0].GetComponent<SortingGroup>().sortingOrder = 0;
            aiObject[0].transform.position = previousAnimationPosition;
        }
    }

    private void UpdateSkipFieldData()
    {
        if (!isSkipObjectActive) return;

        TimeSpan ts = DateTime.Parse(BackendServerManager.GetInstance().fieldEndTime[skipField]) - DateTime.UtcNow;
        if(ts.TotalSeconds > 60)
        {
            skipObject.transform.GetChild(3).GetComponentInChildren<TextMeshProUGUI>().text = Mathf.FloorToInt(((int)ts.TotalSeconds)) >= 0 ? (Mathf.FloorToInt((int)ts.TotalSeconds) / 60).ToString() + "m " + (Mathf.FloorToInt((int)ts.TotalSeconds) % 60).ToString() + "s" : "0s";
        }
        else if(ts.TotalSeconds < 60 && ts.TotalSeconds > 0)
        {
            skipObject.transform.GetChild(3).GetComponentInChildren<TextMeshProUGUI>().text = Mathf.FloorToInt(((int)ts.TotalSeconds)) >= 0 ? Mathf.FloorToInt((int)ts.TotalSeconds).ToString() + "s" : "0" + "s";
        }
        else
        {
            skipObject.SetActive(false);
        }

        skipObject.transform.GetChild(5).GetComponentInChildren<Text>().text = (Mathf.FloorToInt(((int)ts.TotalSeconds)) / 60 + 1).ToString();
    }

    //밭 상태 - 0
    #region 밭을 눌렀을 때
    public void NewFieldClick(int fieldNumber)
    {
        ButtonClick();

        if(selectCanvas.transform.childCount > 0)
        {
            for (int i = 0; i < selectCanvas.transform.childCount; i++)
                Destroy(selectCanvas.transform.GetChild(i).gameObject);
        }

        GameObject selectPrefab = Instantiate(selectPanelPrefab, selectCanvas.transform);
        selectPrefab.transform.position = fieldCanvas.transform.GetChild(fieldNumber).position;

        //비료 모드 버튼
        if(BackendServerManager.GetInstance().myInfo.fertilizer > 0 || BackendServerManager.GetInstance().myInfo.fertilizer2 > 0)
        {
            selectPrefab.transform.GetChild(selectPrefab.transform.childCount - 1).GetComponentInChildren<Text>().text = PlayerPrefs.GetString("Langauge") == "ko" ? "비료" : "Fertilizer";
            selectPrefab.transform.GetChild(selectPrefab.transform.childCount - 1).GetComponent<Button>().onClick.RemoveAllListeners();
            selectPrefab.transform.GetChild(selectPrefab.transform.childCount - 1).GetComponent<Button>().onClick.AddListener(() =>
            {
                ButtonClick();
                 
                //비료 모드 0
                if(selectPrefab.transform.GetChild(selectPrefab.transform.childCount - 1).GetChild(0).gameObject.activeSelf)
                {
                    //일반 비료가 있을 경우
                    if(BackendServerManager.GetInstance().myInfo.fertilizer > 0)
                    {
                        selectPrefab.transform.GetChild(selectPrefab.transform.childCount - 1).GetChild(0).gameObject.SetActive(false);
                        selectPrefab.transform.GetChild(selectPrefab.transform.childCount - 1).GetChild(1).gameObject.SetActive(true);
                        selectPrefab.transform.GetChild(selectPrefab.transform.childCount - 1).GetChild(2).gameObject.SetActive(false);


                        for(int i = 0; i < 9; i++)
                        {
                            if(i < 6)
                            {
                                selectPrefab.transform.GetChild(i).GetComponent<Button>().interactable = true;
                                selectPrefab.transform.GetChild(i).GetChild(0).gameObject.SetActive(true);

                                //소지량이 100개이상일 경우
                                if (BackendServerManager.GetInstance().myInfo.seed[i] > 99)
                                {
                                    selectPrefab.transform.GetChild(i).GetComponentInChildren<Text>().text = "99+";
                                    selectPrefab.transform.GetChild(i).GetComponent<Image>().color = new Color(1f, 1f, 1f);
                                }

                                else if (BackendServerManager.GetInstance().myInfo.seed[i] > 0 && BackendServerManager.GetInstance().myInfo.seed[i] <= 99)
                                {
                                    selectPrefab.transform.GetChild(i).GetComponent<Image>().color = new Color(1f, 1f, 1f);
                                    selectPrefab.transform.GetChild(i).GetComponentInChildren<Text>().text = BackendServerManager.GetInstance().myInfo.seed[i].ToString();
                                }

                                else
                                {
                                    selectPrefab.transform.GetChild(i).GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f);
                                    selectPrefab.transform.GetChild(i).GetComponentInChildren<Text>().text = BackendServerManager.GetInstance().myInfo.seed[i].ToString();
                                    selectPrefab.transform.GetChild(i).GetComponentInChildren<Text>().color = new Color(1f, 0, 0);
                                }
                            }
                            else
                            {
                                selectPrefab.transform.GetChild(i).GetComponent<Image>().color = new Color(0, 0, 0);
                                selectPrefab.transform.GetChild(i).GetComponent<Button>().interactable = false;
                                selectPrefab.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);

                            }
                        }

                    }

                    //특수 비료가 있을 경우
                    else
                    {
                        selectPrefab.transform.GetChild(selectPrefab.transform.childCount - 1).GetChild(0).gameObject.SetActive(false);
                        selectPrefab.transform.GetChild(selectPrefab.transform.childCount - 1).GetChild(1).gameObject.SetActive(false);
                        selectPrefab.transform.GetChild(selectPrefab.transform.childCount - 1).GetChild(2).gameObject.SetActive(true);

                        for (int i = 0; i < 9; i++)
                        {
                            selectPrefab.transform.GetChild(i).GetComponent<Image>().color = new Color(1f, 1f, 1f);
                            selectPrefab.transform.GetChild(i).GetComponent<Button>().interactable = true;
                            selectPrefab.transform.GetChild(i).GetChild(0).gameObject.SetActive(true);

                            //소지량이 100개이상일 경우
                            if (BackendServerManager.GetInstance().myInfo.seed[i] > 99)
                                selectPrefab.transform.GetChild(i).GetComponentInChildren<Text>().text = "99+";

                            else if (BackendServerManager.GetInstance().myInfo.seed[i] > 0 && BackendServerManager.GetInstance().myInfo.seed[i] <= 99)
                                selectPrefab.transform.GetChild(i).GetComponentInChildren<Text>().text = BackendServerManager.GetInstance().myInfo.seed[i].ToString();

                            else
                            {
                                selectPrefab.transform.GetChild(i).GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f);
                                selectPrefab.transform.GetChild(i).GetComponentInChildren<Text>().text = BackendServerManager.GetInstance().myInfo.seed[i].ToString();
                                selectPrefab.transform.GetChild(i).GetComponentInChildren<Text>().color = new Color(1f, 0, 0);
                            }
                        }

                    }
                }

                //일반 비료
                else if(selectPrefab.transform.GetChild(selectPrefab.transform.childCount - 1).GetChild(1).gameObject.activeSelf)
                {
                    if (BackendServerManager.GetInstance().myInfo.fertilizer2 > 0)
                    {
                        selectPrefab.transform.GetChild(selectPrefab.transform.childCount - 1).GetChild(0).gameObject.SetActive(false);
                        selectPrefab.transform.GetChild(selectPrefab.transform.childCount - 1).GetChild(1).gameObject.SetActive(false);
                        selectPrefab.transform.GetChild(selectPrefab.transform.childCount - 1).GetChild(2).gameObject.SetActive(true);


                        for (int i = 0; i < 9; i++)
                        {
                            selectPrefab.transform.GetChild(i).GetComponent<Image>().color = new Color(1f, 1f, 1f);
                            selectPrefab.transform.GetChild(i).GetComponent<Button>().interactable = true;
                            selectPrefab.transform.GetChild(i).GetChild(0).gameObject.SetActive(true);

                            //소지량이 100개이상일 경우
                            if (BackendServerManager.GetInstance().myInfo.seed[i] > 99)
                                selectPrefab.transform.GetChild(i).GetComponentInChildren<Text>().text = "99+";

                            else if (BackendServerManager.GetInstance().myInfo.seed[i] > 0 && BackendServerManager.GetInstance().myInfo.seed[i] <= 99)
                                selectPrefab.transform.GetChild(i).GetComponentInChildren<Text>().text = BackendServerManager.GetInstance().myInfo.seed[i].ToString();

                            else
                            {
                                selectPrefab.transform.GetChild(i).GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f);
                                selectPrefab.transform.GetChild(i).GetComponentInChildren<Text>().text = BackendServerManager.GetInstance().myInfo.seed[i].ToString();
                                selectPrefab.transform.GetChild(i).GetComponentInChildren<Text>().color = new Color(1f, 0, 0);
                            }
                        }

                    }

                    //특수 비료가 없을 때
                    else
                    {
                        selectPrefab.transform.GetChild(selectPrefab.transform.childCount - 1).GetChild(0).gameObject.SetActive(true);
                        selectPrefab.transform.GetChild(selectPrefab.transform.childCount - 1).GetChild(1).gameObject.SetActive(false);
                        selectPrefab.transform.GetChild(selectPrefab.transform.childCount - 1).GetChild(2).gameObject.SetActive(false);

                        for (int i = 0; i < 9; i++)
                        {
                            if (i < 3)
                            {
                                selectPrefab.transform.GetChild(i).GetComponent<Image>().color = new Color(1f, 1f, 1f);
                                selectPrefab.transform.GetChild(i).GetComponent<Button>().interactable = true;
                                selectPrefab.transform.GetChild(i).GetChild(0).gameObject.SetActive(true);

                                //소지량이 100개이상일 경우
                                if (BackendServerManager.GetInstance().myInfo.seed[i] > 99)
                                    selectPrefab.transform.GetChild(i).GetComponentInChildren<Text>().text = "99+";

                                else if (BackendServerManager.GetInstance().myInfo.seed[i] > 0 && BackendServerManager.GetInstance().myInfo.seed[i] <= 99)
                                    selectPrefab.transform.GetChild(i).GetComponentInChildren<Text>().text = BackendServerManager.GetInstance().myInfo.seed[i].ToString();

                                else
                                {
                                    selectPrefab.transform.GetChild(i).GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f);
                                    selectPrefab.transform.GetChild(i).GetComponentInChildren<Text>().text = BackendServerManager.GetInstance().myInfo.seed[i].ToString();
                                    selectPrefab.transform.GetChild(i).GetComponentInChildren<Text>().color = new Color(1f, 0, 0);
                                }
                            }
                            else
                            {
                                selectPrefab.transform.GetChild(i).GetComponent<Image>().color = new Color(0, 0, 0);
                                selectPrefab.transform.GetChild(i).GetComponent<Button>().interactable = false;
                                selectPrefab.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);

                            }
                        }

                    }
                }

                else if (selectPrefab.transform.GetChild(selectPrefab.transform.childCount - 1).GetChild(2).gameObject.activeSelf)
                {
                    selectPrefab.transform.GetChild(selectPrefab.transform.childCount - 1).GetChild(0).gameObject.SetActive(true);
                    selectPrefab.transform.GetChild(selectPrefab.transform.childCount - 1).GetChild(1).gameObject.SetActive(false);
                    selectPrefab.transform.GetChild(selectPrefab.transform.childCount - 1).GetChild(2).gameObject.SetActive(false);

                    for (int i = 0; i < 9; i++)
                    {
                        if (i < 3)
                        {
                            selectPrefab.transform.GetChild(i).GetComponent<Image>().color = new Color(1f, 1f, 1f);
                            selectPrefab.transform.GetChild(i).GetComponent<Button>().interactable = true;
                            selectPrefab.transform.GetChild(i).GetChild(0).gameObject.SetActive(true);

                            //소지량이 100개이상일 경우
                            if (BackendServerManager.GetInstance().myInfo.seed[i] > 99)
                                selectPrefab.transform.GetChild(i).GetComponentInChildren<Text>().text = "99+";

                            else if (BackendServerManager.GetInstance().myInfo.seed[i] > 0 && BackendServerManager.GetInstance().myInfo.seed[i] <= 99)
                                selectPrefab.transform.GetChild(i).GetComponentInChildren<Text>().text = BackendServerManager.GetInstance().myInfo.seed[i].ToString();

                            else
                            {
                                selectPrefab.transform.GetChild(i).GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f);
                                selectPrefab.transform.GetChild(i).GetComponentInChildren<Text>().text = BackendServerManager.GetInstance().myInfo.seed[i].ToString();
                                selectPrefab.transform.GetChild(i).GetComponentInChildren<Text>().color = new Color(1f, 0, 0);
                            }
                        }
                        else
                        {
                            selectPrefab.transform.GetChild(i).GetComponent<Image>().color = new Color(0, 0, 0);
                            selectPrefab.transform.GetChild(i).GetComponent<Button>().interactable = false;
                            selectPrefab.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);

                        }
                    }
                }
            });
        }

        else
        {
            selectPrefab.transform.GetChild(selectPrefab.transform.childCount - 1).GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
            selectPrefab.transform.GetChild(selectPrefab.transform.childCount - 1).GetComponent<Button>().onClick.RemoveAllListeners();
            selectPrefab.transform.GetChild(selectPrefab.transform.childCount - 1).GetComponent<Button>().onClick.AddListener(() =>
            {
                SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? "비료가 부족합니다.\n상점으로 이동하겠습니까?" : "Fertilizer is not enough.\nWould you like to go to the store?", () =>
                {
                    ChangeShopCategory(1);
                    Destroy(selectPrefab);
                });
            });
        }


        for (int i = 0; i < selectPrefab.transform.childCount - 1; i++)
        {
            if (i < 3)
            {
                selectPrefab.transform.GetChild(i).GetComponent<Image>().color = new Color(1f, 1f, 1f);
                selectPrefab.transform.GetChild(i).GetComponent<Button>().interactable = true;
                selectPrefab.transform.GetChild(i).GetChild(0).gameObject.SetActive(true);

                //소지량이 100개이상일 경우
                if (BackendServerManager.GetInstance().myInfo.seed[i] > 99)
                    selectPrefab.transform.GetChild(i).GetComponentInChildren<Text>().text = "99+";

                else if (BackendServerManager.GetInstance().myInfo.seed[i] > 0 && BackendServerManager.GetInstance().myInfo.seed[i] <= 99)
                    selectPrefab.transform.GetChild(i).GetComponentInChildren<Text>().text = BackendServerManager.GetInstance().myInfo.seed[i].ToString();

                else
                {
                    selectPrefab.transform.GetChild(i).GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f);
                    selectPrefab.transform.GetChild(i).GetComponentInChildren<Text>().text = BackendServerManager.GetInstance().myInfo.seed[i].ToString();
                    selectPrefab.transform.GetChild(i).GetComponentInChildren<Text>().color = new Color(1f, 0, 0);
                }
            }
            else
            {
                selectPrefab.transform.GetChild(i).GetComponent<Image>().color = new Color(0, 0, 0);
                selectPrefab.transform.GetChild(i).GetComponent<Button>().interactable = false;
                selectPrefab.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);

            }



            int n = i;  //람다식 방지

            selectPrefab.transform.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
            selectPrefab.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(() =>
            {
                if(BackendServerManager.GetInstance().myInfo.seed[n] > 0)
                {
                    ButtonClick(2);
                    isSeed[fieldNumber] = true;

                    BackendServerManager.GetInstance().myInfo.seed[n]--;
                    if (selectPrefab.transform.GetChild(selectPrefab.transform.childCount - 1).GetChild(1).gameObject.activeSelf)
                    {
                        BackendServerManager.GetInstance().fieldType[fieldNumber] = 1;
                        BackendServerManager.GetInstance().myInfo.fertilizer--;
                    }
                    else if(selectPrefab.transform.GetChild(selectPrefab.transform.childCount - 1).GetChild(2).gameObject.activeSelf)
                    {
                        BackendServerManager.GetInstance().fieldType[fieldNumber] = 2;
                        BackendServerManager.GetInstance().myInfo.fertilizer2--;
                    }

                    BackendServerManager.GetInstance().field[fieldNumber] = 10 + n;
                    BackendServerManager.GetInstance().fieldEndTime[fieldNumber] = DateTime.UtcNow.AddSeconds(BackendServerManager.GetInstance().harvestSheet[n].coolTime).ToString();

                    BackendServerManager.GetInstance().SaveMyInfo(true);

                    Destroy(selectPrefab);
                }

                //재료가 부족할 경우 즉시 구매 창 띄우기
                else
                {
                    ButtonClick();

                    SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? string.Format("{0}씨앗이 부족합니다.\n{0}씨앗을 구매하겠습니까?", BackendServerManager.GetInstance().langaugeSheet[n + 7].ko) : string.Format("There are not enough {0} seeds.\nDo you want to buy {0} seeds?", BackendServerManager.GetInstance().langaugeSheet[n + 7].en), () =>
                    {
                        if(BackendServerManager.GetInstance().shopSheet[13 + n].cash == "Diamond")
                        {
                            if(BackendServerManager.GetInstance().myInfo.diamond >= BackendServerManager.GetInstance().shopSheet[13 + n].price)
                            {
                                ButtonClick(2);
                                isSeed[fieldNumber] = true;

                                BackendServerManager.GetInstance().myInfo.diamond -= BackendServerManager.GetInstance().shopSheet[13 + n].price;

                                //BackendServerManager.GetInstance().myInfo.seed[n]--;
                                if (selectPrefab.transform.GetChild(selectPrefab.transform.childCount - 1).GetChild(1).gameObject.activeSelf)
                                {
                                    BackendServerManager.GetInstance().myInfo.fertilizer--;
                                    BackendServerManager.GetInstance().fieldType[fieldNumber] = 1;
                                }
                                else if (selectPrefab.transform.GetChild(selectPrefab.transform.childCount - 1).GetChild(2).gameObject.activeSelf)
                                {
                                    BackendServerManager.GetInstance().fieldType[fieldNumber] = 2;
                                    BackendServerManager.GetInstance().myInfo.fertilizer2--;
                                }

                                BackendServerManager.GetInstance().field[fieldNumber] = 10 + n;
                                BackendServerManager.GetInstance().fieldEndTime[fieldNumber] = DateTime.UtcNow.AddSeconds(BackendServerManager.GetInstance().harvestSheet[n].coolTime).ToString();

                                Destroy(selectPrefab);

                                BackendServerManager.GetInstance().SaveMyInfo(true);
                            }
                            else
                            {
                                SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? "다이아몬드가 부족합니다.\n상점으로 이동하겠습니까?" : "We're short of diamonds.\nWould you like to move to the store?", () =>
                                {
                                    ChangeShopCategory(0);

                                    Destroy(selectPrefab);
                                });
                            }
                        }

                        else if (BackendServerManager.GetInstance().shopSheet[13 + n].cash == "Gold")
                        {
                            if (BackendServerManager.GetInstance().myInfo.gold >= BackendServerManager.GetInstance().shopSheet[13 + n].price)
                            {
                                ButtonClick(2);

                                isSeed[fieldNumber] = true;

                                BackendServerManager.GetInstance().myInfo.gold -= BackendServerManager.GetInstance().shopSheet[13 + n].price;

                                //BackendServerManager.GetInstance().myInfo.seed[n]--;
                                if (selectPrefab.transform.GetChild(selectPrefab.transform.childCount - 1).GetChild(1).gameObject.activeSelf)
                                {
                                    BackendServerManager.GetInstance().myInfo.fertilizer--;
                                    BackendServerManager.GetInstance().fieldType[fieldNumber] = 1;
                                }
                                else if (selectPrefab.transform.GetChild(selectPrefab.transform.childCount - 1).GetChild(2).gameObject.activeSelf)
                                {
                                    BackendServerManager.GetInstance().fieldType[fieldNumber] = 2;
                                    BackendServerManager.GetInstance().myInfo.fertilizer2--;
                                }

                                BackendServerManager.GetInstance().field[fieldNumber] = 10 + n;
                                BackendServerManager.GetInstance().fieldEndTime[fieldNumber] = DateTime.UtcNow.AddSeconds(BackendServerManager.GetInstance().harvestSheet[n].coolTime).ToString();

                                Destroy(selectPrefab);

                                BackendServerManager.GetInstance().SaveMyInfo(true);
                            }
                            else
                            {
                                SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? "골드가 부족합니다.\n상점으로 이동하겠습니까?" : "We're short of golds.\nWould you like to move to the store?", () =>
                                {
                                    ChangeShopCategory(0);

                                    Destroy(selectPrefab);
                                });
                            }
                        }
                    });
                }
            });
        }
    }

    #region 스킵 기능 (새싹 눌렀을 때)
    public void OpenSkipHarvestCoolTime(int num)
    {
        isSkipObjectActive = true;
        skipField = num;

        skipObject.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = PlayerPrefs.GetString("Langauge") == "ko" ? BackendServerManager.GetInstance().langaugeSheet[BackendServerManager.GetInstance().field[num] - 3].ko : BackendServerManager.GetInstance().langaugeSheet[BackendServerManager.GetInstance().field[num] - 3].en;

        skipObject.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("Langauge") == "ko" ? BackendServerManager.GetInstance().langaugeSheet[1].ko : BackendServerManager.GetInstance().langaugeSheet[1].en;
        skipObject.transform.GetChild(2).GetComponent<Image>().sprite = inventoryHarvestImage[BackendServerManager.GetInstance().field[num] - 10];
        skipObject.transform.GetChild(5).GetComponent<Button>().onClick.RemoveAllListeners();
        skipObject.transform.GetChild(5).GetComponent<Button>().onClick.AddListener(() =>
        {
            //현재 스킵할 수 있는 다이아량을 소지하고 있을 때
            if(BackendServerManager.GetInstance().myInfo.diamond >= int.Parse(skipObject.transform.GetChild(5).GetComponentInChildren<Text>().text))
            {
                skipObject.SetActive(false);

                BackendServerManager.GetInstance().myInfo.diamond -= int.Parse(skipObject.transform.GetChild(5).GetComponentInChildren<Text>().text);
                BackendServerManager.GetInstance().fieldEndTime[num] = DateTime.UtcNow.ToString();

                BackendServerManager.GetInstance().SaveMyInfo(true);

                fieldUIObject[num].transform.GetChild(2).GetChild(0).gameObject.SetActive(false);

                Debug.LogError(BackendServerManager.GetInstance().field[num] - 9);

                fieldUIObject[num].transform.GetChild(2).GetChild(BackendServerManager.GetInstance().field[num] - 7).gameObject.SetActive(true);
                fieldUIObject[num].transform.GetChild(2).GetChild(BackendServerManager.GetInstance().field[num] - 7).GetChild(fieldUIObject[num].transform.GetChild(2).GetChild(BackendServerManager.GetInstance().field[num] - 7).childCount - 1).GetComponent<Button>().onClick.RemoveAllListeners();

                fieldUIObject[num].transform.GetChild(2).GetChild(BackendServerManager.GetInstance().field[num] - 7).GetChild(fieldUIObject[num].transform.GetChild(2).GetChild(BackendServerManager.GetInstance().field[num] - 7).childCount - 1).GetComponent<Button>().onClick.AddListener(() =>
                {

                    switch (BackendServerManager.GetInstance().field[num])
                    {
                        case 10:    //당근
                            BackendServerManager.GetInstance().myInfo.carrot += 1;
                            break;

                        case 11:    //감자
                            BackendServerManager.GetInstance().myInfo.potato += 1;
                            break;

                        case 12:    //토마토
                            BackendServerManager.GetInstance().myInfo.tomato += 1;
                            break;

                        case 13:    //오이
                            BackendServerManager.GetInstance().myInfo.cucumber += 1;
                            break;

                        case 14:    //복숭아
                            BackendServerManager.GetInstance().myInfo.peach += 1;
                            break;

                        case 15:    //사과
                            BackendServerManager.GetInstance().myInfo.apple += 1;
                            break;

                        case 16:    //호박
                            BackendServerManager.GetInstance().myInfo.pumpkin += 1;
                            break;

                        case 17:    //배
                            BackendServerManager.GetInstance().myInfo.pear += 1;
                            break;

                        case 18:    //체리
                            BackendServerManager.GetInstance().myInfo.cherry += 1;
                            break;
                    }

                    fieldUIObject[num].transform.GetChild(2).GetChild(BackendServerManager.GetInstance().field[num] - 7).gameObject.SetActive(false);

                    BackendServerManager.GetInstance().field[num] = 0;
                    BackendServerManager.GetInstance().fieldType[num] = 0;

                    BackendServerManager.GetInstance().SaveMyInfo(true);
                });
            }

            //돈이 부족할 경우
            else
            {
                CloseSkipHarvestCoolTime();
                SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? "다이아가 부족합니다.\n상점으로 이동하겠습니까?" : "Not enough diamonds.\nWould you like to go to the store?", () =>
                {
                    ChangeShopCategory(0);
                });
            }
            
        });
        skipObject.SetActive(true);
    }

    public void CloseSkipHarvestCoolTime()
    {
        isSkipObjectActive = false;

        skipObject.SetActive(false);
    }
    #endregion


    public IEnumerator fieldCountDown(int num)
    {
        TimeSpan ts = DateTime.Parse(BackendServerManager.GetInstance().fieldEndTime[num]) - DateTime.UtcNow;

        float max = 0;

        switch (BackendServerManager.GetInstance().field[num])
        {
            case 10:    //당근 쿨타임
                max = BackendServerManager.GetInstance().harvestSheet[0].coolTime;
                break;

            case 11:    //감자 쿨타임
                max = BackendServerManager.GetInstance().harvestSheet[3].coolTime;
                break;

            case 12:    //토마토 쿨타임
                max = BackendServerManager.GetInstance().harvestSheet[6].coolTime;
                break;

            case 13:    //오이 쿨타임
                max = BackendServerManager.GetInstance().harvestSheet[9].coolTime;
                break;

            case 14:    //복숭아 쿨타임
                max = BackendServerManager.GetInstance().harvestSheet[13].coolTime;
                break;

            case 15:    //사과 쿨타임
                max = BackendServerManager.GetInstance().harvestSheet[11].coolTime;
                break;

            case 16:    //호박 쿨타임
                max = BackendServerManager.GetInstance().harvestSheet[15].coolTime;
                break;

            case 17:    //배 쿨타임
                max = BackendServerManager.GetInstance().harvestSheet[16].coolTime;
                break;

            case 18:    //체리 쿨타임
                max = BackendServerManager.GetInstance().harvestSheet[17].coolTime;
                break;
        }

        while (ts.TotalSeconds > 0)
        {
            
            ts = DateTime.Parse(BackendServerManager.GetInstance().fieldEndTime[num]) - DateTime.UtcNow;

            if ((max - ts.TotalSeconds) < 1 && fieldUIObject[num].transform.GetChild(2).GetChild(0).gameObject.activeSelf)
            {
                if (!fieldUIObject[num].transform.GetChild(2).GetChild(0).GetComponent<Animator>().GetBool("isStart"))
                    fieldUIObject[num].transform.GetChild(2).GetChild(0).GetComponent<Animator>().SetBool("isStart", true);
            }

            //30% 이하일 경우 씨앗
            if ((1 - (float)ts.TotalSeconds / max) < 0.5f)
            {
                //fieldUIObject[num].transform.GetChild(2).GetChild(0).GetComponent<Animator>().SetBool("isStart", true);

                isSeed[num] = true;

                fieldUIObject[num].transform.GetChild(2).GetChild(0).gameObject.SetActive(true);
                fieldUIObject[num].transform.GetChild(2).GetChild(1).gameObject.SetActive(false);
            }
            //새싹
            else
            {
                fieldUIObject[num].transform.GetChild(2).GetChild(0).gameObject.SetActive(false);
                fieldUIObject[num].transform.GetChild(2).GetChild(1).gameObject.SetActive(true);

                if (isSeed[num])
                {
                    fieldUIObject[num].transform.GetChild(2).GetChild(1).GetComponent<Animator>().SetBool("isSeed", true);
                    isSeed[num] = false;
                    isHarvest[num] = true;
                }
            }

            if(!fieldUIObject[num].transform.GetChild(2).GetChild(2).gameObject.activeSelf)
                fieldUIObject[num].transform.GetChild(2).GetChild(2).gameObject.SetActive(true);
            fieldUIObject[num].transform.GetChild(2).GetChild(2).GetChild(0).GetComponent<Image>().fillAmount = 1 - (float)ts.TotalSeconds / max;

            if (ts.TotalSeconds > 60)
            {
                fieldUIObject[num].transform.GetChild(2).GetChild(2).GetChild(2).GetComponent<Text>().text = Mathf.FloorToInt(((int)ts.TotalSeconds)) >= 0 ? (Mathf.FloorToInt((int)ts.TotalSeconds) / 60).ToString() + "m " + (Mathf.FloorToInt((int)ts.TotalSeconds) % 60).ToString() + "s" : "0" + "s";
            }
            else
            {
                fieldUIObject[num].transform.GetChild(2).GetChild(2).GetChild(2).GetComponent<Text>().text = Mathf.FloorToInt(((int)ts.TotalSeconds)) >= 0 ? Mathf.FloorToInt((int)ts.TotalSeconds).ToString() + "s" : "0" + "s";
            }

            yield return null;
        }

        Debug.LogError("THE END");


        fieldUIObject[num].transform.GetChild(2).GetChild(0).gameObject.SetActive(false);
        fieldUIObject[num].transform.GetChild(2).GetChild(1).gameObject.SetActive(false);
        fieldUIObject[num].transform.GetChild(2).GetChild(2).gameObject.SetActive(false);

        fieldUIObject[num].transform.GetChild(2).GetChild(BackendServerManager.GetInstance().field[num] - 7).gameObject.SetActive(true);
        fieldUIObject[num].transform.GetChild(2).GetChild(BackendServerManager.GetInstance().field[num] - 7).GetChild(fieldUIObject[num].transform.GetChild(2).GetChild(BackendServerManager.GetInstance().field[num] - 7).childCount - 1).GetComponent<Button>().onClick.RemoveAllListeners();

        if (isHarvest[num])
        {
            isHarvest[num] = false;
            fieldUIObject[num].transform.GetChild(2).GetChild(BackendServerManager.GetInstance().field[num] - 7).GetComponent<Animator>().SetBool("isHarvest", true);
        }

        fieldUIObject[num].transform.GetChild(2).GetChild(BackendServerManager.GetInstance().field[num] - 7).GetChild(fieldUIObject[num].transform.GetChild(2).GetChild(BackendServerManager.GetInstance().field[num] - 7).childCount - 1).GetComponent<Button>().onClick.AddListener(() =>
        {
            
            switch (BackendServerManager.GetInstance().field[num])
            {
                case 10:    //당근
                    BackendServerManager.GetInstance().myInfo.carrot += 1;
                    break;

                case 11:    //감자
                    BackendServerManager.GetInstance().myInfo.potato += 1;
                    break;

                case 12:    //토마토
                    BackendServerManager.GetInstance().myInfo.tomato += 1;
                    break;

                case 13:    //오이
                    BackendServerManager.GetInstance().myInfo.cucumber += 1;
                    break;

                case 14:    //복숭아
                    BackendServerManager.GetInstance().myInfo.peach += 1;
                    break;

                case 15:    //사과
                    BackendServerManager.GetInstance().myInfo.apple += 1;
                    break;

                case 16:    //호박
                    BackendServerManager.GetInstance().myInfo.pumpkin += 1;
                    break;

                case 17:    //배
                    BackendServerManager.GetInstance().myInfo.pear += 1;
                    break;

                case 18:    //체리
                    BackendServerManager.GetInstance().myInfo.cherry += 1;
                    break;
            }

            fieldUIObject[num].transform.GetChild(2).GetChild(BackendServerManager.GetInstance().field[num] - 7).gameObject.SetActive(false);

            BackendServerManager.GetInstance().field[num] = 0;
            BackendServerManager.GetInstance().fieldType[num] = 0;

            BackendServerManager.GetInstance().SaveMyInfo(true);
        });
    }
}
