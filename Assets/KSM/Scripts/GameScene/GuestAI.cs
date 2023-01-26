using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GuestAI : MonoBehaviour
{
    [SerializeField] private GameObject bubbleObject;
    [SerializeField] private SkeletonGraphic emogeObject;
    [SerializeField] private Image itemImage;
    [SerializeField] private Image coinImage;
    [SerializeField] private Button purchaseButton;

    [System.Serializable]
    public class Item
    {
        public string name;
        public int itemCode;
        public Sprite itemSprite;
    }

    public Item[] items;

    public int myIndex = -1;
    public int itemCode = -1;
    public int wantTableNumber = -1;
    public bool isSad = false;
    public bool isAngry = false;
    public bool isHappy = false;

    public int waitNumber = 0;
    private bool isWait = false;
    private float waitCoolTime = 0;
    public enum State
    {
        No_Enter,
        Enter,
        Stop,
        Exit,
        
        Search,
        GoTo,
        Choose,
        
        Wait,
        Purchase,
        
    }

    #region AnimationName

    private const string IDLE_FRONT = "idle_front_r";
    private const string IDLE_BACK = "idle_back_r";

    private const string WALK_FRONT = "walk_front_r";
    private const string WALK_BACK = "walk_back_r";

    private const string IDLE_SIGH = "sigh_front_r";
    private const string WALK_SIGH_FRONT = "walk_sigh_front_r";
    private const string WALK_SIGH_BACK = "walk_sigh_back_r";

    private const string IDLE_ANGRY = "angry_front_r";
    private const string WALK_ANGRY_FRONT = "walk_angry_front_r";
    private const string WALK_ANGRY_BACK = "walk_angry_back_r";


    private const string PICKUP = "pickup_front_r";

    #endregion
    

    public State nowState;

    private NavMeshAgent navMeshAgent;
    private SkeletonAnimation animation;

    #region Unity_Method

    void Awake()
    {
        animation = GetComponent<SkeletonAnimation>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        
        navMeshAgent.Warp(GameManager.Mart.Guest.startPoint.position);
        
        SetBubble(string.Empty);
    }

    void Start()
    {
        
    }

    void Update()
    {
        AdjustSortingLayer();

        #region 애니메이션 상태

        if (nowState == State.Enter || nowState == State.No_Enter || nowState == State.GoTo || nowState == State.Exit && !isSad && !isAngry || nowState == State.Wait)
        {
            if (navMeshAgent.velocity.x > 0 && navMeshAgent.velocity.y > 0)
            {
                animation.skeleton.ScaleX = 1f;
                if (animation.AnimationName != WALK_BACK)
                    animation.state.SetAnimation(0, WALK_BACK, true);
            }

            if (navMeshAgent.velocity.x > 0 && navMeshAgent.velocity.y < 0)
            {
                animation.skeleton.ScaleX = 1f;
                if (animation.AnimationName != WALK_FRONT)
                    animation.state.SetAnimation(0, WALK_FRONT, true);
            }
        
            if (navMeshAgent.velocity.x < 0 && navMeshAgent.velocity.y > 0)
            {
                animation.skeleton.ScaleX = -1f;
                if (animation.AnimationName != WALK_BACK)
                    animation.state.SetAnimation(0, WALK_BACK, true);
            }
        
            if (navMeshAgent.velocity.x < 0 && navMeshAgent.velocity.y < 0)
            {
                animation.skeleton.ScaleX = -1f;
                if (animation.AnimationName != WALK_FRONT)
                    animation.state.SetAnimation(0, WALK_FRONT, true);
            }
        }

        else if (nowState == State.Exit && isSad)
        {
            if (navMeshAgent.velocity.x > 0 && navMeshAgent.velocity.y > 0)
            {
                animation.skeleton.ScaleX = 1f;
                if (animation.AnimationName != WALK_SIGH_BACK)
                    animation.state.SetAnimation(0, WALK_SIGH_BACK, true);
            }

            if (navMeshAgent.velocity.x > 0 && navMeshAgent.velocity.y < 0)
            {
                animation.skeleton.ScaleX = 1f;
                if (animation.AnimationName != WALK_SIGH_FRONT)
                    animation.state.SetAnimation(0, WALK_SIGH_FRONT, true);
            }
        
            if (navMeshAgent.velocity.x < 0 && navMeshAgent.velocity.y > 0)
            {
                animation.skeleton.ScaleX = -1f;
                if (animation.AnimationName != WALK_SIGH_BACK)
                    animation.state.SetAnimation(0, WALK_SIGH_BACK, true);
            }
        
            if (navMeshAgent.velocity.x < 0 && navMeshAgent.velocity.y < 0)
            {
                animation.skeleton.ScaleX = -1f;
                if (animation.AnimationName != WALK_SIGH_FRONT)
                    animation.state.SetAnimation(0, WALK_SIGH_FRONT, true);
            }
        }
        
        else if (nowState == State.Exit && isAngry)
        {
            if (navMeshAgent.velocity.x > 0 && navMeshAgent.velocity.y > 0)
            {
                animation.skeleton.ScaleX = 1f;
                if (animation.AnimationName != WALK_ANGRY_BACK)
                    animation.state.SetAnimation(0, WALK_ANGRY_BACK, true);
            }

            if (navMeshAgent.velocity.x > 0 && navMeshAgent.velocity.y < 0)
            {
                animation.skeleton.ScaleX = 1f;
                if (animation.AnimationName != WALK_ANGRY_FRONT)
                    animation.state.SetAnimation(0, WALK_ANGRY_FRONT, true);
            }
        
            if (navMeshAgent.velocity.x < 0 && navMeshAgent.velocity.y > 0)
            {
                animation.skeleton.ScaleX = -1f;
                if (animation.AnimationName != WALK_ANGRY_BACK)
                    animation.state.SetAnimation(0, WALK_ANGRY_BACK, true);
            }
        
            if (navMeshAgent.velocity.x < 0 && navMeshAgent.velocity.y < 0)
            {
                animation.skeleton.ScaleX = -1f;
                if (animation.AnimationName != WALK_ANGRY_FRONT)
                    animation.state.SetAnimation(0, WALK_ANGRY_FRONT, true);
            }
        }

        #region 목표지점 도달

        if (navMeshAgent.hasPath)
        {
            switch (nowState)
            {
                case State.Enter:
                    if (IsReached(GameManager.Mart.Guest.enterPoint.position))
                    {
                        ChangeState(State.Search);
                    }
                    break;
                
                case State.GoTo:
                    if (IsReached(GameManager.Mart.Guest.wantPoint[wantTableNumber].position))
                    {
                        ChangeState(State.Choose);
                    }

                    break;
                
                case State.Wait:
                    if (IsReached(GameManager.Mart.Guest.waitPoint[waitNumber].position))
                    {
                        if (GetComponent<SkeletonAnimation>().AnimationName != IDLE_FRONT || GetComponent<SkeletonAnimation>().AnimationName != IDLE_ANGRY)
                            GetComponent<SkeletonAnimation>().state.SetAnimation(0, IDLE_FRONT, true);
                        
                        ChangeState(State.Purchase);
                    }

                    break;
            }
        }

        #endregion

        #region 대기시간 관련

        if (nowState == State.Wait || nowState == State.Purchase)
        {
            if (!isWait) return;

            if (waitCoolTime > 0)
            {
                waitCoolTime -= Time.deltaTime;

                if (waitCoolTime < 3)
                {
                    if (waitNumber == 0)
                    {
                        GetComponent<SkeletonAnimation>().skeleton.ScaleX = -1;
                    }
                    else
                    {
                        GetComponent<SkeletonAnimation>().skeleton.ScaleX = 1f;
                    }

                    if (GetComponent<SkeletonAnimation>().AnimationName != IDLE_ANGRY)
                        GetComponent<SkeletonAnimation>().state.SetAnimation(0, IDLE_ANGRY, true);
                }
            }
            else
            {
                SetBubble("Angry");
                
                //한숨쉬는 소리 나오게 하기
                if (GameManager.Instance.nowMode == GameManager.Mode.Mart)
                {
                    // GetComponent<AudioSource>().clip = StaticManager.Sound.SearchClip("Sigh");
                    // GetComponent<AudioSource>().Play();
                }
                
                //물품 가방안에 넣기
                StaticManager.Backend.backendGameData.InventoryData.AddItem(itemCode, 1);

                GameManager.Mart.Guest.waitGuests.Remove(GetComponent<GuestAI>());
                for (int i = GameManager.Mart.Guest.waitGuests.Count - 1; i >= 0; i--)
                {
                    GameManager.Mart.Guest.waitGuests[i].ReloadList();
                }
                
                ChangeState(State.Exit);
            }
        }

        #endregion

        #endregion
    }

    #endregion

    public void Initialize(int myIndex, int itemCode)
    {
        this.myIndex = myIndex;
        this.itemCode = itemCode;
        navMeshAgent.avoidancePriority = myIndex;
    }

    public void ChangeState(State state)
    {
        nowState = state;

        switch (state)
        {
            case State.No_Enter:
                Invoke("DestroyObject", 15);
                navMeshAgent.SetDestination(GameManager.Mart.Guest.noEnterPoint.position);

                break;
            
            case State.Enter:
                GameManager.Mart.Guest.nowGuest++;
                navMeshAgent.SetDestination(GameManager.Mart.Guest.enterPoint.position);
                break;

            
            case State.Exit:
                GameManager.Mart.Guest.nowGuest--;
                Invoke("DestroyObject", 15);
                navMeshAgent.SetDestination(GameManager.Mart.Guest.exitPoint.position);

                break;
            
            case State.Search:
                if (StaticManager.Backend.backendGameData.UserData.Level <= 7)
                {
                    int[] random = { 0, 1, 2 };
                    Initialize(GameManager.Mart.Guest.guests.Count, random[Random.Range(0, 2)]);
                }
                    
                else if (StaticManager.Backend.backendGameData.UserData.Level <= 10)
                {
                    int[] random = { 0, 1, 2, 3, 4 };
                    Initialize(GameManager.Mart.Guest.guests.Count, random[Random.Range(0, 4)]);
                }
                else if (StaticManager.Backend.backendGameData.UserData.Level <= 15)
                {
                    int[] random = { 0, 1, 2, 3, 4, 5, 20 };
                    Initialize(GameManager.Mart.Guest.guests.Count, random[Random.Range(0, 6)]);
                }
                else if (StaticManager.Backend.backendGameData.UserData.Level <= 20)
                {
                    int[] random = { 0, 1, 2, 3, 4, 5, 6, 18, 20 };
                    Initialize(GameManager.Mart.Guest.guests.Count, random[Random.Range(0, 8)]);
                }
                else
                {
                    int[] random = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 18, 20 };
                    Initialize(GameManager.Mart.Guest.guests.Count, random[Random.Range(0, 11)]);
                }

                GameManager.Mart.Guest.guests.Add(GameManager.Mart.Guest.guests.Count, GetComponent<GuestAI>());
                
                StartCoroutine(SearchHarvest());
                break;
            
            case State.GoTo:
                navMeshAgent.SetDestination(GameManager.Mart.Guest.wantPoint[wantTableNumber].position);
                break;
            
            case State.Choose:
                StartCoroutine(ChooseItem());
                break;
            
            case State.Purchase:
                StartCoroutine((WaitAndPurchaseItem()));
                break;
        }
    }

    #region 물품 찾기

    private IEnumerator SearchHarvest()
    {
        
        SetBubble("Item");
        
        animation.state.SetAnimation(0, IDLE_BACK, true);

        #region 두리번

        animation.skeleton.ScaleX = -1f;

        yield return new WaitForSeconds(0.5f);
        
        animation.skeleton.ScaleX = 1f;

        yield return new WaitForSeconds(0.5f);
        
        animation.skeleton.ScaleX = -1f;

        yield return new WaitForSeconds(0.5f);
        
        animation.skeleton.ScaleX = 1f;

        yield return new WaitForSeconds(0.5f);

        #endregion
        //마트에 인원이 적당할 경우
        if (GameManager.Mart.Guest.nowGuest <= GameManager.Mart.Guest.maxGuest)
        {
            bool isWant = false;
            bool isBest = false;
            for (int i = 0; i < 15; i++)
            {
                //원하는 물품이 있을 경우
                if (StaticManager.Backend.backendGameData.MartData.IsExistItem(i, itemCode))
                {
                    //테이블 선택
                    wantTableNumber = i;
                    isWant = true;
                }
            }
            
            if (itemCode == 0 || itemCode == 1 || itemCode == 2)
            {
                for (int i = 0; i < 15; i++)
                {
                    if (StaticManager.Backend.backendGameData.MartData.IsExistItem(i, itemCode + 15))
                    {
                        isBest = true;
                        wantTableNumber = i;
                        itemCode += 15;
                        isWant = true;
                        break;
                    }
                    
                    if (StaticManager.Backend.backendGameData.MartData.IsExistItem(i, itemCode + 9) && !isBest)
                    {
                        wantTableNumber = i;
                        itemCode += 9;
                        isWant = true;
                        break;
                    }
                    
                    
                }
            }
            
            else if (itemCode == 3 || itemCode == 4 || itemCode == 5)
            {
                for (int i = 0; i < 15; i++)
                {
                    if (StaticManager.Backend.backendGameData.MartData.IsExistItem(i, itemCode + 9))
                    {
                        wantTableNumber = i;
                        itemCode += 9;
                        isWant = true;
                        break;
                    }
                }
            }

            else if (itemCode == 18 || itemCode == 20)
            {
                for (int i = 0; i < 15; i++)
                {
                    if (StaticManager.Backend.backendGameData.MartData.IsExistItem(i, itemCode + 1))
                    {
                        Debug.LogError(itemCode + ", " + i);
                        wantTableNumber = i;
                        itemCode += 1;
                        isWant = true;
                        break;
                    }
                }
            }

            if (!isWant)
            {
                //한숨쉬는 소리 나오게 하기
                if (GameManager.Instance.nowMode == GameManager.Mode.Mart)
                {
                    // GetComponent<AudioSource>().clip = StaticManager.Sound.SearchClip("Sigh");
                    // GetComponent<AudioSource>().Play();
                }
                
                SetBubble(string.Empty);
                animation.state.SetAnimation(0, IDLE_SIGH, false);

                yield return new WaitForSeconds(1.3f);

                ChangeState(State.Exit);
                SetBubble("Sad");
            }
            else
            {
                ChangeState(State.GoTo);
            }
        }
        else
        {
            //한숨쉬는 소리 나오게 하기
            if (GameManager.Instance.nowMode == GameManager.Mode.Mart)
            {
                // GetComponent<AudioSource>().clip = StaticManager.Sound.SearchClip("Sigh");
                // GetComponent<AudioSource>().Play();
            }
            
            SetBubble(string.Empty);
            animation.state.SetAnimation(0, IDLE_SIGH, false);

            yield return new WaitForSeconds(1.3f);
            
            ChangeState(State.Exit);
            SetBubble("Sad");
        }
        
        
    }

    #endregion

    #region 아이템 집기

    private IEnumerator ChooseItem()
    {
        if (wantTableNumber == 0 || wantTableNumber == 1 || wantTableNumber == 2 || wantTableNumber == 4 || wantTableNumber == 5 || wantTableNumber == 9 || wantTableNumber == 10 ||
            wantTableNumber == 13)
            GetComponent<SkeletonAnimation>().skeleton.ScaleX = -1f;
        else
            GetComponent<SkeletonAnimation>().skeleton.ScaleX = 1f;

        if (GetComponent<SkeletonAnimation>().AnimationName != IDLE_FRONT)
            GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, IDLE_FRONT, true);

        if (StaticManager.Backend.backendGameData.MartData.IsExistItem(wantTableNumber, itemCode))
        {
            if (GetComponent<SkeletonAnimation>().AnimationName != PICKUP)
                GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, PICKUP, false);

            StaticManager.Backend.backendGameData.MartData.AddItem(wantTableNumber, -1);
            
            GameManager.Mart.InitializeData(wantTableNumber);

            yield return new WaitForSeconds(0.8f);
            
            GameManager.Mart.Guest.waitGuests.Add(GetComponent<GuestAI>());

            for (int i = 0; i < GameManager.Mart.Guest.waitGuests.Count; i++)
            {
                if (GameManager.Mart.Guest.waitGuests[i] == GetComponent<GuestAI>())
                {
                    waitNumber = i;
                    navMeshAgent.SetDestination(GameManager.Mart.Guest.waitPoint[i].position);
                }
            }

            SetBubble(string.Empty);
            
            ChangeState(State.Wait);
        }

        else
        {
            SetBubble("Question");

            yield return new WaitForSeconds(1f);
            
            SetBubble("Sad");
            
            ChangeState(State.Exit);
        }
    }

    #endregion

    #region 계산 대기하기

    private IEnumerator WaitAndPurchaseItem()
    {
        isWait = true;

        navMeshAgent.avoidancePriority = 50 - waitNumber;
        navMeshAgent.ResetPath();

        if (waitCoolTime == 0)
            waitCoolTime = Random.Range(10f, 15f);

        if (waitNumber == 0)
        {
            GetComponent<SkeletonAnimation>().skeleton.ScaleX = -1f;
            SetBubble("Purchase");
            purchaseButton.onClick.RemoveAllListeners();
            purchaseButton.onClick.AddListener(() =>
            {
                StaticManager.Sound.SetSFX();
                StartCoroutine(PurchaseItem());
            });
            
            //알바생이 있을 경우 자동 계산
            if (StaticManager.Backend.backendGameData.PartTimeData.Type != -1)
            {
                GameManager.PartTime.SetWorkAnimation(StaticManager.Backend.backendGameData.PartTimeData.Type);
                StartCoroutine(PurchaseItem());
            }
        }
        else
        {
            GetComponent<SkeletonAnimation>().skeleton.ScaleX = 1f;
            SetBubble(string.Empty);
        }
        
        yield return null;
    }

    #endregion

    #region 구매하기

    private IEnumerator PurchaseItem()
    {
        if (waitCoolTime > 3)
        {
            SetBubble("Happy");
        }
        else
        {
            SetBubble(string.Empty);
        }
        
        GameManager.Mart.Guest.waitGuests.Remove(GetComponent<GuestAI>());
        for (int i = GameManager.Mart.Guest.waitGuests.Count - 1; i >= 0; i--)
        {
            GameManager.Mart.Guest.waitGuests[i].ReloadList();
        }

        int amount = 0;

        if (itemCode >= 0 && itemCode < 9)
        {
            amount = StaticManager.Backend.backendChart.Harvest.harvestSheet[itemCode].Reward[0];
            StaticManager.Backend.backendGameData.UserData.AddGold(StaticManager.Backend.backendChart.Harvest.harvestSheet[itemCode].Reward[0]);
        }
        else if (itemCode >= 9 && itemCode < 15)
        {
            amount = StaticManager.Backend.backendChart.Harvest.harvestSheet[itemCode - 9].Reward[1];
            StaticManager.Backend.backendGameData.UserData.AddGold(StaticManager.Backend.backendChart.Harvest.harvestSheet[itemCode - 9].Reward[1]);
        }
        else if (itemCode >= 15 && itemCode < 18)
        {
            amount = StaticManager.Backend.backendChart.Harvest.harvestSheet[itemCode - 15].Reward[2];
            StaticManager.Backend.backendGameData.UserData.AddGold(StaticManager.Backend.backendChart.Harvest.harvestSheet[itemCode - 15].Reward[2]);
        }
        else if (itemCode == 18)
        {
            amount = StaticManager.Backend.backendChart.Harvest.harvestSheet[9].Reward[0];
            StaticManager.Backend.backendGameData.UserData.AddGold(StaticManager.Backend.backendChart.Harvest.harvestSheet[9].Reward[0]);
        }
        else if (itemCode == 19)
        {
            amount = StaticManager.Backend.backendChart.Harvest.harvestSheet[9].Reward[1];
            StaticManager.Backend.backendGameData.UserData.AddGold(StaticManager.Backend.backendChart.Harvest.harvestSheet[9].Reward[1]);
        }
        else if (itemCode == 20)
        {
            amount = StaticManager.Backend.backendChart.Harvest.harvestSheet[10].Reward[0];
            StaticManager.Backend.backendGameData.UserData.AddGold(StaticManager.Backend.backendChart.Harvest.harvestSheet[10].Reward[0]);
        }
        else if (itemCode == 21)
        {
            amount = StaticManager.Backend.backendChart.Harvest.harvestSheet[10].Reward[1];
            StaticManager.Backend.backendGameData.UserData.AddGold(StaticManager.Backend.backendChart.Harvest.harvestSheet[10].Reward[1]);
        }
        
        //퀘스트
        StaticManager.Backend.backendGameData.QuestData.AddCount(3, 1);
        
        GameManager.Instance.SaveAllData();
        
        ChangeState(State.Exit);
        GetComponent<AudioSource>().clip = StaticManager.Sound.SearchClip("Cash");
        GetComponent<AudioSource>().Play();
        
        GameObject goldObject = Instantiate(Resources.Load<GameObject>("Prefabs/GameScene/Effect/RewardEffect"), GameManager.Instance.worldCanvas.transform);
        goldObject.transform.position = new Vector3(45,-4.7f,0) + new Vector3(-1, 3.5f, 0);
        goldObject.GetComponent<RewardEffect>().Initialize(amount);
        
        
        //알바생이 아닐 경우
        if(StaticManager.Backend.backendGameData.PartTimeData.Type == -1)
            GameManager.Bunny.GetWorkBunny().SetWorkAnimation();

        yield return null;
    }

    #endregion

    public void ReloadList()
    {
        for (int i = 0; i < GameManager.Mart.Guest.waitGuests.Count; i++)
        {
            if (GameManager.Mart.Guest.waitGuests[i] == GetComponent<GuestAI>())
                waitNumber = i;

            navMeshAgent.SetDestination(GameManager.Mart.Guest.waitPoint[waitNumber].position);
            ChangeState(State.Wait);
        }
    }

    //알바생 구입한 시기 한번만 발동
    public void PurchaseInitial()
    {
        //알바생이 있을 경우 자동 계산
        if (StaticManager.Backend.backendGameData.PartTimeData.Type != -1 && waitNumber == 0)
        {
            GameManager.PartTime.SetWorkAnimation(StaticManager.Backend.backendGameData.PartTimeData.Type);
            StartCoroutine(PurchaseItem());
        }
    }
    
    
    private bool IsReached(Vector3 pos)
    {
        return Vector3.Distance(transform.position, pos) < 0.3f;
    }
    
    public void AdjustSortingLayer()
    {
        GetComponent<SortingGroup>().sortingOrder = (int)(transform.position.y * -100);
    }

    private void SetBubble(string type)
    {
        switch (type)
        {
            case "Item":
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].itemCode == itemCode)
                    {
                        itemImage.sprite = items[i].itemSprite;
                        itemImage.GetComponent<RectTransform>().sizeDelta = new Vector2(items[i].itemSprite.textureRect.width, items[i].itemSprite.textureRect.height);

                        if (items[i].itemCode == 18 || items[i].itemCode == 20)
                            itemImage.GetComponent<RectTransform>().sizeDelta = new Vector2(256, 256);
                    }
                }
                bubbleObject.SetActive(true);
                itemImage.gameObject.SetActive(true);
                emogeObject.gameObject.SetActive(false);
                coinImage.gameObject.SetActive(false);
                break;
            
            case "Purchase":
                bubbleObject.SetActive(true);
                itemImage.gameObject.SetActive(false);
                emogeObject.gameObject.SetActive(false);
                coinImage.gameObject.SetActive(true);
                break;
            
            case "Question":
                bubbleObject.SetActive(false);
                itemImage.gameObject.SetActive(false);
                emogeObject.gameObject.SetActive(true);
                coinImage.gameObject.SetActive(false);
                emogeObject.AnimationState.SetAnimation(0, "question", true);
                EditorForceReloadSkeletonDataAssetAndComponent(emogeObject.GetComponent<SkeletonRenderer>());
                break;
            
            case "Angry":
                isAngry = true;
                bubbleObject.SetActive(false);
                itemImage.gameObject.SetActive(false);
                //emogeObject.gameObject.SetActive(true);
                coinImage.gameObject.SetActive(false);
                emogeObject.AnimationState.SetAnimation(0, "angry", true);
                EditorForceReloadSkeletonDataAssetAndComponent(emogeObject.GetComponent<SkeletonRenderer>());
                break;
            
            case "Happy":
                isHappy = true;
                bubbleObject.SetActive(false);
                itemImage.gameObject.SetActive(false);
                emogeObject.gameObject.SetActive(true);
                coinImage.gameObject.SetActive(false);
                emogeObject.AnimationState.SetAnimation(0, "happy", true);
                EditorForceReloadSkeletonDataAssetAndComponent(emogeObject.GetComponent<SkeletonRenderer>());
                break;
            
            case "Sad":
                isSad = true;
                bubbleObject.SetActive(false);
                itemImage.gameObject.SetActive(false);
                //emogeObject.gameObject.SetActive(true);
                coinImage.gameObject.SetActive(false);
                emogeObject.AnimationState.SetAnimation(0, "sad", true);
                EditorForceReloadSkeletonDataAssetAndComponent(emogeObject.GetComponent<SkeletonRenderer>());
                break;
            
            case "":
                bubbleObject.SetActive(false);
                itemImage.gameObject.SetActive(false);
                emogeObject.gameObject.SetActive(false);
                coinImage.gameObject.SetActive(false);
                break;
        }
    }

    public void DestroyObject()
    {
        Destroy(this.gameObject);
    }
    
    private void EditorForceReloadSkeletonDataAssetAndComponent (SkeletonRenderer component) {
        if (component == null) return;

        // Clear all and reload.
        if (component.skeletonDataAsset != null) {
            foreach (AtlasAssetBase aa in component.skeletonDataAsset.atlasAssets) {
                if (aa != null) aa.Clear();
            }
            component.skeletonDataAsset.Clear();
        }
        component.skeletonDataAsset.GetSkeletonData(true);

        // Reinitialize.
        EditorForceInitializeComponent(component);
    }

    private void EditorForceInitializeComponent (SkeletonRenderer component) {
        if (component == null) return;
        if (!SkeletonDataAssetIsValid(component.SkeletonDataAsset)) return;
        component.Initialize(true);

#if BUILT_IN_SPRITE_MASK_COMPONENT
         SpineMaskUtilities.EditorAssignSpriteMaskMaterials(component);
#endif

        component.LateUpdate();
    }

    private bool SkeletonDataAssetIsValid (SkeletonDataAsset asset) {
        return asset != null && asset.GetSkeletonData(quiet: true) != null;
    }
}
