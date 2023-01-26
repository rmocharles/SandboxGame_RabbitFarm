using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer farmMap, martMap;
    [SerializeField] private float maxDistanceForTap = 100;  //탭으로 처리할 터치 동작의 최대 이동
    [SerializeField] private float maxDurationForTap = 0.5f;    //터치 동작이 탭으로 처리되는 최대 지속 시간
    [SerializeField] private bool ignoreUI = true;  //UI터치 방지

    private Vector2 touchStartPosition;
    private Vector2 touchLastPosition;
    private float touchStartTime;
    private bool isTouching = false;
    private float lastTouchTime;

    private float[] boundX, boundY;
    private bool isInitialize = false;

    public void Initialize()
    {
        boundX = new float[2];
        boundY = new float[2];
        lastTouchTime = Time.time;
        
        //카메라 초기 설정 (핸드폰 : 7, 패드 : 10)
        Camera.main.orthographicSize = IsPad() ? 10 : 7;
        
        #if UNITY_ANDROID || UNITY_IOS
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
        #endif

        isInitialize = true;
    }

    void Update()
    {
        if (!isInitialize) return;

        switch (GameManager.Instance.nowMode)
        {
            case GameManager.Mode.Farm:
                boundX[0] = farmMap.transform.position.x - farmMap.bounds.size.x / 2;
                boundX[1] = farmMap.transform.position.x + farmMap.bounds.size.x / 2;
                boundY[0] = farmMap.transform.position.y - farmMap.bounds.size.y / 2;
                boundY[1] = farmMap.transform.position.y + farmMap.bounds.size.y / 2;
                break;
            
            case GameManager.Mode.Mart:
                boundX[0] = martMap.transform.position.x - martMap.bounds.size.x / 2;
                boundX[1] = martMap.transform.position.x + martMap.bounds.size.x / 2;
                boundY[0] = martMap.transform.position.y - martMap.bounds.size.y / 2;
                boundY[1] = martMap.transform.position.y + martMap.bounds.size.y / 2;
                break;
        }
        

        UpdateWithTouch();  //모바일

        CameraInBounds();   //카메라 밖 체크
    }

    private void UpdateWithTouch()
    {
        int touchCount = Input.touches.Length;
        if (touchCount == 1)
        {
            Touch touch = Input.touches[0];
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (ignoreUI || !IsPointerOverUIObject())
                    {
                        touchStartPosition = touch.position;
                        touchStartTime = Time.time;
                        touchLastPosition = touchStartPosition;

                        isTouching = true;
                    }

                    break;
                
                case TouchPhase.Moved:
                    touchLastPosition = touch.position;
                    if (touch.deltaPosition != Vector2.zero && isTouching)
                    {
                        OnSwipe(touch.deltaPosition);
                    }

                    break;
                
                case TouchPhase.Ended:
                    lastTouchTime = Time.time;

                    if (Time.time - touchStartTime <= maxDurationForTap && isTouching)
                    {
                        OnClick();
                    }
                    
                    isTouching = false;
                    break;
            }
        }
        
        else if (touchCount == 2)
        {
            Touch touch0 = Input.touches[0];
            Touch touch1 = Input.touches[1];

            if (touch0.phase == TouchPhase.Ended || touch1.phase == TouchPhase.Ended) return;

            isTouching = true;

            float previousDistance = Vector2.Distance(touch0.position - touch0.deltaPosition, touch1.position - touch1.deltaPosition);
            float currentDistance = Vector2.Distance(touch0.position, touch1.position);

            if (previousDistance != currentDistance)
            {
                OnPinch((touch0.position + touch1.position) / 2, previousDistance, currentDistance, (touch1.position - touch0.position).normalized);
            }
        }

        else
        {
            if (isTouching)
                isTouching = false;
        }
    }

    private void OnClick()
    {
        if (GameManager.Field.selectHarvestUI != null && GameManager.Field.selectHarvestUI.activeSelf)
        {
            Destroy(GameManager.Field.harvestInfoUI);
            GameManager.Field.selectHarvestUI.GetComponent<RectTransform>().DOMoveY(-50, 1f);
            Invoke("DestroyHarvestUI", .1f);
        }

        if (GameManager.Mart.slotUI != null && GameManager.Mart.slotUI.activeSelf)
        {
            Destroy(GameManager.Mart.slotUI);
        }
    }

    private void OnSwipe(Vector2 deltaPos)
    {
        Camera.main.transform.position -= Camera.main.ScreenToWorldPoint(deltaPos) - Camera.main.ScreenToWorldPoint(Vector2.zero);
    }

    private void OnPinch(Vector2 center, float oldDistance, float newDistance, Vector3 touchDelta)
    {
        Debug.LogWarning($"Center : {center}, OldDistance : {oldDistance}, NewDistance : {newDistance}, touchDelta : {touchDelta}");

        if (Camera.main.orthographic)
        {
            var currentPinchPosition = Camera.main.ScreenToWorldPoint(center);
            Camera.main.orthographicSize = Mathf.Max(5, Camera.main.orthographicSize * oldDistance / newDistance);

            var newPinchPosition = Camera.main.ScreenToWorldPoint(center);
            Camera.main.transform.position -= newPinchPosition - currentPinchPosition;
        }
    }

    private void CameraInBounds()
    {
        if (Camera.main.orthographic)
        {
            Camera.main.orthographicSize = Mathf.Min(Camera.main.orthographicSize, ((boundX[1] - boundY[0]) / 2));
            Camera.main.orthographicSize = Mathf.Min(Camera.main.orthographicSize, (Screen.height * (boundX[1] - boundX[0]) / (2 * Screen.width)));

            Vector2 margin = Camera.main.ScreenToWorldPoint((Vector2.up * Screen.height / 2) + (Vector2.right * Screen.width / 2)) - Camera.main.ScreenToWorldPoint(Vector2.zero);
            
            float marginX = margin.x;
            float marginY = margin.y;

            float camMaxX = boundX[1] - marginX;
            float camMaxY = boundY[1] - marginY;
            float camMinX = boundX[0] + marginX;
            float camMinY = boundY[0] + marginY;

            float camX = Mathf.Clamp(Camera.main.transform.position.x, camMinX, camMaxX);
            float camY = Mathf.Clamp(Camera.main.transform.position.y, camMinY, camMaxY);

            Camera.main.transform.position = new Vector3(camX, camY, Camera.main.transform.position.z);
        }
    }
    
    private bool IsPointerOverUIObject()
    {
        if (EventSystem.current == null) return false;
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();

        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public bool IsPad()
    {
        
        float rate = Screen.width / Screen.height + float.Parse("0." + Screen.width % Screen.height);
        return rate < 1.8f;
    }

    private void DestroyHarvestUI()
    {
        GameManager.Field.CloseUI();
    }
}
