using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class ADManager : MonoBehaviour
{

//    public enum FLATFORM
//    {
//        NONE,
//        ANDROID,
//        IOS,
//        CUSTOM
//    }

//    public FLATFORM _flatform = FLATFORM.CUSTOM;

    public enum ADTYPE
    {
        NONE = 0,
        BANNER,
        INTERSTITIAL,
        VIDEO,
    }

//    public ADTYPE adType = ADTYPE.NONE;
	public List<ADTYPE> listADType = new List<ADTYPE> ();
	public AdPosition adPosition = AdPosition.Top;
    public bool isRunStart = false;

	string APP_ID = "ca-app-pub-3027258358711939~5986891218";
	string BANNER_ID = "ca-app-pub-3027258358711939/9810107223";
	string INTERSTITIAL_ID = "ca-app-pub-3027258358711939/8305453866";
	string VIDEO_ID = "";

	public BannerView bannerView = null;
	public InterstitialAd interstitialAd = null;
	public RewardBasedVideoAd rewardAd = null;

    // Use this for initializatin
    void Start()
    {
		init ();
		if (isRunStart) {
			requestAD ();
		}
    }

//    // Update is called once per frame
//    void Update()
//    {
//
//    }

    void init()
	{
		MonoBehaviour.print("------------------->AD Init AD");
		MonoBehaviour.print("------------------->AD Init BANNER_ID:  " + BANNER_ID);
		if (APP_ID != "")
		{
			MobileAds.Initialize(APP_ID);
		}
		for (int i = 0; i < listADType.Count; i++) {
			switch (listADType[i]) {
			case ADTYPE.NONE:
				break;
			case ADTYPE.BANNER:
				{
					bannerView = new BannerView (BANNER_ID, AdSize.Banner, adPosition);
					// Called when an ad request has successfully loaded.
					bannerView.OnAdLoaded += HandleOnAdLoaded;
					// Called when an ad request failed to load.
					bannerView.OnAdFailedToLoad += HandleOnAdFailedToLoad;
					// Called when an ad is clicked.
					bannerView.OnAdOpening += HandleOnAdOpened;
					// Called when the user returned from the app after an ad click.
//					bannerView.OnAdClosed += HandleOnAdClosed;
					// Called when the ad click caused the user to leave the application.
					bannerView.OnAdLeavingApplication += HandleOnAdLeavingApplication;
					break;
				}
			case ADTYPE.INTERSTITIAL:
				{
					interstitialAd = new InterstitialAd (INTERSTITIAL_ID);

					// Called when an ad request has successfully loaded.
					interstitialAd.OnAdLoaded += HandleOnAdLoaded;
					// Called when an ad request failed to load.
					interstitialAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
					// Called when an ad is shown.
					interstitialAd.OnAdOpening += HandleOnAdOpened;
					// Called when the ad is closed.
					interstitialAd.OnAdClosed += HandleOnAdClosed;
					// Called when the ad click caused the user to leave the application.
					interstitialAd.OnAdLeavingApplication += HandleOnAdLeavingApplication;
					break;
				}
			case ADTYPE.VIDEO:
				{
					rewardAd = RewardBasedVideoAd.Instance;

					// Called when an ad request has successfully loaded.
					rewardAd.OnAdLoaded += HandleRewardBasedVideoLoaded;
					// Called when an ad request failed to load.
					rewardAd.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
					// Called when an ad is shown.
					rewardAd.OnAdOpening += HandleRewardBasedVideoOpened;
					// Called when the ad starts to play.
					rewardAd.OnAdStarted += HandleRewardBasedVideoStarted;
					// Called when the user should be rewarded for watching a video.
					rewardAd.OnAdRewarded += HandleRewardBasedVideoRewarded;
					// Called when the ad is closed.
					rewardAd.OnAdClosed += HandleRewardBasedVideoClosed;
					// Called when the ad click caused the user to leave the application.
					rewardAd.OnAdLeavingApplication += HandleRewardBasedVideoLeftApplication;

					break;
				}
			}
		}
    }

	public void requestAD(){
		// Create an empty ad request.
		for (int i = 0; i < listADType.Count; i++) {
			AdRequest request = new AdRequest.Builder ().Build ();
			switch (listADType [i]) {
			case ADTYPE.NONE:
				break;
			case ADTYPE.BANNER:
			// Load the banner with the request.
				bannerView.LoadAd (request);
				break;
			case ADTYPE.INTERSTITIAL:
			// Load the interstitial with the request.
				interstitialAd.LoadAd (request);
				break;
			case ADTYPE.VIDEO:
			// Load the rewarded video ad with the request.
				this.rewardAd.LoadAd (request, APP_ID);
				break;
			default:
				throw new ArgumentOutOfRangeException ();
			}
		}
	}

    public void showBanner()
    {
		if (bannerView != null) {
			bannerView.Show ();
		}
    }

    public void hideBanner()
    {
		if (bannerView != null) {
			bannerView.Hide ();
		}
    }

    public void showInterstital()
    {
		if (interstitialAd.IsLoaded()) {
			interstitialAd.Show();
		}
    }

    public void showVideo()
    {
		if (rewardAd.IsLoaded()) {
			rewardAd.Show();
		}
    }

	//Banner Handle
	public void HandleOnAdLoaded(object sender, EventArgs args)
	{
		MonoBehaviour.print("------------------->AD HandleAdLoaded event received");
//		switch(adType){
//		case ADTYPE.BANNER:
//			showBanner ();
//			break;
//		case ADTYPE.INTERSTITIAL:
//			showInterstital ();
//			break;
//
//		}
	}

	public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		MonoBehaviour.print("------------------->AD HandleFailedToReceiveAd event received with message: " + args.Message);
	}

	public void HandleOnAdOpened(object sender, EventArgs args)
	{
		MonoBehaviour.print("------------------->AD HandleAdOpened event received");
	}

	public void HandleOnAdClosed(object sender, EventArgs args)
	{
		MonoBehaviour.print("------------------->AD HandleAdClosed event received");
	}

	public void HandleOnAdLeavingApplication(object sender, EventArgs args)
	{
		MonoBehaviour.print("------------------->AD HandleAdLeavingApplication event received");
	}



	//Video Handle
	public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
	{
		MonoBehaviour.print("------------------->AD HandleRewardBasedVideoLoaded event received");
	}

	public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		MonoBehaviour.print(
			"------------------->AD HandleRewardBasedVideoFailedToLoad event received with message: "
			+ args.Message);
	}

	public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
	{
		MonoBehaviour.print("------------------->AD HandleRewardBasedVideoOpened event received");
	}

	public void HandleRewardBasedVideoStarted(object sender, EventArgs args)
	{
		MonoBehaviour.print("------------------->AD HandleRewardBasedVideoStarted event received");
	}

	public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
	{
		MonoBehaviour.print("------------------->AD HandleRewardBasedVideoClosed event received");
	}

	public void HandleRewardBasedVideoRewarded(object sender, Reward args)
	{
		string type = args.Type;
		double amount = args.Amount;
		MonoBehaviour.print(
			"------------------->AD HandleRewardBasedVideoRewarded event received for "
			+ amount.ToString() + " " + type);
	}

	public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
	{
		MonoBehaviour.print("------------------->AD HandleRewardBasedVideoLeftApplication event received");
	}
	//END Video Handle
}
