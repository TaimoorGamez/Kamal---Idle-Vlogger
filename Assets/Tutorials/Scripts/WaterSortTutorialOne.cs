using UnityEngine;
using DG.Tweening;
using Core.Events;
using System.Collections;
using Core.GamePlay.Coloring;

namespace Core.GamePlay.WaterSort
{
    public class WaterSortTutorialOne : MonoBehaviour
    {
        [SerializeField] CapsuleCollider MyCollider, OtherCollider;
        [SerializeField] TubeHandler CurrenTube;
        [SerializeField] GameObject HandObj, InfoTextObj;
        [SerializeField] Color CurrentColor;
        [SerializeField] int TubeCounter;
        [SerializeField] BowlColorHandler BowlObj;

        Vector3 _bowlScale = new Vector3(1.5f,0.1f,1.5f), _bowlPos = new Vector3(0,4.5f, 0);

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
            for (int c=0; c<2; c++)
            {
                CurrenTube.SetColor(CurrentColor);
                yield return new WaitForSeconds(0.1f);
            }
            if (TubeCounter == 1)
            {
                MyCollider.enabled = true;
                SingleIntegerEventsHolder.SwitchProtectorEvent?.Invoke(0);
                LevelsManager.I.CanPlay = true;
                HandObj.SetActive(true);
            }
        }

        private void OnMouseDown()
        {
            if (LevelsManager.I.CanPlay)
            {
                if (TubeCounter == 1)
                {
                    MyCollider.enabled = false;
                    OtherCollider.enabled = true;
                    HandObj.transform.DOLocalMoveX(130f, 0.35f).SetEase(Ease.InOutBack);
                }
                else if (TubeCounter == 2)
                {
                    MyCollider.enabled = false;
                    HandObj.SetActive(false);
                }
            }
        }

        void ColoringPreparation()
        {
            float tweenTime = 1;
            InfoTextObj.SetActive(false);
            transform.DOKill();
            transform.DOScale(_bowlScale, tweenTime);
            transform.DOLocalMove(_bowlPos, tweenTime).OnKill(() =>
            {
                if (CurrenTube != null && CurrenTube.WaterColors.Count > 0)
                {
                    BowlColorHandler colorBowl = Instantiate(BowlObj, transform.parent);
                    colorBowl.transform.localPosition = transform.localPosition;
                    colorBowl.SetColor(CurrenTube.CurrentColor);
                }
                if (gameObject != null)
                    Destroy(gameObject, 0.15f);
            });

        }
    }
}
