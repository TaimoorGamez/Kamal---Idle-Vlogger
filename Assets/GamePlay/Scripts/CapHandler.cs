using UnityEngine;
using Core.DB.Variables;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.GamePlay.WaterSort
{
    public class CapHandler : MonoBehaviour
    {
        string _capsPath = "GamePlay/Cap/";
        CapAnimation _myAnimation;
        AsyncOperationHandle _capHandle;
        bool _capReleaseInProgress = false;

        private void Start()
        {
            LoadCapItem(_capsPath + DBVariablesHolder.CurrentActiveCap.Value);
        }

        async void LoadCapItem(string path)
        {
            _capHandle = Addressables.LoadAssetAsync<GameObject>(path);
            await _capHandle.Task;

            if (_capHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"Failed to load Addressable prefab at: {path}");
                return;
            }

            _myAnimation = null;
            GameObject obj = Instantiate(_capHandle.Result as GameObject, transform);
            await Task.Yield();
            await Task.Yield();
            _myAnimation = obj.GetComponent<CapAnimation>();

            await Task.Yield();
        }

        public void PlayCelebration(Color currentColor)
        {
            _myAnimation.gameObject.SetActive(true);
            _myAnimation.PlayCapAnimation(currentColor);
        }

        public void HideCap()
        {
           _myAnimation.gameObject.SetActive(false);
        }

        private async void OnDisable()
        {
            await ReleaseCapHandleSafelyAsync();
        }

        async Task ReleaseCapHandleSafelyAsync()
        {
            if (_capReleaseInProgress)
                return;

            _capReleaseInProgress = true;

            await Task.Yield();

            if (_capHandle.IsValid())
            {
                Addressables.Release(_capHandle);
                _capHandle = default;
            }

            await Task.Yield();

            _capReleaseInProgress = false;
        }

    }
}