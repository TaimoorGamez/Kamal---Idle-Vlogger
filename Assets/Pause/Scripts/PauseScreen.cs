using Core.Events;
using Core.States;
using DG.Tweening;
using UnityEngine;
using Core.GamePlay;
using Core.DB.Variables;
using Core.Plugins.Firebase;

namespace Core.Screen
{
    public class PauseScreen : UiScreens
    {
        [SerializeField] GameObject MusicOff, SoundOff;

        private void OnEnable()
        {
            UpdateMusicUI();
            UpdateSoundUI();
            OnOpen();
        }
        public void ToggleMusic()
        {
            DBVariablesHolder.Music.Value = DBVariablesHolder.Music.Value == 1 ? 0 : 1;
            SimpleEventsHolder.UpdateMusicStateEvent?.Invoke();
            UpdateMusicUI();
            FirebaseHandler.I?.LogEvent($"Pas_Music_{DBVariablesHolder.Music.Value}");
        }

        public void ToggleSound()
        {
            DBVariablesHolder.Sound.Value = DBVariablesHolder.Sound.Value == 1 ? 0 : 1;
            SimpleEventsHolder.UpdateSoundStateEvent?.Invoke();
            UpdateSoundUI();
            FirebaseHandler.I?.LogEvent($"Pas_Sound_{DBVariablesHolder.Sound.Value}");
        }

        void UpdateMusicUI()
        {
            MusicOff.SetActive(DBVariablesHolder.Music.Value != 1);
        }

        void UpdateSoundUI()
        {
            SoundOff.SetActive(DBVariablesHolder.Sound.Value != 1);
        }

        public void RestartLevel()
        {
            SimpleEventsHolder.RestartLevelEvent?.Invoke();
            OnClose();
        }

        public void GoHome()
        {
            SimpleEventsHolder.DestroyLevelEvent?.Invoke();
            StateManager.I.ActiveState(StateManager.I.MainMenuStatePath);
            StateManager.I.DestroyState(StateManager.I.GamePlayStatePath);
            OnClose();
        }

        public override void OnOpen()
        {
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(3);
            Body.DOScale(1, _transitionDuration).SetEase(Ease.OutBack);
            FirebaseHandler.I?.LogEvent("Pas_Open");
        }

        public override void OnClose()
        {
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(2);
            Body.DOScale(0, _transitionDuration / 2).SetEase(Ease.InBack).OnComplete(() => {
                LevelsManager.I.CanPlay = true;
                StateManager.I.DestroyState(StateManager.I.PauseStatePath);
            });
            FirebaseHandler.I?.LogEvent("Pas_Close");
        }
    }
}
