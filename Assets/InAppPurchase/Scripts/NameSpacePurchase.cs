using System;
using UnityEngine;
using Core.Events;
using Core.Economy;
using Core.DB.Variables;
using Core.Plugins.Firebase;

namespace Core.Purchase
{

    [Serializable]
    public class StoreProduct
    {
        public string ProductID;

        public virtual void BuyProduct()
        {

        }
    }

    [Serializable]
    public class NonConsumableProduct : StoreProduct
    {
        public override void BuyProduct()
        {
            DBVariableDictionariesHolder.NonConsumableProductsData[ProductID].Value = 1;
            NonConsumableProductsEventsHandler.I.TriggerEvent(ProductID);
            FirebaseHandler.I?.LogEvent($"IAP_{ProductID}");
        }

    }

    [Serializable]
    public class ConsumableProduct : StoreProduct
    {
        public int Amount;
        public string CurrencyName;

        public override void BuyProduct()
        {
            CurrencyDictionariesHolder.AllCurrencies[CurrencyName].Amount += Amount;
            FirebaseHandler.I?.LogEvent($"IAP_{CurrencyName}_Amt{Amount}");
        }
    }
}

