using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Core.GamePlay
{
    public class MultiplierBar : MonoBehaviour
    {
        public int CurrentMultiplier;

        [SerializeField] TextMeshProUGUI RvText;
        [SerializeField] RectTransform MovingTarget;
        [SerializeField] RectTransform[] MovingPoints;
        [SerializeField] Image[] BarShine;

        float _durationPerStep = 0.2f, _transitionDuration = 0.25f;
        int _currentIndex = 0, _previousShine = -1;
        bool _canMove = true, _movingForward = true;
        Tween _currentTween;
        Coroutine _moveCoroutine;

        private void OnEnable()
        {
            transform.DOScale(0.5f, _transitionDuration).SetEase(Ease.OutBack).OnComplete(() => _moveCoroutine = StartCoroutine(MoveToNextPoint()));
        }

        private IEnumerator MoveToNextPoint()
        {
            while (_canMove)
            {
                HighlightImg(GetShineIndex(_currentIndex));
                _currentTween = MovingTarget.DOAnchorPosX(MovingPoints[_currentIndex].anchoredPosition.x, _durationPerStep).SetEase(Ease.Linear).OnComplete(() =>
                {
                    CurrentMultiplier = GetMultipler(_currentIndex);
                    RvText.text = CurrentMultiplier.ToString() + "X";
                    if (_movingForward)
                    {
                        _currentIndex++;
                        if (_currentIndex >= MovingPoints.Length)
                        {
                            _currentIndex = MovingPoints.Length - 1;
                            _movingForward = false;
                        }
                    }
                    else
                    {
                        _currentIndex--;
                        if (_currentIndex < 0)
                        {
                            _currentIndex = 0;
                            _movingForward = true;
                        }
                    }
                });
                yield return _currentTween.WaitForCompletion();
            }
        }

        int GetMultipler(int index)
        {
            if (_movingForward)
            {
                switch (index)
                {
                    case 0: return 2;
                    case 1: return 3;
                    case 2: return 5;
                    case 3: return 3;
                    case 4: return 2;
                    case 5: return 2;
                    default: return 1;
                }
            }
            else
            {
                switch (index)
                {
                    case 0: return 2;
                    case 1: return 2;
                    case 2: return 3;
                    case 3: return 5;
                    case 4: return 3;
                    case 5: return 2;
                    default: return 1;
                }
            }
        }

        int GetShineIndex(int index)
        {
            if (_movingForward)
            {
                switch (index)
                {
                    case 0: return 0;
                    case 1: return 0;
                    case 2: return 1;
                    case 3: return 2;
                    case 4: return 3;
                    case 5: return 4;
                    default: return BarShine.Length;
                }
            }
            else
            {
                switch (index)
                {
                    case 0: return 0;
                    case 1: return 1;
                    case 2: return 2;
                    case 3: return 3;
                    case 4: return 4;
                    case 5: return 4;
                    default: return BarShine.Length;
                }
            }
        }

        void HighlightImg(int index)
        {
            if (index < BarShine.Length)
            {
                if (_previousShine != -1 && _previousShine != index)
                { BarShine[_previousShine].DOFade(0, 0); }
                BarShine[index].DOFade(0.32f, _durationPerStep).SetEase(Ease.Linear);
                _previousShine = index;
            }
        }

        public void OnClose()
        {
            StopMovement();
            transform.DOScale(0,_transitionDuration).SetEase(Ease.InBack).OnComplete(()=>gameObject.SetActive(false));
        }

        void StopMovement()
        {
            _canMove = false;
            if (_currentTween != null && _currentTween.IsActive())
            {
                _currentTween.Kill();
            }
            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
                _moveCoroutine = null;
            }
        }

        private void OnDisable()
        {
            DOTween.Kill(this);
        }
    }
}
