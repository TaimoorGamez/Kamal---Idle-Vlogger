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

                    if (CashCurrency.Amount >= cost)
                    {
                        CashCurrency.Amount -= cost;
                        DBVariablesHolder.BasicIncome.Value += Increments[0];
                        DBVariablesHolder.CharismaLvl.Value++;
                        UpdateCost(itemIndex, lvl + 1);
                        return;
                    }
                    break;

                case 1:
                    lvl = DBVariablesHolder.EruditionLvl.Value;
                    cost = GetCost(lvl);

                    if (CashCurrency.Amount >= cost)
                    {
                        CashCurrency.Amount -= cost;
                        DBVariablesHolder.BasicIncome.Value += Increments[1];
                        DBVariablesHolder.EruditionLvl.Value++;
                        UpdateCost(itemIndex, lvl + 1);
                        return;
                    }
                    break;

                case 2:
                    lvl = DBVariablesHolder.ImprovisationLvl.Value;
                    cost = GetCost(lvl);

                    if (CashCurrency.Amount >= cost)
                    {
                        CashCurrency.Amount -= cost;
                        DBVariablesHolder.BasicIncome.Value += Increments[2];
                        DBVariablesHolder.ImprovisationLvl.Value++;
                        UpdateCost(itemIndex, lvl + 1);
                        return;
                    }
                    break;


                case 3:
                    lvl = DBVariablesHolder.WitLvl.Value;
                    cost = GetCost(lvl);

                    if (CashCurrency.Amount >= cost)
                    {
                        CashCurrency.Amount -= cost;
                        DBVariablesHolder.BasicIncome.Value += Increments[3];
                        DBVariablesHolder.WitLvl.Value++;
                        UpdateCost(itemIndex, lvl + 1);
                        return;
                    }
                    break;
            }

            SingleIntegerEventsHolder.ShowToastEvent?.Invoke(0); // Not enough cash
        }
    }
}
