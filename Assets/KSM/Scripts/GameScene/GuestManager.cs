using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * GuestManager
 *
 * 1. 일정확률로 손님이 마트에 진입 시도
 */
public class GuestManager : MonoBehaviour
{
    public Dictionary<int, GuestAI> guests = new Dictionary<int, GuestAI>();    //<번호>
    public List<GuestAI> waitGuests = new List<GuestAI>();

    public Transform startPoint;
    public Transform enterPoint;
    public Transform noEnterPoint;
    public Transform exitPoint;
    public Transform workPoint;

    public Transform[] wantPoint;
    public Transform[] waitPoint;

    public GameObject guestPool;

    [Header("손님 출현 속도")] public float spawnGuestTime = 1f;
    [Header("손님 마트에 들어갈 확률")] public int enterPercent = 70;
    [Header("마트에 최대 들어갈 수 있는 인원")] public int maxGuest = 7;

    public int nowGuest = 0;

    public void Initialize()
    {
        if(StaticManager.Backend.backendGameData.UserData.Level >= 5 && StaticManager.Backend.backendGameData.UserData.Tutorial >= 5)
            StartCoroutine(SpawnGuest());
    }

    private IEnumerator SpawnGuest()
    {
        if (StaticManager.Backend.backendGameData.UserData.Level <= 7)
            spawnGuestTime = Random.Range(5, 7);
        else if (StaticManager.Backend.backendGameData.UserData.Level <= 10)
            spawnGuestTime = Random.Range(4, 6);
        else if (StaticManager.Backend.backendGameData.UserData.Level <= 15)
            spawnGuestTime = Random.Range(3, 5);
        else if (StaticManager.Backend.backendGameData.UserData.Level <= 20)
            spawnGuestTime = Random.Range(2, 4);
        else
            spawnGuestTime = Random.Range(2f, 4f);

        yield return new WaitForSeconds(spawnGuestTime);

        GameObject guestPlayer = Instantiate(Resources.Load<GameObject>("Prefabs/GameScene/Guest/Guest_" + Random.Range(0, 6)), guestPool.transform);
        //guests.Add(guests.Count, guestPlayer.GetComponent<GuestAI>());

        //손님이 마트에 들어갈 확률
        if (Random.Range(1, 101) <= enterPercent)
        {
            guestPlayer.GetComponent<GuestAI>().ChangeState(GuestAI.State.Enter);
        }
        else
        {
            guestPlayer.GetComponent<GuestAI>().ChangeState(GuestAI.State.No_Enter);
        }
        StartCoroutine(SpawnGuest());
    }
}
