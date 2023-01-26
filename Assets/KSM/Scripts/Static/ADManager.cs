using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADManager : MonoBehaviour
{
    public void Initialize()
    {
        IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
        IronSourceEvents.onRewardedVideoAdClickedEvent += RewardedVideoAdClickedEvent;
        IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent; 
        IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
        IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
        IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;
        //IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent; 
        //IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
        
        IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAdReadyEvent;
        IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;        
        IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent; 
        IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailedEvent; 
        IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;
        IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
        IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;
        
        IronSource.Agent.shouldTrackNetworkState (true);
    }

    public delegate void RewardAD();

    public delegate void InterstitialAD();

    public void ShowRewardAD(RewardAD rewardAD = null)
    {
      IronSource.Agent.showRewardedVideo();

      IronSourceEvents.onRewardedVideoAdRewardedEvent += (sender) =>
      {
        rewardAD?.Invoke();
      };

      IronSourceEvents.onRewardedVideoAdShowFailedEvent += error =>
      {
        Debug.LogError(error);
        StaticManager.UI.AlertUI.OpenUI(error.ToString());
      };
    }

    public void ShowAD(InterstitialAD rewardAD = null)
    {
      IronSource.Agent.loadInterstitial();

      IronSourceEvents.onInterstitialAdClosedEvent += () =>
      {
        rewardAD?.Invoke();
      };
    }
    
  void RewardedVideoAdOpenedEvent() {
  }  

  void RewardedVideoAdClosedEvent() {
  }

  void RewardedVideoAvailabilityChangedEvent(bool available) {
    bool rewardedVideoAvailability = available;
  }

  void RewardedVideoAdRewardedEvent(IronSourcePlacement placement) {
      Debug.LogError($"{placement.getPlacementName()}, {placement.getRewardName()}, {placement.getRewardAmount()}");
  }

  void RewardedVideoAdShowFailedEvent (IronSourceError error){
  }

  // ----------------------------------------------------------------------------------------
  // Note: the events below are not available for all supported rewarded video ad networks. 
  // Check which events are available per ad network you choose to include in your build. 
  // We recommend only using events which register to ALL ad networks you include in your build. 
  // ----------------------------------------------------------------------------------------

  //Invoked when the video ad starts playing. 
  void RewardedVideoAdStartedEvent() { 
  } 
  //Invoked when the video ad finishes playing. 
  void RewardedVideoAdEndedEvent() { 
  }
  //Invoked when the video ad is clicked. 
  void RewardedVideoAdClickedEvent(IronSourcePlacement placement) { 
  }



  
  
  // Invoked when the initialization process has failed.
// @param description - string - contains information about the failure.
  void InterstitialAdLoadFailedEvent (IronSourceError error) {
  }
// Invoked when the ad fails to show.
// @param description - string - contains information about the failure.
  void InterstitialAdShowFailedEvent(IronSourceError error) {
  }
// Invoked when end user clicked on the interstitial ad
  void InterstitialAdClickedEvent () {
  }
// Invoked when the interstitial ad closed and the user goes back to the application screen.
  void InterstitialAdClosedEvent () {
  }
// Invoked when the Interstitial is Ready to shown after load function is called
  void InterstitialAdReadyEvent() {
  }
// Invoked when the Interstitial Ad Unit has opened
  void InterstitialAdOpenedEvent() {
  }
// Invoked right before the Interstitial screen is about to open.
// NOTE - This event is available only for some of the networks. 
// You should not treat this event as an interstitial impression, but rather use InterstitialAdOpenedEvent
  void InterstitialAdShowSucceededEvent() {
  }
}
