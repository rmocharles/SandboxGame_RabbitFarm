using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PetInfo : MonoBehaviour
{
    #region Animation Name

    private const string WALK_FRONT = "walk_front_r";
    private const string WALK_BACK = "walk_back_r";

    private string IDLE = "idle_front_r";

    private string[] TOUCH = {
        "touch1_front_r", "touch2_front_r"
    };
    #endregion
    
    public enum State
    {
        Idle, Walk, TOUCH
    }
    public State nowState = State.Idle;

    public int direction = 0;

    private NavMeshAgent navMeshAgent;

    private Coroutine nowCoroutine = null;

    [SerializeField] private GameObject bubbleObject;
    [SerializeField] private Button touchButton;

    public float originRemainTime = 30f;
    private float remainTime = 30f;
    private bool isSound = false;   //말풍선 떴을 때 효과음
    
    #region Unity Methods

    void Awake()
    {
        if (gameObject.name == "Pet_0")
            originRemainTime = StaticManager.Backend.backendChart.Price.GetPrice("Pet_0");
    }

    void Start()
    {
        //Navmesh 초기화
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        navMeshAgent.Warp(transform.position);

        ActiveBubble(false);
        remainTime = originRemainTime;
        
        if (nowCoroutine != null)
            StopCoroutine(nowCoroutine);

        nowCoroutine = StartCoroutine(PlayerController(.5f));
        
        bubbleObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            //퀘스트
            StaticManager.Backend.backendGameData.QuestData.AddCount(7, 3);
            
            StaticManager.Backend.backendGameData.InventoryData.AddItem("Fertilizer", 3);
            GameManager.Instance.SaveAllData();

            isSound = false;

            if (gameObject.name == "Pet_0")
            {
                StaticManager.AD.ShowRewardAD(() =>
                {
                    StaticManager.Sound.SetSFX("Get");
                    GameObject itemObject = Instantiate(Resources.Load<GameObject>("Prefabs/GameScene/Effect/RewardEffect"), GameManager.Instance.worldCanvas.transform);
                    itemObject.transform.position = transform.position + new Vector3(0, 2, 0);
                    itemObject.GetComponent<RewardEffect>().Initialize(22, 1);
                });
            }
            else
            {
                StaticManager.Sound.SetSFX("Get");
                GameObject itemObject = Instantiate(Resources.Load<GameObject>("Prefabs/GameScene/Effect/RewardEffect"), GameManager.Instance.worldCanvas.transform);
                itemObject.transform.position = transform.position + new Vector3(0, 2, 0);
                itemObject.GetComponent<RewardEffect>().Initialize(22, 1);
            }
            
            remainTime = originRemainTime;
        });
        
        touchButton.onClick.AddListener(() =>
        {
            ChangeState(State.TOUCH);
        });
    }

    private void Update()
    {
        //Y축 레이어 순서 정하기
        AdjustSortingLayer();

        //방향 정하기
        if (direction % 2 == 0)
            GetComponent<SkeletonAnimation>().skeleton.ScaleX = -1f;
        else
            GetComponent<SkeletonAnimation>().skeleton.ScaleX = 1f;
        
        //상태 실시간 체크
        switch (nowState)
        {
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
        }
        
        
        
        //비료 쿨타임
        if (remainTime > 0)
        {
            ActiveBubble(false);
            remainTime -= Time.deltaTime;
        }
        else
        {
            if (!isSound)
            {
                isSound = true;
                
                if (GameManager.Instance.nowMode == GameManager.Mode.Farm)
                {
                    for (int i = 0; i <= 5; i++)
                    {
                        if (gameObject.name == "Pet_" + i)
                        {
                            if(i == 0)
                                GetComponent<AudioSource>().clip = StaticManager.Sound.SearchClip("Rabbit");
                            if(i == 1 || i == 3 || i == 4)
                                GetComponent<AudioSource>().clip = StaticManager.Sound.SearchClip("Cat_" + i);
                            if(i == 2)
                                GetComponent<AudioSource>().clip = StaticManager.Sound.SearchClip("Dog");
                        }
                    }
                    GetComponent<AudioSource>().Play();
                }
            }
                
            ActiveBubble(true);
        }
    }

    #endregion

    private IEnumerator PlayerController(float delay = 0f)
    {
        yield return new WaitForSeconds(delay);
        //걷거나 가만히 있기
        if (Random.Range(0, 1) == 0)
            ChangeState(State.Idle);
        else
            ChangeState(State.Walk);
    }

    private IEnumerator RandomIdle(float delay = 0f, bool isWork = false)
    {
        if(navMeshAgent.hasPath)
            navMeshAgent.ResetPath();
        
        yield return new WaitForSeconds(delay);

        //방향 정하기
        direction = Random.Range(0, 4);
        GetComponent<SkeletonAnimation>().state.SetAnimation(0, IDLE, true);

        yield return new WaitForSeconds(6f);

        if (isWork)
        {
            ChangeState(State.Idle);
        }
        else
        {
            //걷거나 가만히 있기
            if (Random.Range(0, 100) <= 10)
                ChangeState(State.Idle);
            else
                ChangeState(State.Walk);
        }
    }

    private IEnumerator RandomWalk(float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        float randomWalkTime = Random.Range(1, 2f);

        direction = Random.Range(0, 4);

        yield return new WaitForSeconds(randomWalkTime);
        
        //걷거나 가만히 있기
        if (Random.Range(1, 101) <= 90)
            ChangeState(State.Idle);
        else
            ChangeState(State.Walk);
    }

    private IEnumerator RandomTouch(float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        int random = Random.Range(0, 2);
        GetComponent<SkeletonAnimation>().state.SetAnimation(0, TOUCH[random], true);

        yield return new WaitForSeconds(2.5f);
        
        ChangeState(State.Idle);
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
            
            case State.TOUCH:
                nowCoroutine = StartCoroutine(RandomTouch());
                break;
        }
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
        transform.position = pos;
    }
    
    #endregion
}
