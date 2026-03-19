using Core.Events;
using Core.Economy;
using Core.DB.Variables;

namespace Core.GamePlay
{
    public class ApartmentUpdates : UpdateSystem
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateCost(0, DBVariablesHolder.HouseLvl.Value);
            UpdateCost(1, DBVariablesHolder.VehicleLvl.Value);
            UpdateCost(2, DBVariablesHolder.StatueLvl.Value);
        }

        public override void UpdateItem(int itemIndex)
        {

            int cost = -1, lvl = -1;
            switch (itemIndex)
            {
                case 0:
                    lvl = DBVariablesHolder.HouseLvl.Value;
                    cost = GetCost(lvl);

                    if (CashCurrency.Amount >= cost)
                    {
                        CashCurrency.Amount -= cost;
                        DBVariablesHolder.BasicIncome.Value += Increments[0];
                        DBVariablesHolder.HouseLvl.Value++;
                        UpdateCost(itemIndex, lvl + 1);
                        return;
                    }
                    break;

                case 1:
                    lvl = DBVariablesHolder.VehicleLvl.Value;
                    cost = GetCost(lvl);

                    if (CashCurrency.Amount >= cost)
                    {
                        CashCurrency.Amount -= cost;
                        DBVariablesHolder.BasicIncome.Value += Increments[1];
                        DBVariablesHolder.VehicleLvl.Value++;
                        UpdateCost(itemIndex, lvl + 1);
                        return;
                    }
                    break;

                case 2:
                    lvl = DBVariablesHolder.StatueLvl.Value;
                    cost = GetCost(lvl);

                    if (CashCurrency.Amount >= cost)
                    {
                        CashCurrency.Amount -= cost;
                        DBVariablesHolder.BasicIncome.Value += Increments[2];
                        DBVariablesHolder.StatueLvl.Value++;
                        UpdateCost(itemIndex, lvl + 1);
                        return;
                    }
                    break;
            }

            SingleIntegerEventsHolder.ShowToastEvent?.Invoke(0); // Not enough cash
        }
    }
}
