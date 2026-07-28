using TMPro;
using UnityEngine;
using Core.Events;


namespace Core.GamePlay
{
    public class NotificationHandler : MonoBehaviour
    {
        [SerializeField] GameObject NotificationObj;
        [SerializeField] TextMeshProUGUI NotificationText;
        [SerializeField] UpdateSystem UpdatePanel;

        private void OnEnable()
        {
            SimpleEventsHolder.UpdateCashTxtEvent += UpdateNotification;
        }

        private void OnDisable()
        {
            SimpleEventsHolder.UpdateCashTxtEvent -= UpdateNotification;
        }

        void UpdateNotification()
        {
            int availableUpdates = UpdatePanel.GetAvailableUpdates();
            if (availableUpdates > 0)
            {
                NotificationText.text = availableUpdates.ToString();
                NotificationObj.SetActive(true);
            }
            else
            {
                NotificationObj.SetActive(false);
            }
        }
    }
}
