using TMPro;
using UnityEngine;
using Core.Events;
using DG.Tweening;
using Core.Economy;
using UnityEngine.UI;

namespace Core.Screen
{
    public class SubscriberBarController : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI SubscribarsTxt, RewardTxt;
        [SerializeField] Image FillBar;
        [SerializeField] Button RewardButton;
        [SerializeField] GameObject RewardPanel;

        double _currentTarget = 1000;
        int _targetMultiplier = 10;
        float _cashMultipler = 1.2f, _tweenTiming = 0.5f, _pulseScale = 1.1f;

        private void OnEnable()
        {
            SimpleEventsHolder.UpdateSubscribeTxtEvent += UpdateCashText;
        }

        private void OnDisable()
        {
            SimpleEventsHolder.UpdateSubscribeTxtEvent -= UpdateCashText;
        }

        void UpdateCashText()
        {
            double subscribers = Subscribers.Amount;
            SubscribarsTxt.text = subscribers.ToString();
            FillBar.fillAmount = (float)(subscribers / _currentTarget);
            if (subscribers >= _currentTarget)
            {
                transform.DOScale(_pulseScale, _tweenTiming).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            }
        }

        public void OnClickSubscriberReward()
        {
            if (Subscribers.Amount >= _currentTarget)
            {
                RewardTxt.text = ($"+{_currentTarget * _cashMultipler}");
                RewardPanel.SetActive(true);
                transform.DOKill();
            }
        }

        public void OnClickClaimReward()
        {
            CashCurrency.Amount += (_currentTarget * _cashMultipler);
            _currentTarget *= _targetMultiplier;
            RewardPanel.SetActive(false);
        }
    }
}