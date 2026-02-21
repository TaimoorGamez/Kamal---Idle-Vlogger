using TMPro;
using System;
using UnityEngine;
using Core.Events;
using Core.DB.Variables;
using System.Collections;

namespace Core.DailyReward
{
    public class DayChecker : MonoBehaviour
    {
        [SerializeField] GameObject DailyRewardView, NotificationObj;
        [SerializeField] TextMeshProUGUI RemainingTimeText, PanelTimer;

        Coroutine _timerRotine;

        private void OnEnable()
        {
            SimpleEventsHolder.UpDateDailyRewardState += ChangeDay;
        }

        private void OnDisable()
        {
            SimpleEventsHolder.UpDateDailyRewardState -= ChangeDay;
            
            if(_timerRotine != null)
            StopCoroutine(_timerRotine);
        }

        private void Start()
        {
            ChangeDay();
        }

        public void ChangeDay()
        {
            if (_timerRotine != null)
                StopCoroutine(_timerRotine);

            NotificationObj.SetActive(false);
            RemainingTimeText.gameObject.SetActive(false);
            DateTime currentDate = DateTime.Now;
            DateTime lastRewardDate = DateTime.Parse(DBVariablesHolder.LastDate.Value);
            int daysSinceLastReward = (currentDate - lastRewardDate).Days;
            int daysGreater = daysSinceLastReward / 1;

            if (daysGreater >= 1)
            {
                SimpleEventsHolder.GenerateDailyTasksEvent?.Invoke();
                SimpleEventsHolder.ResetSpinWheelEvent?.Invoke();
                DBVariablesHolder.RewardClaimed.Value = 0;
                NotificationObj.SetActive(true);
                if (daysGreater == 1 && DBVariablesHolder.ToDay.Value < 7)
                {
                    DBVariablesHolder.ToDay.Value = (DBVariablesHolder.ToDay.Value + 1);
                }
                else
                {
                    DBVariablesHolder.ToDay.Value = 1;
                }
                DailyRewardView.SetActive(true);
                DBVariablesHolder.LastDate.Value = (DateTime.Now.ToString());
            }
            else if (daysGreater < 1 && DBVariablesHolder.RewardClaimed.Value == 0)
            {
                NotificationObj.SetActive(true);
                DailyRewardView.SetActive(true);
            }
            else
            {
                RemainingTimeText.gameObject.SetActive(true);
                DateTime nextRewardDate = DateTime.Parse(DBVariablesHolder.LastDate.Value).AddDays(1);
                TimeSpan remainingTime = nextRewardDate - DateTime.Now;
                float secondsRemaining = (float)remainingTime.TotalSeconds;
                _timerRotine = StartCoroutine(UpdateRemainingTime(secondsRemaining));
            }
        }

        private IEnumerator UpdateRemainingTime(float seconds)
        {
            WaitForSeconds duration = new WaitForSeconds(2);
            while (seconds > 1)
            {
                RemainingTimeText.text = string.Format("{0:00}:{1:00}:{2:00}", Mathf.Floor(seconds / 3600), Mathf.Floor((seconds % 3600) / 60), Mathf.Floor(seconds % 60));
                PanelTimer.text = string.Format("{0:00}:{1:00}:{2:00}", Mathf.Floor(seconds / 3600), Mathf.Floor((seconds % 3600) / 60), Mathf.Floor(seconds % 60));
                seconds--;
                yield return duration;
            }

            if (seconds < 1)
            {
                ChangeDay();
            }
        }
    }

}
