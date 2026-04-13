using TMPro;
using DG.Tweening;
using UnityEngine;
using Core.Events;
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
        [SerializeField] Vector2[] StatuePositions, WatchPositions, CameraPositions, MicPositions, TripodPositions;
        [SerializeField] string[] BaseStreamAnimation;

        int _cameraIndex = 3, _tripodIndex = 4, _micIndex = 5, _statueIndex = 10;
        float _tappedMultipler = 1, _maxTapped = 1.8f, _perSecond = 0.25f, _maxLvlToggleAnchor = 20, _maxLvlAnimationDuration = 0.25f,
              _updatingAnimationDuration = 0.45f, _visualDuration = 5;
        bool _canStream = false, _canEarn = false;
        string[] _mainStreamAnimations;
        Coroutine _streamRoutine, _earningRotine;

        void OnEnable()
        {
        }

        private void OnDisable()
        {
            
        }

        void Start()
        {
            ChangePriceText(DBVariablesHolder.MaxLevels.Value);
        }

        public void CountinueGameplay()
        {
            McAnimator.SetTrigger("Default");
            LoadCameraFirst();
            LoadTripod();
            LoadMicrophone();
            LoadStatue();
            StartStreaming();
        }

        void LoadCameraFirst()
        {
            int currentCamera = (DBVariablesHolder.CameraLvl.Value / GameManager.Instance.SpriteChangeCount);
            string key = $"Camera_{currentCamera}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnCameraLoaded;
            CameraImg.transform.position = CameraPositions[currentCamera];
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

        void LoadTripod()
        {
            int currentTripod = (DBVariablesHolder.TripodLvl.Value / GameManager.Instance.SpriteChangeCount);
            string key = $"Tripod_{currentTripod}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnTripodLoaded;
            TripodImg.transform.position = TripodPositions[currentTripod];
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

        void LoadMicrophone()
        {
            int currentMic = (DBVariablesHolder.MicrophoneLvl.Value / GameManager.Instance.SpriteChangeCount);
            string key = $"Microphone_{currentMic}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnMicrophoneLoaded;
            MicrophoneImg.transform.position = MicPositions[currentMic];
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

        void LoadStatue()
        {
            int currentStatue = (DBVariablesHolder.StatueLvl.Value / GameManager.Instance.SpriteChangeCount);
            string key = $"Statue_{currentStatue}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnStatueLoaded;
            StatueImg.transform.position = StatuePositions[currentStatue];
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
                int i = Random.Range(0, _mainStreamAnimations.Length);
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