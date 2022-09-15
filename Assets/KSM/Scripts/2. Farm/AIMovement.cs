using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
    [Header("< AI Info >")]
    public float movementSpeed = 5f;
    public float limitX = 16.5f;
    public float limitY = 12f;

    private bool isWandering = false;

    [HideInInspector]
    public bool isHarvest = false;

    private bool isWalking = false;

    private int nowRotatedState = 0;    //0 : 북서, 1 : 북동, 2 : 남서, 3 : 남동

    private const string WALK_BACK_RIGHT = "walk_back_r";
    private const string WALK_BACK_LEFT = "walk_back_l";
    private const string WALK_FRONT_RIGHT = "walk_front_r";
    private const string WALK_FRONT_LEFT = "walk_front_l";

    private const string IDLE1_BACK_RIGHT = "idle1_back_r";
    private const string IDLE1_BACK_LEFT = "idle1_back_l";
    private const string IDLE1_FRONT_RIGHT = "idle1_front_r";
    private const string IDLE1_FRONT_LEFT = "idle1_front_l";

    private const string IDLE2_FRONT_RIGHT = "idle2_front_r";
    private const string IDLE2_FRONT_LEFT = "idle2_front_l";

    private const string IDLE3_FRONT_RIGHT = "idle3_front_r";
    private const string IDLE3_FRONT_LEFT = "idle3_front_l";

    private const string REWARD_FRONT_LEFT = "reward_front_l";
    private const string REWARD_FRONT_RIGHT = "reward_front_r";

    void Start()
    {
        var agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        /* 맵 밖 나가는거 방지용 */
        LimitPosition(limitX, limitY);

        /* AI 움직임 정보 */
        InitializeAI();
    }

    IEnumerator Wander()
    {
        int rotationTime = Random.Range(0, 1);
        int rotateWait = 0;//Random.Range(1, 3);
        int rotateDirection = Random.Range(0, 4);
        int walkWait = Random.Range(10, 20);
        int walkTime = Random.Range(1, 3);

        isWandering = true;

        nowRotatedState = rotateDirection;

        if(!isHarvest)
            StartCoroutine(SetIdleSpine());

        yield return new WaitForSeconds(walkWait);

        if (!isHarvest)
            SetRotateSpine();

        isWalking = true;

        yield return new WaitForSeconds(walkTime);

        isWalking = false;
        StartCoroutine(SetIdleSpine());

        yield return new WaitForSeconds(rotateWait);

        nowRotatedState = rotateDirection;

        yield return new WaitForSeconds(rotationTime);

        isWandering = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (nowRotatedState)
        {
            case 0:
                nowRotatedState = 3;
                break;
            case 1:
                nowRotatedState = 2;
                break;
            case 2:
                nowRotatedState = 1;
                break;
            case 3:
                nowRotatedState = 0;
                break;
        }
        SetRotateSpine();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Farm_Background")
        {
            nowRotatedState = Random.Range(nowRotatedState + 1, nowRotatedState + 4);
            SetRotateSpine();
        }
    }

    public IEnumerator SetIdleSpine()
    {
        if (nowRotatedState == 0 || nowRotatedState == 2)
        {
            switch (Random.Range(0, 3))
            {
                case 0:
                    GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, IDLE1_FRONT_LEFT, true);
                    break;

                case 1:
                    GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, IDLE2_FRONT_LEFT, false);

                    yield return new WaitForSeconds(2f);

                    GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, IDLE1_FRONT_LEFT, true);
                    break;

                case 2:
                    GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, IDLE2_FRONT_LEFT, false);

                    yield return new WaitForSeconds(2f);

                    GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, IDLE1_FRONT_LEFT, true);
                    break;
            }

        }
        else
        {
            switch (Random.Range(0, 3))
            {
                case 0:
                    GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, IDLE1_FRONT_RIGHT, true);
                    break;

                case 1:
                    GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, IDLE2_FRONT_RIGHT, false);

                    yield return new WaitForSeconds(2f);

                    GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, IDLE1_FRONT_RIGHT, true);
                    break;

                case 2:
                    GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, IDLE3_FRONT_RIGHT, false);

                    yield return new WaitForSeconds(2f);

                    GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, IDLE1_FRONT_RIGHT, true);
                    break;
            }
        }
    }
    public void SetRotateSpine()
    {
        switch (nowRotatedState)
        {
            case 0: //NorthWest
                GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, WALK_BACK_LEFT, true);
                break;

            case 1: //NorthEast
                GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, WALK_BACK_RIGHT, true);
                break;

            case 2: //SouthWest
                GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, WALK_FRONT_LEFT, true);
                break;

            case 3: //SouthEast
                GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, WALK_FRONT_RIGHT, true);
                break;
        }
    }

    #region 위치 제한
    private void LimitPosition(float lx = 16.5f, float ly = 12)
    {
        if (transform.position.x < -lx)
        {
            transform.position = new Vector3(-lx, transform.position.y, transform.position.z);
            isWalking = false;
        }
        if (transform.position.x > lx)
        {
            transform.position = new Vector3(lx, transform.position.y, transform.position.z);
            isWalking = false;
        }
        if (transform.position.y > ly)
        {
            transform.position = new Vector3(transform.position.x, ly, transform.position.z);
            isWalking = false;
        }
        if (transform.position.y < -ly)
        {
            transform.position = new Vector3(transform.position.x, -ly, transform.position.z);
            isWalking = false;
        }
    }
    #endregion

    #region AI 초기화
    private void InitializeAI()
    {
        if (!isWandering)
            StartCoroutine(Wander());

        //걷고 있을 때
        if (isWalking && !isHarvest)
        {
            Debug.LogError(gameObject.name);
            switch (nowRotatedState)
            {
                case 0: //NorthWest
                    transform.Translate(new Vector3(-1 * movementSpeed * Time.deltaTime, 1 * movementSpeed * Time.deltaTime, 0));
                    break;

                case 1: //NorthEast
                    transform.Translate(new Vector3(1 * movementSpeed * Time.deltaTime, 1 * movementSpeed * Time.deltaTime, 0));
                    break;

                case 2: //SouthWest
                    transform.Translate(new Vector3(-1 * movementSpeed * Time.deltaTime, -1 * movementSpeed * Time.deltaTime, 0));
                    break;

                case 3: //SouthEast
                    transform.Translate(new Vector3(1 * movementSpeed * Time.deltaTime, -1 * movementSpeed * Time.deltaTime, 0));
                    break;
            }


        }
    }
    #endregion

    //public void RewardAD(int num)
    //{
    //    switch (num)
    //    {
    //        case 0:
    //            ADManager.GetInstance().ShowRewardAD(10);
    //            break;

    //        case 1:
    //            ADManager.GetInstance().ShowRewardAD(11);
    //            break;

    //        case 2:
    //            ADManager.GetInstance().ShowRewardAD(12);
    //            break;

    //        case 3:
    //            ADManager.GetInstance().ShowRewardAD(13);
    //            break;
    //    }
    //}

}
