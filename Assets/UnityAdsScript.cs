using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Monetization;
using ShowResult = UnityEngine.Monetization.ShowResult;

public class UnityAdsScript : MonoBehaviour
{
    string gameId = "3058406";
    bool testMode = true;
    public string placementId = "video";
    public string placementIdRewarded = "rewardedVideo";
    public string bannerPlacement = "banner";

    void Start () {
        Advertisement.Initialize (gameId, testMode);
        Monetization.Initialize (gameId, testMode);
        
        Debug.Log("Unity Ads initialized: " + Monetization.isInitialized);
        Debug.Log("Unity Ads is supported: " + Monetization.isSupported);
        //Debug.Log("Unity Ads test mode enabled: " + Monetization.);
        
        //ShowAd();
        //ShowAdRewarded();
        ShowBanner();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void ShowAd () {
        StartCoroutine (ShowAdWhenReady ());
    }

    private IEnumerator ShowAdWhenReady () {
        while (!Monetization.IsReady (placementId)) {
            yield return new WaitForSeconds(0.25f);
        }

        ShowAdPlacementContent ad = null;
        ad = Monetization.GetPlacementContent (placementId) as ShowAdPlacementContent;

        if(ad != null) {
            ad.Show ();
        }
    }
    
    public void ShowAdRewarded () {
        StartCoroutine (WaitForAd ());
    }

    IEnumerator WaitForAd () {
        while (!Monetization.IsReady (placementId)) {
            yield return null;
        }

        ShowAdPlacementContent ad = null;
        ad = Monetization.GetPlacementContent (placementId) as ShowAdPlacementContent;

        if (ad != null) {
            ad.Show (AdFinished);
        }
    }

    void AdFinished (ShowResult result) {
        if (result == ShowResult.Finished) {
            // Reward the player
        } else if (result == ShowResult.Skipped) {
            Debug.LogWarning ("The player skipped the video - DO NOT REWARD!");
        } else if (result == ShowResult.Failed) {
            Debug.LogError ("Video failed to show");
        }
    }

    void ShowBanner()
    {
        StartCoroutine(ShowBannerWhenReady());
    }
    
    IEnumerator ShowBannerWhenReady () {
        while (!Advertisement.IsReady ("banner")) {
            yield return new WaitForSeconds (0.5f);
        }
        Advertisement.Banner.Show (bannerPlacement);
    }
    
}
