using Core.Events;
using DG.Tweening;
using UnityEngine;
using Core.DB.Variables;
using System.Collections;
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
using System.Collections.Generic;

namespace Core.Plugins.Ads
{
    public class AdsManager : MonoBehaviour
    {
        public RectTransform[] AdButtons;
        public AdConfig AdsConfig;
        public bool AdTimerComplete = false, AdPlaying = false, CanMultiply = false, CanDoubleDailyReward = false,
                    CanSpin = false, CanBlockAds = false;

        [SerializeField] AdHandler RewardedAdOne, RewardedAdTwo, InterstitialAd;

         public bool IsInitialized = false;

        Coroutine _rewardRotine = null, _adsRotine = null;
        bool _isEnable = false;
        float _yPos = -400, yPosDiff = 110, _tweenDuration = 0.25f;
        int buttonCount = 0;
        Queue<RectTransform> _activeButtons = new Queue<RectTransform>();

        private void OnEnable()
        {
            SimpleEventsHolder.GrantRewardEvent += PlayRewardCorotine;
            SimpleEventsHolder.StartCountingAdBreak += StartCountingAdBreak;
            SimpleEventsHolder.RemoveAds += StopAds;
        }

        private void OnDisable()
        {
            SimpleEventsHolder.GrantRewardEvent -= PlayRewardCorotine;
            SimpleEventsHolder.StartCountingAdBreak -= StartCountingAdBreak;
            SimpleEventsHolder.RemoveAds -= StopAds;
            CustomDisable();
        }

        public static AdsManager I { get; private set; }

        private void Awake()
        {
            if (I == null)
            {
                I = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        public void InitPlugin()
        {
            Debug.Log("get into init");
            if (!RemoteDataHolder.AdData.CanShowAds || IsInitialized)
                return;

#if UNITY_EDITOR
            InitAds();
#else
                RequestConsentInfo();
#endif
        }

        void RequestConsentInfo()
        {
            if (!RemoteDataHolder.AdData.CanShowAds)
                return;

            ConsentRequestParameters request = new ConsentRequestParameters
            {
                TagForUnderAgeOfConsent = false
            };

            ConsentInformation.Update(request, (formError) =>
            {
                if (formError != null)
                {
                    Debug.Log("ConsentInfo error: " + formError.Message);
                    return;
                }

                if (ConsentInformation.IsConsentFormAvailable())
                {
                    LoadConsentForm();
                }
                else
                {
                    InitAds();
                }
            });
        }

        void LoadConsentForm()
        {
            ConsentForm.Load((form, loadError) =>
            {
                if (loadError != null)
                {
                    Debug.Log("ConsentForm load error: " + loadError.Message);
                    return;
                }

                form.Show((formError) =>
                {
                    if (formError != null)
                    {
                        Debug.Log("ConsentForm show error: " + formError.Message);
                    }

                    if (ConsentInformation.CanRequestAds())
                    {
                        InitAds();
                    }
                });
            });
        }

        void InitAds()
        {
            Debug.Log("Initializing AdMob...");
            try
            {
                MobileAds.Initialize((InitializationStatus initstatus) =>
                {
                    if (initstatus == null)
                    {
                        Debug.Log("InitializationStatus is null!");
                        return;
                    }
                    MobileAds.RaiseAdEventsOnUnityMainThread = true;
                    IsInitialized = true;
                    if (RemoteDataHolder.AdData.Rewarded)
                    {
                        RewardedAdOne.LoadAd();
                        RewardedAdTwo.LoadAd();
                        StartCoroutine(CheckRewardedButtons());
                    }

                    if (RemoteDataHolder.AdData.Interstitial && DBVariablesHolder.RemoveAds.Value == 0)
                    {
                        InterstitialAd.LoadAd();
                    }
                });
            }
            catch (System.Exception ex)
            {
                Debug.Log("AdMob Initialization crashed: " + ex.Message);
            }
        }

        void PlayRewardCorotine()
        {
            _isEnable = true;
            _rewardRotine = StartCoroutine(RewardCorotine());
        }

        IEnumerator RewardCorotine()
        {
            WaitForSeconds wait = new WaitForSeconds(0.01f);
            while (_isEnable)
            {
                yield return wait;
                if (CanMultiply)
                {
                    CanMultiply = false;
                    SimpleEventsHolder.MultiplayRewardEvent?.Invoke();
                    _isEnable = false;
                }
                else if (CanDoubleDailyReward)
                {
                    CanDoubleDailyReward = false;
                    SimpleEventsHolder.DoubleDailyRewardEvent?.Invoke();
                    _isEnable = false;
                }
                else if (CanSpin)
                {
                    CanSpin = false;
                    SimpleEventsHolder.RewardSpinWheelEvent?.Invoke();
                    _isEnable = false;
                }
                else if (CanBlockAds)
                {
                    CanBlockAds = false;
                    SimpleEventsHolder.AdsBlockerEvent?.Invoke();
                    _isEnable = false;
                }
            }
            if (_rewardRotine != null)
            {
                StopCoroutine(_rewardRotine);
                _rewardRotine = null;
            }
        }

        void StartCountingAdBreak()
        {
            if (DBVariablesHolder.RemoveAds.Value != 1 && _adsRotine == null)
            {
                _adsRotine = StartCoroutine(CountAdBreak());
            }
        }

        IEnumerator CountAdBreak()
        {
            yield return new WaitForSeconds(RemoteDataHolder.AdData.AdShowTime);
            AdTimerComplete = true;

            if (_adsRotine != null)
            {
                StopCoroutine(_adsRotine);
                _adsRotine = null;
            }
        }

        void StopAds()
        {
            if (_adsRotine != null)
            {
                StopCoroutine(_adsRotine);
                _adsRotine = null;
            }
            AdTimerComplete = false;
        }

        void CustomDisable()
        {
            _isEnable = false;
            if (_rewardRotine != null)
            {
                StopCoroutine(_rewardRotine);
                _rewardRotine = null;
            }
            if (_adsRotine != null)
            {
                StopCoroutine(_adsRotine);
                _adsRotine = null;
            }
        }

        public void ShowRewardedAd(string reward)
        {
            //RewardedAd.ShowAd(reward);
        }

        public void ShowInterstitialAd(string detail = "")
        {
            if (RemoteDataHolder.AdData.Interstitial && DBVariablesHolder.RemoveAds.Value == 0)
            {
                InterstitialAd.ShowAd(detail);
            }
        }

        IEnumerator CheckRewardedButtons()
        {
            Debug.Log("CheckRewardedButtons started");
            WaitForSeconds wait = new WaitForSeconds(1f);
            while (true)
            {
                Debug.Log("Checking rewarded buttons...");
                yield return wait;
                if (_activeButtons.Count < AdButtons.Length)
                {
                    Debug.Log("Checking if rewarded ads are available...");
                    if (RewardedAdOne.IsAdAvailable || RewardedAdTwo.IsAdAvailable)
                    {
                        Debug.Log("Rewarded ad is available.");
                        _activeButtons.Enqueue(AdButtons[buttonCount]);
                        buttonCount++;
                        if (buttonCount >= AdButtons.Length)
                        {
                            buttonCount = 0;
                        }
                        int positionIndex = 0;
                        for (int i = 0; i < _activeButtons.Count; i++)
                        {
                            RectTransform buttTransform = _activeButtons.Dequeue();
                            buttTransform.gameObject.SetActive(true);
                            buttTransform.DOScale(Vector3.one, _tweenDuration).From(Vector3.zero).SetEase(Ease.OutBack);
                            buttTransform.DOAnchorPosY(_yPos - (positionIndex * yPosDiff), _tweenDuration).SetEase(Ease.Linear);
                            positionIndex++;
                        }
                    }
                }
            }
        }
        

    }
}