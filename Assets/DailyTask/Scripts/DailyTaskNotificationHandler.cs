using UnityEngine;
using Core.DailyTasks;

namespace Core.Screen
{
    public class DailyTaskNotificationHandler : MonoBehaviour
    {
        [SerializeField] GameObject NotificationObj;

        private void OnEnable()
        {
            DailyTaskData[] activeTasks = TasksManager.I.GetActiveTasks();
            for (int i = 0; i < activeTasks.Length; i++)
            {
                if (activeTasks[i].Progress >= activeTasks[i].Goal && activeTasks[i].TaskClaimed != 1)
                {
                    NotificationObj.SetActive(true);
                    break;
                }
            }
        }
    }
}
