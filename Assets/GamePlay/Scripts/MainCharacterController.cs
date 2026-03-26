using System;
using UnityEngine;
using Core.Events;
using DG.Tweening;
using Core.DB.Variables;
using UnityEngine.U2D.Animation;

namespace Core.GamePlay
{
    public class MainCharacterController : MonoBehaviour
    {
        [SerializeField] SpriteResolver[] McResolvers;
        [SerializeField] string[] ItemsNames;
        [SerializeField] Material ClothesMaterial, BodyMaterial;

        float _updatingAnimationDuration = 0.1f, _revelAnimationDuration = 0.5f, _clotesScale = 1.1f;
        string[] _categoryName = {"LeftArm", "LeftLeg", "RightLeg", "Body", "RightArm"};
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
            for (int c = 0; c < McResolvers.Length; c++)
            {
                //Debug.Log("clothIndex : " + clotheIndex + " c: " + c);
                McResolvers[c].SetCategoryAndLabel(_categoryName[c], clotheIndex.ToString());
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
                ClothesMaterial.SetFloat("_Reveal", 0f);
                DOTween.To(() => ClothesMaterial.GetFloat("_Reveal"), x => ClothesMaterial.SetFloat("_Reveal", x), 1f, _revelAnimationDuration);
                UpdateClothesAnimation();
            });
        }

        void UpdateClothesAnimation()
        {
            _upgradeStates[0].IsUpdating = false;
            int clotheIndex = DBVariablesHolder.ClothesLvl.Value / GameManager.Instance.SpriteChangeCount;
            for (int c = 0; c < McResolvers.Length; c++)
            {
                //Debug.Log("clothIndex : " + clotheIndex + " c: " + c);
                McResolvers[c].SetCategoryAndLabel(_categoryName[c], clotheIndex.ToString());
                Transform clotheTransform = McResolvers[c].transform;
                clotheTransform.DOKill();
                clotheTransform.DOScale(_clotesScale, _updatingAnimationDuration).SetEase(Ease.OutBack)
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
                            ClothesMaterial.SetFloat("_Reveal", 0f);
                            DOTween.To(() => ClothesMaterial.GetFloat("_Reveal"), x => ClothesMaterial.SetFloat("_Reveal", x), 1f, _revelAnimationDuration);
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
}
