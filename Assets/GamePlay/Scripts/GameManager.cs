using DG.Tweening;
using UnityEngine;
using Core.DB.Variables;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.GamePlay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] SpriteRenderer BgImg, GroundImg, HouseImg, VehicleImg;
        [SerializeField] Vector2[] HousePositions, VehiclePositions;

        int SpriteChangeCount = 20;
        int _currentBG, _currentGround, _currentHouse, _currentVehicle;
        float _scaleDuration = 0.5f, _revealDuration = 0.25f;

        private void Start()
        {
            if (DBVariablesHolder.FFT.Value == 0)
            {
                DBVariablesHolder.FFT.Value = 1;
            }
            LoadBG();
            LoadGround();
            LoadHouse();
            LoadVehicle();
        }

        void LoadBG()
        {
            _currentBG = DBVariablesHolder.CurrentMap.Value;
            string key = $"BG_{_currentBG}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnBgLoaded;
        }
        void OnBgLoaded(AsyncOperationHandle<Sprite> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                BgImg.sprite = handle.Result;
            }
            else
            {
                Debug.Log("Background load failed!");
            }
        }

        void LoadGround()
        {
            _currentGround = DBVariablesHolder.GroundLvl.Value / SpriteChangeCount;
            string key = $"Ground_{_currentGround}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnGroundLoaded;
        }
        void OnGroundLoaded(AsyncOperationHandle<Sprite> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GroundImg.sprite = handle.Result;
            }
            else
            {
                Debug.Log("Ground load failed!");
            }
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
                HouseImg.transform.DOScale(Vector3.one, _scaleDuration).SetEase(Ease.OutBack).OnComplete(()=> {
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
                VehicleImg.transform.DOScale(Vector3.one, _scaleDuration).SetEase(Ease.OutBack).OnComplete(() => {
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


    }
}
