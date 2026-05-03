using Core.Events;
using UnityEngine;

namespace Core.GamePlay
{
    public class StoryTwo : StoryLine
    {
        [SerializeField] McTalking MC;
        [SerializeField] Animation BestFriend;
        [SerializeField] Animator McAnimator;

        void Start()
        {
            CurrentCurtainController.gameObject.SetActive(true);
            _currentMsgIndex = 0;
            MsgObj.SetActive(true);
            BestFriend.gameObject.SetActive(true);
            ShowMsg();
        }

        protected override void ShowMsg()
        {
            if (_currentMsgIndex == 0 || _currentMsgIndex == 2 || _currentMsgIndex == 4)
            {
                BestFriend.Play();
            }
            else if (_currentMsgIndex == 1 || _currentMsgIndex == 3 || _currentMsgIndex == 5)
            {
                MC.StartTalking(false);
            }
            base.ShowMsg();
        }
        public override void NextMsg()
        {
            base.NextMsg();
        }
    }
}
