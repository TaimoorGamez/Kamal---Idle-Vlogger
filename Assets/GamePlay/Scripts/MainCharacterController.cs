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
        [SerializeField] Material ClothesMaterial, BodyMaterial, WatchMaterial;

        int _currentWatch;
        float _updatingAnimationDuration = 0.1f, _revelAnimationDuration = 0.5f, _clotesScale = 1.1f;
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
            UpdateWatchFirst();
            UpdateClothesFirst();
            UpdateHeadSpritesFirst();
        }

        void UpdateHeadSpritesFirst()
        {
            string headCategory = _headCategory + (DBVariablesHolder.HairsLvl.Value / GameManager.Instance.SpriteChangeCount).ToString();
            HeadResolver.SetCategoryAndLabel(headCategory, "0");
        }
        void UpdateClothesFirst()
        {
            int clotheIndex = DBVariablesHolder.ClothesLvl.Value / GameManager.Instance.SpriteChangeCount;
            for (int c = 0; c < McResolvers.Length; c++)
            {
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
        void UpdateWatchFirst()
        {
            _currentWatch = (DBVariablesHolder.StatueLvl.Value / GameManager.Instance.SpriteChangeCount);
            string key = $"Watch_{_currentWatch}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnWatchLoaded;
        }
        void OnWatchLoaded(AsyncOperationHandle<Sprite> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                WatchImg.sprite = handle.Result;
                WatchMaterial.SetFloat("_Reveal", 0f);
                WatchImg.transform.DOScale(Vector3.one, _updatingAnimationDuration).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    DOTween.To(
                    () => WatchMaterial.GetFloat("_Reveal"),
                    x => WatchMaterial.SetFloat("_Reveal", x),
                    1f, _updatingAnimationDuration);
                });
            }
            else
            {
                Debug.Log("Statue load failed!");
            }
        }

        void UpdateClothesWithDelay(int eventIndex)
        {
            if (eventIndex != 0)
                return;

            float delay = GameManager.Instance.UpdateDelay;
            float currentTime = delay;
            DOTween.To(() => currentTime, x => currentTime = x, 0, delay).OnComplete(() =>
            {
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

        void UpdateHairsWithDelay(int eventIndex)
        {
            if (eventIndex != 1)
                return;

            float delay = GameManager.Instance.UpdateDelay;
            float currentTime = delay;
            DOTween.To(() => currentTime, x => currentTime = x, 0, delay).OnComplete(() =>
            {
                BodyMaterial.SetFloat("_Reveal", 0f);
                McTalkingComponent.StopTalking();
                DOTween.To(() => BodyMaterial.GetFloat("_Reveal"), x => BodyMaterial.SetFloat("_Reveal", x), 1f, _revelAnimationDuration)
                .OnComplete(() => McTalkingComponent.StartTalking(true));
                UpdateHairsAnimation();
            });
        }

        void UpdateHairsAnimation()
        {
            _upgradeStates[1].IsUpdating = false;
            int hairIndex = DBVariablesHolder.HairsLvl.Value / GameManager.Instance.SpriteChangeCount;
            string headCategory = _headCategory + (DBVariablesHolder.HairsLvl.Value / GameManager.Instance.SpriteChangeCount).ToString();
            HeadResolver.SetCategoryAndLabel(headCategory, "0");
        }

        void UpdateWatchWithDelay(int eventIndex)
        {
            if (eventIndex != 2)
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
            _upgradeStates[2].IsUpdating = false;
            int watchIndex = DBVariablesHolder.StatueLvl.Value / GameManager.Instance.SpriteChangeCount;
            string key = $"Watch_{watchIndex}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnWatchLoaded;
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
