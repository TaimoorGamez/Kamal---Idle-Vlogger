using TMPro;
using Core.Events;
using DG.Tweening;
using UnityEngine;
using Core.Economy;
using Core.GamePlay;
using UnityEngine.UI;
using Core.DB.Variables;
using System.Collections;
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
using System.Collections.Generic;

namespace Core.Plugins.Ads
{
    public class AdsManager : MonoBehaviour
    {
        public RectTransform[] AdButtons2X, AdButtons10X;
        public AdConfig AdsConfig = new AdConfig();
        public bool AdTimerComplete = false, AdPlaying = false, CanMultiply = false, CanDoubleDailyReward = false,
                    CanSpin = false, CanBlockAds = false, CanX2Income = false, CanX2Tap = false, CanX10Income = false
                    ,CanRewardCash, CanRewardGold;

        [SerializeField] AdHandler RewardedAdOne, RewardedAdTwo, InterstitialAd;
        [SerializeField] Image AdPanel;
        [SerializeField] TextMeshProUGUI AdPanelDescription;
        [SerializeField] Sprite[] AdPanelSprites;

         public bool IsInitialized = false;

        Coroutine _rewardRotine = null, _adsRotine = null;
        bool _adPanelActive = false;
        float _yPos = -400, yPosDiff = 110, _tweenDuration = 0.25f, adDelay = 5;
        int _currentAdIndex = -1, _goldReward = -1, _cashPrizeMultipler = 100, _basicGoldPrize = 5, _goldPrizeMultipler = 4;
        double _cashReward = -1;
        Transform _adPanelParent;
        List<RectTransform> _activeButtons2X = new List<RectTransform>();
        List<RectTransform> _activeButtons10X = new List<RectTransform>();

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
            if (!AdsConfig.CanShowAds || IsInitialized)
                return;

#if UNITY_EDITOR
            InitAds();
#else
                RequestConsentInfo();
#endif
        }

        void RequestConsentInfo()
        {
            if (!AdsConfig.CanShowAds)
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
                    if (AdsConfig.Rewarded)
                    {
                        _adPanelParent = AdPanel.transform.parent;
                        RewardedAdOne.LoadAd();
                        RewardedAdTwo.LoadAd();
                        StartCoroutine(CheckRewardedButtons());
                    }

                    if (AdsConfig.Interstitial && DBVariablesHolder.RemoveAds.Value == 0)
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
            _rewardRotine = StartCoroutine(RewardCorotine());
        }

        IEnumerator RewardCorotine()
        {
            yield return new WaitForSeconds(0.01f);

            if (CanMultiply)
            {
                CanMultiply = false;
                SimpleEventsHolder.MultiplayRewardEvent?.Invoke();
            }
            else if (CanDoubleDailyReward)
            {
                CanDoubleDailyReward = false;
                SimpleEventsHolder.DoubleDailyRewardEvent?.Invoke();
            }
            else if (CanSpin)
            {
                CanSpin = false;
                SimpleEventsHolder.RewardSpinWheelEvent?.Invoke();
            }
            else if (CanBlockAds)
            {
                CanBlockAds = false;
                SimpleEventsHolder.AdsBlockerEvent?.Invoke();
            }
            else if(CanX2Income)
            {
                CanX2Income = false;
                SimpleEventsHolder.X2IncomeEvent?.Invoke();
            }
            else if (CanX2Tap)
            {
                CanX2Tap = false;
                SimpleEventsHolder.X2TappedEvent?.Invoke();
            }
            else if (CanX10Income)
            {
                CanX10Income = false;
                SimpleEventsHolder.X10IncomeEvent?.Invoke();
            }
            else if (CanRewardCash)
            {
                CanRewardCash = false;
                CashCurrency.Amount+=_cashReward;
            }
            else if (CanRewardGold)
            {
                CanRewardGold = false;
                GoldCurrency.Amount += _goldReward;
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
            yield return new WaitForSeconds(AdsConfig.AdShowTime);
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

        IEnumerator CheckRewardedButtons()
        {
            WaitForSeconds wait = new WaitForSeconds(adDelay);
            while (true)
            {
                if (RewardedAdOne.IsAdAvailable)
                {
                    RectTransform newButton2x = GetButton2X();
                    if (newButton2x  != null)
                    {
                        _activeButtons2X.Add(newButton2x);
                    }

                    int position2x = 0;
                    for (int i = 0; i < _activeButtons2X.Count; i++)
                    {
                        RectTransform buttTransform = _activeButtons2X[i];
                        buttTransform.gameObject.SetActive(true);
                        buttTransform.DOScale(Vector3.one, _tweenDuration).From(Vector3.zero).SetEase(Ease.OutBack);
                        buttTransform.DOAnchorPosY(_yPos - (position2x * yPosDiff), _tweenDuration).SetEase(Ease.Linear);
                        position2x++;
                    }
                }

                if (RewardedAdTwo.IsAdAvailable) 
                {
                    RectTransform newButton10x = GetButton10X();
                    if (newButton10x != null)
                    {
                        _activeButtons10X.Add(newButton10x);
                    }

                    int position10x = 0;
                    for (int i = 0; i < _activeButtons10X.Count; i++)
                    {
                        RectTransform buttTransform = _activeButtons10X[i];
                        buttTransform.gameObject.SetActive(true);
                        buttTransform.DOScale(Vector3.one, _tweenDuration).From(Vector3.zero).SetEase(Ease.OutBack);
                        buttTransform.DOAnchorPosY(_yPos - (position10x * yPosDiff), _tweenDuration).SetEase(Ease.Linear);
                        position10x++;
                    }
                }
                yield return wait;
            }
        }
        
        RectTransform GetButton2X()
        {
            for(int b =0; b< AdButtons2X.Length; b++)
            {
                if(!AdButtons2X[b].gameObject.activeInHierarchy)
                {
                    return AdButtons2X[b];
                }
            }
            return null;
        }

        RectTransform GetButton10X()
        {
            for (int b = 0; b < AdButtons10X.Length; b++)
            {
                if (!AdButtons10X[b].gameObject.activeInHierarchy)
                {
                    return AdButtons10X[b];
                }
            }
            return null;
        }
    
        public void ShowAdDetails(int adIndex)
        {
            if (!_adPanelActive)
            {
                _adPanelActive = true;
                _currentAdIndex = adIndex;
                AdPanel.sprite = AdPanelSprites[_currentAdIndex];
                AdPanelDescription.gameObject.SetActive(false);
                _adPanelParent.gameObject.SetActive(true);
                AdPanel.transform.DOScale(Vector3.one, _tweenDuration).From(Vector3.zero).SetEase(Ease.OutBack);
                if (adIndex == 3) 
                {
                    _cashReward = DBVariablesHolder.BasicIncome.Value * _cashPrizeMultipler;
                    AdPanelDescription.text = GameManager.Instance.FormatMoney(_cashReward);
                    AdPanelDescription.gameObject.SetActive(true);
                }
                if (adIndex == 4)
                {
                    _goldReward = _basicGoldPrize + (DBVariablesHolder.CurrentMap.Value * _goldPrizeMultipler);
                    AdPanelDescription.text = GameManager.Instance.FormatMoney(_goldReward);
                    AdPanelDescription.gameObject.SetActive(true);
                }
            }
        }

        public void OnWatchAd()
        {
            string AdName = string.Empty;
            switch (_currentAdIndex) 
            {
                case 0:
                    AdName = "Ad_2X";
                    break;

                case 1:
                    AdName = "Ad_2XTap";
                    break;

                case 2:
                    AdName = "Ad_10X";
                    break;

                case 3:
                    AdName = "Ad_CashPrize";
                    break;

                case 4:
                    AdName = "Ad_GoldPrize";
                    break;
            }
            if (RewardedAdOne.IsAdAvailable) 
            {
                RewardedAdOne.ShowAd(AdName);
            }
            else if (RewardedAdTwo.IsAdAvailable) 
            {
                RewardedAdTwo.ShowAd(AdName);
            }
            HideAdDetails();
        }

        public void HideAdDetails() 
        {
            AdPanel.transform.DOScale(Vector3.zero,_tweenDuration).SetEase(Ease.InBack).OnComplete(()=>
            {
                _adPanelActive = false;
                _adPanelParent.gameObject.SetActive(false);
            });
        }
    }
}