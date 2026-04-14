using TMPro;
using System;
using UnityEngine;
using Core.Events;
using DG.Tweening;
using Core.Economy;
using UnityEngine.UI;
using Core.DB.Variables;
using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.GamePlay
{
    public class GameplayHandler : MonoBehaviour
    {
        [SerializeField] RectTransform MaxLvlToggleIcon;
        [SerializeField] Image MaxLvlToggleBar;
        [SerializeField] McTalking McTalk;
        [SerializeField] Animator McAnimator;
        [SerializeField] SpriteRenderer StatueImg, WatchImg, CameraImg, MicrophoneImg, TripodImg;
        [SerializeField] TextMeshProUGUI IncomeTxt;
        [SerializeField] Vector2[] StatuePositions, CameraPositions, MicPositions, TripodPositions;
        [SerializeField] string[] BaseStreamAnimation, ItemsNames;

        int _cameraIndex = 3, _tripodIndex = 4, _micIndex = 5, _statueIndex = 10;
        float _tappedMultipler = 1, _maxTapped = 1.8f, _perSecond = 0.25f, _maxLvlToggleAnchor = 20, _maxLvlAnimationDuration = 0.25f,
              _updatingAnimationDuration = 0.45f, _visualDuration = 0.5f;
        bool _canStream = false, _canEarn = false;
        string[] _mainStreamAnimations;
        Coroutine _streamRoutine, _earningRotine;
        UpgradeStateData[] _upgradeStates;

        void OnEnable()
        {
            SingleIntegerEventsHolder.UpdateItemEvent += UpdateCameraWithDelay;
            SingleIntegerEventsHolder.UpdateItemEvent += UpdateTripodWithDelay;
            SingleIntegerEventsHolder.UpdateItemEvent += UpdateMicrophoneWithDelay;
            SingleIntegerEventsHolder.UpdateItemEvent += UpdateStatueWithDelay;
        }

        private void OnDisable()
        {
            SingleIntegerEventsHolder.UpdateItemEvent -= UpdateCameraWithDelay;
            SingleIntegerEventsHolder.UpdateItemEvent -= UpdateTripodWithDelay;
            SingleIntegerEventsHolder.UpdateItemEvent -= UpdateMicrophoneWithDelay;
            SingleIntegerEventsHolder.UpdateItemEvent -= UpdateStatueWithDelay;
        }

        void Start()
        {
            ChangePriceText(DBVariablesHolder.MaxLevels.Value);
        }

        public void ContinueGameplay()
        {
            _upgradeStates = new UpgradeStateData[ItemsNames.Length];
            McAnimator.SetTrigger("Default");
            LoadCameraFirst();
            LoadTripodFirst();
            LoadMicrophoneFirst();
            LoadStatueFirst();
            StartStreaming();
        }

        void LoadCameraFirst()
        {
            int currentCamera = (DBVariablesHolder.CameraLvl.Value / GameManager.Instance.SpriteChangeCount);
            string key = $"Camera_{currentCamera}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnCameraLoaded;
            CameraImg.transform.position = CameraPositions[currentCamera];

            if (PlayerPrefs.HasKey($"{ItemsNames[0]}_UpgradeState"))
            {
                _upgradeStates[0] = JsonDB.Load<UpgradeStateData>($"{ItemsNames[0]}_UpgradeState");
                if (_upgradeStates[0].IsUpdating)
                    CheckRemainingTime(0, DBVariablesHolder.CameraLvl);
            }
            else
            {
                _upgradeStates[0] = new UpgradeStateData { IsUpdating = false, UpdateStartTime = "" };
            }
        }
        void OnCameraLoaded(AsyncOperationHandle<Sprite> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                CameraImg.sprite = handle.Result; 
                Material mat = CameraImg.material;
                DOTween.To(() => mat.GetFloat("_Reveal"), x => mat.SetFloat("_Reveal", x), 1f, _visualDuration).From(0f).SetEase(Ease.Linear);
                CameraImg.transform.DOScale(Vector3.one, _updatingAnimationDuration).From(Vector3.zero).SetEase(Ease.OutBack);
            }
            else
            {
                Debug.Log("Camera load failed!");
            }
        }

        void LoadTripodFirst()
        {
            int currentTripod = (DBVariablesHolder.TripodLvl.Value / GameManager.Instance.SpriteChangeCount);
            string key = $"Tripod_{currentTripod}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnTripodLoaded;
            TripodImg.transform.position = TripodPositions[currentTripod];

            if (PlayerPrefs.HasKey($"{ItemsNames[1]}_UpgradeState"))
            {
                _upgradeStates[1] = JsonDB.Load<UpgradeStateData>($"{ItemsNames[1]}_UpgradeState");
                if (_upgradeStates[1].IsUpdating)
                    CheckRemainingTime(1, DBVariablesHolder.TripodLvl);
            }
            else
            {
                _upgradeStates[1] = new UpgradeStateData { IsUpdating = false, UpdateStartTime = "" };
            }
        }
        void OnTripodLoaded(AsyncOperationHandle<Sprite> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                TripodImg.sprite = handle.Result;
                Material mat = TripodImg.material;
                DOTween.To(() => mat.GetFloat("_Reveal"), x => mat.SetFloat("_Reveal", x), 1f, _visualDuration).From(0f).SetEase(Ease.Linear);
                TripodImg.transform.DOScale(Vector3.one, _updatingAnimationDuration).From(Vector3.zero).SetEase(Ease.OutBack);
            }
            else
            {
                Debug.Log("Tripod load failed!");
            }
        }

        void LoadMicrophoneFirst()
        {
            int currentMic = (DBVariablesHolder.MicrophoneLvl.Value / GameManager.Instance.SpriteChangeCount);
            string key = $"Microphone_{currentMic}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnMicrophoneLoaded;
            MicrophoneImg.transform.position = MicPositions[currentMic];

            if (PlayerPrefs.HasKey($"{ItemsNames[2]}_UpgradeState"))
            {
                _upgradeStates[2] = JsonDB.Load<UpgradeStateData>($"{ItemsNames[2]}_UpgradeState");
                if (_upgradeStates[2].IsUpdating)
                    CheckRemainingTime(2, DBVariablesHolder.MicrophoneLvl);
            }
            else
            {
                _upgradeStates[2] = new UpgradeStateData { IsUpdating = false, UpdateStartTime = "" };
            }
        }
        void OnMicrophoneLoaded(AsyncOperationHandle<Sprite> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                MicrophoneImg.sprite = handle.Result;
                Material mat = MicrophoneImg.material;
                DOTween.To(() => mat.GetFloat("_Reveal"), x => mat.SetFloat("_Reveal", x), 1f, _visualDuration).From(0f).SetEase(Ease.Linear);
                MicrophoneImg.transform.DOScale(Vector3.one, _updatingAnimationDuration).From(Vector3.zero).SetEase(Ease.OutBack);
            }
            else
            {
                Debug.Log("Microphone load failed!");
            }
        }

        void LoadStatueFirst()
        {
            int currentStatue = (DBVariablesHolder.StatueLvl.Value / GameManager.Instance.SpriteChangeCount);
            string key = $"Statue_{currentStatue}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnStatueLoaded;
            StatueImg.transform.position = StatuePositions[currentStatue];

            if (PlayerPrefs.HasKey($"{ItemsNames[3]}_UpgradeState"))
            {
                _upgradeStates[3] = JsonDB.Load<UpgradeStateData>($"{ItemsNames[3]}_UpgradeState");
                if (_upgradeStates[3].IsUpdating)
                    CheckRemainingTime(3, DBVariablesHolder.StatueLvl);
            }
            else
            {
                _upgradeStates[3] = new UpgradeStateData { IsUpdating = false, UpdateStartTime = "" };
            }
        }
        void OnStatueLoaded(AsyncOperationHandle<Sprite> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                StatueImg.sprite = handle.Result;
                Material mat = StatueImg.material;
                DOTween.To(() => mat.GetFloat("_Reveal"), x => mat.SetFloat("_Reveal", x), 1f, _visualDuration).From(0f).SetEase(Ease.Linear);
                StatueImg.transform.DOScale(Vector3.one, _updatingAnimationDuration).From(Vector3.zero).SetEase(Ease.OutBack);
            }
            else
            {
                Debug.Log("Statue load failed!");
            }
        }

        void UpdateCameraWithDelay(int eventIndex)
        {
            if (eventIndex != _cameraIndex)
                return;

            float delay = GameManager.Instance.UpdateDelay;
            float currentTime = delay;
            DOTween.To(() => currentTime, x => currentTime = x, 0, delay).OnComplete(() =>
            {
                UpdateCameraAnimation();
            });
        }
        void UpdateCameraAnimation()
        {
            _upgradeStates[0].IsUpdating = false;
            JsonDB.Save($"{ItemsNames[0]}_UpgradeState", _upgradeStates[0]);

            int currentCamera = (DBVariablesHolder.CameraLvl.Value / GameManager.Instance.SpriteChangeCount);
            string key = $"Camera_{currentCamera}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnCameraLoaded;
            CameraImg.transform.position = CameraPositions[currentCamera];
        }

        void UpdateTripodWithDelay(int eventIndex)
        {
            if (eventIndex != _tripodIndex)
                return;

            float delay = GameManager.Instance.UpdateDelay;
            float currentTime = delay;
            DOTween.To(() => currentTime, x => currentTime = x, 0, delay).OnComplete(() =>
            {
                UpdateTripodAnimation();
            });
        }
        void UpdateTripodAnimation()
        {
            _upgradeStates[1].IsUpdating = false;
            JsonDB.Save($"{ItemsNames[1]}_UpgradeState", _upgradeStates[1]);

            int currentTripod = (DBVariablesHolder.TripodLvl.Value / GameManager.Instance.SpriteChangeCount);
            string key = $"Tripod_{currentTripod}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnTripodLoaded;
            TripodImg.transform.position = TripodPositions[currentTripod];
        }

        void UpdateMicrophoneWithDelay(int eventIndex)
        {
            if (eventIndex != _micIndex)
                return;

            float delay = GameManager.Instance.UpdateDelay;
            float currentTime = delay;
            DOTween.To(() => currentTime, x => currentTime = x, 0, delay).OnComplete(() =>
            {
                UpdateMicrophoneAnimation();
            });
        }
        void UpdateMicrophoneAnimation()
        {
            _upgradeStates[2].IsUpdating = false;
            JsonDB.Save($"{ItemsNames[2]}_UpgradeState", _upgradeStates[2]);

            int currentMic = (DBVariablesHolder.MicrophoneLvl.Value / GameManager.Instance.SpriteChangeCount);
            string key = $"Microphone_{currentMic}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnMicrophoneLoaded;
            MicrophoneImg.transform.position = MicPositions[currentMic];
        }

        void UpdateStatueWithDelay(int eventIndex)
        {
            if (eventIndex != _statueIndex)
                return;

            float delay = GameManager.Instance.UpdateDelay;
            float currentTime = delay;
            DOTween.To(() => currentTime, x => currentTime = x, 0, delay).OnComplete(() =>
            {
                UpdateStatueAnimation();
            });
        }
        void UpdateStatueAnimation()
        {
            _upgradeStates[3].IsUpdating = false;
            JsonDB.Save($"{ItemsNames[3]}_UpgradeState", _upgradeStates[3]);

            int currentStatue = (DBVariablesHolder.StatueLvl.Value / GameManager.Instance.SpriteChangeCount);
            string key = $"Statue_{currentStatue}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnStatueLoaded;
            StatueImg.transform.position = StatuePositions[currentStatue];
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
                            UpdateCameraAnimation();
                            break;

                        case 1:
                            UpdateTripodAnimation();
                            break;

                        case 2:
                            UpdateMicrophoneAnimation();
                            break;

                        case 3:
                            UpdateStatueAnimation();
                            break;
                    }
                });
            }
            else
            {
                _upgradeStates[index].IsUpdating = false;
            }
        }

        void StartStreaming()
        {
            if (_streamRoutine != null)
                StopCoroutine(_streamRoutine);

            _mainStreamAnimations = BaseStreamAnimation;
            _canStream = true;
            _streamRoutine = StartCoroutine(StreamCoroutine());
        }

        public void StopStreaming()
        {
            _canStream = false;
            if (_streamRoutine != null)
                StopCoroutine(_streamRoutine);
        }

        IEnumerator StreamCoroutine()
        {
            yield return new WaitForSeconds(2f);
            McTalk.StartTalking(true);
            StartEarning();
            while (_canStream)
            {
                int i = UnityEngine.Random.Range(0, _mainStreamAnimations.Length);
                yield return new WaitForSeconds(5f);
                McAnimator.Play(_mainStreamAnimations[i]);
            }
        }

        void StartEarning()
        {
            if (_earningRotine != null)
                StopCoroutine(_earningRotine);

            _canEarn = true;
            _earningRotine = StartCoroutine(EarningCoroutine());
        }

        IEnumerator EarningCoroutine()
        {
            while (_canEarn)
            {
                Subscribers.Amount += 1;
                float income = GetIncomePerSecond();
                if (CashParticle._isTapped) 
                {
                    if(_tappedMultipler < _maxTapped)
                        _tappedMultipler += 0.1f;

                    IncomeTxt.color = Color.green;
                }
                else if (_tappedMultipler > 1)
                {
                    _tappedMultipler -= 0.1f;
                    IncomeTxt.color = Color.black;
                }
                income *= _tappedMultipler;
                IncomeTxt.text = $"{income:F2}/s";
                CashCurrency.Amount += income;
                yield return new WaitForSecondsRealtime(_perSecond);
            }
        }

        float GetIncomePerSecond()
        {
            int hundreds = Subscribers.Amount / 100;
            float subscriberIncome = hundreds * 0.01f;

            return DBVariablesHolder.BasicIncome.Value + subscriberIncome;
        }

        public void ToggleMaxLvl()
        {
            DBVariablesHolder.MaxLevels.Value = 1 - DBVariablesHolder.MaxLevels.Value;
            ChangePriceText(DBVariablesHolder.MaxLevels.Value);
        }

        void ChangePriceText(int index)
        {
            if (DBVariablesHolder.MaxLevels.Value == 1)
            {
                MaxLvlToggleIcon.DOAnchorPosX(_maxLvlToggleAnchor, _maxLvlAnimationDuration).SetEase(Ease.InOutBack);
                MaxLvlToggleBar.DOBlendableColor(Color.green, _maxLvlAnimationDuration);
            }
            else
            {
                MaxLvlToggleIcon.DOAnchorPosX(-_maxLvlToggleAnchor, _maxLvlAnimationDuration).SetEase(Ease.InOutBack);
                MaxLvlToggleBar.DOBlendableColor(Color.clear, _maxLvlAnimationDuration);
            }
            SimpleEventsHolder.UpdatePriceTxt?.Invoke();
        }

    }
}