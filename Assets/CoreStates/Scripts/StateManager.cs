using UnityEngine;
using DG.Tweening;
using Core.Events;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.States
{
    public class StateManager : MonoBehaviour
    {
        public string SplashStatePath = "UI/State/SplashScreen", MainMenuStatePath = "UI/State/MainMenuScreen", 
                      GamePlayStatePath = "UI/State/GamePlayScreen",PauseStatePath = "UI/State/PauseScreen", 
                      LevelFailStatePath = "UI/State/FailScreen", LevelCompleteStatePath = "UI/State/CompleteScreen";

        bool _isClearing = false, _stateReleaseInProgress = false;
        int _sceneCounter = 0, _maxSceneCount = 5;
        Dictionary<string, GameObject> _loadedStates;
        AsyncOperationHandle<GameObject> _statehandle;

        public static StateManager I { get; private set; }

        private void Awake()
        {
            if (I == null)
            {
                I = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            _loadedStates = new Dictionary<string, GameObject>();
            ActiveState(SplashStatePath);
        }

        public async void ActiveState(string statePath)
        {
            await ReleaseStateHandleSafelyAsync();
            await Task.Delay(10);
            _statehandle = Addressables.LoadAssetAsync<GameObject>(statePath);
            await _statehandle.Task;

            if (_statehandle.Status == AsyncOperationStatus.Succeeded)
            {
                _loadedStates[statePath] = Instantiate(_statehandle.Result, transform);
                await Task.Delay(10);
                if (statePath == MainMenuStatePath)
                {
                    SimpleEventsHolder.CheckPluginStatus?.Invoke();
                    await ClearMemoryRoutine();
                }
            }
            else
            {
                Debug.Log("Failed to load AdsManager from Addressables");
                Addressables.Release(_statehandle);
            }
        }

        public void DestroyState(string statePath)
        {
            if (_loadedStates.TryGetValue(statePath, out GameObject state))
            {
                DOTween.Kill(state, complete: false);
                DOTween.Kill(state.transform);

                Destroy(state);
                _loadedStates.Remove(statePath);
            }
        }
        async Task ReleaseStateHandleSafelyAsync()
        {
            if (_stateReleaseInProgress)
                return;

            _stateReleaseInProgress = true;

            // wait one frame – Unity safe point
            await Task.Yield();

            if (_statehandle.IsValid())
            {
                Addressables.Release(_statehandle);
                _statehandle = default;
            }

            // optional extra frame (Android / scene safety)
            await Task.Yield();

            _stateReleaseInProgress = false;
        }


        async Task ClearMemoryRoutine()
        {
            if (_isClearing)
                return;

            _isClearing = true;

            System.GC.Collect();
            await Task.Yield();

            try
            {
                Caching.ClearCache();
            }
            catch (System.Exception e)
            {
                Debug.Log(e.ToString());
            }
            _isClearing = false;

            _sceneCounter++;
            if (_sceneCounter > _maxSceneCount)
            {
                await ReleaseStateHandleSafelyAsync();
                await Task.Yield();
                SceneManager.LoadScene(0);
            }
        }
    }
}