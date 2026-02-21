using Core.Events;
using DG.Tweening;
using UnityEngine;
using Core.DB.Variables;
using Core.Plugins.Firebase;

namespace Core.Screen
{
    public class SettingScreen : UiScreens
    {
        [SerializeField] RectTransform MusicBtn, SoundBtn;
        [SerializeField] GameObject MusicOff, SoundOff;

        private void OnEnable()
        {
            OnOpen();
        }

        public override void OnOpen()
        {
            UpdateMusicUI();
            UpdateSoundUI();
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(2);
            MusicBtn.DOAnchorPosY(-150, _transitionDuration).SetEase(Ease.OutBack);
            SoundBtn.DOAnchorPosY(-240, _transitionDuration).SetEase(Ease.OutBack);
            FirebaseHandler.I?.LogEvent("Stg_Open");
        }

        public void ToggleMusic()
        {
            DBVariablesHolder.Music.Value = DBVariablesHolder.Music.Value == 1 ? 0 : 1;
            SimpleEventsHolder.UpdateMusicStateEvent?.Invoke();
            UpdateMusicUI();
            FirebaseHandler.I?.LogEvent($"Stg_Music_{DBVariablesHolder.Music.Value}");
        }

        public void ToggleSound()
        {
            DBVariablesHolder.Sound.Value = DBVariablesHolder.Sound.Value == 1 ? 0 : 1;
            SimpleEventsHolder.UpdateSoundStateEvent?.Invoke();
            UpdateSoundUI();
            FirebaseHandler.I?.LogEvent($"Stg_Sound_{DBVariablesHolder.Sound.Value}");
        }

        void UpdateMusicUI()
        {
            MusicOff.SetActive(DBVariablesHolder.Music.Value != 1);
        }

        void UpdateSoundUI()
        {
            SoundOff.SetActive(DBVariablesHolder.Sound.Value != 1);
        }

        public override void OnClose()
        {
            MusicBtn.DOAnchorPosY(-60, _transitionDuration).SetEase(Ease.InBack);
            SoundBtn.DOAnchorPosY(-60, _transitionDuration).SetEase(Ease.InBack).OnComplete(()=>gameObject.SetActive(false));
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(2);
            FirebaseHandler.I?.LogEvent("Stg_Close");
        }
    }
}
