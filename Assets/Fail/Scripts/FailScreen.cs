using Core.Events;
using Core.States;
using UnityEngine;
using DG.Tweening;
using Core.GamePlay;
using Core.DB.Variables;
using Core.Plugins.Firebase;

namespace Core.Screen
{
    public class FailScreen : UiScreens
    {
        int _extraMoves = 10;

        private void OnEnable()
        {
            SimpleEventsHolder.AddMovesEvent += AddMoreMoves;
            OnOpen();
        }

        private void OnDisable()
        {
            SimpleEventsHolder.AddMovesEvent -= AddMoreMoves;
        }

        void AddMoreMoves()
        {
            LevelsManager.I.TotalMoves += _extraMoves;
            SimpleEventsHolder.UpdateMovesEvent?.Invoke();
            LevelsManager.I.CanPlay = true;
            OnClose();
        }

        public void RestartLevel()
        {
            SimpleEventsHolder.RestartLevelEvent?.Invoke();
            OnClose();
            FirebaseHandler.I?.LogEvent($"fail_lvl_{DBVariablesHolder.LvlIndex.Value}_Rst");
        }
        public void GoHome()
        {
            SimpleEventsHolder.DestroyLevelEvent?.Invoke();
            StateManager.I.ActiveState(StateManager.I.MainMenuStatePath);
            StateManager.I.DestroyState(StateManager.I.GamePlayStatePath);
            OnClose();
            FirebaseHandler.I?.LogEvent($"fail_lvl_{DBVariablesHolder.LvlIndex.Value}_Home");
        }

        public override void OnOpen()
        {
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(3);
            Body.DOScale(1, _transitionDuration).SetEase(Ease.OutBack);
            FirebaseHandler.I?.LogEvent($"fail_Open_lvl_{DBVariablesHolder.LvlIndex.Value}");
        }

        public override void OnClose()
        {
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(2);
            Body.DOScale(0, _transitionDuration/2).SetEase(Ease.InBack).OnComplete(() => StateManager.I.DestroyState(StateManager.I.LevelFailStatePath));
        }
    }
}
