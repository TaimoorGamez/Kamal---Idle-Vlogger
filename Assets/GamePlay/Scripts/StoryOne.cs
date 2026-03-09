using UnityEngine;

namespace Core.GamePlay
{
    public class StoryOne : StoryLine
    {
        [SerializeField] Animation MC, GirlFriend, Enemy;

        private void Start()
        {
            CurrentCurtainController.gameObject.SetActive(true);
            CurrentMsgIndex = 0;
            MsgObj.SetActive(true);
            ShowMsg();
        }

        protected override void ShowMsg()
        {
            MsgTxt.text = Messages[CurrentMsgIndex];
            NameTxt.text = MessengerName[CurrentMsgIndex];
            MsgBubble.rotation = Quaternion.Euler(0, BubbleRotation[CurrentMsgIndex], 0);
            switch(CurrentMsgIndex)
            {
                case 0:
                    GirlFriend.gameObject.SetActive(true);
                    GirlFriend.Play();
                    break;

                case 1:
                    MC.Play("TalkingDefault");
                    break;
            }
        }

        public override void NextMsg()
        {
            base.NextMsg();
        }
    }
}
