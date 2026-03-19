using Core.Events;
using Core.Economy;
using Core.DB.Variables;

namespace Core.GamePlay
{
    public class CameraUpdates : UpdateSystem
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateCost(0, DBVariablesHolder.CameraLvl.Value);
            UpdateCost(1, DBVariablesHolder.TripodLvl.Value);
            UpdateCost(2, DBVariablesHolder.MicrophoneLvl.Value);
        }

        public override void UpdateItem(int itemIndex)
        {

            int cost = -1, lvl = -1;
            switch (itemIndex)
            {
                case 0:
                    lvl = DBVariablesHolder.CameraLvl.Value;
                    cost = GetCost(lvl);

                    if (CashCurrency.Amount >= cost)
                    {
                        CashCurrency.Amount -= cost;
                        DBVariablesHolder.BasicIncome.Value += Increments[0];
                        DBVariablesHolder.CameraLvl.Value++;
                        UpdateCost(itemIndex, lvl + 1);
                        return;
                    }
                    break;

                case 1:
                    lvl = DBVariablesHolder.TripodLvl.Value;
                    cost = GetCost(lvl);

                    if (CashCurrency.Amount >= cost)
                    {
                        CashCurrency.Amount -= cost;
                        DBVariablesHolder.BasicIncome.Value += Increments[1];
                        DBVariablesHolder.TripodLvl.Value++;
                        UpdateCost(itemIndex, lvl + 1);
                        return;
                    }
                    break;

                case 2:
                    lvl = DBVariablesHolder.MicrophoneLvl.Value;
                    cost = GetCost(lvl);

                    if (CashCurrency.Amount >= cost)
                    {
                        CashCurrency.Amount -= cost;
                        DBVariablesHolder.BasicIncome.Value += Increments[2];
                        DBVariablesHolder.MicrophoneLvl.Value++;
                        UpdateCost(itemIndex, lvl + 1);
                        return;
                    }
                    break;
            }

            SingleIntegerEventsHolder.ShowToastEvent?.Invoke(0); // Not enough cash
        }
    }
}
