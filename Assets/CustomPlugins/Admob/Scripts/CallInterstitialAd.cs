using UnityEngine;

namespace Core.Plugins.Ads
{
    public class CallInterstitialAd : MonoBehaviour
    {
        public void ShowAd(string detail = "")
        {
            AdsManager.I?.ShowInterstitialAd(detail);
        }
    }
}
