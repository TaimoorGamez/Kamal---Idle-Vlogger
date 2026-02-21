using System;
using Core.Economy;
using Core.DB.Variables;

namespace Core.SpinWheel
{
    public interface ISpinWheel
    {
        void Spin();
    }

    [Serializable]
    public class SpinWheelConfige
    {
        public string RewardName;
        public UnityEngine.Color SegmentColor;
        public UnityEngine.Sprite Icon;
        public int Amount;
        public float Weight;

        public void ClaimReward()
        {
            if (string.Equals(RewardName, "Cash", StringComparison.Ordinal))
            {
                CurrenciesHolder.CashCurrency.Amount += Amount;
            }
            else
            {
                DBVariableDictionariesHolder.PowerStatusData[RewardName].Value += Amount;
            }
        }
    }
}

