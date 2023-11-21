using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public static AdsManager instance;

#if UNITY_ANDROID
    private string gameID = "5266892";
    private string rewardPlacementID = "Rewarded_Android";
    private string interPlacementID = "Interstitial_Android";
#elif UNITY_IOS
    private string gameID = "5266893";
    private string rewardPlacementID = "Rewarded_iOS";
    private string interPlacementID = "Interstitial_iOS";
#endif
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);

        Advertisement.Initialize(gameID, false, this);
    }

    public void ShowRewardAds()
    {
        Advertisement.Show(rewardPlacementID, this);
    }

    public void ShowInterAds()
    {
        Advertisement.Show(interPlacementID, this);
    }

    #region 初始化

    public void OnInitializationComplete()
    {
        Debug.Log("广告初始化成功");
        Advertisement.Load(rewardPlacementID, this);
        Advertisement.Load(interPlacementID, this);
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log("广告初始化失败" + message);
    }

    #endregion

    #region 加载

    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log("广告" + placementId + "加载成功");
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region 显示广告

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        throw new NotImplementedException();
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        //停止音乐
        AudioManager.instance.bgmMusic.Stop();
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        //重新开始游戏
        TransitionManager.instance.Transition("GamePlay");
    }

    #endregion
}