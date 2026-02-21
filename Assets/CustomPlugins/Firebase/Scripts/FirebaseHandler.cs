using Firebase;
using UnityEngine;
using Firebase.Analytics;
using Firebase.RemoteConfig;

namespace Core.Plugins.Firebase
{
    public class FirebaseHandler : MonoBehaviour
    {
        [HideInInspector] public bool IsInitialize = false, IsRemoteFetched = false;

        string _deviceId="=";

        public static FirebaseHandler I { get; private set; }

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

        public void InitPlugin()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                DependencyStatus dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    FetchRemoteConfig();
                    IsInitialize = true;
                }
                else
                {
                    Debug.Log("Could not resolve all Firebase dependencies:" + dependencyStatus);
                }
            });
        }

        public void FetchRemoteConfig()
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                Debug.Log($"Do not use Firebase until it is properly initialized by calling");
                return;
            }

            Debug.Log("Fetching data...");
            FirebaseRemoteConfig remoteConfig = FirebaseRemoteConfig.DefaultInstance;
            remoteConfig.FetchAsync(System.TimeSpan.Zero).ContinueWith(
               Task =>
               {
                   if (!Task.IsCompleted)
                   {
                       Debug.Log($"{nameof(remoteConfig.FetchAsync)} incomplete: Status '{Task.Status}'");
                       return;
                   }
                   ActivateRetrievedRemoteConfigValues();
               });
        }

        private void ActivateRetrievedRemoteConfigValues()
        {
            FirebaseRemoteConfig remoteConfig = FirebaseRemoteConfig.DefaultInstance;
            ConfigInfo info = remoteConfig.Info;
            if (info.LastFetchStatus == LastFetchStatus.Success)
            {
                remoteConfig.ActivateAsync().ContinueWith(
                   Task =>
                   {
                       RemoteDataHolder.MaxLevelsAvailable = (int)remoteConfig.GetValue("MaxLevelsAvailable").LongValue;

                       string adJson = remoteConfig.GetValue("AdConfig").StringValue;
                       RemoteDataHolder.AdData = JsonUtility.FromJson<AdConfig>(adJson);
                          IsRemoteFetched = true;
                   });
            }

            string rawId = SystemInfo.deviceUniqueIdentifier; _deviceId = string.IsNullOrEmpty(rawId)? "unknown": rawId.Length > 10? rawId[^10..]: rawId;
        }

        public void LogEvent(string eventString)
        {
            if (IsInitialize)
            {
                eventString = eventString.Replace(":", "_").Replace("|", "_").Replace(" ", "");
                string finalEvent =  $"{eventString}_DeviceId_{_deviceId}";
                FirebaseAnalytics.LogEvent(finalEvent);
            }
        }

        //public void LevelStartEvent(string lvlNum)
        //{
        //    if (FirebaseInit.IsFirebaseInit)
        //    {
        //        Parameter[] parameters = {
        //    new Parameter ("level", lvlNum),
        //    new Parameter ("current_gold", CurrenciesHolder.CashCurrency.Amount),
        //         };
        //        FirebaseAnalytics.LogEvent("level_start", parameters);
        //    }
        //}

        //public void LevelCompleteEvent(int lvlNum, double lvlTime)
        //{
        //    if (FirebaseInit.IsFirebaseInit)
        //    {
        //        Parameter[] parameters = {
        //    new Parameter ("level", lvlNum.ToString()),
        //    new Parameter ("timeplayed", lvlTime.ToString()),
        //    };
        //        FirebaseAnalytics.LogEvent("level_complete", parameters);
        //    }
        //}

        //public void LevelFailEvent(int lvlNum, string failNum, double lvlTime)
        //{
        //    if (FirebaseInit.IsFirebaseInit)
        //    {
        //        Parameter[] parameters = {
        //    new Parameter ("level", lvlNum.ToString()),
        //    new Parameter ("failcount", failNum),
        //     };
        //        FirebaseAnalytics.LogEvent("level_fail", parameters);
        //    }
        //}

        //public void EarnCoinsEvent(string coinsType, long amount, string sourceOfIncome)
        //{
        //    if (FirebaseInit.IsFirebaseInit)
        //    {
        //        Parameter[] parameters = {
        //    new Parameter ("virtual_currency_name", coinsType),
        //    new Parameter ("value", amount),
        //    new Parameter ("source", sourceOfIncome),
        //    };
        //        FirebaseAnalytics.LogEvent("earn_virtual_currency", parameters);
        //    }
        //}

        //public void SpendCoinsEvent(string coinsType, long amount, string purchaseName)
        //{
        //    if (FirebaseInit.IsFirebaseInit)
        //    {
        //        Parameter[] parameters = {
        //    new Parameter ("virtual_currency_name", coinsType),
        //    new Parameter ("value", amount),
        //    new Parameter ("item_name", purchaseName),
        //     };
        //        FirebaseAnalytics.LogEvent("spend_virtual_currency", parameters);
        //    }
        //}

        //public void LogEvent(string eventString)
        //{
        //    if (FirebaseInit.IsFirebaseInit)
        //    {
        //        FirebaseAnalytics.LogEvent(eventString);
        //    }
        //}

        //public void AdRevenue(MaxSdkBase.AdInfo adInfo)
        //{
        //    var impressionParameters = new[]
        //    {
        //    new Parameter("ad_platform", "AppLovin"),
        //    new Parameter("ad_source", adInfo.NetworkName),
        //    new Parameter("ad_unit_name", adInfo.AdUnitIdentifier),
        //    new Parameter("ad_format", adInfo.AdFormat),
        //    new Parameter("value", adInfo.Revenue),
        //    new Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
        //};

        //    FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
        //}
    }
}
