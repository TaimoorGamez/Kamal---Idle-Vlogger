using TMPro;
using UnityEngine;
using Core.Events;
using DG.Tweening;
using Core.Economy;
using Core.GamePlay;
using UnityEngine.UI;
using Core.DB.Variables;
using Core.Plugins.Firebase;

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
        Tween _pulseTween;


        private void OnEnable()
        {
            int subscriberLvl = DBVariablesHolder.SubscriberLvl.Value;
            if(subscriberLvl > 0)
                _currentTarget = _currentTarget * (subscriberLvl * _targetMultiplier);

            SimpleEventsHolder.UpdateSubscribeTxtEvent += UpdateCashText;
        }

        private void OnDisable()
        {
            SimpleEventsHolder.UpdateSubscribeTxtEvent -= UpdateCashText;
        }

        void UpdateCashText()
        {
            double subscribers = Subscribers.Amount;
            SubscribarsTxt.text = GameManager.Instance.FormatMoney(subscribers);
            FillBar.fillAmount = (float)(subscribers / _currentTarget);
            if (subscribers >= _currentTarget && _pulseTween == null)
            {
                RewardButton.interactable = true;
                _pulseTween = transform.DOScale(_pulseScale, _tweenTiming).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            }
        }

        public void OnClickSubscriberReward()
        {
            if (Subscribers.Amount >= _currentTarget)
            {
                RewardTxt.text = GameManager.Instance.FormatMoney(_currentTarget * _cashMultipler);
                RewardPanel.SetActive(true);
                RewardPanel.transform.DOScale(Vector3.one, _tweenTiming).From(Vector3.zero).SetEase(Ease.OutBack);
                RewardButton.interactable = false;
                transform.localScale = Vector3.one;
            }
        }

        public void OnClickClaimReward()
        {
            FirebaseHandler.I.LogEvent($"Subscriber_{DBVariablesHolder.SubscriberLvl.Value}");
            CashCurrency.Amount += (_currentTarget * _cashMultipler);
            _currentTarget *= _targetMultiplier;
            DBVariablesHolder.SubscriberLvl.Value++;
            RewardPanel.transform.DOScale(Vector3.zero, _tweenTiming).SetEase(Ease.InBack).OnComplete(() =>
            {
                RewardPanel.SetActive(false);
            });
            _pulseTween.Kill();
            _pulseTween = null;
        }
    }
}