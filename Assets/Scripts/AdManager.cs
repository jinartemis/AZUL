using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.UI;
using System;

public class AdManager : MonoBehaviour
{
    public static AdManager instance;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

#if !UNITY_EDITOR
        //Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus => { });
#endif
    }

    private BannerView bannerView;


    //課金有無判定
    bool purchased;


    public void Start()
    {
#if UNITY_ANDROID
        string appId = "ca-app-pub-3940256099942544~3347511713";            //Debug////////////////////////
#elif UNITY_IPHONE
        string appId =  "ca-app-pub-3940256099942544~1458002511";           //Debug////////////////////////
#else
            string appId = "unexpected_platform";
#endif

#if !UNITY_EDITOR
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(appId);
#endif

    }

    public void RequestBanner(bool show)
    {
        Debug.Log("バナーリクエスト");

#if UNITY_EDITOR
        return;
#endif

        if (purchased == true)
        {
            return;
        }

        //広告を消す（セーブ、ロードシーンからでる）
        if(show == false && this.bannerView != null)
        {
            this.bannerView.Destroy();
            return;
        }

        Debug.Log("バナー設定");

#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/6300978111";     //Debug////////////////////////////////////////////////
#elif UNITY_IPHONE
        string adUnitId =  "ca-app-pub-3940256099942544/2934735716";    //Debug////////////////////////////////////////////////
#else
            string adUnitId = "unexpected_platform";
#endif

        // Create a 320x50 banner at the top of the screen.
        this.bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

        this.bannerView.OnAdLoaded += HandleOnAdLoaded;
        this.bannerView.OnAdClosed += HandleOnAdClosed;
        this.bannerView.OnAdFailedToLoad += HandleOnFailed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the banner with the request.
        this.bannerView.LoadAd(request);
        this.bannerView.Show();
        
        Debug.Log("req終了" + this.bannerView);

    }


    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        Debug.Log("成功");
        this.bannerView.Show();
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        Debug.Log("閉じた");
    }

    public void HandleOnFailed(object sender, EventArgs args)
    {
        Debug.Log("失敗");
    }
}
