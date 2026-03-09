using UnityEngine;

namespace Core.GamePlay
{
    public class StoryOne : StoryLine
    {
        [SerializeField] Animation GirlFriend, Enemy;

        private void Start()
        {
            CurrentCurtainController.gameObject.SetActive(true);
            CurrentMsgIndex = 0;
            GirlFriend.gameObject.SetActive(true);
            MsgObj.SetActive(true);
            ShowMsg();
        }

        void ShowMsg()
        {
            MsgTxt.text = Messages[CurrentMsgIndex];
            NameTxt.text = MessengerName[CurrentMsgIndex];
            MsgBubble.rotation = Quaternion.Euler(0, BubbleRotation[CurrentMsgIndex], 0);
        }

        public void NextMsg()
        {
            CurrentMsgIndex++;
            if (CurrentMsgIndex < Messages.Length)
            {
                ShowMsg();
            }
            else
            {
                MsgObj.SetActive(false);
                CurrentCurtainController.gameObject.SetActive(true);
            }
        }
    }
}
