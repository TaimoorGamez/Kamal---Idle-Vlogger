using UnityEngine;
using DG.Tweening;
using Core.Events;
using UnityEngine.UI;
using Core.DB.Variables;

namespace Core.GamePlay
{
    public class MapHandler : MonoBehaviour
    {
        [SerializeField] Image MapFiller;

        int _totalTasks = 15;
        float _tweenTiming = 0.5f, _pulseScale = 1.1f;

        private void OnEnable()
        {
            SimpleEventsHolder.UpdateMapProgress += UpdateMapProgress;
            UpdateMapProgress();
        }

        private void OnDisable()
        {
            SimpleEventsHolder.UpdateMapProgress -= UpdateMapProgress;
        }

        void UpdateMapProgress()
        {
            int progress = DBVariablesHolder.MapProgress.Value;
            float targetFill = (float)progress / _totalTasks;

            MapFiller.DOFillAmount(targetFill, _tweenTiming).SetEase(Ease.OutCubic);

            if (progress >= _totalTasks)
            {
                MapFiller.transform.DOScale(_pulseScale, _tweenTiming).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            }
        }
    }
}
