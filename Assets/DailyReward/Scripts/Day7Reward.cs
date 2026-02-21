using TMPro;
using UnityEngine;
using Core.DB.Variables;

namespace Core.DailyReward
{
    public class Day7Reward : DailyRewardHandler
    {
        [SerializeField] int TubeCounts;
        [SerializeField] TextMeshProUGUI[] TubesText;

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void UpdateState()
        {
            base.UpdateState();
        }

        protected override void UpdateUI()
        {
            base.UpdateUI();
            for (int i = 0; i < AmountText.Length; i++)
            {
                AmountText[i].text = Amount.ToString();
            }

            for (int i = 0; i < TubesText.Length; i++)
            {
                TubesText[i].text = TubeCounts.ToString();
            }
        }

        protected override void OnClickBuyButton()
        {
            base.OnClickBuyButton();
            DBVariablesHolder.RemainingTubes.Value += TubeCounts;
        }

        protected override void GrantDoubleReward()
        {
            DBVariablesHolder.RemainingTubes.Value += (TubeCounts * 2);
            base.GrantDoubleReward();
        }
    }
}
