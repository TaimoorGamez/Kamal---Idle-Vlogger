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
            int progress = GetMapProgress();
            float targetFill = (float)progress / _totalTasks;

            MapFiller.DOFillAmount(targetFill, _tweenTiming).SetEase(Ease.OutCubic);

            if (progress >= _totalTasks)
            {
                DBVariablesHolder.IsGameplay.Value = 0;
                GameManager.Instance.StartGame();
            }
        }

        int GetMapProgress()
        {
            int progress = 0, mapLvls = GameManager.Instance.MapChangeCount;

            if (DBVariablesHolder.CharismaLvl.Value % mapLvls == 0)
                progress++;
            if (DBVariablesHolder.EruditionLvl.Value % mapLvls == 0)
                progress++;
            if (DBVariablesHolder.ImprovisationLvl.Value % mapLvls == 0)
                progress++;
            if (DBVariablesHolder.WitLvl.Value % mapLvls == 0)
                progress++;
            if (DBVariablesHolder.CameraLvl.Value % mapLvls == 0)
                progress++;
            if (DBVariablesHolder.TripodLvl.Value % mapLvls == 0)
                progress++;
            if (DBVariablesHolder.MicrophoneLvl.Value % mapLvls == 0)
                progress++;
            if (DBVariablesHolder.ClothesLvl.Value % mapLvls == 0)
                progress++;
            if (DBVariablesHolder.HairsLvl.Value % mapLvls == 0)
                progress++;
            if (DBVariablesHolder.WatchLvl.Value % mapLvls == 0)
                progress++;
            if (DBVariablesHolder.HouseLvl.Value % mapLvls == 0)
                progress++;
            if (DBVariablesHolder.GroundLvl.Value % mapLvls == 0)
                progress++;
            if (DBVariablesHolder.VehicleLvl.Value % mapLvls == 0)
                progress++;
            if (DBVariablesHolder.StatueLvl.Value % mapLvls == 0)
                progress++;
            if (DBVariablesHolder.BackyardLvl.Value % mapLvls == 0)
                progress++;

            return progress;
        }
    }
}
