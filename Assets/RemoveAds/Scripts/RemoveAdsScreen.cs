using Core.DB.Variables;
using Core.Events;
using Core.Plugins.Firebase;
using DG.Tweening;
using UnityEngine;

namespace Core.Screen
{
    public class RemoveAdsScreen : UiScreens
    {
        [SerializeField] GameObject RvBtn, TimerObj;

        void OnEnable()
        {
            SimpleEventsHolder.AdsBlockerEvent += BlockAdForTime;
            if (DBVariablesHolder.AdBlocked.Value == 1)
            {
                RvBtn.SetActive(false);
                TimerObj.SetActive(true);
            }
            else
            {
                RvBtn.SetActive(true);
                TimerObj.SetActive(false);
            }
            OnOpen();
        }

        void OnDisable()
        {
            SimpleEventsHolder.AdsBlockerEvent -= BlockAdForTime;
        }

        void BlockAdForTime()
        {
            RvBtn.SetActive(false);
            TimerObj.SetActive(true);
            OnClose();
        }

        public override void OnOpen()
        {
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(3);
            Body.DOAnchorPosX(0, _transitionDuration).SetEase(Ease.OutBack);
            FirebaseHandler.I?.LogEvent("ReAds_Open");
        }

        public override void OnClose()
        {
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(2);
            Body.DOAnchorPosX(1500, _transitionDuration/2).SetEase(Ease.InBack).OnComplete(() => gameObject.SetActive(false));
            FirebaseHandler.I?.LogEvent("ReAds_close");
        }
    }
}
