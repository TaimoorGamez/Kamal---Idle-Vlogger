using UnityEngine;
using DG.Tweening;
using Core.Events;
using System.Collections;
using Core.GamePlay.Coloring;

namespace Core.GamePlay.WaterSort
{
    public class WaterSortTutorialThree : MonoBehaviour
    {
        [SerializeField] TubeHandler FirstTube, SecondTube, ThirdTube, ForthTube, ExtraTube;
        [SerializeField] GameObject InfoTextObj, TubeBtn;
        [SerializeField] Transform HandObj, TutorialCircle;
        [SerializeField] Color[] CurrentColors;
        [SerializeField] BowlColorHandler BowlObj;

        Vector3 _bowlScale = new Vector3(1.5f, 0.1f, 1.5f), _firstBowlPos = new Vector3(1, 4.5f, 0), _secondBowlPos = new Vector3(-1, 4.5f, 0), 
                _thirdBowlPos = new Vector3(-2, 4.5f, 0), _forthBowlPos = new Vector3(0, 4.5f, 0), _extraBowlPos = new Vector3(2, 4.5f, 0),
                _btnPosition = new Vector3(-100,-510,0);
        int _colorIndex = 0;

        private void OnEnable()
        {
            SimpleEventsHolder.StartColoringEvent += ColoringPreparation;
        }

        private void OnDisable()
        {
            SimpleEventsHolder.StartColoringEvent -= ColoringPreparation;
        }

        private void Start()
        {
            StartCoroutine(SetColors());
        }

        IEnumerator SetColors()
        {
            yield return new WaitForSeconds(0.5f);
            for (int c = 0; c < 4; c++)
            {
                FirstTube.SetColor(CurrentColors[_colorIndex]);
                yield return new WaitForSeconds(0.1f);
                if (_colorIndex == 2)
                {
                    _colorIndex = 0;
                }
                else
                {
                    _colorIndex++;
                }
                SecondTube.SetColor(CurrentColors[_colorIndex]);
                yield return new WaitForSeconds(0.1f); 
                if (_colorIndex == 2)
                {
                    _colorIndex = 0;
                }
                else
                {
                    _colorIndex++;
                }
                ThirdTube.SetColor(CurrentColors[_colorIndex]);
                yield return new WaitForSeconds(0.1f);
            }
            SingleIntegerEventsHolder.SwitchProtectorEvent?.Invoke(0);
            LevelsManager.I.CanPlay = true;
            HandObj.gameObject.SetActive(true);
            TutorialCircle.gameObject.SetActive(true);
            TutorialCircle.DOScale(1, 1f).SetEase(Ease.Linear);
            HandObj.DOLocalMove(_btnPosition, 1f).SetEase(Ease.InBack).OnComplete(()=>TubeBtn.SetActive(true));
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
                FirstTube.transform.DOLocalMove(_firstBowlPos, tweenTime).OnKill(() =>
                {
                    BowlColorHandler colorBowl = Instantiate(BowlObj, transform.parent);
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
                SecondTube.transform.DOLocalMove(_secondBowlPos, tweenTime).OnKill(() =>
                {
                    BowlColorHandler colorBowl = Instantiate(BowlObj, transform.parent);
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
                ThirdTube.transform.DOLocalMove(_thirdBowlPos, tweenTime).OnKill(() =>
                {
                    BowlColorHandler colorBowl = Instantiate(BowlObj, transform.parent);
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
                ForthTube.transform.DOLocalMove(_forthBowlPos, tweenTime).OnKill(() =>
                {
                    BowlColorHandler colorBowl = Instantiate(BowlObj, transform.parent);
                    colorBowl.transform.localPosition = ForthTube.transform.localPosition;
                    colorBowl.SetColor(ForthTube.CurrentColor);
                    Destroy(ForthTube.gameObject, 0.15f);
                });
            }
            else
            {
                Destroy(ForthTube.gameObject);
            }

            if (ExtraTube.WaterColors.Count > 0)
            {
                ExtraTube.TubeCap.gameObject.SetActive(false);
                ExtraTube.transform.DOKill();
                ExtraTube.transform.DOScale(_bowlScale, tweenTime);
                ExtraTube.transform.DOLocalMove(_extraBowlPos, tweenTime).OnKill(() =>
                {
                    BowlColorHandler colorBowl = Instantiate(BowlObj, transform.parent);
                    colorBowl.transform.localPosition = ExtraTube.transform.localPosition;
                    colorBowl.SetColor(ExtraTube.CurrentColor);
                    Destroy(ExtraTube.gameObject, 0.15f);
                });
            }
            else
            {
                Destroy(ExtraTube.gameObject);
            }
        }
    }
}
