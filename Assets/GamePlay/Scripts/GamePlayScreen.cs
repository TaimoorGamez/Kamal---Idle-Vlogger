using TMPro;
using Core.States;
using UnityEngine;
using Core.Events;
using Core.GamePlay;
using Core.DB.Variables;

namespace Core.Screen
{
    public class GamePlayScreen : UiScreens
    {
        [SerializeField] GameObject SwipeColorsModePanel, PowerButtons, TopBar, PauseBtn, LoadingPanel;
        [SerializeField] TextMeshProUGUI MovesText;

        private void OnEnable()
        {
            SingleIntegerEventsHolder.SwitchProtectorEvent += SwitchProtector;
            SimpleEventsHolder.UpdateMovesEvent += UpdateMovesText;
            SimpleEventsHolder.StartColoringEvent += PrepareForColoring;
        }

        private void OnDisable()
        {
            SingleIntegerEventsHolder.SwitchProtectorEvent -= SwitchProtector;
            SimpleEventsHolder.UpdateMovesEvent -= UpdateMovesText;
            SimpleEventsHolder.StartColoringEvent -= PrepareForColoring;
        }

        private void Start()
        {
            if (DBVariablesHolder.LvlNum.Value < LevelsManager.I.MinLvlCount)
            {
                PowerButtons.SetActive(false);
                PauseBtn.SetActive(false);
                LoadingPanel.SetActive(false);
            }
            else
            {
                LoadingPanel.SetActive(true);
                PowerButtons.SetActive(true);
                PauseBtn.SetActive(true);
            }
        }

        void SwitchProtector(int state)
        {
            SwipeColorsModePanel.SetActive(state == 1);
            LoadingPanel.SetActive(false);
        }


        void UpdateMovesText()
        {
            if (DBVariablesHolder.LvlNum.Value >= LevelsManager.I.MinLvlCount)
            {
                MovesText.text = LevelsManager.I.TotalMoves.ToString();

                if (LevelsManager.I.TotalMoves < 1)
                {
                    LevelsManager.I.CanPlay = false;
                    Invoke(nameof(OnMovesEnd), 3);
                }
            }
        }

        void OnMovesEnd()
        {
            if (LevelsManager.I.CompletedTubes != LevelsManager.I.CurrrentLvl)
            {
                StateManager.I.ActiveState(StateManager.I.LevelFailStatePath);
            }
        }

        void PrepareForColoring()
        {
            PowerButtons.SetActive(false);
            TopBar.SetActive(false);
        }

        public void OnClickPause()
        {
            LevelsManager.I.CanPlay = false;
            StateManager.I.ActiveState(StateManager.I.PauseStatePath);
        }
    }
}
