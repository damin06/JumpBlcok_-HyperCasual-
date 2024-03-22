using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using System.Security.Cryptography;

public enum AdType 
{
    None,
    Banner,
    Interstitial,

}

[Serializable]
public struct AdUnitId
{
    [SerializeField] private string ID;
    public string _id => ID;

    [SerializeField] private string TestID;
    public string _testId => TestID;
}

public class AdmobManager : MonoBehaviour
{
    public static AdmobManager Instance;

    [SerializeField]
    private SerializableDict<AdType, AdUnitId> m_adUnitId = new SerializableDict<AdType, AdUnitId>();

    private BannerView m_bannerView;
    private InterstitialAd m_interstitialAd;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        MobileAds.Initialize(initStatus => { });

        LoadBannerAd();
        LoadInterstitialAd();
    }

#region Banner

    public void LoadBannerAd()
    {
        Debug.Log("Creating banner view");

        // If we already have a banner, destroy the old one.
        if (m_bannerView != null)
        {
            DestroyBannerAd();
        }

        string _id;

        if(Application.platform == RuntimePlatform.Android)
        {
            _id = m_adUnitId.GetValue(AdType.Banner)._id;
        }
        else
        {
            _id = m_adUnitId.GetValue(AdType.Banner)._testId;
        }

        AdSize _size = AdSize.GetPortraitAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

        // Create a 320x50 banner at top of the screen
        m_bannerView = new BannerView(_id, _size, AdPosition.Bottom);
    }

    public void ShowBannerAd()
    {
        // create an instance of a banner view first.
        if (m_bannerView == null)
        {
            LoadBannerAd();
        }

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        Debug.Log("Loading banner ad.");
        m_bannerView.LoadAd(adRequest);
    }

    public void DestroyBannerAd()
    {
        if (m_bannerView != null)
        {
            Debug.Log("Destroying banner view.");
            m_bannerView.Destroy();
            m_bannerView = null;
        }
    }

    #endregion

    #region Interstitial

    public void LoadInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (m_interstitialAd != null)
        {
            m_interstitialAd.Destroy();
            m_interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");

        // create our request used to load the ad.

        string _id;

        if (Application.platform == RuntimePlatform.Android)
        {
            _id = m_adUnitId.GetValue(AdType.Interstitial)._id;
        }
        else
        {
            _id = m_adUnitId.GetValue(AdType.Interstitial)._testId;
        }

        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(_id, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                m_interstitialAd = ad;
            });
    }


    public void ShowInterstitialAd()
    {
        LoadInterstitialAd();

        if (m_interstitialAd != null && m_interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad.");
            m_interstitialAd.Show();
        }
        else
        {
            Debug.LogError("Interstitial ad is not ready yet.");
        }
    }

    #endregion
}
