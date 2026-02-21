using Core.Events;
using UnityEngine;
using Core.DB.Variables;
using System.Collections;
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;

namespace Core.Plugins.Ads
{
    public class AdsManager : MonoBehaviour
    {
        [SerializeField] AdHandler RewardedAd, IntertitialAd;

        [HideInInspector] public bool IsInitialized = false;

        public bool AdTimerComplete = false, AdPlaying = false, CanAddMoves = false, CanMultiply = false, CanDoubleDailyReward = false,
                    CanSpin = false, CanCap = false, CanSpray = false, CanBlockAds = false, CanFlame = false, CanUndo = false,
                                    CanAddExtraTube = false, CanSwitchColor = false;

        Coroutine _rewardRotine = null, _adsRotine = null;
        bool _isEnable = false;

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
            if(!RemoteDataHolder.AdData.CanShowAds)
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
                        RewardedAd.LoadAd();
                    }

                    if (RemoteDataHolder.AdData.Interstitial && DBVariablesHolder.RemoveAds.Value == 0)
                    {
                        IntertitialAd.LoadAd();
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
                if (CanAddMoves)
                {
                    CanAddMoves = false;
                    SimpleEventsHolder.AddMovesEvent?.Invoke();
                    _isEnable = false;
                }
                else if (CanUndo)
                {
                    CanUndo = false;
                    SimpleEventsHolder.RewardUndoEvent?.Invoke();
                    _isEnable = false;
                }
                else if (CanAddExtraTube)
                {
                    CanAddExtraTube = false;
                    SimpleEventsHolder.RewardExtraTubeEvent?.Invoke();
                    _isEnable = false;
                }
                else if (CanSwitchColor)
                {
                    CanSwitchColor = false;
                    SimpleEventsHolder.RewardSwapColor?.Invoke();
                    _isEnable = false;
                }
                else if (CanMultiply)
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
                else if (CanCap)
                {
                    CanCap = false;
                    SimpleEventsHolder.BuyCaps?.Invoke();
                    _isEnable = false;
                }
                else if (CanSpray)
                {
                    CanSpray = false;
                    SimpleEventsHolder.BuySprays?.Invoke();
                    _isEnable = false;
                }
                else if (CanFlame)
                {
                    CanFlame = false;
                    SimpleEventsHolder.BuyFlames?.Invoke();
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
            RewardedAd.ShowAd(reward);
        }

        public void ShowInterstitialAd(string detail = "")
        {
            if (RemoteDataHolder.AdData.Interstitial && DBVariablesHolder.RemoveAds.Value == 0)
            {
                IntertitialAd.ShowAd(detail);
            }
        }
    }
}