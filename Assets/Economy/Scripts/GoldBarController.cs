using TMPro;
using UnityEngine;
using Core.Events;
using Core.Economy;

namespace Core.Screen
{
    public class GoldBarController : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI CurrencyTxt;

        private void OnEnable()
        {
            SimpleEventsHolder.UpdateGoldTxtEvent += UpdateCashText;
        }

        private void OnDisable()
        {
            SimpleEventsHolder.UpdateGoldTxtEvent -= UpdateCashText;
        }

        void UpdateCashText()
        {
            CurrencyTxt.text = GoldCurrency.Amount.ToString();
        }
    }
}
