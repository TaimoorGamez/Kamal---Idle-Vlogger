using TMPro;
using UnityEngine;
using Core.Events;
using Core.Economy;

namespace Core.Screen
{
    public class CashBarController : MonoBehaviour
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
            CurrencyTxt.text = CashCurrency.Amount.ToString();
        }
    }
}
