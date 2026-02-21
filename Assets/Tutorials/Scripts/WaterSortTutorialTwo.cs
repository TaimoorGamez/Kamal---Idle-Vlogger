using UnityEngine;
using DG.Tweening;
using Core.Events;
using System.Collections;
using Core.GamePlay.Coloring;

namespace Core.GamePlay.WaterSort
{
    public class WaterSortTutorialTwo : MonoBehaviour
    {
        [SerializeField] CapsuleCollider MyCollider, SecondCollider, ThirdCollider;
        [SerializeField] TubeHandler MyLiquid, OtherLiquid, ThirdLiquid;
        [SerializeField] GameObject InfoTextObj, UndoBtn;
        [SerializeField] Transform HandObj, TutorialCircle;
        [SerializeField] Color[] CurrentColors;
        [SerializeField] bool IsUndoBtn, ExtraTube;
        [SerializeField] BowlColorHandler BowlObj;

        int _colorIndex = 0;
        bool _isFirstClick = true;
        Vector3 _bowlScale = new Vector3(1.5f, 0.1f, 1.5f), _firstBowlPos = new Vector3(0, 4.5f, 0), _otherBowlPos = new Vector3(-1, 4.5f, 0),
                _thirdBowlPos = new Vector3(1, 4.5f, 0);

        private void OnEnable()
        {

            if (IsUndoBtn)
            {
                SimpleEventsHolder.StartColoringEvent += ColoringPreparation;
            }
        }

        private void OnDisable()
        {
            if (IsUndoBtn)
            {
                SimpleEventsHolder.StartColoringEvent -= ColoringPreparation;
            }
        }

        private void Start()
        {
            if (!IsUndoBtn && !ExtraTube)
            { StartCoroutine(SetColors()); }
        }

        IEnumerator SetColors()
        {
            yield return new WaitForSeconds(0.5f);
            for (int c = 0; c < 4; c++)
            {
                MyLiquid.SetColor(CurrentColors[_colorIndex]);
                yield return new WaitForSeconds(0.1f);
                if (_colorIndex == 1)
                {
                    _colorIndex = 0;
                }
                else
                {
                    _colorIndex++;
                }
                OtherLiquid.SetColor(CurrentColors[_colorIndex]);
                yield return new WaitForSeconds(0.1f);
            }
            SingleIntegerEventsHolder.SwitchProtectorEvent?.Invoke(0);
            MyCollider.enabled = true;
            LevelsManager.I.CanPlay = true;
            HandObj.gameObject.SetActive(true);
        }

        void OnMouseDown()
        {
            if (!LevelsManager.I.CanPlay)
                return;

            if (ExtraTube && _isFirstClick)
            {
                _isFirstClick = false;
                MyCollider.enabled = false;
                Invoke(nameof(ShowUndoBtn), 1.5f);
            }
            else if (_isFirstClick)
            {
                _isFirstClick = false;
                MyCollider.enabled = false;
                HandObj.DOLocalMoveX(135f, 0.35f).SetEase(Ease.InOutBack);
                ThirdCollider.enabled = true;
            }
        }

        public void TutorialUndoButton()
        {
            if (_isFirstClick)
            {
                _isFirstClick = false;
                HandObj.gameObject.SetActive(false);
                MyCollider.enabled = true;
                SecondCollider.enabled = true;
                ThirdCollider.enabled = true;
            }
            SimpleEventsHolder.UndoEvent?.Invoke();
        }

        void ShowUndoBtn()
        {
            InfoTextObj.SetActive(true);
            TutorialCircle.gameObject.SetActive(true);
            TutorialCircle.DOScale(1, 1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                HandObj.DOLocalMove(new Vector3(175f, -510f, 0), 1f).SetEase(Ease.InBack);
            });
            UndoBtn.SetActive(true);
        }

        void ColoringPreparation()
        {
            float tweenTime = 1;
            InfoTextObj.SetActive(false);
            if (IsUndoBtn)
            {
                if (ThirdLiquid.WaterColors.Count > 0)
                {
                    ThirdLiquid.TubeCap.gameObject.SetActive(false);
                    ThirdLiquid.transform.DOKill();
                    ThirdLiquid.transform.DOScale(_bowlScale, tweenTime);
                    ThirdLiquid.transform.DOLocalMove(_thirdBowlPos, tweenTime).OnKill(() =>
                    {
                        BowlColorHandler colorBowl = Instantiate(BowlObj, ThirdLiquid.transform.parent);
                        colorBowl.transform.localPosition = ThirdLiquid.transform.localPosition;
                        colorBowl.SetColor(ThirdLiquid.CurrentColor);
                        Destroy(ThirdLiquid.gameObject, 0.2f);
                    });
                }
                else
                {
                    Destroy(ThirdLiquid.gameObject);
                }

                if (OtherLiquid.WaterColors.Count > 0)
                {
                    OtherLiquid.TubeCap.gameObject.SetActive(false);
                    OtherLiquid.transform.DOKill();
                    OtherLiquid.transform.DOScale(_bowlScale, tweenTime);
                    OtherLiquid.transform.DOLocalMove(_otherBowlPos, tweenTime).OnKill(() =>
                    {
                        BowlColorHandler colorBowl = Instantiate(BowlObj, OtherLiquid.transform.parent);
                        colorBowl.transform.localPosition = OtherLiquid.transform.localPosition;
                        colorBowl.SetColor(OtherLiquid.CurrentColor);
                        Destroy(OtherLiquid.gameObject, 0.2f);
                    });
                }
                else
                {
                    Destroy(OtherLiquid.gameObject);
                }

                if (MyLiquid.WaterColors.Count > 0)
                {
                    MyLiquid.TubeCap.gameObject.SetActive(false);
                    MyLiquid.transform.DOKill();
                    MyLiquid.transform.DOScale(_bowlScale, tweenTime);
                    MyLiquid.transform.DOLocalMove(_firstBowlPos, tweenTime).OnKill(() =>
                    {
                        BowlColorHandler colorBowl = Instantiate(BowlObj, MyLiquid.transform.parent);
                        colorBowl.transform.localPosition = MyLiquid.transform.localPosition;
                        colorBowl.SetColor(MyLiquid.CurrentColor);
                        Destroy(MyLiquid.gameObject, 0.2f);
                    });
                }
                else
                {
                    Destroy(MyLiquid.gameObject);
                }
                //Destroy(transform.parent.gameObject, 2f);
            }
        }
    }
}
