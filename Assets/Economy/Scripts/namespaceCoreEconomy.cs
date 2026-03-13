using System;
using Core.Events;
using Core.DB.Variables;
using System.Collections.Generic;

namespace Core.Economy
{
    public class Currencies
    {
        public DBInt CurrencyWallet;

        public Currencies(DBInt wallet)
        {
            CurrencyWallet = wallet;
        }

        public virtual int Amount
        {
            get
            {
                return CurrencyWallet.Value;
            }
            set
            {
                CurrencyWallet.Value = (value);
                SimpleEventsHolder.UpdateCashTxtEvent.Invoke();
                if (value > CurrencyWallet.Value)
                {
                }
                else if (value < CurrencyWallet.Value)
                {
                    DoubleIntegerEventHolder.TaskEvent?.Invoke(2, value);
                }
            }
        }
    }

    public static class CurrenciesHolder 
    {
        public static Currencies CashCurrency = new Currencies(DBVariablesHolder.CashWallet);
    }
}
