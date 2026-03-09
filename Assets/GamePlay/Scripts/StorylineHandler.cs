using UnityEngine;

namespace Core.GamePlay
{
    public class StorylineHandler : MonoBehaviour
    {
        [SerializeField] CurtainController CurrentCurtainController;
        [SerializeField] StoryLine[] Stories;

        public void CountinueStory(int storyIndex)
        {
            CurrentCurtainController.gameObject.SetActive(true);
            CurrentCurtainController.CloseCurtains();
            Stories[storyIndex].gameObject.SetActive(true);
        }
    }
}
