using Core.Events;
using Core.DB.Variables;

namespace Core.Economy
{
    public static class CashCurrency
    {

        public static float Amount
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
                    //DoubleIntegerEventHolder.TaskEvent?.Invoke(2, value)
                }
                DBVariablesHolder.CashWallet.Value = value;
                SimpleEventsHolder.UpdateCashTxtEvent?.Invoke();
            }
        }
    }

    public static class GoldCurrency
    {
        public static int Amount
        {
            get
            {
                return DBVariablesHolder.GoldWallet.Value;
            }
            set
            {
                if (value > DBVariablesHolder.GoldWallet.Value)
                {
                }
                else if (value < DBVariablesHolder.GoldWallet.Value)
                {
                    //DoubleIntegerEventHolder.TaskEvent?.Invoke(2, value);
                }
                DBVariablesHolder.GoldWallet.Value = value;
                SimpleEventsHolder.UpdateGoldTxtEvent?.Invoke();
            }
        }
    }

    public static class Subscribers
    {
        public static int Amount
        {
            get
            {
                return DBVariablesHolder.SubscribeWallet.Value;
            }
            set
            {
                if (value > DBVariablesHolder.SubscribeWallet.Value)
                {
                }
                else if (value < DBVariablesHolder.SubscribeWallet.Value)
                {
                    //DoubleIntegerEventHolder.TaskEvent?.Invoke(2, value);
                }
                DBVariablesHolder.SubscribeWallet.Value = value;
                SimpleEventsHolder.UpdateSubscribeTxtEvent?.Invoke();
            }
        }
    }
}
