using Core.Events;
using DG.Tweening;
using UnityEngine;
using System.Collections;
using Core.GamePlay.Coloring;

namespace Core.GamePlay.WaterSort
{
    public class WaterSortTutorialFour : MonoBehaviour
    {
        [SerializeField] CapsuleCollider ColliderOne, ColliderTwo, ColliderThree, ColliderFour, ColliderFive;
        [SerializeField] TubeHandler FirstTube, SecondTube, ThirdTube, ForthTube, FifthTube, SixthTube;
        [SerializeField] GameObject InfoTextObj, SwapBtn;
        [SerializeField] Color[] CurrentColors;
        [SerializeField] bool IsSwipButton, IsFirstTube, IsLastTube;
        [SerializeField] BowlColorHandler BowlObj;
        [SerializeField] Transform BowlParent, Circle, HandObj;

        Vector3 _bowlScale = new Vector3(1.5f, 0.1f, 1.5f);
        int _colorIndex = 0;
        float _first5Pos = 4.8f, _nextPos = 3.5f;
        bool _isFirstClick = true;
        float _tutorialAnimationTime = 1;

        private void OnEnable()
        {
            if (IsSwipButton)
            {
                SimpleEventsHolder.StartColoringEvent += ColoringPreparation;
            }
        }

        private void OnDisable()
        {
            if (IsSwipButton)
            {
                SimpleEventsHolder.StartColoringEvent -= ColoringPreparation;
            }
        }

        private void Start()
        {
            if (IsSwipButton)
            { 
                StartCoroutine(SetColors());
            }
        }

        IEnumerator SetColors()
        {
            yield return new WaitForSeconds(0.5f);
            for (int c = 0; c < 4; c++)
            {
                FirstTube.SetColor(CurrentColors[_colorIndex]);
                yield return new WaitForSeconds(0.1f);
                if (_colorIndex == 3)
                {
                    _colorIndex = 0;
                }
                else
                {
                    _colorIndex++;
                }
                SecondTube.SetColor(CurrentColors[_colorIndex]);
                yield return new WaitForSeconds(0.1f); 
                if (_colorIndex == 3)
                {
                    _colorIndex = 0;
                }
                else
                {
                    _colorIndex++;
                }
                ThirdTube.SetColor(CurrentColors[_colorIndex]);
                yield return new WaitForSeconds(0.1f); 
                if (_colorIndex == 3)
                {
                    _colorIndex = 0;
                }
                else
                {
                    _colorIndex++;
                }
                ForthTube.SetColor(CurrentColors[_colorIndex]);
            }
            SingleIntegerEventsHolder.SwitchProtectorEvent?.Invoke(0);
            LevelsManager.I.CanPlay = true;
            Circle.gameObject.SetActive(true);
            HandObj.gameObject.SetActive(true);
            HandObj.DOLocalMoveY(-510, _tutorialAnimationTime).SetEase(Ease.InBack);
            Circle.DOScale(1, _tutorialAnimationTime).SetEase(Ease.Linear).OnComplete(()=>SwapBtn.SetActive(true));
        }

        private void OnMouseDown()
        {
            if (LevelsManager.I.CanPlay)
            {
                if (IsFirstTube)
                {
                    if (_isFirstClick)
                    {
                        _isFirstClick = false;
                        HandObj.DOLocalMove(new Vector3(-45f, 0, 0), _tutorialAnimationTime).SetEase(Ease.InOutBack);
                        ColliderThree.enabled = false;
                        ColliderTwo.enabled = true;
                        enabled = false;
                    }
                }
                else if (IsLastTube)
                {
                    if (_isFirstClick)
                    {
                        _isFirstClick = false;
                        Destroy(HandObj.gameObject);
                        ColliderOne.enabled = true;
                        ColliderTwo.enabled = true;
                        ColliderThree.enabled = true;
                        ColliderFour.enabled = true;
                        ColliderFive.enabled = true;
                    }
                }
            }
        }

        public void SwapeColor()
        {
            if (_isFirstClick)
            {
                _isFirstClick = false;
                HandObj.DOLocalMove(new Vector3(225, 0, 0), _tutorialAnimationTime).SetEase(Ease.InOutBack);
                ColliderThree.enabled = true;
            }
            SimpleEventsHolder.SwapColorsEvent?.Invoke();
        }

        void ColoringPreparation()
        {
            float tweenTime = 1;
            InfoTextObj.SetActive(false);
            if (FirstTube.WaterColors.Count > 0)
            {
                FirstTube.TubeCap.gameObject.SetActive(false);
                FirstTube.transform.DOKill();
                FirstTube.transform.DOScale(_bowlScale, tweenTime);
                FirstTube.transform.DOLocalMoveZ(0, tweenTime);
                FirstTube.transform.DOLocalMoveY(_first5Pos, tweenTime).OnKill(() =>
                {
                    BowlColorHandler colorBowl = Instantiate(BowlObj, BowlParent);
                    colorBowl.transform.localPosition = FirstTube.transform.localPosition;
                    colorBowl.SetColor(FirstTube.CurrentColor);
                    Destroy(FirstTube.gameObject, 0.15f);
                });
            }
            else
            {
                Destroy(FirstTube.gameObject);
            }

            if (SecondTube.WaterColors.Count > 0)
            {
                SecondTube.TubeCap.gameObject.SetActive(false);
                SecondTube.transform.DOKill();
                SecondTube.transform.DOScale(_bowlScale, tweenTime);
                SecondTube.transform.DOLocalMoveZ(0, tweenTime);
                SecondTube.transform.DOLocalMoveY(_first5Pos, tweenTime).OnKill(() =>
                {
                    BowlColorHandler colorBowl = Instantiate(BowlObj, BowlParent);
                    colorBowl.transform.localPosition = SecondTube.transform.localPosition;
                    colorBowl.SetColor(SecondTube.CurrentColor);
                    Destroy(SecondTube.gameObject, 0.15f);
                });
            }
            else
            {
                Destroy(SecondTube.gameObject);
            }

            if (ThirdTube.WaterColors.Count > 0)
            {
                ThirdTube.TubeCap.gameObject.SetActive(false);
                ThirdTube.transform.DOKill();
                ThirdTube.transform.DOScale(_bowlScale, tweenTime);
                ThirdTube.transform.DOLocalMoveZ(0, tweenTime);
                ThirdTube.transform.DOLocalMoveY(_first5Pos, tweenTime).OnKill(() =>
                {
                    BowlColorHandler colorBowl = Instantiate(BowlObj, BowlParent);
                        colorBowl.transform.localPosition = ThirdTube.transform.localPosition;
                        colorBowl.SetColor(ThirdTube.CurrentColor);
                        Destroy(ThirdTube.gameObject, 0.15f);
                });
            }
            else
            {
                Destroy(ThirdTube.gameObject);
            }

            if (ForthTube.WaterColors.Count > 0)
            {
                ForthTube.TubeCap.gameObject.SetActive(false);
                ForthTube.transform.DOKill();
                ForthTube.transform.DOScale(_bowlScale, tweenTime);
                ForthTube.transform.DOLocalMoveZ(0, tweenTime);
                ForthTube.transform.DOLocalMoveY(_first5Pos, tweenTime).OnKill(() =>
                {
                    BowlColorHandler colorBowl = Instantiate(BowlObj, BowlParent);
                    colorBowl.transform.localPosition = ForthTube.transform.localPosition;
                    colorBowl.SetColor(ForthTube.CurrentColor);
                    Destroy(ForthTube.gameObject, 0.15f);
                });
            }
            else
            {
                Destroy(ForthTube.gameObject);
            }

            if (FifthTube.WaterColors.Count > 0)
            {
                FifthTube.TubeCap.gameObject.SetActive(false);
                FifthTube.transform.DOKill();
                FifthTube.transform.DOScale(_bowlScale, tweenTime);
                FifthTube.transform.DOLocalMoveZ(0, tweenTime);
                FifthTube.transform.DOLocalMoveY(_first5Pos, tweenTime).OnKill(() =>
                {
                    BowlColorHandler colorBowl = Instantiate(BowlObj, BowlParent);
                    colorBowl.transform.localPosition = FifthTube.transform.localPosition;
                    colorBowl.SetColor(FifthTube.CurrentColor);
                    Destroy(FifthTube.gameObject, 0.15f);

                });
            }
            else
            {
                Destroy(FifthTube.gameObject);
            }

            if (SixthTube.WaterColors.Count > 0)
            {
                SixthTube.TubeCap.gameObject.SetActive(false);
                SixthTube.transform.DOKill();
                SixthTube.transform.DOScale(_bowlScale, tweenTime);
                SixthTube.transform.DOLocalMoveZ(0, tweenTime);
                SixthTube.transform.DOLocalMoveY(_nextPos, tweenTime).OnKill(() =>
                {
                    BowlColorHandler colorBowl = Instantiate(BowlObj, BowlParent);
                    colorBowl.transform.localPosition = SixthTube.transform.localPosition;
                    colorBowl.SetColor(SixthTube.CurrentColor);
                    Destroy(SixthTube.gameObject, 0.15f);
                });
            }
            else
            {
                Destroy(SixthTube.gameObject);
            }
        }

    }
}
