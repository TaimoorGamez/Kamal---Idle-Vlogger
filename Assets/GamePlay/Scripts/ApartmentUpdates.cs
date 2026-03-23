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
        }

        protected override void UpdatePriceForAll()
        {
            UpdateCost(0, DBVariablesHolder.HouseLvl.Value);
            UpdateCost(1, DBVariablesHolder.VehicleLvl.Value);
            UpdateCost(2, DBVariablesHolder.StatueLvl.Value);
            UpdateCost(3, DBVariablesHolder.BackyardLvl.Value);
            UpdateCost(4, DBVariablesHolder.GroundLvl.Value);
        }

        public override void UpdateItem(int itemIndex)
        {
            int cost = -1, lvl = -1;
            switch (itemIndex)
            {
                case 0:
                    lvl = DBVariablesHolder.HouseLvl.Value;
                    cost = GetCost(lvl);
                    if (CashCurrency.Amount >= _priceData[itemIndex].Cost)
                    {
                        CashCurrency.Amount -= _priceData[itemIndex].Cost;
                        DBVariablesHolder.BasicIncome.Value += Increments[0] * _priceData[itemIndex].Levels;
                        DBVariablesHolder.HouseLvl.Value += _priceData[itemIndex].Levels;
                        if (DBVariablesHolder.MaxLevels.Value > 0)
                        {
                            UpdatePriceForAll();
                        }
                        else
                        {
                            UpdateCost(itemIndex, DBVariablesHolder.HouseLvl.Value);
                        }
                        return;
                    }
                    break;

                case 1:
                    lvl = DBVariablesHolder.VehicleLvl.Value;
                    cost = GetCost(lvl);
                    if (CashCurrency.Amount >= _priceData[itemIndex].Cost)
                    {
                        CashCurrency.Amount -= _priceData[itemIndex].Cost;
                        DBVariablesHolder.BasicIncome.Value += Increments[0] * _priceData[itemIndex].Levels;
                        DBVariablesHolder.VehicleLvl.Value += _priceData[itemIndex].Levels;
                        if (DBVariablesHolder.MaxLevels.Value > 0)
                        {
                            UpdatePriceForAll();
                        }
                        else
                        {
                            UpdateCost(itemIndex, DBVariablesHolder.VehicleLvl.Value);
                        }
                        return;
                    }
                    break;

                case 2:
                    lvl = DBVariablesHolder.StatueLvl.Value;
                    cost = GetCost(lvl);
                    if (CashCurrency.Amount >= _priceData[itemIndex].Cost)
                    {
                        CashCurrency.Amount -= _priceData[itemIndex].Cost;
                        DBVariablesHolder.BasicIncome.Value += Increments[0] * _priceData[itemIndex].Levels;
                        DBVariablesHolder.StatueLvl.Value += _priceData[itemIndex].Levels;
                        if (DBVariablesHolder.MaxLevels.Value > 0)
                        {
                            UpdatePriceForAll();
                        }
                        else
                        {
                            UpdateCost(itemIndex, DBVariablesHolder.StatueLvl.Value);
                        }
                        return;
                    }
                    break;

                case 3:
                    lvl = DBVariablesHolder.BackyardLvl.Value;
                    cost = GetCost(lvl);
                    if (CashCurrency.Amount >= _priceData[itemIndex].Cost)
                    {
                        CashCurrency.Amount -= _priceData[itemIndex].Cost;
                        DBVariablesHolder.BasicIncome.Value += Increments[0] * _priceData[itemIndex].Levels;
                        DBVariablesHolder.BackyardLvl.Value += _priceData[itemIndex].Levels;
                        if (DBVariablesHolder.MaxLevels.Value > 0)
                        {
                            UpdatePriceForAll();
                        }
                        else
                        {
                            UpdateCost(itemIndex, DBVariablesHolder.BackyardLvl.Value);
                        }
                        return;
                    }
                    break;

                case 4:
                    lvl = DBVariablesHolder.GroundLvl.Value;
                    cost = GetCost(lvl);
                    if (CashCurrency.Amount >= _priceData[itemIndex].Cost)
                    {
                        CashCurrency.Amount -= _priceData[itemIndex].Cost;
                        DBVariablesHolder.BasicIncome.Value += Increments[0] * _priceData[itemIndex].Levels;
                        DBVariablesHolder.GroundLvl.Value += _priceData[itemIndex].Levels;
                        if (DBVariablesHolder.MaxLevels.Value > 0)
                        {
                            UpdatePriceForAll();
                        }
                        else
                        {
                            UpdateCost(itemIndex, DBVariablesHolder.GroundLvl.Value);
                        }
                        return;
                    }
                    break;
            }

            SingleIntegerEventsHolder.ShowToastEvent?.Invoke(0); // Not enough cash
        }
    }
}
