using System;
using Core.Events;
using DG.Tweening;
using UnityEngine;
using Core.DB.Variables;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.GamePlay
{
    public class GameManager : MonoBehaviour
    {
        public int SpriteChangeCount = 20, MapChangeCount = 100;
        public float UpdateDelay = 10;

        [SerializeField] GameObject GameplayEnvironment, GameplayUI, StorylineUI;
        [SerializeField] SpriteRenderer BgImg, GroundImg, HouseImg, VehicleImg, BackyardImg;
        [SerializeField] StorylineHandler CurrentStorylineHandler;
        [SerializeField] GameplayHandler CurrentGameplayHandler;
        [SerializeField] Transform MC;
        [SerializeField] Vector2[] HousePositions, VehiclePositions, BackyardPositions;
        [SerializeField] string[] ItemsNames;

        int _houseIndex = 6, _vehicleIndex = 7, _backyardIndex = 8, _groundIndex = 9;
        float _updatingAnimationDuration = 0.45f, _visualDuration = 0.5f;
        UpgradeStateData[] _upgradeStates;

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

        void OnEnable()
        {
            SingleIntegerEventsHolder.UpdateItemEvent += UpdateHouseWithDelay;
            SingleIntegerEventsHolder.UpdateItemEvent += UpdateVehicleWithDelay;
            SingleIntegerEventsHolder.UpdateItemEvent += UpdateBackyardWithDelay;
            SingleIntegerEventsHolder.UpdateItemEvent += UpdateGroundWithDelay;
        }

        void OnDisable()
        {
            SingleIntegerEventsHolder.UpdateItemEvent -= UpdateHouseWithDelay;
            SingleIntegerEventsHolder.UpdateItemEvent -= UpdateVehicleWithDelay;
            SingleIntegerEventsHolder.UpdateItemEvent -= UpdateBackyardWithDelay;
            SingleIntegerEventsHolder.UpdateItemEvent -= UpdateGroundWithDelay;
        }

        private void Start()
        {
            if (DBVariablesHolder.FFT.Value == 0)
            {
                DBVariablesHolder.FFT.Value = 1;
            }
            _upgradeStates = new UpgradeStateData[ItemsNames.Length];
            LoadBG();
            LoadHouseFirst();
            LoadVehicleFirst();
            LoadBackyardFirst();
            LoadGroundFirst();
        }

        void LoadBG()
        {
            int bgIndex = DBVariablesHolder.CurrentMap.Value;
            string key = $"BG_{bgIndex}";
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

        void LoadGroundFirst()
        {
            int groundIndex = (DBVariablesHolder.GroundLvl.Value / SpriteChangeCount);
            string key = $"Ground_{groundIndex}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnGroundLoaded;

            if (PlayerPrefs.HasKey($"{ItemsNames[3]}_UpgradeState"))
            {
                _upgradeStates[3] = JsonDB.Load<UpgradeStateData>($"{ItemsNames[3]}_UpgradeState");
                if (_upgradeStates[3].IsUpdating)
                    CheckRemainingTime(3, DBVariablesHolder.CameraLvl);
            }
            else
            {
                _upgradeStates[3] = new UpgradeStateData { IsUpdating = false, UpdateStartTime = "" };
            }
        }
        void OnGroundLoaded(AsyncOperationHandle<Sprite> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GroundImg.sprite = handle.Result;
                Material mat = GroundImg.material;
                DOTween.To(() => mat.GetFloat("_Reveal"), x => mat.SetFloat("_Reveal", x), 1f, _visualDuration).From(0f).SetEase(Ease.Linear);
            }
            else
            {
                Debug.Log("Ground load failed!");
            }
        }

        void LoadHouseFirst()
        {
            int houseIndex = (DBVariablesHolder.HouseLvl.Value / SpriteChangeCount);
            string key = $"House_{houseIndex}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnHouseLoaded;
            HouseImg.transform.position = HousePositions[houseIndex];

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
        void OnHouseLoaded(AsyncOperationHandle<Sprite> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                HouseImg.sprite = handle.Result;
                Material mat = HouseImg.material;
                DOTween.To(() => mat.GetFloat("_Reveal"), x => mat.SetFloat("_Reveal", x), 1f, _visualDuration).From(0f).SetEase(Ease.Linear);
                HouseImg.transform.DOScale(Vector3.one, _updatingAnimationDuration).From(Vector3.zero).SetEase(Ease.OutBack);
            }
            else
            {
                Debug.Log("House load failed!");
            }
        }

        void LoadVehicleFirst()
        {
            int vehicleIndex = (DBVariablesHolder.VehicleLvl.Value / SpriteChangeCount);
            string key = $"Vehicle_{vehicleIndex}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnVehicleLoaded;
            VehicleImg.transform.position = VehiclePositions[vehicleIndex];

            if (PlayerPrefs.HasKey($"{ItemsNames[1]}_UpgradeState"))
            {
                _upgradeStates[1] = JsonDB.Load<UpgradeStateData>($"{ItemsNames[1]}_UpgradeState");
                if (_upgradeStates[1].IsUpdating)
                    CheckRemainingTime(1, DBVariablesHolder.CameraLvl);
            }
            else
            {
                _upgradeStates[1] = new UpgradeStateData { IsUpdating = false, UpdateStartTime = "" };
            }
        }
        void OnVehicleLoaded(AsyncOperationHandle<Sprite> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                VehicleImg.sprite = handle.Result;
                Material mat = VehicleImg.material;
                DOTween.To(() => mat.GetFloat("_Reveal"), x => mat.SetFloat("_Reveal", x), 1f, _visualDuration).From(0f).SetEase(Ease.Linear);
                VehicleImg.transform.DOScale(Vector3.one, _updatingAnimationDuration).From(Vector3.zero).SetEase(Ease.OutBack);
            }
            else
            {
                Debug.Log("Vehicle load failed!");
            }
        }

        void LoadBackyardFirst()
        {
            int backyardIndex = (DBVariablesHolder.BackyardLvl.Value / SpriteChangeCount);
            string key = $"Backyard_{backyardIndex}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnBackyardLoaded;
            BackyardImg.transform.position = BackyardPositions[backyardIndex];

            if (PlayerPrefs.HasKey($"{ItemsNames[2]}_UpgradeState"))
            {
                _upgradeStates[2] = JsonDB.Load<UpgradeStateData>($"{ItemsNames[2]}_UpgradeState");
                if (_upgradeStates[2].IsUpdating)
                    CheckRemainingTime(2, DBVariablesHolder.CameraLvl);
            }
            else
            {
                _upgradeStates[2] = new UpgradeStateData { IsUpdating = false, UpdateStartTime = "" };
            }
        }
        void OnBackyardLoaded(AsyncOperationHandle<Sprite> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                BackyardImg.sprite = handle.Result;
                Material mat = BackyardImg.material;
                DOTween.To(() => mat.GetFloat("_Reveal"), x => mat.SetFloat("_Reveal", x), 1f, _visualDuration).From(0f).SetEase(Ease.Linear);
                BackyardImg.transform.DOScale(Vector3.one, _updatingAnimationDuration).From(Vector3.zero).SetEase(Ease.OutBack);
            }
            else
            {
                Debug.Log("Backyard load failed!");
            }
        }

        void UpdateHouseWithDelay(int eventIndex)
        {
            if (eventIndex != _houseIndex)
                return;

            float delay = GameManager.Instance.UpdateDelay;
            float currentTime = delay;
            DOTween.To(() => currentTime, x => currentTime = x, 0, delay).OnComplete(() =>
            {
                UpdateHouseAnimation();
            });
        }
        void UpdateHouseAnimation()
        {
            _upgradeStates[0].IsUpdating = false;
            JsonDB.Save($"{ItemsNames[0]}_UpgradeState", _upgradeStates[0]);

            int houseIndex = (DBVariablesHolder.HouseLvl.Value / SpriteChangeCount);
            string key = $"House_{houseIndex}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnHouseLoaded;
            HouseImg.transform.position = HousePositions[houseIndex];
        }

        void UpdateVehicleWithDelay(int eventIndex)
        {
            if (eventIndex != _vehicleIndex)
                return;

            float delay = GameManager.Instance.UpdateDelay;
            float currentTime = delay;
            DOTween.To(() => currentTime, x => currentTime = x, 0, delay).OnComplete(() =>
            {
                UpdateVehicleAnimation();
            });
        }
        void UpdateVehicleAnimation()
        {
            _upgradeStates[1].IsUpdating = false;
            JsonDB.Save($"{ItemsNames[1]}_UpgradeState", _upgradeStates[1]);

            int vehicleIndex = (DBVariablesHolder.VehicleLvl.Value / SpriteChangeCount);
            string key = $"Vehicle_{vehicleIndex}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnVehicleLoaded;
            VehicleImg.transform.position = VehiclePositions[vehicleIndex];
        }

        void UpdateBackyardWithDelay(int eventIndex)
        {
            if (eventIndex != _backyardIndex)
                return;

            float delay = GameManager.Instance.UpdateDelay;
            float currentTime = delay;
            DOTween.To(() => currentTime, x => currentTime = x, 0, delay).OnComplete(() =>
            {
                UpdateBackyardAnimation();
            });
        }
        void UpdateBackyardAnimation()
        {
            _upgradeStates[2].IsUpdating = false;
            JsonDB.Save($"{ItemsNames[2]}_UpgradeState", _upgradeStates[2]);

            int backyardIndex = (DBVariablesHolder.BackyardLvl.Value / SpriteChangeCount);
            string key = $"Backyard_{backyardIndex}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnBackyardLoaded;
            BackyardImg.transform.position = BackyardPositions[backyardIndex];
        }

        void UpdateGroundWithDelay(int eventIndex)
        {
            if (eventIndex != _groundIndex)
                return;

            float delay = GameManager.Instance.UpdateDelay;
            float currentTime = delay;
            DOTween.To(() => currentTime, x => currentTime = x, 0, delay).OnComplete(() =>
            {
                UpdateGroundAnimation();
            });
        }
        void UpdateGroundAnimation()
        {
            _upgradeStates[3].IsUpdating = false;
            JsonDB.Save($"{ItemsNames[3]}_UpgradeState", _upgradeStates[3]);

            int groundIndex = (DBVariablesHolder.GroundLvl.Value / SpriteChangeCount);
            string key = $"Ground_{groundIndex}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnGroundLoaded;
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
                            UpdateHouseAnimation();
                            break;

                        case 1:
                            UpdateVehicleAnimation();
                            break;

                        case 2:
                            UpdateBackyardAnimation();
                            break;

                        case 3:
                            UpdateGroundAnimation();
                            break;
                    }
                });
            }
            else
            {
                _upgradeStates[index].IsUpdating = false;
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
            CurrentGameplayHandler.ContinueGameplay();
        }
    }
}
