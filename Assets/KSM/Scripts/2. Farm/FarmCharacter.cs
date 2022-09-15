using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.AI;

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

    //----------------------------------------------------
    [Header("< Character Info >")]
    public float characterSpeed = 5f;
    public int idleEvent1 = 20, idleEvent2 = 20;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;

        navMeshAgent.Warp(transform.position);

        StartCoroutine(PlayerController());
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

                navMeshAgent.SetDestination(transform.GetChild(1).GetChild(direction).position);
                break;

            case State.Idle:
                break;
        }
    }

    IEnumerator PlayerController()
    {
        float relayTime = 0f;

        StartCoroutine(RandomLoopIdle());

        yield return new WaitUntil(() => !loopIdle);

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
                SetRotateSpine();
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
        switch (num)
        {
            case 0: //엄마
                ADManager.GetInstance().ShowRewardedAD(() =>
                {
                    Debug.LogError("SSSSS");
                },
                () =>
                {
                    Debug.LogError("FFFF");
                },
                () =>
                {
                    Debug.LogError("LF");
                });
                break;
        }
    }

    private void SetAnimation(string animation, bool loop = true)
    {
        if(GetComponent<SkeletonAnimation>().AnimationName != animation)
            GetComponent<SkeletonAnimation>().state.SetAnimation(0, animation, loop);
    }

    private void AddAnimation(string animation, bool loop = true, int delay = 0) => GetComponent<SkeletonAnimation>().state.AddAnimation(0, animation, loop, delay);

    private string GetAnimation()
    {
        return GetComponent<SkeletonAnimation>().AnimationName;
    }
}
