using Core.DB.Variables;
using Core.Events;
using UnityEngine;

namespace Core.GamePlay
{
    public class StorylineHandler : MonoBehaviour
    {
        [SerializeField] StoryLine[] Stories;

        int _storyIndex = -1;

        private void OnEnable()
        {
            SimpleEventsHolder.StoryPartComplete += OnStoryPartEnd;
        }

        private void OnDisable()
        {
            SimpleEventsHolder.StoryPartComplete -= OnStoryPartEnd;
        }

        public void CountinueStory(int storyIndex)
        {
            _storyIndex = storyIndex;
            Stories[_storyIndex].gameObject.SetActive(true);
        }

        public void Next()
        {
            Stories[_storyIndex].NextMsg();
        }

        void OnStoryPartEnd()
        {
            DBVariablesHolder.IsGameplay.Value = 1;
            _storyIndex++;
            DBVariablesHolder.StoryProgress.Value = _storyIndex;
            GameManager.Instance.StartGame();
        }
    }
}
