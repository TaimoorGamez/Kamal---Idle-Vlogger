using Core.Events;
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

        protected override void UpdatePriceForAll()
        {
            UpdateCost(0, DBVariablesHolder.ClothesLvl.Value);
            UpdateCost(1, DBVariablesHolder.HairsLvl.Value);
            UpdateCost(2, DBVariablesHolder.WatchLvl.Value);
        }

        public override void UpdateItem(int itemIndex)
        {
            int cost = -1, lvl = -1;
            switch (itemIndex)
            {
                case 0:
                    lvl = DBVariablesHolder.ClothesLvl.Value;
                    cost = GetCost(lvl);
                    if (CashCurrency.Amount >= _priceData[itemIndex].Cost)
                    {
                        CashCurrency.Amount -= _priceData[itemIndex].Cost;
                        DBVariablesHolder.BasicIncome.Value += Increments[0]* _priceData[itemIndex].Levels;
                        DBVariablesHolder.ClothesLvl.Value += _priceData[itemIndex].Levels;
                        if(DBVariablesHolder.MaxLevels.Value > 0)
                        {
                            UpdatePriceForAll();
                        }
                        else
                        {
                            UpdateCost(itemIndex, DBVariablesHolder.ClothesLvl.Value);
                        }
                        return;
                    }
                    break;

                case 1:
                    lvl = DBVariablesHolder.HairsLvl.Value;
                    cost = GetCost(lvl);
                    if (CashCurrency.Amount >= _priceData[itemIndex].Cost)
                    {
                        CashCurrency.Amount -= _priceData[itemIndex].Cost;
                        DBVariablesHolder.BasicIncome.Value += Increments[0] * _priceData[itemIndex].Levels;
                        DBVariablesHolder.HairsLvl.Value += _priceData[itemIndex].Levels;
                        if (DBVariablesHolder.MaxLevels.Value > 0)
                        {
                            UpdatePriceForAll();
                        }
                        else
                        {
                            UpdateCost(itemIndex, DBVariablesHolder.HairsLvl.Value);
                        }
                        return;
                    }
                    break;

                case 2:
                    lvl = DBVariablesHolder.WatchLvl.Value;
                    cost = GetCost(lvl);
                    if (CashCurrency.Amount >= _priceData[itemIndex].Cost)
                    {
                        CashCurrency.Amount -= _priceData[itemIndex].Cost;
                        DBVariablesHolder.BasicIncome.Value += Increments[0] * _priceData[itemIndex].Levels;
                        DBVariablesHolder.WatchLvl.Value += _priceData[itemIndex].Levels;
                        if (DBVariablesHolder.MaxLevels.Value > 0)
                        {
                            UpdatePriceForAll();
                        }
                        else
                        {
                            UpdateCost(itemIndex, DBVariablesHolder.WatchLvl.Value);
                        }
                        return;
                    }
                    break;
            }

            SingleIntegerEventsHolder.ShowToastEvent?.Invoke(0); // Not enough cash
        }
    }
}
