using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FarmCharacter : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;

    private const string WALK_DOWN = "walk_front_r";
    private const string WALK_UP = "walk_back_r";

    private string[] IDLE = { "idle1_front_r", "idle2_front_r", "idle3_front_r"};

    private const string REWARD = "reward_front_r";

    private enum State { Walk, Idle, Reward};
    private State nowState = State.Idle;

    private int direction = 0;

    private bool loopIdle, loopWalk;

    private bool isReward;

    private const string NETWORK_FAIL = "네트워크가 불안정합니다.\n다시 시도해주세요.";
    private const string NETWORK_FAIL_EN = "Network is unstable.\nPlease try again.";

    //----------------------------------------------------
    [Header("< Character Info >")]
    public float characterSpeed = 5f;
    public int idleEvent1 = 20, idleEvent2 = 20;

    private bool isGift = false; 
    public float giftCoolTime = 10;
    private float giftInit;
    public int giftPercent = 90;
    public int giftReward = 5;

    public bool isHarvest;

    void Start()
    {
        //선물 주는 쿨타임 초기화
        giftInit = giftCoolTime;

        transform.GetChild(0).GetChild(0).gameObject.SetActive(false);  //선물 아이콘 초기화

        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;

        navMeshAgent.Warp(transform.position);

        GetComponent<SkeletonAnimation>().state.Start += delegate (TrackEntry trackEntry)
        {
            if (trackEntry.Animation.Name == REWARD)
            {
                navMeshAgent.ResetPath();

                isReward = true;

                Debug.LogError("Reward Start");
            }
        };

        GetComponent<SkeletonAnimation>().state.End += delegate (TrackEntry trackEntry) {

            if (trackEntry.Animation.Name == REWARD)
            {
                isReward = false;

                isGift = false;
                giftCoolTime = giftInit;

                Debug.LogError("Reward!");
            }
        };

        StartCoroutine(PlayerController());
    }

    void Update()
    {
        navMeshAgent.speed = characterSpeed;

        //수확중일 때 비활성화
        if (transform.GetChild(0).GetChild(0).gameObject.activeSelf)
        {
            transform.GetChild(0).GetChild(0).GetComponent<Image>().color = isHarvest ? new Color(0, 0, 0, 0) : new Color(1, 1, 1, 1);
            transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().color = isHarvest ? new Color(0, 0, 0, 0) : new Color(1, 1, 1, 1);
            transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Button>().interactable = !isHarvest;
        }
        

        if (!isGift)
        {
            if (giftCoolTime > 0)
            {
                giftCoolTime -= Time.deltaTime;
                transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                if (Random.Range(0, 100) <= giftPercent)
                {
                    isGift = true;
                    transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                }
                else
                {
                    giftCoolTime = giftInit;
                }
            }
        }
    }

    void LateUpdate()
    {
        if (!isReward && !isHarvest)
        {
            if (direction - 2 < 0)
                GetComponent<SkeletonAnimation>().skeleton.ScaleX = -0.9f;
            else
                GetComponent<SkeletonAnimation>().skeleton.ScaleX = 0.9f;

            switch (nowState)
            {
                case State.Walk:
                    SetRotateSpine();

                    navMeshAgent.SetDestination(transform.GetChild(1).GetChild(direction).position);
                    break;

                case State.Idle:
                    break;
            }
        }
    }

    IEnumerator PlayerController()
    {
        float relayTime = 0f;

        StartCoroutine(RandomLoopIdle());

        yield return new WaitUntil(() => !loopIdle);

        if(!isReward && !isHarvest)
            StartCoroutine(RandomLoopWalk());

        yield return new WaitUntil(() => !loopWalk);

        yield return new WaitForSeconds(relayTime);

        StartCoroutine(PlayerController());
    }

    private IEnumerator RandomLoopIdle()
    {
        float randomLoopIdleTime = Random.Range(4.9f, 5f);

        loopIdle = true;

        int rotateDirection = Random.Range(0, 4);
        direction = rotateDirection;

        if(!isReward && !isHarvest)
            ChangeState(State.Idle);


        yield return new WaitForSeconds(randomLoopIdleTime);

        if (Random.Range(0, 100) <= 70)
            StartCoroutine(RandomLoopIdle());
        else
            loopIdle = false;
    }

    private IEnumerator RandomLoopWalk()
    {
        float randomLoopWalkTime = Random.Range(4f, 5f);

        loopWalk = true;

        int rotateDirection = Random.Range(0, 4);

        direction = rotateDirection;

        ChangeState(State.Walk);

        yield return new WaitForSeconds(randomLoopWalkTime);

        if (Random.Range(0, 100) <= 30)
            StartCoroutine(RandomLoopWalk());
        else
            loopWalk = false;
    }

    private void ChangeState(State state)
    {
        switch (state)
        {
            case State.Walk:
                break;
            case State.Idle:
                navMeshAgent.ResetPath();

                StartCoroutine(SetIdleSpine());
                break;
        }

        nowState = state;
    }

    private IEnumerator SetIdleSpine()
    {
        int num = Random.Range(0, 100);

        if(num >= 0 && num < 100 - (idleEvent1 + idleEvent2))
        {
            SetAnimation(IDLE[0]);
        }
        else if(num >= 100 - (idleEvent1 + idleEvent2) && num < 100 - idleEvent2)
        {
            SetAnimation(IDLE[1], false);

            AddAnimation(IDLE[0]);
        }
        else if(num >= 100 - idleEvent2)
        {
            SetAnimation(IDLE[2], false);

            AddAnimation(IDLE[0]);
        }

        yield return null;
    }

    private void SetRotateSpine()
    {
        if(direction % 2 == 0)
        {
            SetAnimation(WALK_DOWN);
        }
        else
        {
            SetAnimation(WALK_UP);
        }
    }

    public void RewardedAD(int num)
    {
        FarmUI.GetInstance().ButtonClick();

        ADManager.GetInstance().ShowRewardedAD(
            () =>
            {
                transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                transform.GetChild(0).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = "+" + giftReward;

                SetAnimation(REWARD, false);
                AddAnimation(IDLE[0], true, 0);

                switch (num)
                {
                    case 0: //엄마
                        BackendServerManager.GetInstance().myInfo.diamond += giftReward;
                        break;

                    case 1: //아빠
                        BackendServerManager.GetInstance().myInfo.gold += giftReward;
                        break;

                    case int n when(2 <= n && n <= 3): //아들, 딸
                        int ran = Random.Range(0, 3);
                        int ran2 = Random.Range(1, 101);
                        transform.GetChild(0).GetChild(1).GetComponentInChildren<Image>().sprite = FarmUI.GetInstance().inventoryHarvestImage[ran];
                        transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(FarmUI.GetInstance().inventoryHarvestImage[ran].textureRect.width, FarmUI.GetInstance().inventoryHarvestImage[ran].textureRect.height);
                        transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(0.35f, 0.35f, 1);

                        if(ran2 > 0 && ran2 <= 50)
                        {
                            BackendServerManager.GetInstance().myInfo.harvest[ran] += 1;
                            transform.GetChild(0).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = "+" + 1;
                        }
                        else if(ran2 > 50 && ran2 <= 80)
                        {
                            BackendServerManager.GetInstance().myInfo.harvest[ran] += 2;
                            transform.GetChild(0).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = "+" + 2;
                        }
                        else
                        {
                            BackendServerManager.GetInstance().myInfo.harvest[ran] += 3;
                            transform.GetChild(0).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = "+" + 3;
                        }
                        break;
                }

                BackendServerManager.GetInstance().SaveMyInfo(true);
            },
                () =>
                {
                    FarmUI.GetInstance().SetErrorObject(PlayerPrefs.GetString("Langauge") == "ko" ? NETWORK_FAIL : NETWORK_FAIL_EN, () =>
                    {

                    });
                });
    }

    private void SetAnimation(string animation, bool loop = true)
    {
        if(GetComponent<SkeletonAnimation>().AnimationName != animation)
            GetComponent<SkeletonAnimation>().state.SetAnimation(0, animation, loop);
    }

    private void AddAnimation(string animation, bool loop = true, int delay = 0)
    {
        GetComponent<SkeletonAnimation>().state.AddAnimation(0, animation, loop, delay);
    }

    private string GetAnimation()
    {
        return GetComponent<SkeletonAnimation>().AnimationName;
    }

    public void TEST()
    {
        Debug.LogError("?");
        isHarvest = false;
    }
}
