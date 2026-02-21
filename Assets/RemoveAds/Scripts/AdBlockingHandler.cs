using TMPro;
using System;
using UnityEngine;
using Core.Events;
using Core.Plugins;
using Core.DB.Variables;
using System.Collections;

namespace Core.Screen
{
    public class AdBlockingHandler : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI RemainingTimeText, PanelTimer;

        Coroutine _timerRotine;

        void OnEnable()
        {
            SimpleEventsHolder.AdsBlockerEvent += BlockAds;
        }

        private void OnDisable()
        {
            SimpleEventsHolder.AdsBlockerEvent -= BlockAds;
            if (_timerRotine != null)
            {
                StopCoroutine(_timerRotine);
            }
        }

        private void Start()
        {
            StartTimer();
        }

        void BlockAds()
        {
            DBVariablesHolder.AdBlockingTime.Value = DateTime.Now.ToString();
            DBVariablesHolder.AdBlocked.Value = 1;
            StartTimer();
        }

        void StartTimer()
        {
            if (DBVariablesHolder.AdBlocked.Value == 1)
            {
                DateTime lastDate = DateTime.Parse(DBVariablesHolder.AdBlockingTime.Value);
                TimeSpan passedTime = DateTime.Now - lastDate;
                float remainingSeconds = RemoteDataHolder.AdData.AdBlockTime - (float)passedTime.TotalSeconds;
                if (remainingSeconds > 0)
                {
                    _timerRotine = StartCoroutine(UpdateRemainingTime(remainingSeconds));
                    RemainingTimeText.gameObject.SetActive(true);
                }
                else
                {
                    DBVariablesHolder.AdBlocked.Value = 0;
                }
            }
        }

        private IEnumerator UpdateRemainingTime(float seconds)
        {
            WaitForSeconds duration = new WaitForSeconds(1);
            while (seconds > 0)
            {
                RemainingTimeText.text = string.Format("{0:00}:{1:00}", Mathf.Floor((seconds % 3600) / 60), Mathf.Floor(seconds % 60));
                PanelTimer.text = string.Format("{0:00}:{1:00}", Mathf.Floor((seconds % 3600) / 60),Mathf.Floor(seconds % 60));
                seconds--;
                yield return duration;
            }

            if (seconds < 1)
            {
                DBVariablesHolder.AdBlocked.Value = 0;
                RemainingTimeText.text = "Remove Ads";
            }
        }
    }
}