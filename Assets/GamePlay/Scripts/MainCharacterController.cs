using System;
using UnityEngine;
using Core.Events;
using DG.Tweening;
using Core.DB.Variables;

namespace Core.GamePlay
{
    public class MainCharacterController : MonoBehaviour
    {
        [SerializeField] SpriteRenderer[] McRenders;
        [SerializeField] SpritesArray[] ClotheSprites;
        [SerializeField] string[] ItemsNames;

        float _updatingAnimationDuration = 0.1f, _revelAnimationDuration = 0.5f;
        UpgradeStateData[] _upgradeStates;

        private void OnEnable()
        {
            DoubleIntegerEventHolder.UpdateItemEvent += UpdateClothesWithDelay;
        }

        void OnDisable()
        {
            DoubleIntegerEventHolder.UpdateItemEvent -= UpdateClothesWithDelay;
        }

        private void Start()
        {
            UpdateClothesFirst();
        }

        void UpdateClothesFirst()
        {
            int clotheIndex = DBVariablesHolder.ClothesLvl.Value / GameManager.Instance.SpriteChangeCount;
            for (int c = 0; c < McRenders.Length; c++)
            {
                //Debug.Log("clothIndex : " + clotheIndex + " c: " + c);
                McRenders[c].sprite = ClotheSprites[clotheIndex].Sprites[c];
            }

            _upgradeStates = new UpgradeStateData[ItemsNames.Length];
            for (int i = 0; i < _upgradeStates.Length; i++)
            {
                if (PlayerPrefs.HasKey($"{ItemsNames[i]}_UpgradeState"))
                {
                    _upgradeStates[i] = JsonDB.Load<UpgradeStateData>($"{ItemsNames[i]}_UpgradeState");
                    if (_upgradeStates[i].IsUpdating)
                        CheckRemainingUpdates(i);
                }
                else
                {
                    _upgradeStates[i] = new UpgradeStateData { IsUpdating = false, UpdateStartTime = "" };
                }
            }
        }

        void UpdateClothesWithDelay(int eventIndex, int lvls)
        {
            if (eventIndex != 0)
                return;

            float delay = GameManager.Instance.UpdateDelay;
            float currentTime = delay;
            DOTween.To(() => currentTime, x => currentTime = x, 0, delay).OnComplete(() =>
            {
                DBVariablesHolder.ClothesLvl.Value += lvls;
                _upgradeStates[0].IsUpdating = false;
                UpdateClothesAnimation();
            });
        }

        void UpdateClothesAnimation()
        {
            int clotheIndex = DBVariablesHolder.ClothesLvl.Value / GameManager.Instance.SpriteChangeCount;
            Material mat = McRenders[0].material;
            mat.SetFloat("_Reveal", 0f);
            DOTween.To(() => mat.GetFloat("_Reveal"), x => mat.SetFloat("_Reveal", x), 1f, _revelAnimationDuration);
            for (int c = 0; c < McRenders.Length; c++)
            {
                //Debug.Log("clothIndex : " + clotheIndex + " c: " + c);
                McRenders[c].sprite = ClotheSprites[clotheIndex].Sprites[c];
                Transform clotheTransform = McRenders[c].transform;
                clotheTransform.DOKill();
                clotheTransform.DOScale(1.2f, _updatingAnimationDuration).SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        clotheTransform.DOScale(1f, _updatingAnimationDuration).SetEase(Ease.InOutSine);
                    });
            }
        }

        void CheckRemainingUpdates(int index)
        {
            switch (index) 
            {
                case 0:
                    CheckRemainingTime(index,DBVariablesHolder.ClothesLvl);
                    break;
            }
        }

        void CheckRemainingTime(int index, DBInt lvlData)
        {

            TimeSpan timePassed = DateTime.Now - DateTime.Parse(_upgradeStates[index].UpdateStartTime);
            float updateDelay = GameManager.Instance.UpdateDelay;
            float remainingTime = updateDelay - (float)timePassed.TotalSeconds;
            if (remainingTime > 0)
            {
                float currentTime = remainingTime;
                DOTween.To(() => currentTime, x => currentTime = x, 0, remainingTime).OnComplete(() =>
                {
                    lvlData.Value += _upgradeStates[index].Levels;
                    _upgradeStates[index].IsUpdating = false;
                    switch (index)
                    {
                        case 0:
                            UpdateClothesAnimation();
                            break;
                    }
                });
            }
            else
            {
                _upgradeStates[index].IsUpdating = false;
            }
        }
    }

    [Serializable]
    public class SpritesArray
    {
        public Sprite[] Sprites;
    }
}
