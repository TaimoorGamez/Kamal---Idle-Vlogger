using UnityEngine;
using System.Collections;
using UnityEngine.U2D.Animation;

namespace Core.GamePlay
{
    public class McTalking : MonoBehaviour
    {
        [SerializeField] SpriteResolver HeadspriteResolver;
        [SerializeField] string[] SpriteLabel;

        float _delay = 0.15f;  
        bool _canTalk = false;
        int _currentIndex;
        string _categoryName = "Head";
        Coroutine _animationCoroutine;

        public void StartTalking(bool loop)
        {
            if (_animationCoroutine != null)
                StopCoroutine(_animationCoroutine);

            _currentIndex = 0;
            _canTalk = true;
            _animationCoroutine = StartCoroutine(AnimateSprites(loop));
        }

        public void StopTalking()
        {
            if (_animationCoroutine != null)
                StopCoroutine(_animationCoroutine);

            _canTalk = false;
            _currentIndex = 0;
        }

        private IEnumerator AnimateSprites(bool loop)
        {
            do
            {
                HeadspriteResolver.SetCategoryAndLabel(_categoryName, SpriteLabel[_currentIndex]);
                _currentIndex++;
                yield return new WaitForSeconds(_delay);

                if (_currentIndex >= SpriteLabel.Length)
                {
                    if (loop)
                        _currentIndex = 0;
                    else
                    {
                        HeadspriteResolver.SetCategoryAndLabel(_categoryName, SpriteLabel[0]);
                        _canTalk = false;
                        yield break;
                    }
                }

            }
            while (_canTalk);
        }
    }
}