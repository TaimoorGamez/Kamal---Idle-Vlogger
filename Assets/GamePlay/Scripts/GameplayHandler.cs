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
        [SerializeField] GameObject BoosterParticle, CashDonation;
        [SerializeField] RectTransform MaxLvlToggleIcon;
        [SerializeField] Image MaxLvlToggleBar;
        [SerializeField] McTalking McTalk;
        [SerializeField] Animator McAnimator;
        [SerializeField] SpriteRenderer StatueImg, CameraImg, MicrophoneImg, TripodImg;
        [SerializeField] TextMeshProUGUI IncomeTxt;
        [SerializeField] Vector2[] StatuePositions, CameraPositions, MicPositions, TripodPositions;
        [SerializeField] string[] BaseStreamAnimation, ItemsNames;
        [SerializeField] MultiplierBar MultiplierIncome;

        int _cameraIndex = 3, _tripodIndex = 4, _micIndex = 5, _statueIndex = 10;
        float _tappedMultipler = 1, _maxTapped = 1.8f, _tappedSpeed = 0.1f, _maxLvlToggleAnchor = 20, _maxLvlAnimationDuration = 0.25f,
              _updatingAnimationDuration = 0.45f, _visualDuration = 0.5f, _maxDonationDelay = 50, _donationCap = 2.5f, 
              _donationTimer = 50, _x2IncomeDuration = 3600, _x2TapDuration = 3600, _x10IncomeDuration = 10;
        bool _canStream = false, _canEarn = false, _canDonate = false, _can2xIncome, _can10xIncome;
        string[] _mainStreamAnimations;
        Coroutine _streamRoutine, _earningRotine, _donationRotine, _x10Routine, _x2IncomeRoutine, _x2TapRoutine;
        UpgradeStateData[] _upgradeStates;

        void OnEnable()
        {
            SingleIntegerEventsHolder.UpdateItemEvent += UpdateCameraWithDelay;
            SingleIntegerEventsHolder.UpdateItemEvent += UpdateTripodWithDelay;
            SingleIntegerEventsHolder.UpdateItemEvent += UpdateMicrophoneWithDelay;
            SingleIntegerEventsHolder.UpdateItemEvent += UpdateStatueWithDelay;
            SimpleEventsHolder.StopStreaming += StopStreaming;
            SimpleEventsHolder.X10IncomeEvent += TenTimesIncome;
            SimpleEventsHolder.X2IncomeEvent += TwoTimesIncome;
            SimpleEventsHolder.X2TappedEvent += TwoTimesTap;
        }

        private void OnDisable()
        {
            SingleIntegerEventsHolder.UpdateItemEvent -= UpdateCameraWithDelay;
            SingleIntegerEventsHolder.UpdateItemEvent -= UpdateTripodWithDelay;
            SingleIntegerEventsHolder.UpdateItemEvent -= UpdateMicrophoneWithDelay;
            SingleIntegerEventsHolder.UpdateItemEvent -= UpdateStatueWithDelay;
            SimpleEventsHolder.StopStreaming -= StopStreaming;
            SimpleEventsHolder.X10IncomeEvent -= TenTimesIncome;
            SimpleEventsHolder.X2IncomeEvent -= TwoTimesIncome;
            SimpleEventsHolder.X2TappedEvent -= TwoTimesTap;
            DBVariablesHolder.ClosingTime.Value = DateTime.Now.ToString();
        }

        void Start()
        {
            ChangePriceText(DBVariablesHolder.MaxLevels.Value);
        }

        public void ContinueGameplay()
        {
            _upgradeStates = new UpgradeStateData[ItemsNames.Length];
            _x10IncomeDuration = DBVariablesHolder.X10Duration.Value;
            CheckBoostRemainingTime();
            McAnimator.SetTrigger("Default");
            LoadCameraFirst();
            LoadTripodFirst();
            LoadMicrophoneFirst();
            LoadStatueFirst();
            StartStreaming();
        }

        void CheckBoostRemainingTime()
        {
            if (!string.IsNullOrEmpty(DBVariablesHolder.X2Time.Value))
            {
                DateTime startTimeX2Income = DateTime.Parse(DBVariablesHolder.X2Time.Value);
                TimeSpan offlineTime = DateTime.Now - startTimeX2Income;

                float remainingX2Income = _x2IncomeDuration - (float)offlineTime.TotalSeconds;

                if (remainingX2Income > 0)
                {
                    _x2IncomeDuration = remainingX2Income;
                    TwoTimesIncome();
                } 
            }

            if(!string.IsNullOrEmpty(DBVariablesHolder.X2TapTime.Value))
            {
                DateTime startTimeX2Tap = DateTime.Parse(DBVariablesHolder.X2TapTime.Value);
                TimeSpan offlineTime = DateTime.Now - startTimeX2Tap;
                float remainingX2Tap = _x2TapDuration - (float)offlineTime.TotalSeconds;
                if (remainingX2Tap > 0)
                {
                    _x2TapDuration = remainingX2Tap;
                    TwoTimesTap();
                }
            }

        }

        void LoadCameraFirst()
        {
            int currentCamera = GetItemIndex(DBVariablesHolder.CameraLvl.Value);
            string key = $"Camera_{currentCamera}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnCameraLoaded;
            CameraImg.transform.position = CameraPositions[DBVariablesHolder.CurrentMap.Value];

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
            int currentTripod = GetItemIndex(DBVariablesHolder.TripodLvl.Value);
            string key = $"Tripod_{currentTripod}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnTripodLoaded;
            TripodImg.transform.position = TripodPositions[DBVariablesHolder.CurrentMap.Value];

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
            int currentMic = GetItemIndex(DBVariablesHolder.MicrophoneLvl.Value);
            string key = $"Microphone_{currentMic}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnMicrophoneLoaded;
            MicrophoneImg.transform.position = MicPositions[DBVariablesHolder.CurrentMap.Value];

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
            int currentStatue = GetItemIndex(DBVariablesHolder.StatueLvl.Value);
            string key = $"Statue_{currentStatue}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnStatueLoaded;
            StatueImg.transform.position = StatuePositions[DBVariablesHolder.CurrentMap.Value];

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
                SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(1);
            });
        }
        void UpdateCameraAnimation()
        {
            _upgradeStates[0].IsUpdating = false;
            JsonDB.Save($"{ItemsNames[0]}_UpgradeState", _upgradeStates[0]);

            int currentCamera = GetItemIndex(DBVariablesHolder.CameraLvl.Value);
            string key = $"Camera_{currentCamera}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnCameraLoaded;
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
                SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(1);
            });
        }
        void UpdateTripodAnimation()
        {
            _upgradeStates[1].IsUpdating = false;
            JsonDB.Save($"{ItemsNames[1]}_UpgradeState", _upgradeStates[1]);

            int currentTripod = GetItemIndex(DBVariablesHolder.TripodLvl.Value);
            string key = $"Tripod_{currentTripod}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnTripodLoaded;
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
                SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(1);
            });
        }
        void UpdateMicrophoneAnimation()
        {
            _upgradeStates[2].IsUpdating = false;
            JsonDB.Save($"{ItemsNames[2]}_UpgradeState", _upgradeStates[2]);

            int currentMic = GetItemIndex(DBVariablesHolder.MicrophoneLvl.Value);
            string key = $"Microphone_{currentMic}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnMicrophoneLoaded;
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
                SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(1);
            });
        }
        void UpdateStatueAnimation()
        {
            _upgradeStates[3].IsUpdating = false;
            JsonDB.Save($"{ItemsNames[3]}_UpgradeState", _upgradeStates[3]);

            int currentStatue = GetItemIndex(DBVariablesHolder.StatueLvl.Value);
            string key = $"Statue_{currentStatue}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnStatueLoaded;
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

        void StopStreaming()
        {
            _canStream = false;
            _canEarn = false;
            _canDonate = false;

            McTalk.StartTalking(false);
            McAnimator.Play("Default State");

            if (_streamRoutine != null)
                StopCoroutine(_streamRoutine);

            if (_earningRotine != null)
                StopCoroutine(_earningRotine);

            if(_donationRotine != null)
                StopCoroutine(_donationRotine);
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
            StartDonations();
            while (_canEarn)
            {
                Subscribers.Amount += 1;
                float income = GetIncomePerSecond();
                if (CashParticle._isTapped) 
                {
                    if(_tappedMultipler < _maxTapped)
                        _tappedMultipler += 0.1f;

                    IncomeTxt.color = Color.green;
                    DoubleIntegerEventHolder.TaskEvent?.Invoke(0, (income*_tappedMultipler));
                    DoubleIntegerEventHolder.TaskEvent?.Invoke(4, 1);
                    DoubleIntegerEventHolder.TaskEvent?.Invoke(5, 1);
                }
                else if (_tappedMultipler > 1)
                {
                    _tappedMultipler -= 0.1f;
                    IncomeTxt.color = Color.black;
                }
                if (_can10xIncome)
                {
                    income *= 10;
                }
                if(_can2xIncome)
                {
                    income *= 2;
                }
                income *= _tappedMultipler;
                IncomeTxt.text = $"{GameManager.Instance.FormatMoney(income)}/s";
                CashCurrency.Amount += income;
                DoubleIntegerEventHolder.TaskEvent?.Invoke(1, income);
                yield return new WaitForSecondsRealtime(_tappedSpeed);
            }
        }

        void StartDonations()
        {
            if (_donationRotine != null)
                StopCoroutine(_donationRotine);

            _donationTimer = _maxDonationDelay - (DBVariablesHolder.DonationLvl.Value * _donationCap);
            _canDonate = true;
            _donationRotine = StartCoroutine(DonationCoroutine());
        }

        IEnumerator DonationCoroutine() 
        {
            yield return new WaitForSeconds(_donationTimer);
            Camera mainCamera = Camera.main;
            RectTransform donationRect = CashDonation.GetComponent<RectTransform>();
            while (_canDonate)
            {
                if (!CashDonation.activeInHierarchy)
                {
                    Vector2 screenPos = mainCamera.WorldToScreenPoint(CameraImg.transform.position);

                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        CashDonation.transform.parent as RectTransform,
                        screenPos,
                        null,
                        out Vector2 localPos
                    );

                    donationRect.anchoredPosition = localPos;
                    CashDonation.SetActive(true);
                }
                yield return new WaitForSeconds(_donationTimer);
            }
        }

        float GetIncomePerSecond()
        {
            double hundreds = Subscribers.Amount / 100;
            float subscriberIncome = (float)(hundreds * 0.01f);

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

        int GetItemIndex(int lvl)
        {
            int range = lvl / GameManager.Instance.MapChangeCount;
            int spriteIndex = GameManager.Instance.SpriteChangeCount;
            int mapIndex = DBVariablesHolder.CurrentMap.Value;
            while (range != mapIndex)
            {
                lvl -= spriteIndex;
                range = lvl / GameManager.Instance.MapChangeCount;
            }
            return lvl / spriteIndex;
        }
        void TenTimesIncome()
        {
            if (_x10Routine != null)
                StopCoroutine(_x10Routine);

            _x10Routine = StartCoroutine(X10IncomeRoutine());
        }

        IEnumerator X10IncomeRoutine()
        {
            _can10xIncome = true;
            _canStream = false;
            if (_streamRoutine != null)
            {
                StopCoroutine(_streamRoutine);
                _streamRoutine = null;
            }
            BoosterParticle.SetActive(true);
            McAnimator.Play("Booster Start");
            yield return new WaitForSecondsRealtime(_x10IncomeDuration);
            BoosterParticle.SetActive(false);
            McAnimator.Play("Booster End");
            _can10xIncome = false;
            StartStreaming();
            _x10Routine = null;
        }

        void TwoTimesIncome()
        {
            if (_x2IncomeRoutine != null)
                StopCoroutine(_x2IncomeRoutine);

            _x2IncomeRoutine = StartCoroutine(X2IncomeRoutine());
        }

        IEnumerator X2IncomeRoutine()
        {
            _can2xIncome = true;
            DBVariablesHolder.X2Time.Value = DateTime.Now.ToString();
            yield return new WaitForSecondsRealtime(_x2IncomeDuration);

            _can2xIncome = false;
            _x2IncomeDuration = 3600;
            _x2IncomeRoutine = null;
        }

        void TwoTimesTap()
        {
            if (_x2TapRoutine != null)
                StopCoroutine(_x2TapRoutine);

            _x2TapRoutine = StartCoroutine(X2TapRoutine());
        }

        IEnumerator X2TapRoutine()
        {
            _maxTapped *= 2;
            DBVariablesHolder.X2TapTime.Value = DateTime.Now.ToString();
            yield return new WaitForSecondsRealtime(_x2TapDuration);

            _maxTapped /= 2;
            _x2TapDuration = 3600;
            _x2TapRoutine = null;
        }

        public void MultipleOfflineIncome()
        {
            
        }

        public void ClaimOfflineIncome()
        {

        }
    }
}