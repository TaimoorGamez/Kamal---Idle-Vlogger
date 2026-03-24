using DG.Tweening;
using UnityEngine;
using Core.DB.Variables;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.GamePlay
{
    public class GameManager : MonoBehaviour
    {
        public int SpriteChangeCount = 20;
        public float UpdatingAnimationDuration = 0.25f;

        [SerializeField] GameObject GameplayEnvironment, GameplayUI, StorylineUI;
        [SerializeField] SpriteRenderer BgImg, GroundImg, HouseImg, VehicleImg, BackyardImg;
        [SerializeField] StorylineHandler CurrentStorylineHandler;
        [SerializeField] GameplayHandler CurrentGameplayHandler;
        [SerializeField] Transform MC;
        [SerializeField] Vector2[] HousePositions, VehiclePositions, BackyardPositions;


        int _currentBG, _currentGround, _currentHouse, _currentVehicle, _currentBackyard;

        public static GameManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); 
            }
            else
            {
                Destroy(gameObject);
            }
        }

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
            LoadBackyard();
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
            _currentGround = (DBVariablesHolder.GroundLvl.Value / SpriteChangeCount)+1;
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
            _currentHouse = (DBVariablesHolder.HouseLvl.Value / SpriteChangeCount) + 1;
            string key = $"House_{_currentHouse}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnHouseLoaded;
            HouseImg.transform.position = HousePositions[_currentHouse - 1];
        }
        void OnHouseLoaded(AsyncOperationHandle<Sprite> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                HouseImg.sprite = handle.Result;
                Material mat = HouseImg.material;
                mat.SetFloat("_Reveal", 0f);
                HouseImg.transform.DOScale(Vector3.one, UpdatingAnimationDuration).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    DOTween.To(
                    () => mat.GetFloat("_Reveal"),
                    x => mat.SetFloat("_Reveal", x),
                    1f, UpdatingAnimationDuration);
                });
            }
            else
            {
                Debug.Log("House load failed!");
            }
        }

        void LoadVehicle()
        {
            _currentVehicle = (DBVariablesHolder.VehicleLvl.Value / SpriteChangeCount) + 1;
            string key = $"Vehicle_{_currentVehicle}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnVehicleLoaded;
            VehicleImg.transform.position = VehiclePositions[_currentVehicle - 1];
        }
        void OnVehicleLoaded(AsyncOperationHandle<Sprite> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                VehicleImg.sprite = handle.Result;
                Material mat = VehicleImg.material;
                mat.SetFloat("_Reveal", 0f);
                VehicleImg.transform.DOScale(Vector3.one, UpdatingAnimationDuration).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    DOTween.To(
                    () => mat.GetFloat("_Reveal"),
                    x => mat.SetFloat("_Reveal", x),
                    1f, UpdatingAnimationDuration);
                });
            }
            else
            {
                Debug.Log("Vehicle load failed!");
            }
        }

        void LoadBackyard()
        {
            _currentBackyard = (DBVariablesHolder.BackyardLvl.Value / SpriteChangeCount) + 1;
            string key = $"Backyard_{_currentBackyard}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnBackyardLoaded;
            BackyardImg.transform.position = BackyardPositions[_currentBackyard - 1];
        }
        void OnBackyardLoaded(AsyncOperationHandle<Sprite> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                BackyardImg.sprite = handle.Result;
                Material mat = BackyardImg.material;
                mat.SetFloat("_Reveal", 0f);
                BackyardImg.transform.DOScale(Vector3.one, UpdatingAnimationDuration).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    DOTween.To(
                    () => mat.GetFloat("_Reveal"),
                    x => mat.SetFloat("_Reveal", x),
                    1f, UpdatingAnimationDuration);
                });
            }
            else
            {
                Debug.Log("Backyard load failed!");
            }
        }


        public void StartGame()
        {
            int storyIndex = DBVariablesHolder.StoryProgress.Value;
            if (storyIndex < 1 || ConditionFullfillForStoryProgress(storyIndex))
            {
                SwitchToStoryline(storyIndex);
            }
            else
            {
                SwitchToGameplay();
            }
        }

        bool ConditionFullfillForStoryProgress(int index)
        {
            return false;
        }

        public void SwitchToStoryline(int storyIndex)
        {
            GameplayEnvironment.SetActive(false);
            GameplayUI.SetActive(false);
            StorylineUI.SetActive(true);
            CurrentStorylineHandler.CountinueStory(storyIndex);

        }

        public void SwitchToGameplay()
        {
            GameplayEnvironment.SetActive(true);
            GameplayUI.SetActive(true);
            StorylineUI.SetActive(false);
            CurrentGameplayHandler.CountinueGameplay();
        }
    }
}
