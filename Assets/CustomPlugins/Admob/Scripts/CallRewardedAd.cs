using UnityEngine;
using Core.Plugins.Ads;


public class CallRewardedAd : MonoBehaviour
{
    public void ShowAd(string reward)
    {
        AdsManager.I?.ShowRewardedAd(reward);
    }
}
