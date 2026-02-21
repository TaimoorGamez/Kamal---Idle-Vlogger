using Core.Events;
using UnityEngine;
using Core.GamePlay;
using Core.DB.Variables;
using GoogleMobileAds.Api;
using Core.Plugins.Firebase;

namespace Core.Plugins.Ads
{
    public class InterstitialAdHandler : AdHandler
    {
        [SerializeField] GameObject AdLoading;

        InterstitialAd _interstitialAd;
        string _adUnitId;

        public override void LoadAd()
        {
            if (!AdsManager.I.IsInitialized || DBVariablesHolder.RemoveAds.Value == 1)
                return;

            if (IsTestAd)
            {
                _adUnitId = TestId;
            }
            else
            {
                _adUnitId = AdId;
            }

            if (_interstitialAd != null)
            {
                _interstitialAd.Destroy();
                _interstitialAd = null;
            }

            var adRequest = new AdRequest();

            InterstitialAd.Load(_adUnitId, adRequest,
                (InterstitialAd ad, LoadAdError error) =>
                {
                    if (error != null || ad == null)
                    {
                        return;
                    }
                    _interstitialAd = ad;
                    RegisterEventHandlers(ad);
                });
        }

        public override bool IsAdAvailable
        {
            get
            {
                return _interstitialAd != null && _interstitialAd.CanShowAd() && AdsManager.I.AdTimerComplete && !AdsManager.I.AdPlaying
                    && DBVariablesHolder.LvlNum.Value > LevelsManager.I.MinLvlCount && DBVariablesHolder.RemoveAds.Value != 1
                    && DBVariablesHolder.AdBlocked.Value != 1;
            }
        }

        public override void ShowAd(string detail = "")
        {
            if (IsAdAvailable)
            {
                AdsManager.I.AdPlaying = true;
                if (AdLoading != null)
                    Instantiate(AdLoading);
                FirebaseHandler.I?.LogEvent($"InterAd_{detail}");
                Invoke(nameof(ShowIntertitia), 0.5f);
            }
            else if(_interstitialAd == null)
            {
                LoadAd();
            }
        }

        void ShowIntertitia()
        {
            _interstitialAd.Show();
        }

        private void RegisterEventHandlers(InterstitialAd interstitialAd)
        {
            // Raised when the ad is estimated to have earned money.
            interstitialAd.OnAdPaid += (AdValue adValue) =>
            {
                //Debug.Log("Interstitial ad paid {0} {1}.",
                //    adValue.Value,
                //    adValue.CurrencyCode);
            };
            // Raised when an impression is recorded for an ad.
            interstitialAd.OnAdImpressionRecorded += () =>
            {
                //Debug.Log("Interstitial ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            interstitialAd.OnAdClicked += () =>
            {
                //Debug.Log("Interstitial ad was clicked.");
            };
            // Raised when an ad opened full screen content.
            interstitialAd.OnAdFullScreenContentOpened += () =>
            {
                //Debug.Log("Interstitial ad full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                AdsManager.I.AdPlaying = false;
                AdsManager.I.AdTimerComplete = false;
                SimpleEventsHolder.StartCountingAdBreak?.Invoke();
                LoadAd();
            };
            // Raised when the ad failed to open full screen content.
            interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
            {
                AdsManager.I.AdPlaying = false;
                AdsManager.I.AdTimerComplete = false;
                SimpleEventsHolder.StartCountingAdBreak?.Invoke();
                LoadAd();
            };
        }
    }
}
