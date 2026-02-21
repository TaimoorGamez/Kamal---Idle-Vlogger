using Core.Events;
using GoogleMobileAds.Api;
using Core.Plugins.Firebase;

namespace Core.Plugins.Ads
{
    public class RewardedAdHandler : AdHandler
    {

        RewardedAd _rewardedAd;
        string _adUnitId;

        public override void LoadAd()
        {
            if(!AdsManager.I.IsInitialized)
                return;
            

            if (IsTestAd)
            {
                _adUnitId = TestId;
            }
            else
            {
                _adUnitId = AdId;
            }

            if (_rewardedAd != null)
            {
                _rewardedAd.Destroy();
                _rewardedAd = null;
            }

            var adRequest = new AdRequest();
            RewardedAd.Load(_adUnitId, adRequest,
                (RewardedAd ad, LoadAdError error) =>
                {
                    if (error != null || ad == null)
                    {
                        return;
                    }
                    _rewardedAd = ad;
                    RegisterEventHandlers(ad);
                });
        }

        public override bool IsAdAvailable
        {
            get
            {
                return _rewardedAd != null && _rewardedAd.CanShowAd() && !AdsManager.I.AdPlaying; 
            }
        }

        public override void ShowAd(string detail = "")
        {
            if (IsAdAvailable)
            {
                AdsManager.I.AdPlaying = true;
                _rewardedAd.Show((Reward reward) =>
                {
                    GrantReward(detail);
                });
            }
            else
            {
                SingleIntegerEventsHolder.ShowToastEvent?.Invoke(3);
                LoadAd();
            }
        }

        private void RegisterEventHandlers(RewardedAd ad)
        {
            ad.OnAdPaid += (AdValue adValue) =>
            {
                //Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                //    adValue.Value,
                //    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () =>
            {
                //Debug.Log("Rewarded ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                //Debug.Log("Rewarded ad was clicked.");
            };
            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () =>
            {
                //Debug.Log("Rewarded ad full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                //Debug.Log("Rewarded ad full screen content closed.");
                AdsManager.I.AdPlaying = false;
                AdsManager.I.AdTimerComplete = false;
                SimpleEventsHolder.StartCountingAdBreak?.Invoke();
                LoadAd();
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                //Debug.Log("Rewarded ad failed to open full screen content " +
                //               "with error : " + error);
                AdsManager.I.AdPlaying = false;
                AdsManager.I.AdTimerComplete = false;
                SimpleEventsHolder.StartCountingAdBreak?.Invoke();
                LoadAd();
            };
        }

        void GrantReward(string rewardName)
        {
            switch (rewardName)
            {
                case "AddMoves":
                    AdsManager.I.CanAddMoves = true;
                    break;

                case "MultiplyReward":
                    AdsManager.I.CanMultiply = true;
                    break;

                case "SortUndo":
                    AdsManager.I.CanUndo = true;
                    break;

                case "ExtraTube":
                    AdsManager.I.CanAddExtraTube = true;
                    break;

                case "SwapColor":
                    AdsManager.I.CanSwitchColor = true;
                    break;

                case "DoubleDailyReward":
                    AdsManager.I.CanDoubleDailyReward = true;
                    break;

                case "Spin":
                    AdsManager.I.CanSpin = true;
                    break;

                case "AdBlocker":
                    AdsManager.I.CanBlockAds = true;
                    break;

                case "Caps":
                    AdsManager.I.CanCap = true;
                    break;

                case "Sprays":
                    AdsManager.I.CanSpray = true;
                    break;

                case "FlameThrowers":
                    AdsManager.I.CanFlame = true;
                    break;
            }
            SimpleEventsHolder.GrantRewardEvent?.Invoke();
            DoubleIntegerEventHolder.TaskEvent?.Invoke(0,1);
            FirebaseHandler.I?.LogEvent($"rvAd_{rewardName}");
        }
    }
}
