using TMPro;
using UnityEngine;
using DG.Tweening;
using Core.Economy;
using Core.GamePlay;
using Core.Plugins.Ads;
using System.Collections;
using Core.Plugins.Firebase;

namespace Core.Screen
{
    public class SplashScreen : MonoBehaviour
    {
        [SerializeField] Transform FillImage;
        [SerializeField] TextMeshProUGUI LoadingText;

        float _loadingTime = 2f;
        string _loadingTxt = "Loading...     ";
        Coroutine _initCorotine = null;
        bool _isEnded = false;

        private void Start()
        {
            _isEnded = true;
            _initCorotine = StartCoroutine(InitializeGame());
        }

        IEnumerator InitializeGame()
        {
            FillImage.DOScaleX(1f, _loadingTime)
                .SetEase(Ease.Linear)
                .OnUpdate(() =>
                {
                    float currentX = FillImage.localScale.x;
                    int percent = (int)(currentX * 100f);
                    LoadingText.text = _loadingTxt + percent + "%";
                })
                .OnComplete(() =>
                {
                    GameManager.Instance.StartGame();
                    if (_initCorotine != null)
                    {
                        StopCoroutine(_initCorotine);
                    }
                    Destroy(gameObject, 0.1f);
                });

            CashCurrency.LoadEconomy();
            Subscribers.LoadSubscribers();

            // Firebase first
            FirebaseHandler.I.InitPlugin();

            while (_isEnded)
            {
                yield return new WaitForSeconds(1f);
                if (FirebaseHandler.I.IsInitialize)
                {
                    if (FirebaseHandler.I.IsRemoteFetched)
                    {
                        if (!AdsManager.I.IsInitialized)
                        {
                            AdsManager.I.InitPlugin();
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
            }
        }
    }
}