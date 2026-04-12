using System;
using UnityEngine;
using Core.Events;
using DG.Tweening;
using Core.DB.Variables;
using UnityEngine.U2D.Animation;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.GamePlay
{
    public class MainCharacterController : MonoBehaviour
    {
        [SerializeField] SpriteRenderer WatchRenderer;
        [SerializeField] McTalking McTalkingComponent;
        [SerializeField] SpriteResolver HeadResolver;
        [SerializeField] SpriteRenderer WatchImg;
        [SerializeField] SpriteResolver[] McResolvers;
        [SerializeField] string[] ItemsNames;
        [SerializeField] Material[] CharacterMaterials;

        int _clothesIndex = 0, _hairsIndex = 1, _watchIndex = 2;
        float _animationDuration = 0.5f, _UpdateAnimationScale = 1.1f;
        string[] _categoryName = {"LeftArm", "LeftLeg", "RightLeg", "Body", "RightArm"};
        string _headCategory = "Head_";
        UpgradeStateData[] _upgradeStates;

        private void OnEnable()
        {
            SingleIntegerEventsHolder.UpdateItemEvent += UpdateClothesWithDelay;
            SingleIntegerEventsHolder.UpdateItemEvent += UpdateHairsWithDelay;
            SingleIntegerEventsHolder.UpdateItemEvent += UpdateWatchWithDelay;
        }

        void OnDisable()
        {
            SingleIntegerEventsHolder.UpdateItemEvent -= UpdateClothesWithDelay;
            SingleIntegerEventsHolder.UpdateItemEvent -= UpdateHairsWithDelay;
            SingleIntegerEventsHolder.UpdateItemEvent -= UpdateWatchWithDelay;
        }

        private void Start()
        {
            _upgradeStates = new UpgradeStateData[ItemsNames.Length];
            UpdateClothesFirst();
            UpdateHeadSpritesFirst();
            UpdateWatchFirst();
        }
        void UpdateClothesFirst()
        {
            int clotheIndex = DBVariablesHolder.ClothesLvl.Value / GameManager.Instance.SpriteChangeCount;
            for (int c = 0; c < McResolvers.Length; c++)
            {
                McResolvers[c].SetCategoryAndLabel(_categoryName[c], clotheIndex.ToString());
            }

            if (PlayerPrefs.HasKey($"{ItemsNames[_clothesIndex]}_UpgradeState"))
            {
                _upgradeStates[_clothesIndex] = JsonDB.Load<UpgradeStateData>($"{ItemsNames[_clothesIndex]}_UpgradeState");
                if (_upgradeStates[_clothesIndex].IsUpdating)
                    CheckRemainingTime(_clothesIndex, DBVariablesHolder.ClothesLvl);
            }
            else
            {
                _upgradeStates[_clothesIndex] = new UpgradeStateData { IsUpdating = false, UpdateStartTime = "" };
            }
        }
        void UpdateHeadSpritesFirst()
        {
            string headCategory = _headCategory + (DBVariablesHolder.HairsLvl.Value / GameManager.Instance.SpriteChangeCount).ToString();
            HeadResolver.SetCategoryAndLabel(headCategory, "0");

            if (PlayerPrefs.HasKey($"{ItemsNames[_hairsIndex]}_UpgradeState"))
            {
                _upgradeStates[_hairsIndex] = JsonDB.Load<UpgradeStateData>($"{ItemsNames[_hairsIndex]}_UpgradeState");
                if (_upgradeStates[_hairsIndex].IsUpdating)
                    CheckRemainingTime(_hairsIndex, DBVariablesHolder.HairsLvl);
            }
            else
            {
                _upgradeStates[_hairsIndex] = new UpgradeStateData { IsUpdating = false, UpdateStartTime = "" };
            }
        }

        void UpdateWatchFirst()
        {
            int currentWatch = (DBVariablesHolder.WatchLvl.Value / GameManager.Instance.SpriteChangeCount);
            string key = $"Watch_{currentWatch}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnWatchLoaded;

            if (PlayerPrefs.HasKey($"{ItemsNames[_watchIndex]}_UpgradeState"))
            {
                _upgradeStates[_watchIndex] = JsonDB.Load<UpgradeStateData>($"{ItemsNames[_watchIndex]}_UpgradeState");
                if (_upgradeStates[_watchIndex].IsUpdating)
                    CheckRemainingTime(_watchIndex, DBVariablesHolder.WatchLvl);
            }
            else
            {
                _upgradeStates[_watchIndex] = new UpgradeStateData { IsUpdating = false, UpdateStartTime = "" };
            }
        }
        void OnWatchLoaded(AsyncOperationHandle<Sprite> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                WatchImg.sprite = handle.Result;
                DOTween.To(() => CharacterMaterials[_watchIndex].GetFloat("_Reveal"), x => CharacterMaterials[_watchIndex].SetFloat("_Reveal", x),
                1f, _animationDuration).From(0f).SetEase(Ease.Linear);
                WatchImg.transform.DOScale(Vector3.one, _animationDuration).From(Vector3.zero).SetEase(Ease.OutBack);
            }
            else
            {
                Debug.Log("Watch load failed!");
            }
        }

        void UpdateClothesWithDelay(int eventIndex)
        {
            if (eventIndex != _clothesIndex)
                return;

            float delay = GameManager.Instance.UpdateDelay;
            float currentTime = delay;
            DOTween.To(() => currentTime, x => currentTime = x, 0, delay).OnComplete(() =>
            {
                UpdateClothesAnimation();
            });
        }
        void UpdateClothesAnimation()
        {
            _upgradeStates[_clothesIndex].IsUpdating = false;
            DOTween.To(() => CharacterMaterials[_clothesIndex].GetFloat("_Reveal"), x => CharacterMaterials[_clothesIndex].SetFloat("_Reveal", x),
            1f, _animationDuration).From(0f).SetEase(Ease.Linear);
            int clotheIndex = DBVariablesHolder.ClothesLvl.Value / GameManager.Instance.SpriteChangeCount;
            for (int c = 0; c < McResolvers.Length; c++)
            {
                McResolvers[c].SetCategoryAndLabel(_categoryName[c], clotheIndex.ToString());
                Transform clotheTransform = McResolvers[c].transform;
                clotheTransform.DOKill();
                clotheTransform.DOScale(Vector3.one, _animationDuration).From(_UpdateAnimationScale).SetEase(Ease.OutBack);
            }
        }

        void UpdateHairsWithDelay(int eventIndex)
        {
            if (eventIndex != _hairsIndex)
                return;

            float delay = GameManager.Instance.UpdateDelay;
            float currentTime = delay;
            DOTween.To(() => currentTime, x => currentTime = x, 0, delay).OnComplete(() =>
            {
                UpdateHairsAnimation();
            });
        }
        void UpdateHairsAnimation()
        {
            _upgradeStates[_hairsIndex].IsUpdating = false;
            McTalkingComponent.StopTalking();
            DOTween.To(() => CharacterMaterials[_hairsIndex].GetFloat("_Reveal"), x => CharacterMaterials[_hairsIndex].SetFloat("_Reveal", x),
            1f, _animationDuration).From(0f).SetEase(Ease.Linear).OnComplete(() => McTalkingComponent.StartTalking(true));
            HeadResolver.transform.DOScale(Vector3.one, _animationDuration).From(_UpdateAnimationScale).SetEase(Ease.OutBack);
            int hairIndex = DBVariablesHolder.HairsLvl.Value / GameManager.Instance.SpriteChangeCount;
            string headCategory = _headCategory + (DBVariablesHolder.HairsLvl.Value / GameManager.Instance.SpriteChangeCount).ToString();
            HeadResolver.SetCategoryAndLabel(headCategory, "0");
        }

        void UpdateWatchWithDelay(int eventIndex)
        {
            if (eventIndex != _watchIndex)
                return;

            float delay = GameManager.Instance.UpdateDelay;
            float currentTime = delay;
            DOTween.To(() => currentTime, x => currentTime = x, 0, delay).OnComplete(() =>
            {
                UpdateWatchAnimation();
            });
        }
        void UpdateWatchAnimation()
        {
            _upgradeStates[_watchIndex].IsUpdating = false;
            int watchIndex = DBVariablesHolder.WatchLvl.Value / GameManager.Instance.SpriteChangeCount;
            string key = $"Watch_{watchIndex}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnWatchLoaded;
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
                    switch (index)
                    {
                        case 0:
                            UpdateClothesAnimation();
                            break;

                        case 1:
                            UpdateHairsAnimation();
                            break;

                        case 2:
                            UpdateWatchAnimation();
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