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

                    if (CashCurrency.Amount >= cost)
                    {
                        CashCurrency.Amount -= cost;
                        DBVariablesHolder.BasicIncome.Value += Increments[0];
                        DBVariablesHolder.ClothesLvl.Value++;
                        UpdateCost(itemIndex, lvl + 1);
                        return;
                    }
                    break;

                case 1:
                    lvl = DBVariablesHolder.HairsLvl.Value;
                    cost = GetCost(lvl);

                    if (CashCurrency.Amount >= cost)
                    {
                        CashCurrency.Amount -= cost;
                        DBVariablesHolder.BasicIncome.Value += Increments[1];
                        DBVariablesHolder.HairsLvl.Value++;
                        UpdateCost(itemIndex, lvl + 1);
                        return;
                    }
                    break;

                case 2:
                    lvl = DBVariablesHolder.WatchLvl.Value;
                    cost = GetCost(lvl);

                    if (CashCurrency.Amount >= cost)
                    {
                        CashCurrency.Amount -= cost;
                        DBVariablesHolder.BasicIncome.Value += Increments[2];
                        DBVariablesHolder.WatchLvl.Value++;
                        UpdateCost(itemIndex, lvl + 1);
                        return;
                    }
                    break;
            }

            SingleIntegerEventsHolder.ShowToastEvent?.Invoke(0); // Not enough cash
        }
    }
}
