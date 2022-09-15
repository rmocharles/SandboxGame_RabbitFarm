using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Battlehub.Dispatcher;
using System.Linq;

public class FriendList
{
    public string nickname;
    public string rowIndate;
    public string ownerIndate;
    public string lastLogin;
    public bool isConnect;

    public int gold;
    public int diamond;
    public string representImage;
}

public partial class FarmUI : MonoBehaviour
{
    [Header("< Friend UI >")]
    public GameObject friendPanel;
    public GameObject addFriendPanel;

    public GameObject friendListButton;
    public GameObject friendRequestListButton;

    public GameObject friendListObject;
    public GameObject recommendFriendListObject;

    public GameObject friendPrefab;
    public GameObject friendRequestPrefab;
    public GameObject friendRecommendPrefab;

    public GameObject friendProfilePanel;
    
    public TMP_InputField insertNicknameField;

    List<string> nowFriendList = new List<string>();

    public Dictionary<string, IsOnline> friendsInfo;

    [HideInInspector]
    public bool isRequestAlarm = false;
    [HideInInspector]
    public bool isAcceptAlarm = false;

    #region 친구 팝업창 켜기
    public void ActiveFriendList()
    {
        DestroySelectPrefab();

        FarmUI.GetInstance().ButtonClick(0);

        friendPanel.SetActive(true);

        ChangeMode(false);
    }
    #endregion

    #region 친구 목록 / 요청 목록 모드 변경
    public void ChangeMode(bool isRequest)
    {
        friendListButton.transform.GetChild(0).gameObject.SetActive(!isRequest);
        friendListButton.transform.GetChild(1).gameObject.SetActive(isRequest);

        friendRequestListButton.transform.GetChild(0).gameObject.SetActive(isRequest);
        friendRequestListButton.transform.GetChild(1).gameObject.SetActive(!isRequest);

        if (isRequest)
        {
            SetReceivedRequestFriendList();
        }
        else
        {
            SetFriendList();
        }
    }
    #endregion

    #region 친구 목록
    public void SetFriendList()
    {
        SetLoading();
        ClearFriendList();

        friendsInfo = new Dictionary<string, IsOnline>();

        BackendServerManager.GetInstance().GetFriendList((bool result, List<FriendList> friendList) =>
        {
            Dispatcher.Current.BeginInvoke(() =>
            {
                SetLoading(false);

                if (friendList == null)
                {
                    Debug.LogError("친구가 존재하지 않습니다.");
                    return;
                }

                if (friendList.Count <= 0)
                {
                    Debug.LogError("친구 수가 0 입니다.");
                    return;
                }

                nowFriendList = new List<string>();

                foreach (var tmp in friendList)
                {
                    nowFriendList.Add(tmp.nickname);
                    InsertFriendPrefab(tmp, 0);
                }
            });
        });
    }
    #endregion

    #region 친구 목록
    public void SetReceivedRequestFriendList()
    {
        SetLoading();
        ClearFriendList();
        BackendServerManager.GetInstance().GetReceivedRequestFriendList((bool result, List<FriendList> friendList) =>
        {
            Dispatcher.Current.BeginInvoke(() =>
            {
                SetLoading(false);

                if (friendList == null)
                {
                    Debug.LogError("친구가 존재하지 않습니다.");
                    return;
                }

                if (friendList.Count <= 0)
                {
                    Debug.LogError("친구 수가 0 입니다.");
                    return;
                }

                foreach (var tmp in friendList)
                {
                    InsertFriendPrefab(tmp, 1);
                }

            });
        });
    }
    #endregion

    #region 친구 요청 목록
    public void SetRecommendFriendList()
    {
        //친구 추가 팝업창 띄우I
        addFriendPanel.SetActive(true);

        SetLoading();
        ClearFriendList(true);

        BackendServerManager.GetInstance().GetRecommendFriendList((bool result, List<FriendList> friendList) =>
        {
            Dispatcher.Current.BeginInvoke(() =>
            {
                SetLoading(false);

                if (friendList == null)
                {
                    Debug.LogError("친구가 존재하지 않습니다.");
                    return;
                }

                if (friendList.Count <= 0)
                {
                    Debug.LogError("친구 수가 0 입니다.");
                    return;
                }

                var result = friendList.Where(i => !nowFriendList.Any(j => j == i.nickname)).ToList();

                foreach (var tmp in result)
                {
                    print(tmp.nickname);
                    InsertFriendPrefab(tmp, 2);
                }
            });
        });
    }
    #endregion

    public void RequestFriend()
    {
        if (loadingObject.activeSelf || errorObject.activeSelf) return;

        if (insertNicknameField.text.Equals(string.Empty)) return;

        SetLoading();

        BackendServerManager.GetInstance().RequestFirend(insertNicknameField.text, (bool result, string error) =>
        {
            Dispatcher.Current.BeginInvoke(() =>
            {
                SetLoading(false);

                if (!result)
                {
                    SetErrorObject(error);
                    return;
                }

                SetErrorObject("친구 요청 성공");
                Debug.LogError("친구 요청 성공");

            });
        });
    }

    private void ClearFriendList(bool isRecommend = false)
    {
        if (isRecommend)
        {
            while (recommendFriendListObject.transform.childCount > 0)
            {
                DestroyImmediate(recommendFriendListObject.transform.GetChild(0).gameObject);
            }
        }
        else
        {
            while (friendListObject.transform.childCount > 0)
            {
                DestroyImmediate(friendListObject.transform.GetChild(0).gameObject);
            }
        }
    }

    public void OpenFriendProfile(string nickname, string representImage)
    {
        friendProfilePanel.SetActive(true);
        friendProfilePanel.GetComponentsInChildren<Image>()[2].sprite = characterImage[int.Parse(representImage)];
        friendProfilePanel.GetComponentsInChildren<TextMeshProUGUI>()[4].text = nickname;
    }

    public void InsertFriendPrefab(FriendList friendList, int mode)
    {
        switch (mode)
        {
            case 0: //친구 리스트
                GameObject friend = GameObject.Instantiate(friendPrefab, Vector3.zero, Quaternion.identity, friendListObject.transform);

                friendsInfo.Add(friendList.ownerIndate, friend.GetComponent<IsOnline>());

                BackEnd.Backend.Notification.UserIsConnectByIndate(friendList.ownerIndate);

                //friend.GetComponentsInChildren<Text>()[0].text = friendList.isConnect ? "온라인" : "마지막 접속 : " + friendList.lastLogin;
                friend.GetComponentsInChildren<Image>()[1].sprite = characterImage[int.Parse(friendList.representImage)];

                //친구 프로필 확인
                friend.GetComponentsInChildren<Button>()[0].onClick.AddListener(() =>
                {
                    FarmUI.GetInstance().ButtonClick(0);
                    OpenFriendProfile(friendList.nickname, friendList.representImage);
                });

                friend.GetComponentsInChildren<Text>()[1].text = friendList.nickname;

                friend.GetComponentsInChildren<Button>()[2].onClick.AddListener(() =>
                {
                    SetLoading();
                    // 친구 삭제
                    BackendServerManager.GetInstance().BreakFriend(friendList.ownerIndate, (bool isSuccess, string error) =>
                    {
                        Dispatcher.Current.BeginInvoke(() =>
                        {
                            SetLoading(false);

                            if (!isSuccess)
                            {
                                //SetErrorObject(error);
                                Debug.LogError(error);
                                return;
                            }


                            for (int i = 0; i < nowFriendList.Count; i++)
                            {
                                if(nowFriendList[i] == friendList.nickname)
                                {
                                    nowFriendList.RemoveAt(i);
                                }
                            }
                        });
                    });

                    FarmUI.GetInstance().ButtonClick(0);


                    friend.SetActive(false);
                    GameObject.Destroy(friend, 0.1f);
                });
                break;

            case 1: //요청 리스트
                GameObject friend2 = GameObject.Instantiate(friendRequestPrefab, Vector3.zero, Quaternion.identity, friendListObject.transform);
                friend2.GetComponentsInChildren<Image>()[1].sprite = characterImage[int.Parse(friendList.representImage)];
                friend2.GetComponentsInChildren<Text>()[1].text = friendList.nickname;

                friend2.GetComponentsInChildren<Button>()[1].onClick.AddListener(() =>
                {
                    SetLoading();
                    // ok버튼 상호작용
                    BackendServerManager.GetInstance().AcceptFriend(friendList.ownerIndate, (bool isSuccess, string error) =>
                    {
                        Dispatcher.Current.BeginInvoke(() =>
                        {
                            SetLoading(false);

                            if (!isSuccess)
                            {
                                SetErrorObject(error);
                                return;
                            }
                        });
                    });

                    FarmUI.GetInstance().ButtonClick(0);

                    friend2.SetActive(false);
                    GameObject.Destroy(friend2, 0.1f);
                });

                friend2.GetComponentsInChildren<Button>()[2].onClick.AddListener(() =>
                {
                    SetLoading();
                    // cancel 버튼 상호작용
                    BackendServerManager.GetInstance().RejectFriend(friendList.ownerIndate, (bool isSuccess, string error) =>
                    {
                        Dispatcher.Current.BeginInvoke(() =>
                        {
                            SetLoading(false);

                            if (!isSuccess)
                            {
                                SetErrorObject(error);
                                return;
                            }
                        });
                    });

                    FarmUI.GetInstance().ButtonClick(0);

                    friend2.SetActive(false);
                    GameObject.Destroy(friend2, 0.1f);
                });
                break;

            case 2: //추천 리스트
                GameObject friend3 = GameObject.Instantiate(friendRecommendPrefab, Vector3.zero, Quaternion.identity, recommendFriendListObject.transform);
                friend3.GetComponentsInChildren<Image>()[1].sprite = characterImage[int.Parse(friendList.representImage)];
                friend3.GetComponentsInChildren<Text>()[1].text = friendList.nickname;

                friend3.GetComponentsInChildren<Button>()[1].onClick.AddListener(() =>
                {
                    SetLoading();
                    // 친구 요청
                    BackendServerManager.GetInstance().RequestFirend(friendList.nickname, (bool isSuccess, string error) =>
                    {
                        Dispatcher.Current.BeginInvoke(() =>
                        {
                            SetLoading(false);

                            if (!isSuccess)
                            {
                                SetErrorObject(error);
                                Debug.LogError(error);
                                return;
                            }

                            //SetErrorObject("친구 요청 성공!");
                        });
                    });

                    FarmUI.GetInstance().ButtonClick(0);

                    friend3.SetActive(false);
                    GameObject.Destroy(friend3, 0.1f);
                });
                break;
        }
    }
}
