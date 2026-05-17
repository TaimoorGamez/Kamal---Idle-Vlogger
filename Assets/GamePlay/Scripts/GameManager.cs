using System;
using UnityEngine;
using Core.Events;
using DG.Tweening;
using Core.Plugins.Ads;
using Core.DB.Variables;
using Core.Plugins.Firebase;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.GamePlay
{
    public class GameManager : MonoBehaviour
    {
        public int SpriteChangeCount = 20, MapChangeCount = 100, MaxStoryIndex = 2, LastMap = 1;
        public float UpdateDelay = 10f;

        [SerializeField] GameObject GameplayEnvironment, GameplayUI, StorylineUI;
        [SerializeField] SpriteRenderer BgImg, GroundImg, HouseImg, VehicleImg, BackyardImg;
        [SerializeField] StorylineHandler CurrentStorylineHandler;
        [SerializeField] GameplayHandler CurrentGameplayHandler;
        [SerializeField] Transform MC;
        [SerializeField] Vector2[] HousePositions, VehiclePositions, BackyardPositions;
        [SerializeField] string[] ItemsNames;

        int _houseIndex = 6, _vehicleIndex = 7, _backyardIndex = 8, _groundIndex = 9;
        float _updatingAnimationDuration = 0.45f, _visualDuration = 0.5f, _initDelay = 2f;
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
            LoadEnvironment();
        }

        void LoadEnvironment()
        {
            LoadBG();
            LoadGroundFirst();
            int mapIndex = DBVariablesHolder.CurrentMap.Value;
            HouseImg.transform.position = HousePositions[mapIndex];
            LoadHouseFirst();
            VehicleImg.transform.position = VehiclePositions[mapIndex];
            LoadVehicleFirst();
            BackyardImg.transform.position = BackyardPositions[mapIndex];
            LoadBackyardFirst();
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
            int groundIndex = GetItemIndex(DBVariablesHolder.GroundLvl.Value);
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
            int houseIndex = GetItemIndex(DBVariablesHolder.HouseLvl.Value);
            string key = $"House_{houseIndex}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnHouseLoaded;

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
            int vehicleIndex = GetItemIndex(DBVariablesHolder.VehicleLvl.Value);
            string key = $"Vehicle_{vehicleIndex}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnVehicleLoaded;

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
            int backyardIndex = GetItemIndex(DBVariablesHolder.BackyardLvl.Value);
            string key = $"Backyard_{backyardIndex}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnBackyardLoaded;

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

            float delay =  UpdateDelay;
            float currentTime = delay;
            DOTween.To(() => currentTime, x => currentTime = x, 0, delay).OnComplete(() =>
            {
                UpdateHouseAnimation(); 
                SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(1);
            });
        }
        void UpdateHouseAnimation()
        {
            _upgradeStates[0].IsUpdating = false;
            JsonDB.Save($"{ItemsNames[0]}_UpgradeState", _upgradeStates[0]);

            int houseIndex = GetItemIndex(DBVariablesHolder.HouseLvl.Value);
            string key = $"House_{houseIndex}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnHouseLoaded;
        }

        void UpdateVehicleWithDelay(int eventIndex)
        {
            if (eventIndex != _vehicleIndex)
                return;

            float delay = UpdateDelay;
            float currentTime = delay;
            DOTween.To(() => currentTime, x => currentTime = x, 0, delay).OnComplete(() =>
            {
                UpdateVehicleAnimation();
                SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(1);
            });
        }
        void UpdateVehicleAnimation()
        {
            _upgradeStates[1].IsUpdating = false;
            JsonDB.Save($"{ItemsNames[1]}_UpgradeState", _upgradeStates[1]);

            int vehicleIndex = GetItemIndex(DBVariablesHolder.VehicleLvl.Value);
            string key = $"Vehicle_{vehicleIndex}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnVehicleLoaded;
        }

        void UpdateBackyardWithDelay(int eventIndex)
        {
            if (eventIndex != _backyardIndex)
                return;

            float delay = UpdateDelay;
            float currentTime = delay;
            DOTween.To(() => currentTime, x => currentTime = x, 0, delay).OnComplete(() =>
            {
                UpdateBackyardAnimation();
                SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(1);
            });
        }
        void UpdateBackyardAnimation()
        {
            _upgradeStates[2].IsUpdating = false;
            JsonDB.Save($"{ItemsNames[2]}_UpgradeState", _upgradeStates[2]);

            int backyardIndex = GetItemIndex(DBVariablesHolder.BackyardLvl.Value);
            string key = $"Backyard_{backyardIndex}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnBackyardLoaded;
        }

        void UpdateGroundWithDelay(int eventIndex)
        {
            if (eventIndex != _groundIndex)
                return;

            float delay = UpdateDelay;
            float currentTime = delay;
            DOTween.To(() => currentTime, x => currentTime = x, 0, delay).OnComplete(() =>
            {
                UpdateGroundAnimation();
                SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(1);
            });
        }
        void UpdateGroundAnimation()
        {
            _upgradeStates[3].IsUpdating = false;
            JsonDB.Save($"{ItemsNames[3]}_UpgradeState", _upgradeStates[3]);

            int groundIndex = GetItemIndex(DBVariablesHolder.GroundLvl.Value);
            string key = $"Ground_{groundIndex}";
            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnGroundLoaded;
        }

        void CheckRemainingTime(int index, DBInt lvlData)
        {
            TimeSpan timePassed = DateTime.Now - DateTime.Parse(_upgradeStates[index].UpdateStartTime);
            float updateDelay = UpdateDelay;
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
            if (DBVariablesHolder.IsGameplay.Value == 0)
            {
                SwitchToStoryline();
            }
            else
            {
                SwitchToGameplay();
            }
        }

        void SwitchToStoryline()
        {
            SimpleEventsHolder.StopStreaming?.Invoke();
            GameplayEnvironment.SetActive(false);
            GameplayUI.SetActive(false);
            StorylineUI.SetActive(true);
            int storyIndex = DBVariablesHolder.StoryProgress.Value;
            CurrentStorylineHandler.CountinueStory(storyIndex);
            if (DBVariablesHolder.CurrentMap.Value < LastMap)
                LoadEnvironment();
        }

        public void SwitchToGameplay()
        {
            GameplayEnvironment.SetActive(true);
            GameplayUI.SetActive(true);
            StorylineUI.SetActive(false);
            CurrentGameplayHandler.ContinueGameplay();
        }
        int GetItemIndex(int lvl)
        {
            int range = lvl / MapChangeCount;
            int spriteIndex = SpriteChangeCount;
            int mapIndex = DBVariablesHolder.CurrentMap.Value;
            while (range != mapIndex)
            {
                lvl -= spriteIndex;
                range = lvl / MapChangeCount;
            }
            return lvl / spriteIndex;
        }

        public string FormatMoney(double value)
        {
            if (value < 1000)
                return value.ToString("0.##");

            string[] suffixes = { "K", "M", "B", "T", "Qa" };

            int index = 0;

            // Go through predefined suffixes
            while (value >= 1000 && index < suffixes.Length)
            {
                value /= 1000;
                index++;
            }

            // If still large switch to AA system
            if (index >= suffixes.Length)
            {
                int extraIndex = 0;

                while (value >= 1000)
                {
                    value /= 1000;
                    extraIndex++;
                }

                return value.ToString("0.#") + GetAlphabetSuffix(extraIndex);
            }

            return value.ToString("0.#") + suffixes[index - 1];
        }

        string GetAlphabetSuffix(int index)
        {
            string result = "";

            while (index >= 0)
            {
                result = (char)('A' + (index % 26)) + result;
                index = index / 26 - 1;
            }

            return result;
        }
    
        void InitAllPlugins()
        {
            bool allInit = false;
            if (FirebaseHandler.I.IsInitialize)
            {
                Debug.Log("Firebase Initialized");
                if (FirebaseHandler.I.IsRemoteFetched)
                {
                    Debug.Log("Firebase Remote Config Fetched");
                    if (!AdsManager.I.IsInitialized)
                    {
                        Debug.Log("Initializing AdsManager...");
                        AdsManager.I.InitPlugin();
                    }
                    else
                    {
                        allInit = true;
                    }
                }
                else
                {
                    FirebaseHandler.I.FetchRemoteConfig();
                }
            }
            else
            {
                FirebaseHandler.I.InitPlugin();
            }

            if (!allInit)
            {
                Invoke(nameof(InitAllPlugins), _initDelay);
            }
        }
    }
}
