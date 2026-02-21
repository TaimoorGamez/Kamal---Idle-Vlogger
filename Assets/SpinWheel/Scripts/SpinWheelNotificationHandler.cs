using UnityEngine;
using Core.Events;
using Core.DB.Variables;

namespace Core.Screen
{
    public class SpinWheelNotificationHandler : MonoBehaviour
    {
        [SerializeField] GameObject NotificationObj;

        private void OnEnable()
        {
            UpdateNotificationStatus();
            SimpleEventsHolder.ResetSpinWheelEvent += ResetSpinWheel;
        }

        private void OnDisable()
        {
            SimpleEventsHolder.ResetSpinWheelEvent -= ResetSpinWheel;
        }

        void ResetSpinWheel()
        {
            DBVariablesHolder.SpinAvailable.Value = 1;
            UpdateNotificationStatus();
        }

        void UpdateNotificationStatus()
        {
            NotificationObj.SetActive(DBVariablesHolder.SpinAvailable.Value == 1);
        }
    }
}
