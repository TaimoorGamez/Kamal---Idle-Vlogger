using UnityEngine;
using Core.Events;
using Core.DB.Variables;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.GamePlay.WaterSort
{
    public class ColoringLevelInit : MonoBehaviour
    {
        [SerializeField] Transform ColoringHolder;
         
        string _coloringPath = "Level/Coloring/";
        AsyncOperationHandle _coloringHandle;
        bool _coloringReleaseInProgress = false;


        private void OnEnable()
        {
            SimpleEventsHolder.InitLvlEvent += InitColoring;
            SimpleEventsHolder.RestartLevelEvent += RegenrateColoring;
            SimpleEventsHolder.DestroyLevelEvent += DestroyColoring;
        }

        private async void OnDisable()
        {
            SimpleEventsHolder.InitLvlEvent -= InitColoring;
            SimpleEventsHolder.RestartLevelEvent -= RegenrateColoring;
            SimpleEventsHolder.DestroyLevelEvent -= DestroyColoring;
            await ReleaseColoringHandleSafelyAsync();
        }

        void InitColoring()
        {
            LoadColoringLvl(_coloringPath + (LevelsManager.I.TempLvlIndex == -1 ? DBVariablesHolder.LvlIndex.Value : LevelsManager.I.TempLvlIndex));
        }

        async void LoadColoringLvl(string path)
        {
            _coloringHandle = Addressables.LoadAssetAsync<GameObject>(path);
            await _coloringHandle.Task;

            if (_coloringHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"Failed to load Addressable prefab at: {path}");
                return;
            }

            GameObject lvlObj = Instantiate(_coloringHandle.Result as GameObject, ColoringHolder);

            await Task.Yield();
        }

        void RegenrateColoring()
        {
            DestroyColoring();
            InitColoring();
        }

        async void DestroyColoring()
        {
            await ReleaseColoringHandleSafelyAsync(); 
            if (ColoringHolder.childCount > 0)
            {
                Destroy(ColoringHolder.GetChild(0).gameObject);
            }

        }

        async Task ReleaseColoringHandleSafelyAsync()
        {
            if (_coloringReleaseInProgress)
                return;

            _coloringReleaseInProgress = true;

            await Task.Yield();

            if (_coloringHandle.IsValid())
            {
                Addressables.Release(_coloringHandle);
                _coloringHandle = default;
            }

            await Task.Yield();

            _coloringReleaseInProgress = false;
        }

    }
}
