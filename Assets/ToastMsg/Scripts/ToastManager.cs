using UnityEngine;
using Core.Events;

namespace Core.ToastMsg
{
    public class ToastManager : MonoBehaviour
    {
        [SerializeField] ToastScreen ToastMsgPrefab;

        ToastScreen _oldMsgScreen;
        int _oldMsgNum = -1;

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
            if (_oldMsgScreen == null)
            {
                _oldMsgScreen = Instantiate(ToastMsgPrefab);
                _oldMsgScreen.ChangeMsg(toastNum);
                _oldMsgNum = toastNum;
            }
            else if(toastNum != _oldMsgNum)
            {
                _oldMsgScreen.ChangeMsg(toastNum);
                _oldMsgNum = toastNum;
            }
        }
    }
}
