using Core.Events;
using Core.DB.Variables;

namespace Core.Economy
{
    public static class CashCurrency
    {

        public static int Amount
        {
            get
            {
                return DBVariablesHolder.CashWallet.Value;
            }
            set
            {
                if (value > DBVariablesHolder.CashWallet.Value)
                {
                }
                else if (value < DBVariablesHolder.CashWallet.Value)
                {
                    DoubleIntegerEventHolder.TaskEvent?.Invoke(2, value);
                }
                DBVariablesHolder.CashWallet.Value = value;
                SimpleEventsHolder.UpdateCashTxtEvent.Invoke();
            }
        }
    }
}
