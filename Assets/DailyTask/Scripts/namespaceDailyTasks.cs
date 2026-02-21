using UnityEngine;

namespace Core.DailyTasks
{
    public class DailyTaskData
    {
        int TaskId;
        public string Description;
        public int Goal;

        public DailyTaskData(int taskId, string description, int goal)
        {
            TaskId = taskId;
            Goal = goal;
            Description = description;
        }

        public int Progress
        {
            get => PlayerPrefs.GetInt("Progress" + TaskId, 0);
            set
            {
                if (value > -1)
                {
                    if (value > Goal)
                        value = Goal;

                    PlayerPrefs.SetInt("Progress" + TaskId, value);
                    PlayerPrefs.Save();
                }
            }
        }

        public int TaskClaimed
        {
            get => PlayerPrefs.GetInt("TaskClaimed" + TaskId, 0);
            set
            {
                if (value == 0 || value == 1)
                {
                    PlayerPrefs.SetInt("TaskClaimed" + TaskId, value);
                    PlayerPrefs.Save();
                }
            }
        }
    }
}