using UnityEngine;
using Core.DB.Variables;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.GamePlay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] GameObject GameplayEnvironment, GameplayUI, StorylineUI;
        [SerializeField] SpriteRenderer BgImg, GroundImg;
        [SerializeField] StorylineHandler CurrentStorylineHandler;
        [SerializeField] GameplayHandler CurrentGameplayHandler;
        [SerializeField] Transform MC;

        int SpriteChangeCount = 20, _currentBG, _currentGround;

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
