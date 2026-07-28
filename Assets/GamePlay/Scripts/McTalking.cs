using UnityEngine;
using Core.DB.Variables;
using System.Collections;
using UnityEngine.U2D.Animation;

namespace Core.GamePlay
{
    public class McTalking : MonoBehaviour
    {
        [SerializeField] SpriteResolver HeadspriteResolver;

        float _delay = 0.15f;  
        bool _canTalk = false;
        int _currentIndex, _spriteLength = 3;
        string _categoryName, _categoryNamePart = "Head_";
        Coroutine _animationCoroutine;

        public void StartTalking(bool loop)
        {
            if (_animationCoroutine != null)
                StopCoroutine(_animationCoroutine);

            _categoryName = _categoryNamePart + GetItemIndex(DBVariablesHolder.HairsLvl.Value).ToString();
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
                HeadspriteResolver.SetCategoryAndLabel(_categoryName, _currentIndex.ToString());
                _currentIndex++;
                yield return new WaitForSeconds(_delay);

                if (_currentIndex >= _spriteLength)
                {
                    if (loop)
                        _currentIndex = 0;
                    else
                    {
                        HeadspriteResolver.SetCategoryAndLabel(_categoryName, "0");
                        _canTalk = false;
                        yield break;
                    }
                }

            }
            while (_canTalk);
        }
        int GetItemIndex(int lvl)
        {
            int range = lvl / GameManager.Instance.MapChangeCount;
            int spriteIndex = GameManager.Instance.SpriteChangeCount;
            int mapIndex = DBVariablesHolder.CurrentMap.Value;
            while (range != mapIndex)
            {
                lvl -= spriteIndex;
                range = lvl / GameManager.Instance.MapChangeCount;
            }
            return lvl / spriteIndex;
        }
    }
}