using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.U2D.Animation;

namespace Core.GamePlay
{
    public class McTalking : MonoBehaviour
    {
        [SerializeField] SpriteResolver HeadspriteResolver;
        [SerializeField] List<string> SpriteLabel;

        float _delay = 10f;  
        bool _canTalk = false;
        int _currentIndex;
        string _categoryName = "Head";
        Coroutine _animationCoroutine;

        private void Start()
        {
            SpriteLibrary library = HeadspriteResolver.spriteLibrary;
            SpriteLabel = new List<string>();
            foreach (var category in library.spriteLibraryAsset.GetCategoryNames())
            {
                Debug.Log("Category: " + category);
                if (category == _categoryName)
                {
                    foreach (var label in library.spriteLibraryAsset.GetCategoryLabelNames(category))
                    {
                        Debug.Log("   Label: " + label);
                        SpriteLabel.Add(label);
                    }
                }
            } 
        }

        public void StartTalking(bool loop)
        {
            Debug.Log("StartTalking called with loop: " + loop);
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
            //do
            //{
                HeadspriteResolver.SetCategoryAndLabel(_categoryName, SpriteLabel[1]);
                //_currentIndex++;
                yield return new WaitForSeconds(0.1f);

                //Debug.Log("StartTalking called with loop: " + _currentIndex);
                //if (_currentIndex >= SpriteLabel.Count)
                //{
                //    if (loop)
                //        _currentIndex = 0;
                //    else
                //    {
                //        HeadspriteResolver.SetCategoryAndLabel(_categoryName, SpriteLabel[0]);
                //        _canTalk = false;
                //        yield break;
                //    }
                //}

            //}
            //while (_canTalk);
        }
    }
}