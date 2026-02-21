using Core.DB.Variables;
using Core.Events;
using Core.Plugins.Firebase;
using System.Collections.Generic;
using UnityEngine;

namespace Core.DailyTasks
{
    public class TasksManager: MonoBehaviour
    {
        public static TasksManager I { get; private set; }

        DailyTaskData[] AllTasks = new DailyTaskData[]
        {
            new DailyTaskData(0, "Watch 2 Rewarded Ads", 2),
            new DailyTaskData(1, "Complete 3 levels", 3),
            new DailyTaskData(2, "Spend 200 coins", 200),
            new DailyTaskData(3, "Use Undo 3 times", 3),
            new DailyTaskData(4, "Use Color Swap 2 times", 2),
            new DailyTaskData(5, "Use Extra Bottle once", 1)
        };

        int _totalTasks = 4;
        DailyTaskData[] _activeTasks;

        private void OnEnable()
        {
            RetrevePreviousTasks();
            SimpleEventsHolder.GenerateDailyTasksEvent += GenerateDailyTasks;
            DoubleIntegerEventHolder.TaskEvent += AddTaskProgress;
        }

        private void OnDisable()
        {
            SimpleEventsHolder.GenerateDailyTasksEvent -= GenerateDailyTasks;
            DoubleIntegerEventHolder.TaskEvent -= AddTaskProgress;
        }

        private void Start()
        {
            if (I == null)
            {
                I = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        void GenerateDailyTasks()
        {
            _activeTasks = new DailyTaskData[_totalTasks];
            DBVariableDictionariesHolder.TaskIndexies[0].Value = 0;
            AllTasks[0].Progress = 0;
            AllTasks[0].TaskClaimed = 0;
            _activeTasks[0] = AllTasks[0];

            List<int> availableIndexes = new List<int>();
            for (int i = 1; i < AllTasks.Length; i++)
            {
                AllTasks[i].Progress = 0;
                AllTasks[i].TaskClaimed = 0;
                availableIndexes.Add(i); 
            }


            for (int i = 1; i < _totalTasks; i++)
            {
                int rand = Random.Range(0, availableIndexes.Count);
                int chosenIndex = availableIndexes[rand];

                DBVariableDictionariesHolder.TaskIndexies[i].Value = chosenIndex;
                _activeTasks[i] = AllTasks[chosenIndex];
                availableIndexes.RemoveAt(rand);
            }
        }

        void RetrevePreviousTasks()
        {
            _activeTasks = new DailyTaskData[_totalTasks];
            for (int i = 0; i < _totalTasks; i++)
            {
                int index = DBVariableDictionariesHolder.TaskIndexies[i].Value;
                _activeTasks[i] = AllTasks[index];
            }
        }

        public DailyTaskData[] GetActiveTasks()
        {
            return _activeTasks;
        }

        void AddTaskProgress(int index, int progress)
        {
            for (int t = 0; t < _totalTasks; t++) 
            {
                if (DBVariableDictionariesHolder.TaskIndexies[t].Value == index)
                {
                    _activeTasks[t].Progress += progress;
                    FirebaseHandler.I?.LogEvent($"DT_ID_{index}_Prog_{progress}");
                }
            }
        }
    }
}
