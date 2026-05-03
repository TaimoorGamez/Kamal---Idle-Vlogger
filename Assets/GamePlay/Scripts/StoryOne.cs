using DG.Tweening;
using UnityEngine;

namespace Core.GamePlay
{
    public class StoryOne : StoryLine
    {
        [SerializeField] McTalking MC;
        [SerializeField] Animation GirlFriend, Rohan;
        [SerializeField] Animator McAnimator;

        float _rohanPosition = 2f, _moveDuration = 0.1f, _outPosition = 5f;

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
            if (_currentMsgIndex == 0 || _currentMsgIndex == 2 || _currentMsgIndex == 7)
            {
                GirlFriend.Play();
            }
            else if (_currentMsgIndex == 1 || _currentMsgIndex == 3 || _currentMsgIndex == 4 || _currentMsgIndex == 6)
            {
                MC.StartTalking(false);
            }
            else if (_currentMsgIndex == 5) 
            {
                Rohan.gameObject.SetActive(true);
                Rohan.transform.DOLocalMoveX(_rohanPosition, _moveDuration);
                GirlFriend.Play();
            }
            else if (_currentMsgIndex == 8) 
            {
                Rohan.Play();
            }
            else if (_currentMsgIndex == 9)
            {
                CurrentCurtainController.gameObject.SetActive(true);
                GirlFriend.transform.DOLocalMoveX(_outPosition, _moveDuration).OnComplete(()=> GirlFriend.gameObject.SetActive(false));
                Rohan.transform.DOLocalMoveX(_outPosition, _moveDuration).OnComplete(() => Rohan.gameObject.SetActive(false));
                McAnimator.SetTrigger("Kneel");
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
