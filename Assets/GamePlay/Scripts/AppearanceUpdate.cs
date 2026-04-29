using Core.Economy;
using Core.DB.Variables;

namespace Core.GamePlay
{
    public class AppearanceUpdate : UpdateSystem
    {
        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void UpdatePriceForAll()
        {
            UpdateCost(0, DBVariablesHolder.ClothesLvl.Value);
            UpdateCost(1, DBVariablesHolder.HairsLvl.Value);
            UpdateCost(2, DBVariablesHolder.WatchLvl.Value);
        }

        public override int GetAvailableUpdates()
        {
            int count = 0;

            int clothLvl = DBVariablesHolder.ClothesLvl.Value + 1;
            if (!AnyRestriction(clothLvl) && CashCurrency.Amount >= GetCost(clothLvl + 1))
                count++;

            int hairLvl = DBVariablesHolder.HairsLvl.Value + 1;
            if (!AnyRestriction(hairLvl) && CashCurrency.Amount >= GetCost(hairLvl + 1))
                count++;

            int watchLvl = DBVariablesHolder.WatchLvl.Value + 1;
            if (!AnyRestriction(watchLvl) && CashCurrency.Amount >= GetCost(watchLvl + 1))
                count++;

            return count;
        }

        public void UpdateItem(int itemIndex)
        {
            switch (itemIndex)
            {
                case 0:
                    UpdateItemProcess(itemIndex, DBVariablesHolder.ClothesLvl, 0, true);
                    break;

                case 1:
                    UpdateItemProcess(itemIndex, DBVariablesHolder.HairsLvl, 1, true);
                    break;

                case 2:
                    UpdateItemProcess(itemIndex, DBVariablesHolder.WatchLvl, 2, true);
                    break;
            }
        }
    }
}
