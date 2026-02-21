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
                if (value > CurrencyWallet.Value)
                {
                    SingleIntegerEventsHolder.DepositEvent?.Invoke(value);
                }
                else if (value < CurrencyWallet.Value)
                {
                    SingleIntegerEventsHolder.TransactionEvent?.Invoke(value);
                    DoubleIntegerEventHolder.TaskEvent?.Invoke(2, value);
                }
                CurrencyWallet.Value = (value);
            }
        }
    }

    public static class CurrenciesHolder 
    {
        public static Currencies CashCurrency = new Currencies(DBVariablesHolder.CashWallet);
    }

    public static class CurrencyDictionariesHolder 
    {
        public static Dictionary<string, Currencies> AllCurrencies = new Dictionary<string, Currencies>(StringComparer.Ordinal)
        {
        };
    }
}
