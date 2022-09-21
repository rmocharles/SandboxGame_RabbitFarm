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

public class PetController : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;

    private const string WALK_DOWN = "cat_walk_front_r";
    private const string WALK_UP = "cat_walk_back_r";

    private const string IDLE_DOWN = "cat_idle_front_r";
    private const string IDLE_UP = "cat_idle_back_r";

    private string[] TOUCH = { "cat_touch1_front_r", "cat_touch2_ front_r" };

    private enum State { Walk, Idle, Reward};
    private State nowState = State.Idle;

    private int direction = 0;

    private bool loopIdle, loopWalk;

    private bool isTouch = false;
    private float isTouchTime = 0;

    private Vector3 touchPosition;


    //----------------------------------------------------
    [Header("< Character Info >")]
    public float characterSpeed = 7f;
    public int idleEvent1 = 20, idleEvent2 = 20;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;

        navMeshAgent.Warp(transform.position);

        GetComponent<SkeletonAnimation>().state.Start += delegate (TrackEntry trackEntry){

            for(int i = 0; i < 2; i++)
            {
                if (trackEntry.Animation.Name == TOUCH[i])
                {
                    isTouch = true;
                    isTouchTime = i == 0 ? 3.7f : 1.5f;
                }

            }
        };

        StartCoroutine(PetAnimator());
    }

    void Update()
    {
        navMeshAgent.speed = characterSpeed;

        if (direction - 2 < 0)
            GetComponent<SkeletonAnimation>().skeleton.ScaleX = -0.9f;
        else
            GetComponent<SkeletonAnimation>().skeleton.ScaleX = 0.9f;

        switch (nowState)
        {
            case State.Walk:
                SetRotateSpine();

                navMeshAgent.SetDestination(transform.GetChild(0).GetChild(direction).position);
                break;

            case State.Idle:
                break;
        }


        //touch

        if (isTouch)
        {
            if (isTouchTime > 0)
                isTouchTime -= Time.deltaTime;
            else
                isTouch = false;
        }
        else
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {

                Vector2 pos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

                if (GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(new Vector2(pos.x, pos.y)))
                {
                    isTouch = true;
                    SetAnimation(TOUCH[Random.Range(0, 2)], false);
                }
            }
        }
        
    }

    IEnumerator PetAnimator()
    {
        float relayTime = 0f;

        StartCoroutine(RandomLoopIdle());

        yield return new WaitUntil(() => !loopIdle);

        if(!isTouch)
            StartCoroutine(RandomLoopWalk());

        yield return new WaitUntil(() => !loopWalk);

        yield return new WaitForSeconds(relayTime);

        StartCoroutine(PetAnimator());
    }

    private IEnumerator RandomLoopIdle()
    {
        float randomLoopIdleTime = Random.Range(3.9f, 4f);

        loopIdle = true;

        int rotateDirection = Random.Range(0, 4);
        direction = rotateDirection;

        if(!isTouch)
            ChangeState(State.Idle);

        yield return new WaitForSeconds(randomLoopIdleTime);

        if (Random.Range(0, 100) <= 70)
            StartCoroutine(RandomLoopIdle());
        else
            loopIdle = false;
    }

    private IEnumerator RandomLoopWalk()
    {
        float randomLoopWalkTime = Random.Range(3.9f, 4f);

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

        SetAnimation(IDLE_DOWN);

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
}
