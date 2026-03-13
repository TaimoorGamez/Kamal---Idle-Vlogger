using TMPro;
using UnityEngine;
using Core.Events;
using Core.Economy;

namespace Core.Screen
{
    public class CurrencyBarController : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI CurrencyTxt;

        private void OnEnable()
        {
            SimpleEventsHolder.UpdateCashTxtEvent += UpdateCashText;
        }

        private void OnDisable()
        {
            SimpleEventsHolder.UpdateCashTxtEvent -= UpdateCashText;
        }

        void UpdateCashText()
        {
            CurrencyTxt.text = CurrenciesHolder.CashCurrency.Amount.ToString();
        }
    }
}
