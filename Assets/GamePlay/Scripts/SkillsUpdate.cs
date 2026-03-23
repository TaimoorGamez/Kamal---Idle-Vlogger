using Core.Events;
using Core.Economy;
using Core.DB.Variables;

namespace Core.GamePlay
{
    public class SkillsUpdate : UpdateSystem
    {
        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void UpdatePriceForAll()
        {
            UpdateCost(0, DBVariablesHolder.CharismaLvl.Value);
            UpdateCost(1, DBVariablesHolder.EruditionLvl.Value);
            UpdateCost(2, DBVariablesHolder.ImprovisationLvl.Value);
            UpdateCost(3, DBVariablesHolder.WitLvl.Value);
        }

        public override void UpdateItem(int itemIndex)
        {
            int cost = -1, lvl = -1;
            switch (itemIndex)
            {
                case 0:
                    lvl = DBVariablesHolder.CharismaLvl.Value;
                    cost = GetCost(lvl);
                    if (CashCurrency.Amount >= _priceData[itemIndex].Cost)
                    {
                        CashCurrency.Amount -= _priceData[itemIndex].Cost;
                        DBVariablesHolder.BasicIncome.Value += Increments[0] * _priceData[itemIndex].Levels;
                        DBVariablesHolder.CharismaLvl.Value += _priceData[itemIndex].Levels;
                        if (DBVariablesHolder.MaxLevels.Value > 0)
                        {
                            UpdatePriceForAll();
                        }
                        else
                        {
                            UpdateCost(itemIndex, DBVariablesHolder.CharismaLvl.Value);
                        }
                        return;
                    }
                    break;

                case 1:
                    lvl = DBVariablesHolder.EruditionLvl.Value;
                    cost = GetCost(lvl);
                    if (CashCurrency.Amount >= _priceData[itemIndex].Cost)
                    {
                        CashCurrency.Amount -= _priceData[itemIndex].Cost;
                        DBVariablesHolder.BasicIncome.Value += Increments[0] * _priceData[itemIndex].Levels;
                        DBVariablesHolder.EruditionLvl.Value += _priceData[itemIndex].Levels;
                        if (DBVariablesHolder.MaxLevels.Value > 0)
                        {
                            UpdatePriceForAll();
                        }
                        else
                        {
                            UpdateCost(itemIndex, DBVariablesHolder.EruditionLvl.Value);
                        }
                        return;
                    }
                    break;

                case 2:
                    lvl = DBVariablesHolder.ImprovisationLvl.Value;
                    cost = GetCost(lvl);
                    if (CashCurrency.Amount >= _priceData[itemIndex].Cost)
                    {
                        CashCurrency.Amount -= _priceData[itemIndex].Cost;
                        DBVariablesHolder.BasicIncome.Value += Increments[0] * _priceData[itemIndex].Levels;
                        DBVariablesHolder.ImprovisationLvl.Value += _priceData[itemIndex].Levels;
                        if (DBVariablesHolder.MaxLevels.Value > 0)
                        {
                            UpdatePriceForAll();
                        }
                        else
                        {
                            UpdateCost(itemIndex, DBVariablesHolder.ImprovisationLvl.Value);
                        }
                        return;
                    }
                    break;


                case 3:
                    lvl = DBVariablesHolder.WitLvl.Value;
                    cost = GetCost(lvl);
                    if (CashCurrency.Amount >= _priceData[itemIndex].Cost)
                    {
                        CashCurrency.Amount -= _priceData[itemIndex].Cost;
                        DBVariablesHolder.BasicIncome.Value += Increments[0] * _priceData[itemIndex].Levels;
                        DBVariablesHolder.WitLvl.Value += _priceData[itemIndex].Levels;
                        if (DBVariablesHolder.MaxLevels.Value > 0)
                        {
                            UpdatePriceForAll();
                        }
                        else
                        {
                            UpdateCost(itemIndex, DBVariablesHolder.WitLvl.Value);
                        }
                        return;
                    }
                    break;
            }

            SingleIntegerEventsHolder.ShowToastEvent?.Invoke(0); // Not enough cash
        }
    }
}
