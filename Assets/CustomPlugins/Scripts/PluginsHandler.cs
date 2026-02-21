using TMPro;
using Core.Events;
using UnityEngine;
using Core.Screen;
using DG.Tweening;
using Core.Plugins.Ads;
using Core.DB.Variables;
using System.Collections;
using Core.Plugins.Firebase;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.Plugins
{
    public class PluginsHandler : MonoBehaviour
    {
        [SerializeField] GameObject StateManager, DownloadingScreen;
        [SerializeField] UiScreens InternetPanel;
        [SerializeField] TextMeshProUGUI LoadingText;
        [SerializeField] Transform FillImage;

        string _loadingTxt = "Downloading...     ";
        bool _isCheckingPlugins = false, _remoteDownloadSuccess = false;
        Coroutine _checkRoutine;
        AsyncOperationHandle<List<string>> _checkCatalogHandle;
        AsyncOperationHandle _updateCatalogHandle;
        AsyncOperationHandle<long> _sizeHandle;
        AsyncOperationHandle _downloadHandle;

        void OnEnable()
        {
            SimpleEventsHolder.CheckPluginStatus += CheckAllPlugins;
        }

        private void OnDisable()
        {
            SimpleEventsHolder.CheckPluginStatus -= CheckAllPlugins;
        }


        IEnumerator Start()
        {
            yield return HasWorkingInternet(isOnline =>
            {
                if (!isOnline)
                {
                    InternetPanel.gameObject.SetActive(true);
                    _checkRoutine = StartCoroutine(CheckInternetLoop());
                }
                else
                {
                    ContinueGame();
                    CheckAllPlugins();
                }
            });
        }

        void ContinueGame()
        {
            if(_checkRoutine != null)
            {
                StopCoroutine(_checkRoutine);
                _checkRoutine = null;
            }
            StateManager.SetActive(true);
        }

        IEnumerator CheckInternetLoop()
        {
            bool isOnline = false;
            while (!isOnline)
            {
                yield return HasWorkingInternet(result =>
                {
                    isOnline = result;
                });

                yield return new WaitForSeconds(4);
            }

            if(isOnline)
            {
                SceneManager.LoadScene(0);
            }
        }

        IEnumerator HasWorkingInternet(System.Action<bool> callback)
        {
            using (UnityWebRequest req =
                   UnityWebRequest.Head("https://www.google.com/generate_204"))
            {
                req.timeout = 4;
                yield return req.SendWebRequest();

                bool ok = req.result == UnityWebRequest.Result.Success;
                callback?.Invoke(ok);
            }
        }

        void CheckAllPlugins()
        {

            if (FirebaseHandler.I == null || AdsManager.I == null)
                return;

            if (FirebaseHandler.I.IsInitialize && AdsManager.I.IsInitialized)
                return;

            if (!_isCheckingPlugins)
            {
                _isCheckingPlugins = true;
                StartCoroutine(PluginsFlow());
            }
        }

        IEnumerator PluginsFlow()
        {
            bool isOnline = false;

            yield return HasWorkingInternet(result =>
            {
                isOnline = result;
            });

            if (!isOnline)
            {
                _isCheckingPlugins = false;
                yield break;
            }

            if (!FirebaseHandler.I.IsInitialize)
            {
                FirebaseHandler.I.InitPlugin();
                float timeout = 10f;
                while (!FirebaseHandler.I.IsRemoteFetched && timeout > 0f)
                {
                    timeout -= Time.deltaTime;
                    yield return null;
                }
            }

            if (RemoteDataHolder.MaxLevelsAvailable > DBVariablesHolder.MaxLvlCount.Value)
            {
                DownloadingScreen.SetActive(true); 
                yield return CheckAndUpdateCatalog();
                yield return DownloadRemoteLevels();
                yield return new WaitForEndOfFrame();
                if (_remoteDownloadSuccess)
                {
                    _remoteDownloadSuccess = false;
                    if (DBVariablesHolder.LvlNum.Value >= RemoteDataHolder.MaxLevelsAvailable)
                    {
                        DBVariablesHolder.LvlIndex.Value = DBVariablesHolder.MaxLvlCount.Value;
                    }
                    yield return new WaitForEndOfFrame();
                    DBVariablesHolder.MaxLvlCount.Value = RemoteDataHolder.MaxLevelsAvailable;
                }
                DownloadingScreen.SetActive(false); 
            }

            if (!AdsManager.I.IsInitialized)
            {
                AdsManager.I.InitPlugin();
            }

            _isCheckingPlugins = false;
        }

        IEnumerator CheckAndUpdateCatalog()
        {
            _checkCatalogHandle = Addressables.CheckForCatalogUpdates(false);
            yield return _checkCatalogHandle;

            if (_checkCatalogHandle.Status == AsyncOperationStatus.Succeeded &&
                _checkCatalogHandle.Result != null &&
                _checkCatalogHandle.Result.Count > 0)
            {
                _updateCatalogHandle =
                    Addressables.UpdateCatalogs(_checkCatalogHandle.Result, false);

                yield return _updateCatalogHandle;
            }
        }

        IEnumerator DownloadRemoteLevels()
        {
            _sizeHandle = Addressables.GetDownloadSizeAsync("remote_lvl");
            yield return _sizeHandle;

            long downloadSize = _sizeHandle.Result;

            if (downloadSize > 0)
            {
                _downloadHandle = Addressables.DownloadDependenciesAsync("remote_lvl", true);

                while (!_downloadHandle.IsDone)
                {
                    UpdateLoadingUI(_downloadHandle.PercentComplete);
                    yield return null;
                }
                _remoteDownloadSuccess = true;
            }

            UpdateLoadingUI(1);
        }

        void UpdateLoadingUI(float progress)
        {
            FillImage.DOScaleX(progress, 0.1f).SetEase(Ease.Linear).SetUpdate(true);
            int txtPercent = (int)(progress * 100f);
            LoadingText.text = _loadingTxt + txtPercent.ToString() + "%";
        }
    }
}
