using Core.Events;
using UnityEngine;
using Core.DB.Variables;
using Core.Plugins.Firebase;

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
            FirebaseHandler.I.LogEvent($"Story_S_{_storyIndex}");
        }

        public void Next()
        {
            Stories[_storyIndex].NextMsg();
        }

        void OnStoryPartEnd()
        {
            FirebaseHandler.I.LogEvent($"Story_E_{_storyIndex}");
            if (DBVariablesHolder.StoryProgress.Value < GameManager.Instance.MaxStoryIndex)
            {
                StartCurrentMap();
                DBVariablesHolder.IsGameplay.Value = 1;
                _storyIndex++;
                DBVariablesHolder.StoryProgress.Value = _storyIndex;
                GameManager.Instance.StartGame();
            }
            else
            {
                GameManager.Instance.SwitchToGameplay();
            }
        }

        void StartCurrentMap()
        {
            int mapStartingLvl = 1 + (GameManager.Instance.MapChangeCount * DBVariablesHolder.CurrentMap.Value);

            DBVariablesHolder.CharismaLvl.Value = mapStartingLvl;
            DBVariablesHolder.ContentCreation.Value = mapStartingLvl;
            DBVariablesHolder.ActingLvl.Value = mapStartingLvl;
            DBVariablesHolder.EditingSkill.Value = mapStartingLvl;
            DBVariablesHolder.CameraLvl.Value = mapStartingLvl;
            DBVariablesHolder.TripodLvl.Value = mapStartingLvl;
            DBVariablesHolder.MicrophoneLvl.Value = mapStartingLvl;
            DBVariablesHolder.ClothesLvl.Value = mapStartingLvl;
            DBVariablesHolder.HairsLvl.Value = mapStartingLvl;
            DBVariablesHolder.WatchLvl.Value = mapStartingLvl;
            DBVariablesHolder.HouseLvl.Value = mapStartingLvl;
            DBVariablesHolder.GroundLvl.Value = mapStartingLvl;
            DBVariablesHolder.VehicleLvl.Value = mapStartingLvl;
            DBVariablesHolder.StatueLvl.Value = mapStartingLvl;
            DBVariablesHolder.BackyardLvl.Value = mapStartingLvl;
        }

    }
}
