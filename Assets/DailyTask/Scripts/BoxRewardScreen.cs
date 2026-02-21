using Core.Store;
using DG.Tweening;
using Core.Events;
using UnityEngine;
using Core.Economy;
using UnityEngine.UI;
using Core.DB.Variables;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.Screen
{
    public class BoxRewardScreen : UiScreens
    {
        [SerializeField] RectTransform[] Cards, ItemHolders;
        [SerializeField] Sprite[] PowerSprites;
        [SerializeField] Image PowerImage;

        float _tweenTime = 0.5f, _unboxingTime = 1, _cardSize = 1.25f;
        int _cardIndex = 0, _spraysLength = 8, _capsLength = 6, _flamesLength = 4;
        Vector2 _cardPosition = new Vector2(0, -350);
        string _flamesPath = "Store/Flame/", _capsPath = "Store/Cap/", _spraysPath = "Store/Spray/";
        AsyncOperationHandle _itemHandle;
        bool _itemReleaseInProgress = false;

        private void Start()
        {
            OnOpen();
        }

        public override void OnOpen()
        {
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(3);
            Body.DOAnchorPosX(0, _tweenTime).SetEase(Ease.OutBack).OnComplete(() => CardReward());
        }

        void CardReward()
        {
            switch (_cardIndex)
            {
                case 0:
                    int randomItem = Random.Range(0, 3);
                    int randomReward = -1;
                    switch (randomItem)
                    {
                        case 0:
                            randomReward = Random.Range(0, _flamesLength);
                            LoadRewardItem(_flamesPath + randomReward);
                            PowerImage.sprite = PowerSprites[0];
                            DBVariableDictionariesHolder.PowersData[0].Value += 1;
                            StorageData.AllItems[StorageData.FlameThrowersKey][randomReward].IsPurchased = true;
                            break;

                        case 1:
                            randomReward = Random.Range(0, _capsLength);
                            LoadRewardItem(_capsPath + randomReward);
                            PowerImage.sprite = PowerSprites[1];
                            DBVariableDictionariesHolder.PowersData[1].Value += 1;
                            StorageData.AllItems[StorageData.CapsKey][randomReward].IsPurchased = true;
                            break;

                        case 2:
                            randomReward = Random.Range(0, _spraysLength);
                            LoadRewardItem(_spraysPath + randomReward);
                            PowerImage.sprite = PowerSprites[2];
                            DBVariableDictionariesHolder.PowersData[2].Value += 1;
                            StorageData.AllItems[StorageData.SpraysKey][randomReward].IsPurchased = true;
                            break;
                    }
                    break;
                case 2:
                   CurrenciesHolder.CashCurrency.Amount += 300;
                    break;
            }
            StartUnBoxsing();
        }

        async void LoadRewardItem(string path)
        {
            _itemHandle = Addressables.LoadAssetAsync<GameObject>(path);
            await _itemHandle.Task;

            if (_itemHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"Failed to load Addressable prefab at: {path}");
                return;
            }

            GameObject itemObj = Instantiate(_itemHandle.Result as GameObject, ItemHolders[0]);
            
            await Task.Yield();
            await Task.Yield();
        }

        void StartUnBoxsing()
        {
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(3);
            Cards[_cardIndex].DOAnchorPos(_cardPosition, _unboxingTime).SetEase(Ease.OutQuad);
            Cards[_cardIndex].DOScale(_cardSize, _unboxingTime).SetEase(Ease.OutBack);
            Cards[_cardIndex].DOLocalRotate(new Vector3(0, 360f, 0), _unboxingTime / 2, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(3, LoopType.Incremental).OnComplete(() => {
                    Vector3 euler = transform.localEulerAngles;
                    euler.z = 0f;
                    Cards[_cardIndex].localEulerAngles = euler;
                    ItemHolders[_cardIndex].gameObject.SetActive(true);
                    Invoke(nameof(DestroyCard), 1.5f);
                });
        }

        void DestroyCard()
        {
            Cards[_cardIndex].gameObject.SetActive(false);
            _cardIndex++;
            if (_cardIndex < Cards.Length)
            {
                CardReward();
            }
            else
            {
                OnClose();
            }
        }

        public override void OnClose()
        {
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(2);
            Body.DOAnchorPosX(1500, _tweenTime).SetEase(Ease.OutBack).OnComplete(() => gameObject.SetActive(false));
        }

        private async void OnDisable()
        {
            await ReleaseItemHandleSafelyAsync();
        }
        async Task ReleaseItemHandleSafelyAsync()
        {
            if (_itemReleaseInProgress)
                return;

            _itemReleaseInProgress = true;

            // wait for next frame (Unity safe point)
            await Task.Yield();

            if (_itemHandle.IsValid())
            {
                Addressables.Release(_itemHandle);
                _itemHandle = default;
            }

            // optional extra frame (Android / scene safety)
            await Task.Yield();

            _itemReleaseInProgress = false;
        }

    }
}
