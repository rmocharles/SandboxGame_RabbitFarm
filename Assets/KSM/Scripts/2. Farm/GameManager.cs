using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public event Action<Vector2> onStartTouch;
    public event Action<Vector2> onEndTouch;
    public event Action<Vector2> onTap;
    public event Action<Vector2> onSwipe;
    public event Action<float, float> onPinch;
    #region Map Control
    private Vector3 touchStart;

    [SerializeField]
    private SpriteRenderer mapRenderer, mapRenderer2;

    [Header("[ Main Camera ]")]
    public CinemachineVirtualCamera mainCamera;

    private float mapMinX, mapMinY, mapMaxX, mapMaxY;
    #endregion
    [Header("< Tap >")]
    [Tooltip("탭으로 처리할 터치 동작의 최대 이동")]
    public float maxDistanceForTap = 40;
    [Tooltip("터치 동작이 탭으로 처리되는 최대 지속 시간")]
    public float maxDurationForTap = 0.4f;

    [Header("< Ignore UI >")]
    [Tooltip("UI 클릭을 방지합니다.")]
    public bool ignoreUI = false;

    [Header("< Map Control >")]
    [Tooltip("맵의 확대정도 부분입니다.")]
    public float zoomMin = 3;
    public float zoomMax = 15;
    private float boundMinX, boundMaxX, boundMinY, boundMaxY;

    [Header("< Desktop Debug >")]
    [Tooltip("PC 확인용입니다.")]
    public bool useMouse = true;
    [Tooltip("마우스를 통한 확대 정도입니다.")]
    public float mouseScrollSpeed = 0.5f;



    [Header("< Scale Ctrl >")]
    public GameObject[] selectObject;

    Vector2 touchStartPosition;
    Vector2 touchLastPosition;
    float touchStartTime;
    bool isTouching = false;

    private float lastTouchTime;
    private float doubleTouchDelay = 0.1f;

    void Awake()
    {
        lastTouchTime = Time.time;

        mainCamera.m_Lens.OrthographicSize = 99;
        //useMouse = Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer && Input.mousePresent;
    }

    void Update()
    {
        if (FarmUI.GetInstance().isMart)
        {
            boundMinX = mapRenderer2.transform.position.x - mapRenderer2.bounds.size.x / 2f;
            boundMaxX = mapRenderer2.transform.position.x + mapRenderer2.bounds.size.x / 2f;

            boundMinY = mapRenderer2.transform.position.y - mapRenderer2.bounds.size.y / 2f;
            boundMaxY = mapRenderer2.transform.position.y + mapRenderer2.bounds.size.y / 2f;
        }
        else
        {
            boundMinX = mapRenderer.transform.position.x - mapRenderer.bounds.size.x / 2f;
            boundMaxX = mapRenderer.transform.position.x + mapRenderer.bounds.size.x / 2f;

            boundMinY = mapRenderer.transform.position.y - mapRenderer.bounds.size.y / 2f;
            boundMaxY = mapRenderer.transform.position.y + mapRenderer.bounds.size.y / 2f;
        }
        

        zoomMax = boundMaxX / mainCamera.m_Lens.Aspect;

        if (useMouse)
        {
            UpdateWithMouse();
        }
        else
        {
            UpdateWithTouch();
        }
    }

    void LateUpdate()
    {
        CameraInBounds();
    }

    void UpdateWithMouse()
    {
        if (FarmUI.GetInstance().IsActiveObject()) return;

        if (Input.GetMouseButtonDown(0))
        {

            if (Time.time - lastTouchTime < doubleTouchDelay)
            {
                FarmUI.GetInstance().ScreenShotMode(false);

                
                
            }

            if (ignoreUI || !IsPointerOverUIObject())
            {
                touchStartPosition = Input.mousePosition;
                touchStartTime = Time.time;
                touchLastPosition = touchStartPosition;

                isTouching = true;

                if (onStartTouch != null) onStartTouch(Input.mousePosition);
            }
        }

        if(Input.GetMouseButtonDown(0) && isTouching)
        {
            for (int i = 0; i < FarmUI.GetInstance().selectCanvas.transform.childCount; i++)
            {
                Destroy(FarmUI.GetInstance().selectCanvas.transform.GetChild(i).gameObject);
            }
        }

        if(Input.GetMouseButton(0) && isTouching)
        {
            Vector2 move = (Vector2)Input.mousePosition - touchLastPosition;
            touchLastPosition = Input.mousePosition;

            if(move != Vector2.zero)
            {
                OnSwipe(move);
            }
        }

        if(Input.GetMouseButtonUp(0) && isTouching)
        {
            lastTouchTime = Time.time;

            if (Time.time - touchStartTime <= maxDurationForTap && Vector2.Distance(Input.mousePosition, touchStartPosition) <= maxDistanceForTap)
            {
                OnClick(Input.mousePosition);
            }

            if (onEndTouch != null) onEndTouch(Input.mousePosition);
            isTouching = false;
        }

        if(Input.mouseScrollDelta.y != 0)
        {
            OnPinch(Input.mousePosition, 1, Input.mouseScrollDelta.y < 0 ? (1 / mouseScrollSpeed) : mouseScrollSpeed, Vector2.right);
        }
    }

    void UpdateWithTouch()
    {
        if (FarmUI.GetInstance().IsActiveObject()) return;

        int touchCount = Input.touches.Length;

        if (touchCount == 1)
        {
            Touch touch = Input.touches[0];

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    {
                        if (Time.time - lastTouchTime < doubleTouchDelay)
                        {
                            if (FarmUI.GetInstance().isScreenShotMode)
                            {
                                FarmUI.GetInstance().ScreenShotMode(false);

                            }
                        }

                        if (ignoreUI || !IsPointerOverUIObject())
                        {
                            touchStartPosition = touch.position;
                            touchStartTime = Time.time;
                            touchLastPosition = touchStartPosition;

                            isTouching = true;

                            if (onStartTouch != null) onStartTouch(touchStartPosition);
                        }

                        break;
                    }
                case TouchPhase.Moved:
                    {
                        touchLastPosition = touch.position;

                        if (touch.deltaPosition != Vector2.zero && isTouching)
                        {
                            OnSwipe(touch.deltaPosition);
                        }
                        break;
                    }
                case TouchPhase.Ended:
                    {
                        lastTouchTime = Time.time;
                        Debug.LogError(lastTouchTime - touchStartTime);


                        if (lastTouchTime - touchStartTime < 0.15f && (!IsPointerOverUIObject() || ignoreUI))
                        {
                            Debug.LogError("isTouch");

                            FarmUI.GetInstance().harvestDragCanvas.transform.GetChild(0).gameObject.SetActive(false);

                            for (int i = 0; i < FarmUI.GetInstance().selectCanvas.transform.childCount; i++)
                            {
                                Destroy(FarmUI.GetInstance().selectCanvas.transform.GetChild(i).gameObject);
                            }

                            for (int i = 0; i < FarmUI.GetInstance().tableSelectCanvas.transform.childCount; i++)
                                FarmUI.GetInstance().tableSelectCanvas.transform.GetChild(i).gameObject.SetActive(false);
                        }

                        if (Time.time - touchStartTime <= maxDurationForTap
                            && Vector2.Distance(touch.position, touchStartPosition) <= maxDistanceForTap
                            && isTouching)
                        {
                            OnClick(touch.position);
                        }

                        if (onEndTouch != null) onEndTouch(touch.position);

                        isTouching = false;
                        break;
                    }
                case TouchPhase.Stationary:
                case TouchPhase.Canceled:
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
            {
                if (onEndTouch != null) onEndTouch(touchLastPosition);
                isTouching = false;
            }

        }
    }

    private void OnClick(Vector2 position)
    {
        if(onTap != null && (ignoreUI || !IsPointerOverUIObject()))
        {
            onTap(position);
        }
    }

    private void OnSwipe(Vector2 deltaPosition)
    {
        if (onSwipe != null)
            onSwipe(deltaPosition);

        mainCamera.transform.position -= (Camera.main.ScreenToWorldPoint(deltaPosition) - Camera.main.ScreenToWorldPoint(Vector2.zero));
    }

    private void OnPinch(Vector2 center, float oldDistance, float newDistance, Vector3 touchDelta)
    {
        if(onPinch != null)
        {
            onPinch(oldDistance, newDistance);
        }

        if (mainCamera.m_Lens.Orthographic)
        {
            var currentPinchPosition = Camera.main.ScreenToWorldPoint(center);

            mainCamera.m_Lens.OrthographicSize = Mathf.Max(zoomMin, mainCamera.m_Lens.OrthographicSize * oldDistance / newDistance);


            //float tx = Mathf.Clamp(0.01f * oldDistance / newDistance, 0.01f, 0.02f);
            //float tx = Mathf.Max(0.01f * oldDistance / newDistance, 0.02f);
            float tx = Mathf.Max(0.3f, selectObject[0].transform.GetChild(1).localScale.x * oldDistance / newDistance);
            print(0.1f * oldDistance / newDistance);

            print("Result : " + oldDistance / newDistance);
            print("OldDistance : " + oldDistance);
            print("newDistance : " + newDistance);
            print("Camera OrthographicSize : " + mainCamera.m_Lens.OrthographicSize * oldDistance / newDistance);

            for(int i = 0; i < BackendServerManager.GetInstance().field.Count; i++)
            {
                //selectObject[i].transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(tx, tx, tx);
                //selectObject[i].transform.GetChild(1).GetComponent<RectTransform>().localScale = new Vector3(tx, tx, tx);
            }

            var newPinchPosition = Camera.main.ScreenToWorldPoint(center);

            mainCamera.transform.position -= newPinchPosition - currentPinchPosition;
        }
        else
        {
            //mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView * oldDistance / newDistance, 0.1f, 179.9f);
        }
    }

    private void CameraInBounds()
    {
        if (mainCamera.m_Lens.Orthographic)
        {

            //var tx = Mathf.Min(selectObject[0].transform.GetChild(1).localScale.x, 0.03f);

            for (int i = 0; i < BackendServerManager.GetInstance().field.Count; i++)
            {
                //selectObject[i].transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(tx, tx, tx);
                //selectObject[i].transform.GetChild(1).GetComponent<RectTransform>().localScale = new Vector3(tx, tx, tx);
            }

            mainCamera.m_Lens.OrthographicSize = Mathf.Min(mainCamera.m_Lens.OrthographicSize, ((boundMaxY - boundMinY) / 2) - 0.001f);
            mainCamera.m_Lens.OrthographicSize = Mathf.Min(mainCamera.m_Lens.OrthographicSize, (Screen.height * (boundMaxX - boundMinX) / (2 * Screen.width)) - 0.001f);

            Vector2 margin = Camera.main.ScreenToWorldPoint((Vector2.up * Screen.height / 2) + (Vector2.right * Screen.width / 2)) - Camera.main.ScreenToWorldPoint(Vector2.zero);

            float marginX = margin.x;
            float marginY = margin.y;

            float camMaxX = boundMaxX - marginX;
            float camMaxY = boundMaxY - marginY;
            float camMinX = boundMinX + marginX;
            float camMinY = boundMinY + marginY;

            float camX = Mathf.Clamp(mainCamera.transform.position.x, camMinX, camMaxX);
            float camY = Mathf.Clamp(mainCamera.transform.position.y, camMinY, camMaxY);

            mainCamera.transform.position = new Vector3(camX, camY, mainCamera.transform.position.z);
        }
    }

    public bool IsPointerOverUIObject()
    {
        if (EventSystem.current == null) return false;
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        //if(eventDataCurrentPosition.selectedObject.tag != "Field")
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
