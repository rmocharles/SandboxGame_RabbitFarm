using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;
using UnityEngine.AI;
using DG.Tweening;
using UnityEngine.UI;

public class GuestAI : MonoBehaviour
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
    private NavMeshAgent navMeshAgent;

    private const string IDLE_DOWN = "idle_front_r";
    private const string IDLE_UP = "idle_back_r";

    private const string WALK_DOWN = "walk_front_r";
    private const string WALK_UP = "walk_back_r";

    private const string SIGH_IDLE = "sigh_front_r";
    private const string SIGH_WALK = "walk_sigh_front_r";

    private const string ANGRY_IDLE = "angry_front_r";
    private const string ANGRY_WALK = "walk_angry_front_r";

    private const string PICKUP = "pickup_front_r";

    public bool isAngry = false;
    public bool isSad = false;


    private int wantTableNumber = 0;
    private bool isWait = false;
    private float waitCoolTime = 0;
    private enum State
    {
        Road,
        Enter,
        Search,
        GoTo,
        Select,
        Wait,
        Purchase,

        PurchaseExit,
        AngryExit,
        SighExit
    }
    private State nowState = State.Road;

    [Header("< Guest Info >")]
    public int harvestInt = 0;
    public int myOrder = 0;

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;

    }
    void Start()
    {
        navMeshAgent.Warp(GuestManager.GetInstance().startPoint.position);

        harvestInt = Random.Range(0, 9);

        //현재 마트안에 인원이 3명 미만일 경우 입장 가느
        if (GuestManager.GetInstance().nowGuestCount < GuestManager.GetInstance().maxGuestCount)
        {
            int enterMartPercent = Random.Range(0, 100);
            if (enterMartPercent < BackendServerManager.GetInstance().guestSheet[0].percentEnterMart)
                ChangeState(State.Enter);
            else
                ChangeState(State.Road);
        }
        else
            ChangeState(State.Road);
    }

    void Update()
    {
        #region 애니메이션 움직임
        if(nowState != State.Search && nowState != State.Select && nowState != State.Purchase)
        {
            if(!isAngry && !isSad)
            {
                if (navMeshAgent.velocity.x > 0 && navMeshAgent.velocity.y > 0)
                {
                    GetComponent<SkeletonAnimation>().skeleton.ScaleX = 1;
                    if (GetComponent<SkeletonAnimation>().AnimationName != WALK_UP)
                        GetComponent<SkeletonAnimation>().state.SetAnimation(0, WALK_UP, true);
                }
                if (navMeshAgent.velocity.x > 0 && navMeshAgent.velocity.y < 0)
                {
                    GetComponent<SkeletonAnimation>().skeleton.ScaleX = 1;
                    if (GetComponent<SkeletonAnimation>().AnimationName != WALK_DOWN)
                        GetComponent<SkeletonAnimation>().state.SetAnimation(0, WALK_DOWN, true);
                }

                if (navMeshAgent.velocity.x < 0 && navMeshAgent.velocity.y > 0)
                {
                    GetComponent<SkeletonAnimation>().skeleton.ScaleX = -1;
                    if (GetComponent<SkeletonAnimation>().AnimationName != WALK_UP)
                        GetComponent<SkeletonAnimation>().state.SetAnimation(0, WALK_UP, true);
                }
                if (navMeshAgent.velocity.x < 0 && navMeshAgent.velocity.y < 0)
                {
                    GetComponent<SkeletonAnimation>().skeleton.ScaleX = -1;
                    if (GetComponent<SkeletonAnimation>().AnimationName != WALK_DOWN)
                        GetComponent<SkeletonAnimation>().state.SetAnimation(0, WALK_DOWN, true);
                }
            }

            if (isSad)
            {
                if (navMeshAgent.velocity.x > 0)
                {
                    GetComponent<SkeletonAnimation>().skeleton.ScaleX = 1f;
                    if (GetComponent<SkeletonAnimation>().AnimationName != SIGH_WALK)
                        GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, SIGH_WALK, true);
                }
                else if (navMeshAgent.velocity.x < 0)
                {
                    GetComponent<SkeletonAnimation>().skeleton.ScaleX = -1f;
                    if (GetComponent<SkeletonAnimation>().AnimationName != SIGH_WALK)
                        GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, SIGH_WALK, true);
                }
            }

            if (isAngry)
            {
                if(navMeshAgent.velocity.x > 0)
                {
                    GetComponent<SkeletonAnimation>().skeleton.ScaleX = 1f;
                    if (GetComponent<SkeletonAnimation>().AnimationName != ANGRY_WALK)
                        GetComponent<SkeletonAnimation>().state.SetAnimation(0, ANGRY_WALK, true);
                }
            }
        }
        #endregion

        #region 도착지점 도달
        if (!navMeshAgent.hasPath)
        {
            switch (nowState)
            {
                case State.Enter:
                    if(isReached(GuestManager.GetInstance().enterPoint.position))
                    {
                        GuestManager.GetInstance().nowGuestCount++;
                        ChangeState(State.Search);
                    }
                    break;

                case State.GoTo:
                    if (isReached(GuestManager.GetInstance().tablePoint[wantTableNumber].position))
                    {
                        ChangeState(State.Select);
                    }
                    break;

                case State.Wait:
                    if (isReached(GuestManager.GetInstance().counterPoint[myOrder].position))
                    {
                        ChangeState(State.Purchase);
                    }
                    break;
            }
        }
        #endregion

        if(nowState == State.Wait || nowState == State.Purchase)
        {
            if (!isWait) return;

            if (waitCoolTime > 0)
            {
                waitCoolTime -= Time.deltaTime;

                if (waitCoolTime < 3)
                {
                    if (myOrder == 0)
                        GetComponent<SkeletonAnimation>().skeleton.ScaleX = -1;
                    else
                        GetComponent<SkeletonAnimation>().skeleton.ScaleX = 1;

                    if (GetComponent<SkeletonAnimation>().AnimationName != ANGRY_IDLE)
                        GetComponent<SkeletonAnimation>().state.SetAnimation(0, ANGRY_IDLE, true);
                }
            }
            else
                ChangeState(State.AngryExit);
        }

        if(Vector2.Distance(navMeshAgent.destination, transform.position) < 0.5f)
        {
            navMeshAgent.ResetPath();
        }
    }


    #region 상태 변화
    private void ChangeState(State state)
    {
        switch (state)
        {
            //마트 입장 X
            case State.Road:
                navMeshAgent.SetDestination(GuestManager.GetInstance().exitPoint[Random.Range(1, 3)].position);
                Destroy(this.gameObject, 15);
                break;

            //마트 입장
            case State.Enter:
                navMeshAgent.areaMask = NavMesh.AllAreas;
                navMeshAgent.SetDestination(GuestManager.GetInstance().enterPoint.position);
                break;

            //물품 찾기
            case State.Search:
                StartCoroutine(SearchHarvest());
                break;

            //물품으로 이동
            case State.GoTo:
                navMeshAgent.SetDestination(GuestManager.GetInstance().tablePoint[wantTableNumber].position);
                break;

            //물품 고르기
            case State.Select:
                StartCoroutine(ChooseHarvest());
                break;

            //물품 구매하기
            case State.Purchase:
                StartCoroutine(WaitPurchaseHarvest());
                break;

            case State.AngryExit:
                StartCoroutine(AngryExit());
                break;

            case State.PurchaseExit:
                StartCoroutine(PurchaseExit());
                break;

            case State.SighExit:
                StartCoroutine(SighExit());
                break;
        }
        nowState = state;
    }
    #endregion

    #region 물품 찾기
    private IEnumerator SearchHarvest()
    {
        //원하는 이미지 활성화
        transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        GetComponentsInChildren<Image>()[0].sprite = FarmUI.GetInstance().martWantImage[harvestInt];

        GetComponent<SkeletonAnimation>().state.SetAnimation(0, IDLE_UP, true);

        #region 두리번 거림
        if (Random.Range(0, 2) == 0)
        {
            GetComponent<SkeletonAnimation>().skeleton.ScaleX = -1;

            yield return new WaitForSeconds(1f);

            GetComponent<SkeletonAnimation>().skeleton.ScaleX = 1;
        }
        else
        {
            GetComponent<SkeletonAnimation>().skeleton.ScaleX = -1;

            yield return new WaitForSeconds(.5f);

            GetComponent<SkeletonAnimation>().skeleton.ScaleX = 1;

            yield return new WaitForSeconds(.5f);

            GetComponent<SkeletonAnimation>().skeleton.ScaleX = -1;

            yield return new WaitForSeconds(.5f);

            GetComponent<SkeletonAnimation>().skeleton.ScaleX = 1;
        }
        #endregion

        yield return new WaitForSeconds(1f);


        //손님 체크
        bool isOk = false;

        if (GuestManager.GetInstance().nowGuestCount > GuestManager.GetInstance().maxGuestCount)
        {
            isOk = false;

            ChangeMode("Problem");

            yield return new WaitForSeconds(1f);

            //기다려도 손님이 많을 경우
            if (GuestManager.GetInstance().nowGuestCount > GuestManager.GetInstance().maxGuestCount)
            {
                ChangeMode("Sigh");

                ChangeState(State.SighExit);
            }
            else
            {
                transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                GetComponentsInChildren<Image>()[0].sprite = FarmUI.GetInstance().martWantImage[harvestInt];

                isOk = true;
            }
        }
        else
            isOk = true;

        if (isOk)
        {
            bool isWant = false;

            for (int i = 0; i < 9; i++)
            {
                //원하는 물품이 있을 경우
                if (BackendServerManager.GetInstance().TableType[i] == harvestInt + 10)
                {
                    wantTableNumber = i;
                    isWant = true;
                }
            }

            if (!isWant)
            {
                ChangeState(State.SighExit);
            }
            else
                ChangeState(State.GoTo);
        }

        yield return null;
    }
    #endregion

    #region 물품 고르기
    private IEnumerator ChooseHarvest()
    {
        //테이블
        switch (wantTableNumber)
        {
            case int n when (0 <= n && n <= 2): //1, 2, 3번 테이블
                GetComponent<SkeletonAnimation>().skeleton.ScaleX = -1;
                break;

            case 3: //4번 테이블
                GetComponent<SkeletonAnimation>().skeleton.ScaleX = 1;
                break;

            case 4: //5번 테이블
                GetComponent<SkeletonAnimation>().skeleton.ScaleX = -1;
                break;

            case 5: //6번 테이블
                GetComponent<SkeletonAnimation>().skeleton.ScaleX = 1;
                break;

            case int m when (6 <= m && m <= 8): //7, 8, 9번 테이블
                GetComponent<SkeletonAnimation>().skeleton.ScaleX = 1;
                break;
        }
        if (GetComponent<SkeletonAnimation>().AnimationName != IDLE_DOWN)
            GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, IDLE_DOWN, true);

        if(BackendServerManager.GetInstance().TableCount[wantTableNumber] > 0)
        {
            if (GetComponent<SkeletonAnimation>().AnimationName != PICKUP)
                GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, PICKUP, false);

            BackendServerManager.GetInstance().TableCount[wantTableNumber] -= 1;

            FarmUI.GetInstance().InitializeTable();

            yield return new WaitForSeconds(0.8f);

            GuestManager.GetInstance().guests.Add(GetComponent<GuestAI>());

            for(int i = 0; i < GuestManager.GetInstance().guests.Count; i++)
            {
                if(GuestManager.GetInstance().guests[i] == GetComponent<GuestAI>())
                {
                    myOrder = i;
                    navMeshAgent.SetDestination(GuestManager.GetInstance().counterPoint[i].position);
                }
            }

            transform.GetChild(0).GetChild(0).gameObject.SetActive(false);

            ChangeState(State.Wait);
        }
        else
        {
            transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            transform.GetChild(0).GetChild(1).gameObject.SetActive(true);

            transform.GetChild(0).GetChild(1).GetChild(1).gameObject.SetActive(true);

            yield return new WaitForSeconds(1f);

            ChangeState(State.SighExit);
        }
    }
    #endregion

    #region 물품 구매하기
    private IEnumerator WaitPurchaseHarvest()
    {
        isWait = true;

        navMeshAgent.avoidancePriority = 50 - myOrder;
        navMeshAgent.ResetPath();

        if (waitCoolTime == 0)
            waitCoolTime = Random.Range(10f, 15f);

        switch (myOrder)
        {
            case 0:
                GetComponent<SkeletonAnimation>().skeleton.ScaleX = -1f;
                transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                transform.GetChild(0).GetChild(1).GetChild(3).gameObject.SetActive(true);
                if (GetComponent<SkeletonAnimation>().AnimationName != IDLE_DOWN)
                    GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, IDLE_DOWN, true);
                break;

            case 1:
                GetComponent<SkeletonAnimation>().skeleton.ScaleX = 1f;
                transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                transform.GetChild(0).GetChild(1).GetChild(3).gameObject.SetActive(false);
                if (GetComponent<SkeletonAnimation>().AnimationName != IDLE_DOWN)
                    GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, IDLE_DOWN, true);
                break;

            case 2:
                GetComponent<SkeletonAnimation>().skeleton.ScaleX = 1f;
                transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                transform.GetChild(0).GetChild(1).GetChild(3).gameObject.SetActive(false);
                if (GetComponent<SkeletonAnimation>().AnimationName != IDLE_DOWN)
                    GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, IDLE_DOWN, true);
                break;
        }

        if(myOrder == 0)
        {
            transform.GetChild(0).GetChild(1).GetChild(3).GetComponent<Button>().onClick.RemoveAllListeners();
            transform.GetChild(0).GetChild(1).GetChild(3).GetComponent<Button>().onClick.AddListener(() =>
            {
                FarmUI.GetInstance().ButtonClick();

                StartCoroutine(PurchaseHarvest());
            });
        }

        yield return null;
    }
    #endregion

    #region 출구로 나가기
    private IEnumerator SighExit()
    {
        ChangeMode("None");

        GetComponent<SkeletonAnimation>().state.SetAnimation(0, SIGH_IDLE, false);

        yield return new WaitForSeconds(1.3f);

        isSad = true;

        ChangeMode("Sigh");

        GuestManager.GetInstance().nowGuestCount--;
        navMeshAgent.SetDestination(GuestManager.GetInstance().exitPoint[0].position);
        Destroy(this.gameObject, 10);
    }

    private IEnumerator PurchaseExit()
    {
        ChangeMode("None");

        GuestManager.GetInstance().nowGuestCount--;
        navMeshAgent.SetDestination(GuestManager.GetInstance().exitPoint[0].position);
        Destroy(this.gameObject, 10);

        yield return null;
    }

    private IEnumerator AngryExit()
    {
        ChangeMode("None");

        GetComponent<SkeletonAnimation>().state.SetAnimation(0, ANGRY_IDLE, false);

        yield return new WaitForSeconds(2f);

        isAngry = true;

        ChangeMode("Angry");

        BackendServerManager.GetInstance().myInfo.harvest[harvestInt]++;

        GuestManager.GetInstance().nowGuestCount--;
        GuestManager.GetInstance().guests.Remove(GetComponent<GuestAI>());
        for (int i = GuestManager.GetInstance().guests.Count - 1; i >= 0; i--)
        {
            StartCoroutine(GuestManager.GetInstance().guests[i].ReloadList());
        }

        navMeshAgent.SetDestination(GuestManager.GetInstance().exitPoint[0].position);
        Destroy(this.gameObject, 10);
    }
    #endregion

    #region 물품 구매하기
    private IEnumerator PurchaseHarvest()
    {
        GuestManager.GetInstance().nowGuestCount--;
        GuestManager.GetInstance().guests.Remove(GetComponent<GuestAI>());
        for (int i = GuestManager.GetInstance().guests.Count - 1; i >= 0; i--)
        {
            StartCoroutine(GuestManager.GetInstance().guests[i].ReloadList());
        }

        BackendServerManager.GetInstance().myInfo.gold += BackendServerManager.GetInstance().martSheet[harvestInt].cost;

        BackendServerManager.GetInstance().SaveMyInfo(true);

        ChangeState(State.PurchaseExit);

        yield return null;
    }
    #endregion

    public IEnumerator ReloadList()
    {
        for (int i = 0; i < GuestManager.GetInstance().guests.Count; i++)
            if (GuestManager.GetInstance().guests[i] == GetComponent<GuestAI>())
                myOrder = i;

        navMeshAgent.SetDestination(GuestManager.GetInstance().counterPoint[myOrder].position);
        ChangeState(State.Wait);
        yield return null;
    }

    //============================================
    private bool isReached(Vector3 pos)
    {
        return Vector3.Distance(transform.position, pos) < 0.5f;
    }

    private void ChangeMode(string mode)
    {
        transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        switch (mode)
        {
            case "None":

                transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                transform.GetChild(0).GetChild(1).GetChild(0).gameObject.SetActive(false);
                transform.GetChild(0).GetChild(1).GetChild(1).gameObject.SetActive(false);
                transform.GetChild(0).GetChild(1).GetChild(2).gameObject.SetActive(false);
                transform.GetChild(0).GetChild(1).GetChild(3).gameObject.SetActive(false);

                break;
            case "Sigh":

                transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                transform.GetChild(0).GetChild(1).GetChild(0).gameObject.SetActive(true);
                transform.GetChild(0).GetChild(1).GetChild(1).gameObject.SetActive(false);
                transform.GetChild(0).GetChild(1).GetChild(2).gameObject.SetActive(false);
                transform.GetChild(0).GetChild(1).GetChild(3).gameObject.SetActive(false);

                break;

            case "Problem":

                transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                transform.GetChild(0).GetChild(1).GetChild(0).gameObject.SetActive(false);
                transform.GetChild(0).GetChild(1).GetChild(1).gameObject.SetActive(true);
                transform.GetChild(0).GetChild(1).GetChild(2).gameObject.SetActive(false);
                transform.GetChild(0).GetChild(1).GetChild(3).gameObject.SetActive(false);

                break;

            case "Angry":

                transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                transform.GetChild(0).GetChild(1).GetChild(0).gameObject.SetActive(false);
                transform.GetChild(0).GetChild(1).GetChild(1).gameObject.SetActive(false);
                transform.GetChild(0).GetChild(1).GetChild(2).gameObject.SetActive(true);
                transform.GetChild(0).GetChild(1).GetChild(3).gameObject.SetActive(false);

                break;

            case "Purchase":

                transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                transform.GetChild(0).GetChild(1).GetChild(0).gameObject.SetActive(false);
                transform.GetChild(0).GetChild(1).GetChild(1).gameObject.SetActive(false);
                transform.GetChild(0).GetChild(1).GetChild(2).gameObject.SetActive(false);
                transform.GetChild(0).GetChild(1).GetChild(3).gameObject.SetActive(true);

                break;
        }
    }
}
