using DG.Tweening;
using UnityEngine;
using Core.Economy;
using UnityEngine.UI;
using Core.DB.Variables;

namespace Core.GamePlay
{
    public class SkillsUpdate : UpdateSystem
    {
        [SerializeField] ScrollRect ItemScroller;

        float _scrollDuration = 0.5f;

        protected override void OnEnable()
        {
            base.OnEnable();
            AnimateScroll();

        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        void AnimateScroll()
        {
            DOTween.To(
                () => ItemScroller.verticalNormalizedPosition,
                x => ItemScroller.verticalNormalizedPosition = x,
                0f,
                _scrollDuration
            ).SetEase(Ease.OutBack).OnComplete(() =>
            {
                DOTween.To(
                    () => ItemScroller.verticalNormalizedPosition,
                    x => ItemScroller.verticalNormalizedPosition = x,
                    1f,
                    _scrollDuration
                ).SetEase(Ease.Linear);
            });
        }

        protected override void UpdatePriceForAll()
        {
            UpdateCost(0, DBVariablesHolder.CharismaLvl.Value);
            UpdateCost(1, DBVariablesHolder.EruditionLvl.Value);
            UpdateCost(2, DBVariablesHolder.ImprovisationLvl.Value);
            UpdateCost(3, DBVariablesHolder.WitLvl.Value);
        }

        public override int GetAvailableUpdates()
        {
            int count = 0;

            int charimaLvl = DBVariablesHolder.CharismaLvl.Value;
            if (!AnyRestriction(charimaLvl) && CashCurrency.Amount >= GetCost(charimaLvl + 1))
                count++;

            int erudLvl = DBVariablesHolder.EruditionLvl.Value;
            if (!AnyRestriction(erudLvl) && CashCurrency.Amount >= GetCost(erudLvl + 1))
                count++;

            int improLvl = DBVariablesHolder.ImprovisationLvl.Value;
            if (!AnyRestriction(improLvl) && CashCurrency.Amount >= GetCost(improLvl + 1))
                count++;

            int witLvl = DBVariablesHolder.WitLvl.Value;
            if (!AnyRestriction(witLvl) && CashCurrency.Amount >= GetCost(witLvl + 1))
                count++;

            return count;
        }

        public void UpdateItem(int itemIndex)
        {
            switch (itemIndex)
            {
                case -1:
                    UpdateItemProcess(itemIndex, DBVariablesHolder.CharismaLvl, 0, false);
                    break;

                case -2:
                    UpdateItemProcess(itemIndex, DBVariablesHolder.EruditionLvl, 1, false);
                    break;

                case -3:
                    UpdateItemProcess(itemIndex, DBVariablesHolder.ImprovisationLvl, 2, false);
                    break;

                case -4:
                    UpdateItemProcess(itemIndex, DBVariablesHolder.WitLvl, 3, false);
                    break;
            }
        }
    }
}
