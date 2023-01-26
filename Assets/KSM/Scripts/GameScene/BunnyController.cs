using System;
using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/*
 * BunnyController
 *
 * 1. 상태 변화 체크
 */
public class BunnyController : MonoBehaviour
{
    #region Animation Name

    private const string WALK_FRONT = "walk_front_r";
    private const string WALK_BACK = "walk_back_r";

    private string[] IDLE = { "idle1_front_r", "idle2_front_r", "idle3_front_r" };

    private const string REWARD = "reward_front_r";

    private const string HARVEST = "harvest_front_l";

    private const string COUNT = "count_front_r";

    #endregion
    
    public enum State
    {
        Idle, Walk, Harvest, Work, Reward
    }
    public State nowState = State.Idle;

    public int direction = 0;

    private NavMeshAgent navMeshAgent;

    private Coroutine nowCoroutine = null;

    [SerializeField] private GameObject bubbleObject;
    public GameObject textBubbleObject;

    [SerializeField] private Button touchButton;

    
    public Vector3 originPos { get; private set; }
    private bool nowHarvesting = false;

    #region Unity Methods

    void Awake()
    {
        //Navmesh 초기화
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        navMeshAgent.Warp(transform.position);

        bubbleObject.SetActive(false);
    }

    void Start()
    {
        if (nowCoroutine != null)
            StopCoroutine(nowCoroutine);

        if (nowState == State.Work) return;
        
        nowCoroutine = StartCoroutine(PlayerController(.5f));
        
        touchButton.onClick.AddListener(() =>
        {
            //StaticManager.Sound.SetSFX();
            ChangeState(State.Idle);
        });
    }

    private void Update()
    {
        //Y축 레이어 순서 정하기
        AdjustSortingLayer();
        
        //터치할 시 애니메이션
        touchButton.gameObject.SetActive(nowState == State.Idle || nowState == State.Walk);

        //말풍선 띄어놓기
        ActiveBubble(false);
        
        //방향 정하기
        if (direction % 2 == 0)
            GetComponent<SkeletonAnimation>().skeleton.ScaleX = -1f;
        else
            GetComponent<SkeletonAnimation>().skeleton.ScaleX = 1f;
        
        //상태 실시간 체크
        switch (nowState)
        {
            case State.Idle:
                
                break;
            
            case State.Walk:

                if (direction - 2 < 0)
                {
                    if (GetComponent<SkeletonAnimation>().AnimationName != WALK_BACK)
                        GetComponent<SkeletonAnimation>().state.SetAnimation(0, WALK_BACK, true);
                }
                else
                {
                    if (GetComponent<SkeletonAnimation>().AnimationName != WALK_FRONT)
                        GetComponent<SkeletonAnimation>().state.SetAnimation(0, WALK_FRONT, true);
                }
                navMeshAgent.SetDestination(transform.GetChild(0).GetChild(direction).position);
                break;
            
            case State.Harvest:
                ActiveBubble(false);
                break;
            
            case State.Work:
                direction = 3;
                navMeshAgent.Warp(GameManager.Mart.Guest.workPoint.position);
                break;
        }
    }

    #endregion

    public void SetWorkAnimation()
    {
        GetComponent<SkeletonAnimation>().state.SetAnimation(0, COUNT, false);
        GetComponent<SkeletonAnimation>().state.AddAnimation(0, IDLE[0], true, 0);
    }

    private IEnumerator PlayerController(float delay = 0f)
    {
        yield return new WaitForSeconds(delay);
        //걷거나 가만히 있기
        if (Random.Range(0, 2) == 0)
            ChangeState(State.Idle);
        else
            ChangeState(State.Walk);
    }

    private IEnumerator RandomIdle(float delay = 0f, bool isWork = false)
    {
        navMeshAgent.ResetPath();
        
        yield return new WaitForSeconds(delay);

        //방향 정하기
        direction = Random.Range(0, 4);
        int randomIdle = Random.Range(0, 3);
        
        if(randomIdle == 0)
            GetComponent<SkeletonAnimation>().state.SetAnimation(0, IDLE[randomIdle], true);
        else
        {
            GetComponent<SkeletonAnimation>().state.SetAnimation(0, IDLE[randomIdle], false);
            GetComponent<SkeletonAnimation>().state.AddAnimation(0, IDLE[0], true, 0);
        }

        yield return new WaitForSeconds(6f);

        if (isWork)
        {
            ChangeState(State.Idle);
        }
        else
        {
            //걷거나 가만히 있기
            if (Random.Range(0, 100) <= 70)
                ChangeState(State.Idle);
            else
                ChangeState(State.Walk);
        }
    }

    private IEnumerator RandomWalk(float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        float randomWalkTime = Random.Range(2, 4f);

        direction = Random.Range(0, 4);

        yield return new WaitForSeconds(randomWalkTime);
        
        //걷거나 가만히 있기
        if (Random.Range(0, 2) == 0)
            ChangeState(State.Idle);
        else
            ChangeState(State.Walk);
    }

    #region 상태 변화

    public void ChangeState(State state)
    {
        if(nowCoroutine != null)
            StopCoroutine(nowCoroutine);

        nowState = state;

        switch (nowState)
        {
            case State.Idle:
                nowCoroutine = StartCoroutine(RandomIdle());
                break;
            
            case State.Walk:
                nowCoroutine = StartCoroutine(RandomWalk());
                break;
            
            case State.Harvest:
                if (!nowHarvesting)
                {
                    //처음 실행될 경우 원래 위치 저장
                    nowHarvesting = true;
                    originPos = transform.position;
                }
                nowCoroutine = StartCoroutine(ReturnPosition());
                break;
            
            case State.Work:
                Debug.LogError("저는 알바를 대기중인 토니랍니다~");
                break;
        }
    }

    private IEnumerator ReturnPosition()
    {
        navMeshAgent.enabled = false;
        GetComponent<SkeletonAnimation>().state.SetAnimation(0, HARVEST, false);
        yield return new WaitForSeconds(1f);
        navMeshAgent.enabled = true;
        navMeshAgent.ResetPath();
        navMeshAgent.Warp(originPos);
        nowHarvesting = false;
        ChangeState(State.Idle);
    }

    public void ActiveBubble(bool isOn)
    {
        bubbleObject.SetActive(isOn);
    }

    public void AdjustSortingLayer()
    {
        GetComponent<SortingGroup>().sortingOrder = (int)(transform.position.y * -100);
    }

    public State NowState()
    {
        return nowState;
    }

    public void MovePosition(Vector3 pos)
    {
        navMeshAgent.enabled = false;
        transform.position = pos;
        navMeshAgent.enabled = true;
        navMeshAgent.Warp(transform.position);
    }
    
    #endregion
}
