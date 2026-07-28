using TMPro;
using UnityEngine;
using Core.Events;
using DG.Tweening;

namespace Core.ToastMsg
{
    public class ToastManager : MonoBehaviour
    {
        public float HiddingDelay = 2;
        public int OldMsgNum = -1;

        [SerializeField] RectTransform ToastMsgPrefab;
        [SerializeField] TextMeshProUGUI MsgText;
        [SerializeField] string[] ToastMsgs;

        float _msgPos = -100, _hidePos = 100, _showTween = 0.25f;

        public void OnEnable()
        {
            SingleIntegerEventsHolder.ShowToastEvent += ShowToastMsg;
        }

        private void OnDisable()
        {
            SingleIntegerEventsHolder.ShowToastEvent -= ShowToastMsg;
        }

        void ShowToastMsg(int toastNum)
        {
            if (OldMsgNum == -1 || toastNum != OldMsgNum)
            {
                HiddingDelay = 2;
                MsgText.text = ToastMsgs[toastNum];
                OldMsgNum = toastNum;
                ToastMsgPrefab.gameObject.SetActive(true);
                ToastMsgPrefab.DOKill();
                ToastMsgPrefab.DOAnchorPosY(_msgPos, _showTween).From(new Vector2(0, _hidePos)).SetEase(Ease.OutBack);
                ToastMsgPrefab.DOScale(Vector3.one, _showTween).From(Vector3.zero).SetEase(Ease.OutBack);
            }
        }
    }
}
