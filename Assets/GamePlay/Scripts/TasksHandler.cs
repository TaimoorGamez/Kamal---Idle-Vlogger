using TMPro;
using UnityEngine;
using DG.Tweening;
using Core.Events;
using Core.Economy;
using Core.DB.Variables;

namespace Core.GamePlay.Tasks
{
    public class TasksHandler : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI NotificationTxt;
        [SerializeField] GameObject TaskPanel, NotificationObj;
        [SerializeField] TaskBox[] TaskBoxes;

        TaskData[] _activeTasks;
        string _dataKey = "Task_";
        int _totalTasks = 9;
        float _tweenDuration = 0.25f;

        private void OnEnable()
        {
            DoubleIntegerEventHolder.TaskEvent += UpdateTaskProgress;
        }

        private void OnDisable()
        {
            DoubleIntegerEventHolder.TaskEvent -= UpdateTaskProgress;
        }

        private void Start()
        {
            _activeTasks = new TaskData[3];
            for (int i = 0; i < _activeTasks.Length; i++)
            {
                if (PlayerPrefs.HasKey($"{_dataKey}{i}"))
                {
                    _activeTasks[i] = JsonDB.Load<TaskData>($"{_dataKey}{i}");
                }
                else
                {
                    _activeTasks[i] = GetNewTask();
                    JsonDB.Save($"{_dataKey}{i}", _activeTasks[i]);
                }
            }
            UpdateTasks();
        }

        void UpdateTasks()
        {
            for (int i = 0; i < _activeTasks.Length; i++)
            {
                TaskData task = _activeTasks[i];
                TaskBoxes[i].DescriptionTxt.text = task.TaskDescription;
                TaskBoxes[i].RewardTxt.text = GameManager.Instance.FormatMoney(task.Reward);
            }
            UpdateNotification();
        }
        TaskData GetNewTask()
        {
            int index = Random.Range(0, _totalTasks);
            while (IsTaskBlocked(index))
            {
                index++;
                if (index >= _totalTasks)
                    index = 0;
            }

            float target = GetTarget(index);
            string description = GetTaskDescription(index, target);
            float reward = GetRewardAmount(index, target);

            return new TaskData
            {
                TaskIndex = index,
                TaskDescription = description,
                Progress = 0,
                Target = target,
                Reward = reward,
                IsCompleted = false
            };
        }
        float GetTarget(int taskIndex)
        {
            float scale = Mathf.Max(1f, DBVariablesHolder.BasicIncome.Value * 0.1f);

            switch (taskIndex)
            {
                case 0: return scale * 10f;   // Earn Cash
                case 1: return scale * 2f; // Increase Income Upto
                case 2: return scale * 5f; // Spend Cash
                case 3: return Random.Range(3, 10); // Collect Donations
                case 4: return Random.Range(10, 30); // Hold Tap Income For Sec
                case 5: return Random.Range(10, 25); // Tap For Times
                case 6: return 1; // Activate 2x Income
                case 7: return 1; // Activate 2x Tap
                case 8: return 1; // Use 10x Booster
                case 9: return 1; // Upgrade 10x Booster Duration
                case 10: return 1; // Upgrade 2x Income Duration
                case 11: return 1; // Upgrade 2x Tap Duration
                case 12: return 1; // Upgrade Donations Limit

                default: return 1;
            }
        }
        string GetTaskDescription(int taskIndex, float target)
        {
            switch (taskIndex)
            {
                case 0: return $"Earn Cash By Tapping {target}";
                case 1: return $"Increase Income To {target}";
                case 2: return $"Spend Cash {target}";
                case 3: return $"Collect Donations {target} Times";
                case 4: return $"Hold Tap Income For {target} Sec";
                case 5: return $"Tap {target} Times";
                case 6: return $"Activate 2x Income";
                case 7: return $"Activate 2x Tap";
                case 8: return $"Use 10x Booster";
                case 9: return $"Upgrade 10x Booster Duration";
                case 10: return $"Upgrade 2x Income Duration";
                case 11: return $"Upgrade 2x Tap Duration";
                case 12: return $"Upgrade Donations Limit";

                default: return "Complete Task";
            }
        }
        float GetRewardAmount(int taskIndex, float target)
        {
            float income = DBVariablesHolder.BasicIncome.Value;
            switch (taskIndex)
            {
                case 0: return target * 0.25f;   // Earn Cash
                case 1: return target * 0.5f; // Increase Income Upto
                case 2: return target * 1.2f; // Spend Cash
                case 3: return target * income; // Collect Donations
                case 4: return target * income; // Hold Tap Income For Sec
                case 5: return target * income; // Tap For Times
                case 6: return income * 5; // Activate 2x Income
                case 7: return income * 5; // Activate 2x Tap
                case 8: return income * 5; // Use 10x Booster
                case 9: return income * 5; // Upgrade 10x Duration
                case 10: return income * 5; // Upgrade 2x Income Duration
                case 11: return income * 5; // Upgrade 2x Tap Duration
                case 12: return income * 5; // Upgrade Donations Limit

                default: return income;
            }
        }
        bool IsTaskBlocked(int index)
        {
            for (int i = 0; i < _activeTasks.Length; i++)
            {
                TaskData task = _activeTasks[i];
                if (task != null && task.TaskIndex == index)
                    return true;
            }
            //switch(index)
            //{
            //    case 6: // Activate 2x Income
            //        return _activeTasks.Any(t => t != null && (t.TaskIndex == 7 || t.TaskIndex == 8));
            //    case 7: // Activate 2x Tap
            //        return _activeTasks.Any(t => t != null && (t.TaskIndex == 6 || t.TaskIndex == 8));
            //    case 8: // Use 10x Booster
            //        return _activeTasks.Any(t => t != null && (t.TaskIndex == 6 || t.TaskIndex == 7));
            //    case 9: // Upgrade 10x Duration
            //        return !_activeTasks.Any(t => t != null && t.TaskIndex == 8);
            //    case 10: // Upgrade 2x Income Duration
            //        return !_activeTasks.Any(t => t != null && t.TaskIndex == 6);
            //    case 11: // Upgrade 2x Tap Duration
            //        return !_activeTasks.Any(t => t != null && t.TaskIndex == 7);
            //    case 12: // Upgrade Donations Limit
            //        return !_activeTasks.Any(t => t != null && t.TaskIndex == 3);
            //}

            return false;
        }
        void UpdateTaskProgress(int index, double progress)
        {
            TaskData task = null;
            int slotIndex = -1;
            for (int i = 0; i < _activeTasks.Length; i++)
            {
                if (_activeTasks[i] != null && _activeTasks[i].TaskIndex == index)
                {
                    task = _activeTasks[i];
                    slotIndex = i;
                    break;
                }
            }

            if (task == null) return;

            if(task.TaskIndex == 0 || task.TaskIndex == 2 || task.TaskIndex == 3 || task.TaskIndex == 4 || task.TaskIndex == 5)
            {
                task.Progress += progress;
            }
            else
            {
                task.Progress = progress;
            }

            if (task.Progress >= task.Target)
            {
                task.IsCompleted = true;
                task.Progress = task.Target; 
                UpdateNotification();
            }
            JsonDB.Save($"{_dataKey}{slotIndex}", _activeTasks[slotIndex]);
        }

        public void ShowTaskPanel()
        {
            TaskPanel.SetActive(true);
            TaskPanel.transform.DOScale(Vector3.one, _tweenDuration).From(Vector3.zero).SetEase(Ease.OutBack)
                .OnComplete(() => 
                {
                    for (int i = 0; i < _activeTasks.Length; i++)
                    {
                        TaskData task = _activeTasks[i];
                        if (task.IsCompleted)
                        {
                            TaskBoxes[i].ProgressBar.fillAmount = 1f;
                            TaskBoxes[i].InActiveBtn.SetActive(false);
                            TaskBoxes[i].ActiveBtn.SetActive(true);
                            TaskBoxes[i].ActiveBtn.transform.DOScale(1.1f, _tweenDuration).From(1).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
                        }
                        else
                        {
                            TaskBoxes[i].ProgressBar.DOFillAmount((float)(task.Progress / task.Target), _tweenDuration).SetEase(Ease.Linear);
                            TaskBoxes[i].InActiveBtn.SetActive(true);
                            TaskBoxes[i].ActiveBtn.SetActive(false);
                        }
                    }
                });
        }
        public void HideTaskPanel()
        {
            TaskPanel.transform.DOScale(Vector3.zero, _tweenDuration).SetEase(Ease.InBack).OnComplete(() => TaskPanel.SetActive(false));
        }
        public void ClaimReward(int index)
        {
            TaskData task = _activeTasks[index];
            if (!task.IsCompleted) return;

            CashCurrency.Amount+= task.Reward;
            task.IsCompleted = false;
            UpdateNotification();

            TaskData newTask = GetNewTask();
            TaskBoxes[index].ProgressBar.fillAmount = 0;
            TaskBoxes[index].ActiveBtn.transform.DOKill();
            TaskBoxes[index].InActiveBtn.SetActive(true);
            TaskBoxes[index].ActiveBtn.SetActive(false);
            _activeTasks[index] = newTask;
            JsonDB.Save($"{_dataKey}{index}", _activeTasks[index]);
            TaskBoxes[index].DescriptionTxt.text = newTask.TaskDescription;
            TaskBoxes[index].RewardTxt.text = GameManager.Instance.FormatMoney(newTask.Reward);
        }
        
        void UpdateNotification()
        {
            NotificationObj.SetActive(false);
            int completedTasks = 0;
            for (int i = 0; i < _activeTasks.Length; i++)
            {
                TaskData task = _activeTasks[i];
                if (task != null && task.IsCompleted)
                {
                    completedTasks++;
                    NotificationTxt.text = completedTasks.ToString();
                    NotificationObj.SetActive(true);
                }
            }
        }
    }

    public class TaskData
    {
        public int TaskIndex;
        public string TaskDescription;
        public double Progress, Target, Reward;
        public bool IsCompleted;
    }
}
