using UnityEngine.EventSystems;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Spine.Unity;

public class ADManager : MonoBehaviour
{
    private RewardedAd rewardedAd;

    private static ADManager instance;

    [Header("[Rewarded AD ]")]
    public string rewardedAdUnitID = "ca-app-pub-1478803844863001/8862420851";

    public Action<bool> endRewardAction;
    public Action loadFail;

    public bool endVideo = false;
    public bool isReward = false;

    public static ADManager GetInstance()
    {
        if (instance == null)
            return null;
        return instance;
    }

    void Awake()
    {
        if (!instance) instance = this;
    }
    void Start()
    {
        MobileAds.SetiOSAppPauseOnBackground(true);

        CreateAndLoadRewardedAd();
    }

    void Update()
    {
        if(endVideo)
        {
            endVideo = false;

            endRewardAction?.Invoke(isReward);
            endRewardAction = null;
            isReward = false;
        }
    }

    private void CreateAndLoadRewardedAd()
    {
        if(rewardedAd != null)
        {
            rewardedAd.Destroy();
        }
        this.rewardedAd = new RewardedAd(rewardedAdUnitID);

        rewardedAd.OnUserEarnedReward += (sender, args) =>
        {
            isReward = true;
        };

        rewardedAd.OnAdClosed += (sender, args) =>
        {

        };
        rewardedAd.OnAdFailedToLoad += (sender, args) =>
        {
            loadFail?.Invoke();
        };

        this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;

        //this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;

        this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;

        //this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;

        //this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;

        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        AdRequest request = new AdRequest.Builder().Build();
        this.rewardedAd.LoadAd(request);
    }



    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        print("HandleRewardedLoaded event received.");
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
        print("HandleRewardedAdFailedToLoad event received with message : " + args.Message);
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        print("HandleRewardedAdOpening event received");
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        print("HandleRewardedAdFailedToShow event received with message: " + args.Message);
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        this.CreateAndLoadRewardedAd();
        endVideo = true;
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
    }

    public void ShowRewardedAD(Action<bool> endReward = null, Action loadFail = null)
    {
        this.endRewardAction = endReward;

        if (this.rewardedAd.IsLoaded())
        {
            this.rewardedAd.Show();
        }
        else
        {
            this.loadFail = loadFail;
        }
    }
}
