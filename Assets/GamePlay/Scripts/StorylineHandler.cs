using UnityEngine;

namespace Core.GamePlay
{
    public class StorylineHandler : MonoBehaviour
    {
        [SerializeField] StoryLine[] Stories;

        public void CountinueStory(int storyIndex)
        {
            Stories[storyIndex].gameObject.SetActive(true);
        }
    }
}
