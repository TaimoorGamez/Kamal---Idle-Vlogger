using UnityEngine;

namespace Core.GamePlay
{
    public class StorylineHandler : MonoBehaviour
    {
        [SerializeField] CurtainController CurrentCurtainController;

        public void CountinueStory(int storyIndex)
        {
            CurrentCurtainController.gameObject.SetActive(true);
        }
    }
}
