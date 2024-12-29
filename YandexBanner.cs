using UnityEngine;
using YandexMobileAds;
using YandexMobileAds.Base;
using System;


public class YandexMobileAdsStickyBannerDemoScript : MonoBehaviour
{
    private Banner banner;

    private int GetScreenWidthDp()
    {
        int screenWidth = (int)Screen.safeArea.width;
        return ScreenUtils.ConvertPixelsToDp(screenWidth);
    }

    private void Awake()
    {
        RequestStickyBanner();
    }

    private void RequestStickyBanner()
    {
        //string adUnitId = "demo-banner-yandex"; // замените на "R-M-XXXXXX-Y"
        string adUnitId = "R-M-12776504-1"; // замените на "R-M-XXXXXX-Y"
        BannerAdSize bannerMaxSize = BannerAdSize.StickySize(GetScreenWidthDp()/4);
        banner = new Banner(adUnitId, bannerMaxSize, AdPosition.BottomRight);

        AdRequest request = new AdRequest.Builder().Build();
        banner.LoadAd(request);

        // Called when rewarded ad was loaded
        banner.OnAdLoaded += HandleAdLoaded;

        //  Called when there was an error loading the ad
        banner.OnAdFailedToLoad += HandleAdFailedToLoad;

        //Called when the app went inactive because the user tapped an ad and is about to switch to a different app (for example, a browser).
        banner.OnLeftApplication += HandleLeftApplication;

        //  Called when the user returns to the app after a tap
        banner.OnReturnedToApplication += HandleReturnedToApplication;

        //  Called when the user clicks the ad
        banner.OnAdClicked += HandleAdClicked;

        //  Called when an impression is registered
        banner.OnImpression += HandleImpression;
    }

    private void HandleAdLoaded(object sender, EventArgs args)
    {
        Debug.Log("AdLoaded event received");
        banner.Show();
    }

    private void HandleAdFailedToLoad(object sender, AdFailureEventArgs args)
    {
        Debug.Log($"AdFailedToLoad event received with message: {args.Message}");
        // We strongly advise against loading a new ad using this method
    }

    private void HandleLeftApplication(object sender, EventArgs args)
    {
        Debug.Log("LeftApplication event received");
    }

    private void HandleReturnedToApplication(object sender, EventArgs args)
    {
        Debug.Log("ReturnedToApplication event received");
    }

    private void HandleAdClicked(object sender, EventArgs args)
    {
        Debug.Log("AdClicked event received");
    }

    private void HandleImpression(object sender, ImpressionData impressionData)
    {
        var data = impressionData == null ? "null" : impressionData.rawData;
        Debug.Log($"HandleImpression event received with data: {data}");
    }




}
