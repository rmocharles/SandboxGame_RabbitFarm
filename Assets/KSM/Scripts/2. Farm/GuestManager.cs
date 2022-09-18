using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuestManager : MonoBehaviour
{
    private static GuestManager instance;

    public static GuestManager GetInstance()
    {
        if (instance == null)
            return null;
        return instance;
    }

    [Header("< Guest Spawn Point >")]
    public Transform startPoint;

    [Header("< Guest Enter Point >")]
    public Transform enterPoint;

    [Header("< Guest Exit Point >")]
    public Transform[] exitPoint;

    [Header("< Table Point >")]
    public Transform[] tablePoint;

    [Header("< Counter Point >")]
    public Transform[] counterPoint;

    [Header("< Guest Manager >")]
    public int maxGuestCount = 3;
    public int nowGuestCount = 0;

    public List<GuestAI> guests = new List<GuestAI>();

    void Awake()
    {
        if (!instance) instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
