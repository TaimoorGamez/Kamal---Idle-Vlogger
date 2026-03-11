using DG.Tweening;
using UnityEngine;

namespace Core.GamePlay
{
    public class StoryOne : StoryLine
    {
        [SerializeField] McTalking MC;
        [SerializeField] Animation GirlFriend, Rohan;

        float _rohanPosition = 2f, _moveDuration = 0.1f;

        private void Start()
        {
            CurrentCurtainController.gameObject.SetActive(true);
            _currentMsgIndex = 0;
            MsgObj.SetActive(true);
            GirlFriend.gameObject.SetActive(true);
            ShowMsg();
        }

        protected override void ShowMsg()
        {
            if (_currentMsgIndex == 0 || _currentMsgIndex == 2)
            {
                GirlFriend.Play();
            }
            else if (_currentMsgIndex == 1 || _currentMsgIndex == 3 || _currentMsgIndex == 4)
            {
                MC.StartTalking(false);
            }
            else if (_currentMsgIndex == 5) 
            {
                Rohan.gameObject.SetActive(true);
                Rohan.transform.DOLocalMoveX(_rohanPosition, _moveDuration);
                GirlFriend.Play();
            }
            base.ShowMsg();
        }

        public override void NextMsg()
        {
            base.NextMsg();
        }
    }
}
