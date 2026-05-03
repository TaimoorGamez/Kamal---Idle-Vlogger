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
            if (_currentMsgIndex == 0 || _currentMsgIndex == 2 || _currentMsgIndex == 4 || _currentMsgIndex == 6 ||
                _currentMsgIndex == 8 || _currentMsgIndex == 10 || _currentMsgIndex == 11)
            {
                BestFriend.Play();
            }
            else if (_currentMsgIndex == 1 || _currentMsgIndex == 3 || _currentMsgIndex == 5 || _currentMsgIndex == 7 ||
                     _currentMsgIndex == 9)
            {
                MC.StartTalking(false);
            }
            else
            {
                CurrentCurtainController.gameObject.SetActive(true);
                BestFriend.gameObject.SetActive(false);
            }
                base.ShowMsg();
        }
        public override void NextMsg()
        {
            base.NextMsg();
        }
    }
}
