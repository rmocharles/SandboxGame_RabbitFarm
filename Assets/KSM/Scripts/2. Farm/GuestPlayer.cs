using UnityEngine;
using System.Collections;
using Spine;
using Spine.Unity;
using UnityEngine.UI;
using UnityEngine.AI;

public class GuestPlayer : MonoBehaviour
{
    /*
     * 손님 움직임 조건
     * 
     * 1. 길을 돌아다니다가 마트 안에 인원이 3명 미만일 경우 일정 확률로 마트 진입
     * 2. 마트에 들어선 후 원하는 물품 띄우고 두리번 거리며 찾기
     * 3-1. (원하는 물품이 있을 경우) 해당 테이블로 이동 (도착했을 때 물건이 없을 경우 돌아가기)
     * 3-2. (원하는 물품이 없을 경우) 돌아가기
     * 4. 일정량의 물품을 들고 계산대로 이동
     * 5. 계산대에 도착하는 순간 일정량의 시간 카운트다운 10-15초 (대기)
     * 6-1. (결제를 했을 경우) 구매
     * 6-2. (시간이 초과할 경우) 나가기
     */

    public Animator guestAnimator;

    private NavMeshAgent navMeshAgent;

    public GameObject wantImage;

    private int harvestRandom = 0, haveHarvest, haveCount;

    private const string WALK_UP_RIGHT = "walk_back_r";
    private const string WALK_UP_LEFT = "walk_back_l";
    private const string WALK_DOWN_RIGHT = "walk_front_r";
    private const string WALK_DOWN_LEFT = "walk_front_l";

    private const string IDLE_UP_RIGHT = "idle_back_r";
    private const string IDLE_UP_LEFT = "idle_back_l";
    private const string IDLE_DOWN_RIGHT = "idle_front_r";
    private const string IDLE_DOWN_LEFT = "idle_front_l";

    private const string ANGRY_DOWN_LEFT = "angry_front_l";
    private const string ANGRY_DOWN_RIGHT = "angry_front_r";

    private const string WALK_ANGRY_DOWN_LEFT = "walk_angry_front_l";
    private const string WALK_ANGRY_DOWN_RIGHT = "walk_angry_front_r";

    private const string SIGH_DOWN_LEFT = "sigh_front_l";
    private const string SIGH_DOWN_RIGHT = "sigh_front_r";

    private const string WALK_SIGH_DOWN_LEFT = "walk_sigh_front_l";
    private const string WALK_SIGH_DOWN_RIGHT = "walk_sigh_front_r";

    private const string PICKUP_DOWN_LEFT = "pickup_front_l";
    private const string PICKUP_DOWN_RIGHT = "pickup_front_r";

    private enum State { Road, Enter, Search, Move, Choose, Wait, Purchase, Exit, SignExit, PurchaseExit };
    private State nowState = State.Road;
    private bool isState = false;

    private bool isAngry = false, isSad = false;

    int wantTable = 0, order = 1;
    float countTime = 0;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;

        navMeshAgent.Warp(FarmUI.GetInstance().startPosition.position);
        harvestRandom = Random.Range(0, 9);

        //현재 마트안에 인원이 3명 미만일 경우 일정 확률로 입구 들어가기
        if (FarmUI.GetInstance().guestCount < 5)
        {
            int enterMartRandom = Random.Range(0, 100);
            if (enterMartRandom <= BackendServerManager.GetInstance().guestSheet[0].percentEnterMart)
            {
                ChangeState(State.Enter);
            }
            else
                ChangeState(State.Road);
        }
        else
            ChangeState(State.Road);
    }

    private void ChangeState(State state)
    {
        switch (state)
        {
            //출구로 나가기
            case State.Road:
                StartCoroutine(Exit());
                break;

            //마트로 들어가기
            case State.Enter:
                navMeshAgent.areaMask = NavMesh.AllAreas;
                navMeshAgent.SetDestination(FarmUI.GetInstance().stopPosition.position);
                break;

            //물품 찾기
            case State.Search:
                StartCoroutine(SearchHarvest());
                break;

            //물품으로 이동
            case State.Move:
                navMeshAgent.SetDestination(FarmUI.GetInstance().tablePosition[wantTable].position);
                break;

            //물품 고르기
            case State.Choose:
                StartCoroutine(ChooseHarvest());
                break;

            //물품 구매하기
            case State.Purchase:
                StartCoroutine(WaitPurchaseHarvest());
                break;

            case State.Exit:
                StartCoroutine(ExitMart());
                break;

            case State.PurchaseExit:
                if (!guestAnimator.GetBool("isSuccess"))
                    guestAnimator.SetBool("isSuccess", true);
                ChangeState(State.Exit);
                break;
        }

        isState = true;
        nowState = state;
    }

    void Update()
    {
        #region 움직이는 애니메이션

        if(nowState != State.Search && nowState != State.Choose && nowState != State.Purchase)
        {
            if (!isAngry && !isSad)
            {
                if (navMeshAgent.velocity.x > 0 && navMeshAgent.velocity.y > 0)
                    if (GetComponent<SkeletonAnimation>().AnimationName != WALK_UP_RIGHT)
                        GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, WALK_UP_RIGHT, true);

                if (navMeshAgent.velocity.x < 0 && navMeshAgent.velocity.y > 0)
                    if (GetComponent<SkeletonAnimation>().AnimationName != WALK_UP_LEFT)
                        GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, WALK_UP_LEFT, true);

                if (navMeshAgent.velocity.x > 0 && navMeshAgent.velocity.y < 0)
                    if (GetComponent<SkeletonAnimation>().AnimationName != WALK_DOWN_RIGHT)
                        GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, WALK_DOWN_RIGHT, true);

                if (navMeshAgent.velocity.x < 0 && navMeshAgent.velocity.y < 0)
                    if (GetComponent<SkeletonAnimation>().AnimationName != WALK_DOWN_LEFT)
                        GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, WALK_DOWN_LEFT, true);
            }

            if(isSad)
            {
                if (navMeshAgent.velocity.x > 0)
                {
                    if (GetComponent<SkeletonAnimation>().AnimationName != WALK_SIGH_DOWN_RIGHT)
                        GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, WALK_SIGH_DOWN_RIGHT, true);
                }
                else if (navMeshAgent.velocity.x < 0)
                {
                    if (GetComponent<SkeletonAnimation>().AnimationName != WALK_SIGH_DOWN_LEFT)
                        GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, WALK_SIGH_DOWN_LEFT, true);
                }
            }

            if (isAngry)
            {
                if (navMeshAgent.velocity.x > 0)
                {
                    if (GetComponent<SkeletonAnimation>().AnimationName != WALK_ANGRY_DOWN_RIGHT)
                        GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, WALK_ANGRY_DOWN_RIGHT, true);
                }
                else if (navMeshAgent.velocity.x < 0)
                {
                    if (GetComponent<SkeletonAnimation>().AnimationName != WALK_ANGRY_DOWN_LEFT)
                        GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, WALK_ANGRY_DOWN_LEFT, true);
                }
            }
        }
        #endregion

        #region 멈추는 애니메이션
        if (!navMeshAgent.hasPath && isState)
        {
            switch (nowState)
            {
                case State.Enter:
                    if (Vector3.Distance(FarmUI.GetInstance().stopPosition.position, transform.position) < 0.5f)
                    {
                        FarmUI.GetInstance().guestCount++;
                        ChangeState(State.Search);
                    }
                    break;

                case State.Move:
                    if(Vector3.Distance(FarmUI.GetInstance().tablePosition[wantTable].position, transform.position) < 0.5f)
                    {
                        ChangeState(State.Choose);
                    }
                    break;

                case State.Wait:
                    if(Vector3.Distance(FarmUI.GetInstance().countPosition[order - 1].position, transform.position) < 0.5f)
                    {
                        ChangeState(State.Purchase);
                    }
                    break;
            }
        }
        #endregion


        if(nowState == State.Wait || nowState == State.Purchase)
        {
            if (countTime > 0)
            {
                countTime -= Time.deltaTime;

                if(countTime < 3)
                {
                    if(order == 1)
                    {
                        if (!guestAnimator.GetBool("isPurchaseAngry"))
                        {
                            guestAnimator.SetBool("isPurchaseAngry", true);
                        }
                    }
                    else
                    {
                        if (!guestAnimator.GetBool("isAngry"))
                            guestAnimator.SetBool("isAngry", true);
                    }
                    
                    //wantImage.GetComponent<Image>().sprite = FarmUI.GetInstance().wantImage[9];
                }
            }

            else
            {
                StartCoroutine(AngryGuest());
                //countTime = 0;
                
            }
        }
        

        if (Vector2.Distance(navMeshAgent.destination, transform.position) < 0.5f)
        {
            navMeshAgent.ResetPath();
        }
    }

    IEnumerator AngryGuest()
    {
        wantImage.SetActive(false);
        transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
        if (GetComponent<SkeletonAnimation>().AnimationName != ANGRY_DOWN_RIGHT)
            GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, ANGRY_DOWN_RIGHT, false);
        yield return new WaitForSeconds(1.5f);
        FarmUI.GetInstance().guestPlayers.Remove(GetComponent<GuestPlayer>());

        switch (harvestRandom)
        {
            case 0: //당근
                BackendServerManager.GetInstance().myInfo.carrot++;
                break;

            case 1: //감자
                BackendServerManager.GetInstance().myInfo.potato++;
                break;

            case 2: //토마토
                BackendServerManager.GetInstance().myInfo.tomato++;
                break;

            case 3: //오이
                BackendServerManager.GetInstance().myInfo.cucumber++;
                break;

            case 4: //복숭아
                BackendServerManager.GetInstance().myInfo.peach++;
                break;

            case 5: //사과
                BackendServerManager.GetInstance().myInfo.apple++;
                break;

            case 6: //호박
                BackendServerManager.GetInstance().myInfo.pumpkin++;
                break;

            case 7: //배
                BackendServerManager.GetInstance().myInfo.pear++;
                break;

            case 8: //체리
                BackendServerManager.GetInstance().myInfo.cherry++;
                break;
        }

        for (int i = FarmUI.GetInstance().guestPlayers.Count - 1; i >= 0; i--)
        {
            StartCoroutine(FarmUI.GetInstance().guestPlayers[i].ReloadList());
        }

        isAngry = true;
        ChangeState(State.Exit);
    }

    IEnumerator SearchHarvest()
    {
        wantImage.SetActive(true);
        wantImage.GetComponent<Image>().sprite = FarmUI.GetInstance().wantImage[harvestRandom];

        if (Random.Range(0, 2) == 0)
        {
            GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, IDLE_UP_LEFT, true);

            yield return new WaitForSeconds(1f);

            GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, IDLE_UP_RIGHT, true);
        }
        else
        {
            GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, IDLE_UP_LEFT, true);

            yield return new WaitForSeconds(0.5f);

            GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, IDLE_UP_RIGHT, true);

            yield return new WaitForSeconds(0.5f);

            GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, IDLE_UP_LEFT, true);

            yield return new WaitForSeconds(0.5f);

            GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, IDLE_UP_RIGHT, true);
        }

        yield return new WaitForSeconds(1f);

        bool isOk = false;

        if(FarmUI.GetInstance().guestCount >= 5)
        {
            isOk = false;

            if (!guestAnimator.GetBool("isProblem"))
                guestAnimator.SetBool("isProblem", true);

            yield return new WaitForSeconds(1f);

            if(FarmUI.GetInstance().guestCount >= 5)
            {
                isSad = true;

                if (!guestAnimator.GetBool("isExit"))
                    guestAnimator.SetBool("isExit", true);

                ChangeState(State.Exit);
            }

            else
            {
                if (guestAnimator.GetBool("isProblem"))
                    guestAnimator.SetBool("isProblem", false);

                isOk = true;
            }
        }
        else
        {
            isOk = true;
        }

        if (isOk)
        {
            bool isWant = false;

            for (int i = 0; i < 9; i++)
            {
                //원하는 물품이 있을 경우
                if (BackendServerManager.GetInstance().TableType[i] == harvestRandom + 10)
                {
                    wantTable = i;
                    isWant = true;
                }
            }

            if (!isWant)
            {
                GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, SIGH_DOWN_RIGHT, false);
                transform.GetChild(0).GetChild(0).gameObject.SetActive(false);

                yield return new WaitForSeconds(1.5f);
                isSad = true;
                ChangeState(State.Exit);
            }
            else
                ChangeState(State.Move);
        }
    }

    IEnumerator ChooseHarvest()
    {
        if (Vector2.Distance(FarmUI.GetInstance().tablePosition[wantTable].position, transform.position) < 0.5f)
        {
            switch (wantTable)
            {
                case int n when (0 <= n && n <= 2): //1, 2, 3번 테이블
                    if (GetComponent<SkeletonAnimation>().AnimationName != PICKUP_DOWN_LEFT)
                        GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, PICKUP_DOWN_LEFT, true);
                    break;

                case 3: //4번 테이블
                    if (GetComponent<SkeletonAnimation>().AnimationName != PICKUP_DOWN_RIGHT)
                        GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, PICKUP_DOWN_RIGHT, true);
                    break;

                case 4: //5번 테이블
                    if (GetComponent<SkeletonAnimation>().AnimationName != PICKUP_DOWN_LEFT)
                        GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, PICKUP_DOWN_LEFT, true);
                    break;

                case 5: //6번 테이블
                    if (GetComponent<SkeletonAnimation>().AnimationName != PICKUP_DOWN_RIGHT)
                        GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, PICKUP_DOWN_RIGHT, true);
                    break;

                case int m when (6 <= m && m <= 8): //7, 8, 9번 테이블
                    if (GetComponent<SkeletonAnimation>().AnimationName != PICKUP_DOWN_RIGHT)
                        GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, PICKUP_DOWN_RIGHT, true);
                    break;
            }

            yield return new WaitForSeconds(0.5f);

            if (BackendServerManager.GetInstance().TableCount[wantTable] > 0)
            {
                int randomCount = Random.Range(1, BackendServerManager.GetInstance().TableCount[wantTable] + 1);

                randomCount = 1;

                BackendServerManager.GetInstance().TableCount[wantTable] -= randomCount;

                haveHarvest = BackendServerManager.GetInstance().TableType[wantTable];
                haveCount = BackendServerManager.GetInstance().TableCount[wantTable];

                FarmUI.GetInstance().InitializeTable();

                FarmUI.GetInstance().guestPlayers.Add(GetComponent<GuestPlayer>());

                for(int i = 0; i < FarmUI.GetInstance().guestPlayers.Count; i++)
                {
                    if(FarmUI.GetInstance().guestPlayers[i] == GetComponent<GuestPlayer>())
                    {
                        order = i + 1;
                        countTime = Random.Range(10, 16);
                        navMeshAgent.SetDestination(FarmUI.GetInstance().countPosition[i].position);
                    }
                }

                wantImage.SetActive(false);

                ChangeState(State.Wait);
            }
            else
            {
                isAngry = true;

                Debug.LogError("왔는데 물품이 없자나...");

                if (!guestAnimator.GetBool("isProblem"))
                    guestAnimator.SetBool("isProblem", true);

                yield return new WaitForSeconds(1f);

                ChangeState(State.Exit);
            }

        }
    }

    IEnumerator WaitPurchaseHarvest()
    {
        navMeshAgent.avoidancePriority = 50 - order;
        navMeshAgent.ResetPath();

        if (countTime == 0)
            countTime = Random.Range(10, 15);
        switch (order)
        {
            //첫 번째 순서
            case 1:
                if (!guestAnimator.GetBool("isPurchase"))
                    guestAnimator.SetBool("isPurchase", true);
                if (GetComponent<SkeletonAnimation>().AnimationName != IDLE_UP_LEFT)
                    GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, IDLE_UP_LEFT, true);
                break;


            //두 번째 순서
            case 2:
                if (GetComponent<SkeletonAnimation>().AnimationName != IDLE_DOWN_LEFT)
                    GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, IDLE_DOWN_LEFT, true);
                break;

            //세 번째 순서
            case 3:
                if (GetComponent<SkeletonAnimation>().AnimationName != IDLE_DOWN_RIGHT)
                    GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, IDLE_DOWN_RIGHT, true);
                break;

            //네 번째 순서
            case 4:
                if (GetComponent<SkeletonAnimation>().AnimationName != IDLE_DOWN_RIGHT)
                    GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, IDLE_DOWN_RIGHT, true);
                break;

            //다섯 번째 순서
            case 5:
                if (GetComponent<SkeletonAnimation>().AnimationName != IDLE_DOWN_RIGHT)
                    GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, IDLE_DOWN_RIGHT, true);
                break;
        }

        if(order == 1)
        {
            transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
            {
                StartCoroutine(PurchaseHarvest());
            });

            transform.GetChild(0).GetChild(1).GetChild(3).GetComponent<Button>().onClick.AddListener(() =>
            {
                StartCoroutine(PurchaseHarvest());
            });
        }
        else
        {
            
        }

        yield return null;
    }

    public IEnumerator ReloadList()
    {
        for (int i = 0; i < FarmUI.GetInstance().guestPlayers.Count; i++)
            if (FarmUI.GetInstance().guestPlayers[i] == GetComponent<GuestPlayer>())
                order = i + 1;

        navMeshAgent.SetDestination(FarmUI.GetInstance().countPosition[order - 1].position);
        ChangeState(State.Wait);

        yield return null;
    }


    IEnumerator ExitMart()
    {
        wantImage.SetActive(false);

        if(!guestAnimator.GetBool("isSuccess"))
            if (!guestAnimator.GetBool("isExit"))
                guestAnimator.SetBool("isExit", true);

        FarmUI.GetInstance().guestCount--;
        navMeshAgent.SetDestination(FarmUI.GetInstance().exitPosition[0].position);

        yield return new WaitForSeconds(3);

        wantImage.SetActive(false);

        yield return new WaitForSeconds(10f);

        Destroy(this.gameObject);
    }

    IEnumerator Exit()
    {
        Vector3 exitVector;

        int exitRandom = Random.Range(0, 2);

        if(exitRandom == 0)
        {
            exitVector = FarmUI.GetInstance().exitPosition[1].position;
        }
        else
        {
            exitVector = FarmUI.GetInstance().exitPosition[2].position;
        }

        navMeshAgent.SetDestination(exitVector);

        yield return new WaitForSeconds(15f);

        Destroy(this.gameObject);
    }

    public IEnumerator PurchaseHarvest()
    {
        FarmUI.GetInstance().guestPlayers.Remove(GetComponent<GuestPlayer>());
        for (int i = FarmUI.GetInstance().guestPlayers.Count - 1; i >= 0; i--)
        {
            StartCoroutine(FarmUI.GetInstance().guestPlayers[i].ReloadList());
        }

        BackendServerManager.GetInstance().myInfo.gold += BackendServerManager.GetInstance().martSheet[haveHarvest - 10].cost;

        BackendServerManager.GetInstance().SaveMyInfo();

        ChangeState(State.PurchaseExit);

        FarmUI.GetInstance().partTimerObject.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "count_front_r", false);

        FarmUI.GetInstance().partTimerObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);

        FarmUI.GetInstance().partTimerObject.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>().text = "+" + BackendServerManager.GetInstance().martSheet[haveHarvest - 10].cost.ToString();
        FarmUI.GetInstance().partTimerObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);

        if (FarmUI.GetInstance().partTimerObject.GetComponent<SkeletonAnimation>().AnimationName == "count_front_r")
        {
            yield return new WaitForSeconds(1.4f);

            FarmUI.GetInstance().partTimerObject.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "idle1_front_r", true);
        }
    }


}
