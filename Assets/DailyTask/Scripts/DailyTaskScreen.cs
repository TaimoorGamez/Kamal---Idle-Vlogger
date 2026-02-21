using Core.Events;
using DG.Tweening;
using UnityEngine;
using Core.Economy;
using Core.DailyTasks;
using Core.Plugins.Firebase;

namespace Core.Screen
{
    public class DailyTaskScreen : UiScreens
    {
        [SerializeField] RectTransform BoxPanel, RewardFillBar;
        [SerializeField] TaskBar[] TaskBars;
        [SerializeField] RectTransform[] RewardImgs;
        [SerializeField] GameObject[] RewardChecks;
        [SerializeField] GameObject NotificationObj;

        DailyTaskData[] _activeTasks;

        float _rewardFillTime = 0.5f;
        int _taskClaimed = 0;

        void OnEnable()
        {
            UpdateTasks();
            OnOpen();
        }

        void UpdateTasks()
        {
            _activeTasks = TasksManager.I.GetActiveTasks();
            int _completedTasks = 0;
            _taskClaimed = 0;
            for (int i = 0; i < TaskBars.Length; i++)
            {
                TaskBars[i].SetTask(_activeTasks[i]);
                if(_activeTasks[i].Progress >= _activeTasks[i].Goal)
                {
                    _completedTasks++;
                    if (_activeTasks[i].TaskClaimed == 1)
                    {
                        RewardChecks[_taskClaimed].SetActive(true);
                        RewardImgs[_taskClaimed].gameObject.SetActive(false);
                        _taskClaimed++;
                    }
                }
                else
                {
                    RewardChecks[i].SetActive(false);
                    RewardImgs[i].gameObject.SetActive(true);
                }
            }
            RewardFillBar.DOScaleX((float)_completedTasks / TaskBars.Length, _rewardFillTime).SetEase(Ease.Linear);
        }

       public void ClaimReward(int taskIndex)
        {
            if(_activeTasks[taskIndex].Progress >= _activeTasks[taskIndex].Goal && _activeTasks[taskIndex].TaskClaimed == 0)
            {
                _activeTasks[taskIndex].TaskClaimed = 1;
                switch(_taskClaimed)
                {
                    case 0:
                        CurrenciesHolder.CashCurrency.Amount += 100;
                        break;
                    case 1:
                        CurrenciesHolder.CashCurrency.Amount += 150;
                        break;
                    case 2:
                        CurrenciesHolder.CashCurrency.Amount += 200;
                        break;
                    case 3:
                        BoxPanel.gameObject.SetActive(true);
                        break;
                }
                UpdateTasks();
                FirebaseHandler.I?.LogEvent($"DT_Claim_{_taskClaimed}");
            }
        }
        public override void OnOpen()
        {
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(3);
            Body.DOScale(0.9f, _transitionDuration).SetEase(Ease.OutBack);
            FirebaseHandler.I?.LogEvent("DT_Open");
        }
        public override void OnClose()
        {
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(2);
            Body.DOScale(0, _transitionDuration/2).SetEase(Ease.InBack).OnComplete(() => {
                NotificationObj.SetActive(false);
                gameObject.SetActive(false);
            });
            FirebaseHandler.I?.LogEvent("DT_Close");
        }
    }
}
