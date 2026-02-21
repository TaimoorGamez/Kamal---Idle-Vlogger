using Core.DB.Variables;
using Core.Events;
using Core.GamePlay;
using Core.Purchase;
using Core.States;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Core.Screen
{
    public class MainMenuScreen : UiScreens
    {
        [SerializeField] InAppPurchase InAppPurchaser;
        [SerializeField] TextMeshProUGUI[] Lvls;
        [SerializeField] Transform LevelView;
        [SerializeField] RectTransform LevelsHolder;
        [SerializeField] GameObject FeedbackBtn;

        int _activeLvl = 3, requiredFeedbackLvl = 15;
        string _privacyLink = "https://sites.google.com/view/sortpaint-privacy-policy/";

        private void Start()
        {
            if (!InAppPurchaser.IsInitialized)
            {
                InAppPurchaser.InitializePurchasing();
            }

            if (DBVariablesHolder.LvlNum.Value > requiredFeedbackLvl)
            {
                FeedbackBtn.SetActive(true);
            }

            LevelsManager.I.TempLvlIndex = -1;
            for (int l = 0; l < Lvls.Length; l++)
            {
                if (l < _activeLvl)
                {
                    Lvls[l].text = (DBVariablesHolder.LvlNum.Value - (_activeLvl - l)).ToString();
                }
                else if (l > _activeLvl)
                {
                    Lvls[l].text = (DBVariablesHolder.LvlNum.Value + (l - _activeLvl)).ToString();
                }
                else
                {
                    Lvls[l].text = DBVariablesHolder.LvlNum.Value.ToString();
                }
            }

            LevelView.DOScale(1, 1).SetEase(Ease.OutBack).OnComplete(() => LevelsHolder.DOAnchorPosY(150, 1).SetEase(Ease.OutBack));
        }

        public void OnClickPlayButton()
        {
            SimpleEventsHolder.InitLvlEvent?.Invoke();
            StateManager.I.ActiveState(StateManager.I.GamePlayStatePath);
            StateManager.I.DestroyState(StateManager.I.MainMenuStatePath);
        }

        public void OpenPrivacyPolicy()
        {
            Application.OpenURL(_privacyLink);
        }

    }
}