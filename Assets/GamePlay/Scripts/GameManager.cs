using UnityEngine;
using UnityEngine.UI;
using Core.DB.Variables;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.GamePlay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] Image BgImage, GroundImg;

        int SpriteChangeCount = 20;
        int _currentMap, _currentGround;

        private void Start()
        {
            if(DBVariablesHolder.FFT.Value == 0)
            {
                DBVariablesHolder.FFT.Value = 1;
            }
            LoadBG();
            LoadGround();

        }

        void LoadBG()
        {
            _currentMap = DBVariablesHolder.CurrentMap.Value;
            string key = $"BG_{_currentMap}";

            Addressables.LoadAssetAsync<Sprite>(key).Completed += OnBgLoaded;
        }

        void OnBgLoaded(AsyncOperationHandle<Sprite> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                BgImage.sprite = handle.Result;
            }
            else
            {
                Debug.Log("Background load failed!");
            }
        }

        void LoadGround()
        {
            _currentGround = DBVariablesHolder.GroundLvl.Value/ SpriteChangeCount;
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
                Debug.Log("Background load failed!");
            }
        }
    }
}
