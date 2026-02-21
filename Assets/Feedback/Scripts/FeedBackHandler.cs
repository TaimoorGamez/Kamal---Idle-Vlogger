using TMPro;
using UnityEngine;
using DG.Tweening;
using Core.Plugins.Firebase;

namespace Core.Screen
{
    public class FeedBackHandler : UiScreens
    {
        [SerializeField] TMP_InputField FeedBackField;

        private void OnEnable()
        {
            OnOpen();
            FirebaseHandler.I.LogEvent("FB_Open");
        }

        public override void OnOpen()
        {
            Body.DOScale(1, _transitionDuration).SetEase(Ease.OutBack);
        }

        public void OnClickSendButton()
        {
            string msg = FeedBackField.text;
            if (msg.Length == 0)
                return;

            msg = msg.Replace(":", "_");
            FirebaseHandler.I.LogEvent(msg);
            FeedBackField.text = "";
            OnClose();
        }

        public override void OnClose()
        {
            Body.DOScale(0, _transitionDuration).SetEase(Ease.InBack).OnComplete(()=> gameObject.SetActive(false));
            FirebaseHandler.I.LogEvent("FB_Close");
        }
    }
}
