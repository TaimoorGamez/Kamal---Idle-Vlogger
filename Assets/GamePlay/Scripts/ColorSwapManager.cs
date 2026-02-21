using Core.Events;
using DG.Tweening;
using UnityEngine;
using Core.DB.Variables;
using Core.Plugins.Firebase;
using System.Collections.Generic;

namespace Core.GamePlay.WaterSort
{
    public class ColorSwapManager : MonoBehaviour
    {
        Dictionary<int,TubeHandler> SwapingTubes = new Dictionary<int, TubeHandler>();

        float _tweenTime = 0.25f;

        private void OnEnable()
        {
            SimpleEventsHolder.SwapColorsEvent += OnClickSwapBtn;
            SimpleEventsHolder.StartColoringEvent += StopSwapping;
        }

        private void OnDisable()
        {
            SimpleEventsHolder.SwapColorsEvent -= OnClickSwapBtn;
            SimpleEventsHolder.StartColoringEvent -= StopSwapping;
        }

        void OnClickSwapBtn()
        {
            if (!LevelsManager.I.SortingCompleted && LevelsManager.I.CompletedTubes < LevelsManager.I.CurrrentLvl -1)
            {
                if (!LevelsManager.I.UsingAnyFeature && !LevelsManager.I.IsSwaping)
                {
                    LevelsManager.I.UsingAnyFeature = true;
                    LevelsManager.I.IsSwaping = true;
                    SingleIntegerEventsHolder.SwitchProtectorEvent?.Invoke(1);
                    SingleIntegerEventsHolder.ShowToastEvent?.Invoke(8);
                }
                else
                {
                    SingleIntegerEventsHolder.ShowToastEvent?.Invoke(4);
                }
            }
        }

        public void AddTubeForSwaping(TubeHandler tube)
        {
            if (SwapingTubes.Count < 2)
            {
                if (tube.WaterColors.Count > 0)
                {
                    if (SwapingTubes.Count < 1)
                    {
                        SingleIntegerEventsHolder.ShowToastEvent?.Invoke(9);
                        tube.transform.DOScale(1.2f,_tweenTime).SetEase(Ease.OutBack);
                    }
                    SwapingTubes.Add(SwapingTubes.Count, tube);
                }
                else
                {
                    SingleIntegerEventsHolder.ShowToastEvent?.Invoke(0);
                }
            }
            
            if(SwapingTubes.Count == 2)
            {
                SwapColorsNow();
            }
        }

        void SwapColorsNow()
        {
            SwapingTubes[0].transform.DOScale(1f, _tweenTime).SetEase(Ease.OutBack);

            if (SwapingTubes[0].CurrentColor != SwapingTubes[1].CurrentColor)
            {
                Color oneColor = SwapingTubes[0].CurrentColor;
                SwapingTubes[0].SwapeColor(SwapingTubes[1].CurrentColor);
                SwapingTubes[1].SwapeColor(oneColor);
                if (DBVariablesHolder.LvlNum.Value >= LevelsManager.I.MinLvlCount)
                {
                    SimpleEventsHolder.UpdateSwapStatusEvent?.Invoke();
                }
                DoubleIntegerEventHolder.TaskEvent?.Invoke(4, 1);
                FirebaseHandler.I?.LogEvent($"swap_lvl_{DBVariablesHolder.LvlIndex.Value}");
            }
            else
            {
                SingleIntegerEventsHolder.ShowToastEvent?.Invoke(1);
            }
            SingleIntegerEventsHolder.SwitchProtectorEvent?.Invoke(0);
            SwapingTubes.Clear();
            LevelsManager.I.IsSwaping = false;
            LevelsManager.I.UsingAnyFeature = false;
        }

        void StopSwapping()
        {
            SingleIntegerEventsHolder.SwitchProtectorEvent?.Invoke(0);
            SwapingTubes.Clear();
            LevelsManager.I.IsSwaping = false;
            LevelsManager.I.UsingAnyFeature = false;
        }
    }
}
