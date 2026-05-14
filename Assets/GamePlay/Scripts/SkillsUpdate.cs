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
            UpdateCost(1, DBVariablesHolder.ContentCreation.Value);
            UpdateCost(2, DBVariablesHolder.ActingLvl.Value);
            UpdateCost(3, DBVariablesHolder.EditingSkill.Value);
        }

        public override int GetAvailableUpdates()
        {
            int count = 0;

            int charimaLvl = DBVariablesHolder.CharismaLvl.Value;
            if (!AnyRestriction(charimaLvl) && CashCurrency.Amount >= GetCost(charimaLvl + 1))
                count++;

            int contentCreationLvl = DBVariablesHolder.ContentCreation.Value;
            if (!AnyRestriction(contentCreationLvl) && CashCurrency.Amount >= GetCost(contentCreationLvl + 1))
                count++;

            int actingLvl = DBVariablesHolder.ActingLvl.Value;
            if (!AnyRestriction(actingLvl) && CashCurrency.Amount >= GetCost(actingLvl + 1))
                count++;

            int editingLvl = DBVariablesHolder.EditingSkill.Value;
            if (!AnyRestriction(editingLvl) && CashCurrency.Amount >= GetCost(editingLvl + 1))
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
                    UpdateItemProcess(itemIndex, DBVariablesHolder.ContentCreation, 1, false);
                    break;

                case -3:
                    UpdateItemProcess(itemIndex, DBVariablesHolder.ActingLvl, 2, false);
                    break;

                case -4:
                    UpdateItemProcess(itemIndex, DBVariablesHolder.EditingSkill, 3, false);
                    break;
            }
        }
    }
}
