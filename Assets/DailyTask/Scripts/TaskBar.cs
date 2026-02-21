using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.DailyTasks
{
    public class TaskBar : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI TaskDescription, TaskProgressTxt;
        [SerializeField] Image TickImage, ProgressImg;
        [SerializeField] Color CompletedColor, IncompletedColor;
        [SerializeField] GameObject ClaimBtn, ProgressBar;


        public void SetTask(DailyTaskData taskData)
        {
            TaskDescription.text = taskData.Description;
            TaskProgressTxt.text = $"{taskData.Progress} / {taskData.Goal}";
            ProgressImg.fillAmount = (float)taskData.Progress / taskData.Goal;
            
            if (taskData.Progress >= taskData.Goal)
            {
                TickImage.color = CompletedColor;
                if (taskData.TaskClaimed == 0)
                {
                    ProgressBar.SetActive(false);
                    TaskDescription.gameObject.SetActive(false);
                    ClaimBtn.SetActive(true);
                }
                else
                {
                    ClaimBtn.SetActive(false);
                    ProgressBar.SetActive(true);
                    TaskDescription.gameObject.SetActive(true);
                }
            }
            else
            {
                TickImage.color = IncompletedColor;
                ClaimBtn.SetActive(false);
                TaskDescription.gameObject.SetActive(true);
                ProgressBar.SetActive(true);
            }
        }
    }
}
