using TMPro;
using UnityEngine;
using Core.Events;
using Core.Economy;

namespace Core.Screen
{
    public class SubscriberBarController : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI CurrencyTxt;

        private void OnEnable()
        {
            SimpleEventsHolder.UpdateSubscribeTxtEvent += UpdateCashText;
        }

        private void OnDisable()
        {
            SimpleEventsHolder.UpdateSubscribeTxtEvent -= UpdateCashText;
        }

        void UpdateCashText()
        {
            CurrencyTxt.text = Subscribers.Amount.ToString();
        }
    }
}
