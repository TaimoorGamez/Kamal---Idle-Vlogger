using Core.Events;
using DG.Tweening;
using UnityEngine;
using Core.DB.Variables;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.GamePlay.Coloring
{
    public class ColoringManager : MonoBehaviour
    {
        [SerializeField] RectTransform ColoringImage;
        [SerializeField] Transform RefferanceBar;

        float _preparationTime = 1;
        string _refferancePath = "Level/Reference/";
        AsyncOperationHandle _referenceHandle;
        bool _referenceReleaseInProgress = false;

        private void OnEnable()
        {
            SimpleEventsHolder.StartColoringEvent += StartColoring;
        }

        private async void OnDisable()
        {
            SimpleEventsHolder.StartColoringEvent -= StartColoring;
            await ReleaseReferenceHandleSafelyAsync();
        }

        void StartColoring()
        {
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(3);
            ColoringImage.DOScale(Vector3.one, _preparationTime);
            ColoringImage.DOAnchorPos(Vector2.zero, _preparationTime).OnComplete(() =>
            {
                RefferanceBar.gameObject.SetActive(true);
                LoadReferenceObj(_refferancePath + (LevelsManager.I.TempLvlIndex == -1 ? DBVariablesHolder.LvlIndex.Value : LevelsManager.I.TempLvlIndex));
            });
        }
        
        async void LoadReferenceObj(string path)
        {
            _referenceHandle = Addressables.LoadAssetAsync<GameObject>(path);
            await _referenceHandle.Task;

            if (_referenceHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"Failed to load Addressable prefab at: {path}");
                return;
            }

            GameObject referenceObj = Instantiate(_referenceHandle.Result as GameObject, RefferanceBar.GetChild(0));

            await Task.Yield();
        }

        async Task ReleaseReferenceHandleSafelyAsync()
        {
            if (_referenceReleaseInProgress)
                return;

            _referenceReleaseInProgress = true;

            await Task.Yield();

            if (_referenceHandle.IsValid())
            {
                Addressables.Release(_referenceHandle);
                _referenceHandle = default;
            }

            await Task.Yield();

            _referenceReleaseInProgress = false;
        }

    }
}
