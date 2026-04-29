using DG.Tweening;
using UnityEngine;
using Core.Economy;
using UnityEngine.UI;
using Core.DB.Variables;

namespace Core.GamePlay
{
    public class ApartmentUpdates : UpdateSystem
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
            base.UpdatePriceForAll();
            UpdateCost(0, DBVariablesHolder.HouseLvl.Value);
            UpdateCost(1, DBVariablesHolder.VehicleLvl.Value);
            UpdateCost(2, DBVariablesHolder.BackyardLvl.Value);
            UpdateCost(3, DBVariablesHolder.GroundLvl.Value);
            UpdateCost(4, DBVariablesHolder.StatueLvl.Value);
        }

        public override int GetAvailableUpdates()
        {
            int count = 0;

            int hairLvl = DBVariablesHolder.HouseLvl.Value;
            if (!AnyRestriction(hairLvl) && CashCurrency.Amount >= GetCost(hairLvl + 1))
                count++;

            int vehicleLvl = DBVariablesHolder.VehicleLvl.Value;
            if (!AnyRestriction(vehicleLvl) && CashCurrency.Amount >= GetCost(vehicleLvl + 1))
                count++;

            int backyardLvl = DBVariablesHolder.BackyardLvl.Value;
            if (!AnyRestriction(backyardLvl) && CashCurrency.Amount >= GetCost(backyardLvl + 1))
                count++;

            int groundLvl = DBVariablesHolder.GroundLvl.Value;
            if (!AnyRestriction(groundLvl) && CashCurrency.Amount >= GetCost(groundLvl + 1))
                count++;

            int statueLvl = DBVariablesHolder.StatueLvl.Value;
            if (!AnyRestriction(statueLvl) && CashCurrency.Amount >= GetCost(statueLvl + 1))
                count++;

            return count;
        }

        public void UpdateItem(int itemIndex)
        {
            switch (itemIndex)
            {
                case 6:
                    UpdateItemProcess(itemIndex,DBVariablesHolder.HouseLvl, 0, true);
                    break;

                case 7:
                    UpdateItemProcess(itemIndex, DBVariablesHolder.VehicleLvl, 1, true);
                    break;

                case 8:
                    UpdateItemProcess(itemIndex, DBVariablesHolder.BackyardLvl, 2, true);
                    break;

                case 9:
                    UpdateItemProcess(itemIndex, DBVariablesHolder.GroundLvl, 3, true);
                    break;

                case 10:
                    UpdateItemProcess(itemIndex, DBVariablesHolder.StatueLvl, 4, true);
                    break;
            }
        }
    }
}
