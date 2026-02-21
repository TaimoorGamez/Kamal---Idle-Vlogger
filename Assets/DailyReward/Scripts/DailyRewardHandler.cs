using Core.DB.Variables;
using Core.Economy;
using Core.Events;
using Core.Plugins.Firebase;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.DailyReward
{
    public class DailyRewardHandler : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI[] DayText, AmountText;
        [SerializeField] protected Button ClaimButton;
        [SerializeField] protected GameObject[] RewardState;
        [SerializeField] protected int RewardDay, Amount;

        protected bool _doubleRewardClicked = false, _giveReward;

        protected virtual void OnEnable()
        {
            UpdateUI();
            UpdateState();
        }

        protected virtual void UpdateUI()
        {
            for (int i = 0; i < DayText.Length; i++)
            {
                DayText[i].text = "DAY " + RewardDay.ToString();
            }

            for (int i = 0; i < AmountText.Length; i++)
            {
                AmountText[i].text = Amount.ToString();
            }
        }

        protected void Update()
        {
            if (_giveReward && DBVariablesHolder.ToDay.Value == RewardDay && DBVariablesHolder.RewardClaimed.Value == 0 && !_doubleRewardClicked)
            {
                _giveReward = false;
                GrantDoubleReward();
            }
        }

        protected virtual void UpdateState()
        {
            if (DBVariablesHolder.ToDay.Value == RewardDay && DBVariablesHolder.RewardClaimed.Value == 0)
            {
                ActiveState(1);
                ClaimButton.onClick.AddListener(OnClickBuyButton);
                SimpleEventsHolder.DoubleDailyRewardEvent += GrantDoubleReward;
            }
            else
            {
                if (DBVariablesHolder.ToDay.Value < RewardDay)
                {
                    ActiveState(2);
                }
                else
                {
                    ActiveState(0);
                }
            }
        }

        private void ActiveState(int stateIndex)
        {
            for (int state = 0; state < RewardState.Length; state++)
            {
                RewardState[state].SetActive(false);
            }
            RewardState[stateIndex].SetActive(true);
        }

        protected virtual void OnClickBuyButton()
        {
            if (DBVariablesHolder.RewardClaimed.Value == 0)
            {
                DBVariablesHolder.RewardClaimed.Value = 1;
                CurrenciesHolder.CashCurrency.Amount += Amount;
                UpdateState();
                ClaimButton.onClick.RemoveListener(OnClickBuyButton);
                SimpleEventsHolder.DoubleDailyRewardEvent -= GrantDoubleReward;
                SimpleEventsHolder.UpDateDailyRewardState?.Invoke();
                FirebaseHandler.I?.LogEvent($"DR_Amt_{Amount}_Day_{RewardDay}");
            }
        }

        protected virtual void GrantDoubleReward()
        {
            if (DBVariablesHolder.ToDay.Value == RewardDay && DBVariablesHolder.RewardClaimed.Value == 0 && !_doubleRewardClicked)
            {
                _doubleRewardClicked = true;
                DBVariablesHolder.RewardClaimed.Value = 1;
                CurrenciesHolder.CashCurrency.Amount += (Amount * 2);
                UpdateState();
                SimpleEventsHolder.UpDateDailyRewardState?.Invoke();
                ClaimButton.onClick.RemoveListener(OnClickBuyButton);
                SimpleEventsHolder.DoubleDailyRewardEvent -= GrantDoubleReward;
            }
        }

    }
}
