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
        }

        protected override void UpdatePriceForAll()
        {
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
                    if (CashCurrency.Amount >= _priceData[itemIndex].Cost)
                    {
                        CashCurrency.Amount -= _priceData[itemIndex].Cost;
                        DBVariablesHolder.BasicIncome.Value += Increments[0] * _priceData[itemIndex].Levels;
                        DBVariablesHolder.CameraLvl.Value += _priceData[itemIndex].Levels;
                        if (DBVariablesHolder.MaxLevels.Value > 0)
                        {
                            UpdatePriceForAll();
                        }
                        else
                        {
                            UpdateCost(itemIndex, DBVariablesHolder.CameraLvl.Value);
                        }
                        return;
                    }
                    break;

                case 1:
                    lvl = DBVariablesHolder.TripodLvl.Value;
                    cost = GetCost(lvl);
                    if (CashCurrency.Amount >= _priceData[itemIndex].Cost)
                    {
                        CashCurrency.Amount -= _priceData[itemIndex].Cost;
                        DBVariablesHolder.BasicIncome.Value += Increments[0] * _priceData[itemIndex].Levels;
                        DBVariablesHolder.TripodLvl.Value += _priceData[itemIndex].Levels;
                        if (DBVariablesHolder.MaxLevels.Value > 0)
                        {
                            UpdatePriceForAll();
                        }
                        else
                        {
                            UpdateCost(itemIndex, DBVariablesHolder.TripodLvl.Value);
                        }
                        return;
                    }
                    break;

                case 2:
                    lvl = DBVariablesHolder.MicrophoneLvl.Value;
                    cost = GetCost(lvl);
                    if (CashCurrency.Amount >= _priceData[itemIndex].Cost)
                    {
                        CashCurrency.Amount -= _priceData[itemIndex].Cost;
                        DBVariablesHolder.BasicIncome.Value += Increments[0] * _priceData[itemIndex].Levels;
                        DBVariablesHolder.MicrophoneLvl.Value += _priceData[itemIndex].Levels;
                        if (DBVariablesHolder.MaxLevels.Value > 0)
                        {
                            UpdatePriceForAll();
                        }
                        else
                        {
                            UpdateCost(itemIndex, DBVariablesHolder.MicrophoneLvl.Value);
                        }
                        return;
                    }
                    break;
            }

            SingleIntegerEventsHolder.ShowToastEvent?.Invoke(0); // Not enough cash
        }
    }
}
