using TMPro;
using DG.Tweening;
using UnityEngine;
using Core.Economy;
using Core.DB.Variables;
using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.GamePlay
{
    public class GameplayHandler : MonoBehaviour
    {
        [SerializeField] McTalking McTalk;
        [SerializeField] Animator McAnimator;
        [SerializeField] SpriteRenderer HouseImg, BackyardImg, VehicleImg, StatueImg, CameraImg, TripodImg;
        [SerializeField] TextMeshProUGUI IncomeTxt;
        [SerializeField] Vector2[] HousePositions, BackyardPositions, VehiclePositions, StatuePositions, CameraPositions, TripodPositions;
        [SerializeField] string[] BaseStreamAnimation;

        int SpriteChangeCount = 20, _currentHouse, _currentBackyard, _currentVehicle, _currentStatue, _currentCamera, _currentTripod;
        float _scaleDuration = 0.5f, _revealDuration = 0.25f, _basicIncome = 0.01f;
        bool _canStream = false, _canEarn = false;
        string[] _mainStreamAnimations;
        Coroutine _streamRoutine, _earningRotine;

        public void CountinueGameplay()
        {
            McAnimator.SetTrigger("Default");
            LoadHouse();
            LoadBackyard();
            LoadVehicle();
            LoadStatue();
            LoadCamera();
            LoadTripod();
            StartStreaming();
        }


        void LoadHouse()
        {
            _currentHouse = DBVariablesHolder.HouseLvl.Value / SpriteChangeCount;
            string key = $"House_{_currentHouse}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnHouseLoaded;
            HouseImg.transform.position = HousePositions[_currentHouse];
        }
        void OnHouseLoaded(AsyncOperationHandle<Sprite> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                HouseImg.sprite = handle.Result;
                Material mat = HouseImg.material;
                mat.SetFloat("_Reveal", 0f);
                HouseImg.transform.DOScale(Vector3.one, _scaleDuration).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    DOTween.To(
                    () => mat.GetFloat("_Reveal"),
                    x => mat.SetFloat("_Reveal", x),
                    1f, _revealDuration);
                });
            }
            else
            {
                Debug.Log("House load failed!");
            }
        }

        void LoadBackyard()
        {
            _currentBackyard = DBVariablesHolder.BackyardLvl.Value / SpriteChangeCount;
            string key = $"Backyard_{_currentBackyard}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnBackyardLoaded;
            BackyardImg.transform.position = BackyardPositions[_currentBackyard];
        }
        void OnBackyardLoaded(AsyncOperationHandle<Sprite> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                BackyardImg.sprite = handle.Result;
                Material mat = BackyardImg.material;
                mat.SetFloat("_Reveal", 0f);
                BackyardImg.transform.DOScale(Vector3.one, _scaleDuration).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    DOTween.To(
                    () => mat.GetFloat("_Reveal"),
                    x => mat.SetFloat("_Reveal", x),
                    1f, _revealDuration);
                });
            }
            else
            {
                Debug.Log("Backyard load failed!");
            }
        }

        void LoadVehicle()
        {
            _currentVehicle = DBVariablesHolder.VehicleLvl.Value / SpriteChangeCount;
            string key = $"Vehicle_{_currentVehicle}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnVehicleLoaded;
            VehicleImg.transform.position = VehiclePositions[_currentVehicle];
        }
        void OnVehicleLoaded(AsyncOperationHandle<Sprite> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                VehicleImg.sprite = handle.Result;
                Material mat = VehicleImg.material;
                mat.SetFloat("_Reveal", 0f);
                VehicleImg.transform.DOScale(Vector3.one, _scaleDuration).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    DOTween.To(
                    () => mat.GetFloat("_Reveal"),
                    x => mat.SetFloat("_Reveal", x),
                    1f, _revealDuration);
                });
            }
            else
            {
                Debug.Log("Vehicle load failed!");
            }
        }

        void LoadStatue()
        {
            _currentStatue = DBVariablesHolder.StatueLvl.Value / SpriteChangeCount;
            string key = $"Statue_{_currentStatue}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnStatueLoaded;
            StatueImg.transform.position = StatuePositions[_currentStatue];
        }
        void OnStatueLoaded(AsyncOperationHandle<Sprite> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                StatueImg.sprite = handle.Result;
                Material mat = StatueImg.material;
                mat.SetFloat("_Reveal", 0f);
                StatueImg.transform.DOScale(Vector3.one, _scaleDuration).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    DOTween.To(
                    () => mat.GetFloat("_Reveal"),
                    x => mat.SetFloat("_Reveal", x),
                    1f, _revealDuration);
                });
            }
            else
            {
                Debug.Log("Statue load failed!");
            }
        }

        void LoadCamera()
        {
            _currentCamera = DBVariablesHolder.CameraLvl.Value / SpriteChangeCount;
            string key = $"Camera_{_currentCamera}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnCameraLoaded;
            CameraImg.transform.position = CameraPositions[_currentCamera];
        }
        void OnCameraLoaded(AsyncOperationHandle<Sprite> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                CameraImg.sprite = handle.Result;
                Material mat = CameraImg.material;
                mat.SetFloat("_Reveal", 0f);
                CameraImg.transform.DOScale(Vector3.one, _scaleDuration).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    DOTween.To(
                    () => mat.GetFloat("_Reveal"),
                    x => mat.SetFloat("_Reveal", x),
                    1f, _revealDuration);
                });
            }
            else
            {
                Debug.Log("Statue load failed!");
            }
        }

        void LoadTripod()
        {
            _currentTripod = DBVariablesHolder.TripodLvl.Value / SpriteChangeCount;
            string key = $"Tripod_{_currentTripod}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnTripodLoaded;
            TripodImg.transform.position = TripodPositions[_currentTripod];
        }
        void OnTripodLoaded(AsyncOperationHandle<Sprite> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                TripodImg.sprite = handle.Result;
                Material mat = TripodImg.material;
                mat.SetFloat("_Reveal", 0f);
                TripodImg.transform.DOScale(Vector3.one, _scaleDuration).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    DOTween.To(
                    () => mat.GetFloat("_Reveal"),
                    x => mat.SetFloat("_Reveal", x),
                    1f, _revealDuration);
                });
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
                IncomeTxt.text = $"{income:F2}/s";
                CashCurrency.Amount += income;
                yield return new WaitForSecondsRealtime(1f);
            }
        }

        float GetIncomePerSecond()
        {
            int hundreds = Subscribers.Amount / 100;
            float subscriberIncome = hundreds * 0.01f;

            return _basicIncome + subscriberIncome;
        }

    }
}
