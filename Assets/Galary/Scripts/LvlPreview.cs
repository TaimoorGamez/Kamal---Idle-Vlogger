using Core.Events;
using Core.States;
using UnityEngine;
using DG.Tweening;
using Core.GamePlay;
using UnityEngine.UI;
using Core.Plugins.Firebase;

namespace Core.Screen
{
    public class LvlPreview : UiScreens
    {
        [SerializeField] RawImage PaintingImg;
        [SerializeField] Image[] Stars;
        [SerializeField] Transform StarsOb;

        int _currentLvlNum, _currentLvlIndex;
        Color _starColor = Color.white;

        private void OnEnable()
        {
            OnOpen();
        }

        public override void OnOpen()
        {
            StarsOb.DOScale(1,_transitionDuration).SetEase(Ease.OutBack);
            FirebaseHandler.I?.LogEvent("Preview_Open");
        }

        public override void OnClose() 
        {
            gameObject.SetActive(false);
        }

        public void LevelDetails(Texture paintingTex, int lvlNum, int lvlIndex, int paintingStars)
        {
            PaintingImg.texture = paintingTex;
            for (int s = 0; s < paintingStars; s++)
            {
                Stars[s].color = _starColor;
            }
            PaintingImg.enabled = true;
            _currentLvlNum = lvlNum;
            _currentLvlIndex = lvlIndex;
        }

        public void OnClickPlay()
        {
            LevelsManager.I.TempLvlNum = _currentLvlNum;
            LevelsManager.I.TempLvlIndex = _currentLvlIndex;
            StateManager.I.ActiveState(StateManager.I.GamePlayStatePath);
            StateManager.I.DestroyState(StateManager.I.MainMenuStatePath);
            SimpleEventsHolder.InitLvlEvent?.Invoke();
            FirebaseHandler.I?.LogEvent($"Glry_Lvl_{_currentLvlNum}");
        }
    }
}