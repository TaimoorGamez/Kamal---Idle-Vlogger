using UnityEngine;

namespace Core.GamePlay
{
    public class StorylineHandler : MonoBehaviour
    {
        [SerializeField] StoryLine[] Stories;

        int _storyIndex = -1;

        public void CountinueStory(int storyIndex)
        {
            _storyIndex = storyIndex;
            Stories[_storyIndex].gameObject.SetActive(true);
        }

        public void Next()
        {
            Stories[_storyIndex].NextMsg();
        }
    }
}
