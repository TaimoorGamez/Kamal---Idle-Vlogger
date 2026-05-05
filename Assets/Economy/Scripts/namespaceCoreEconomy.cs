using Core.Events;
using Core.DB.Variables;
using UnityEngine;

namespace Core.Economy
{
    public static class CashCurrency
    {
        static string _cashPrefName = "CashWallet";
        static double _amount;

        public static double Amount
        {
            get
            {
                return _amount;
            }
            set
            {
                _amount = value;
                PlayerPrefs.SetString(_cashPrefName, _amount.ToString());
                SimpleEventsHolder.UpdateCashTxtEvent?.Invoke();
            }
        }

        public static void LoadEconomy()
        {
            _amount = double.Parse(PlayerPrefs.GetString(_cashPrefName, "0"));
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
        static string _subscriberPrefName = "SubscribeWallet";
        static double _amount;

        public static double Amount
        {
            get
            {
                return _amount;
            }
            set
            {
                _amount = value;
                PlayerPrefs.SetString(_subscriberPrefName, _amount.ToString());
                SimpleEventsHolder.UpdateSubscribeTxtEvent?.Invoke();
            }
        }

        public static void LoadSubscribers()
        {
            _amount = double.Parse(PlayerPrefs.GetString(_subscriberPrefName, "0"));
        }
    }
}
