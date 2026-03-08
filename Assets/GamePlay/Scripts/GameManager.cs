using DG.Tweening;
using UnityEngine;
using Core.DB.Variables;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.GamePlay
{
    public enum GameMod
    {
        Storyline = 0,
        Gameplay = 1
    }


    public class GameManager : MonoBehaviour
    {
        [SerializeField] SpriteRenderer BgImg, GroundImg, HouseImg, BackyardImg, VehicleImg, StatueImg;
        [SerializeField] Vector2[] HousePositions, BackyardPositions, VehiclePositions, StatuePositions;
        [SerializeField] Vector2 GameplayPositionMC, StorylinePositionMC;
        [SerializeField] StorylineHandler CurrentStorylineHandler;
        [SerializeField] GameplayHandler CurrentGameplayHandler;

        int SpriteChangeCount = 20;
        int _currentBG, _currentGround;
        GameMod _currentGameMod;

        private void Start()
        {
            if (DBVariablesHolder.FFT.Value == 0)
            {
                DBVariablesHolder.FFT.Value = 1;
                _currentGameMod = GameMod.Storyline;
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
    }
}
