using TMPro;
using Core.Store;
using UnityEngine;
using Core.Events;
using DG.Tweening;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.Screen
{
    public class StoreScreen : UiScreens
    {
        [SerializeField] Transform FillImage;
        [SerializeField] TextMeshProUGUI LoadingText;
        [SerializeField] GameObject Loading;
        [SerializeField] string FlameItemsPath, SprayItemsPath, CapItemsPath;
        [SerializeField] int MaxFlameThrowers, MaxCaps, MaxSprayCans;

        bool _isClearing = false;
        string _loadingTxt = "Loading...     ";

        private void OnEnable()
        {
            LoadStoreItems();
        }

        private async void OnDisable()
        {
            await ClearDictionariesAsync();
        }

        public override void OnOpen()
        {
            Loading.SetActive(false);
            Body.gameObject.SetActive(true);
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(3);
            Body.DOAnchorPosX(0, _transitionDuration).SetEase(Ease.OutBack);
        }

        async void LoadStoreItems()
        {
            Loading.SetActive(true);
            await ClearDictionariesAsync();

            UpdateLoadingUI(0);

            // total items count for progress
            int totalCount = MaxCaps + MaxFlameThrowers + MaxSprayCans;
            int loadedCount = 0;

            // ============================================================
            // ------------------- LOAD FLAME THROWERS --------------------
            // ============================================================
            Dictionary<int, GameObject> flamesDictionary = new Dictionary<int, GameObject>();
            for (int i = 0; i < MaxFlameThrowers; i++)
            {
                string address = FlameItemsPath + i;

                AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(address);

                await handle.Task;

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    flamesDictionary.Add(i, handle.Result);
                }

                loadedCount++;
                float progress = (float)loadedCount / (float)totalCount;
                UpdateLoadingUI(progress);
            }
            await Task.Yield();
            StorageData.StoreItemsContainer.Add(StorageData.FlameThrowersKey, flamesDictionary);
            await Task.Yield();

            // ============================================================
            // --------------------- LOAD CAPS ----------------------------
            // ============================================================
            Dictionary<int, GameObject> capsDictionary = new Dictionary<int, GameObject>();
            for (int i = 0; i < MaxCaps; i++)
            {
                string address = CapItemsPath + i;   

                AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(address);

                await handle.Task;

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    capsDictionary.Add(i, handle.Result);
                }

                // update progress
                loadedCount++;
                float progress = (float)loadedCount / (float)totalCount;
                UpdateLoadingUI(progress);
            }
            await Task.Yield();
            StorageData.StoreItemsContainer.Add(StorageData.CapsKey, capsDictionary);
            await Task.Yield();

            // ============================================================
            // --------------------- LOAD SPRAY CANS ----------------------
            // ============================================================
            Dictionary<int, GameObject> spraysDictionary = new Dictionary<int, GameObject>();
            for (int i = 0; i < MaxSprayCans; i++)
            {
                string address = SprayItemsPath + i;

                AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(address);

                await handle.Task;

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    spraysDictionary.Add(i, handle.Result);
                }

                loadedCount++;
                float progress = (float)loadedCount / (float)totalCount;
                UpdateLoadingUI(progress);
            }
            await Task.Yield();
            StorageData.StoreItemsContainer.Add(StorageData.SpraysKey, spraysDictionary);
            await Task.Yield();

            // ============================================================
            // ------------------------- DONE ------------------------------
            // ============================================================

            FillImage.DOScaleX(1f, 0.1f);
            OnOpen();
        }
        
        void UpdateLoadingUI(float progress)
        {
            FillImage.DOScaleX(progress, 0.1f).SetEase(Ease.Linear).SetUpdate(true);
            int percent = (int)(progress * 100f);
            LoadingText.text = _loadingTxt + percent.ToString() + "%";
        }

        public override void OnClose()
        {
            Body.DOKill();
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(2);
            Body.DOAnchorPosX(1500, _transitionDuration / 2).SetEase(Ease.InBack).OnComplete(() => {
                Body.gameObject.SetActive(false);
                gameObject.SetActive(false);
                });
        }

        async Task ClearDictionariesAsync()
        {
            if (_isClearing) return;
            _isClearing = true;
            foreach (var outerPair in StorageData.StoreItemsContainer)
            {
                var dict = outerPair.Value;

                foreach (var handlePair in dict)
                {
                    var handle = handlePair.Value;

                    if (handle != null)
                        Addressables.Release(handle);
                }

                dict.Clear();
                await Task.Yield();
            }

            StorageData.StoreItemsContainer.Clear();
            await Task.Yield();
            _isClearing = false;
        }

    }
}
