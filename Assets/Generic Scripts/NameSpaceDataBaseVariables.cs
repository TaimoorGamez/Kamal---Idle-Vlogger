using System;
using UnityEngine;
using System.Collections.Generic;

namespace Core.DB.Variables
{
    public class DBInt
    {
        public string PrefName;
        public int DefaultValue;

        public int Value
        {
            get => PlayerPrefs.GetInt(PrefName, DefaultValue);

            set
            {
                PlayerPrefs.SetInt(PrefName, value);
                PlayerPrefs.Save();
            }
        }

        public DBInt(string name, int defaultValue = 0)
        {
            PrefName = name;
            DefaultValue = defaultValue;

            if (!PlayerPrefs.HasKey(PrefName))
            {
                PlayerPrefs.SetInt(PrefName, DefaultValue);
                PlayerPrefs.Save();
            }
        }
    }

    public class DBString
    {
        public string PrefName;
        public string DefaultValue;

        public string Value
        {
            get => PlayerPrefs.GetString(PrefName, DefaultValue);

            set
            {
                PlayerPrefs.SetString(PrefName, value);
                PlayerPrefs.Save();
            }
        }

        public DBString(string name, string defaultValue = "")
        {
            PrefName = name;
            DefaultValue = defaultValue;

            if (!PlayerPrefs.HasKey(PrefName))
            {
                PlayerPrefs.SetString(PrefName, DefaultValue);
                PlayerPrefs.Save();
            }
        }
    }

    public class DBFloat
    {
        public string PrefName;
        public float DefaultValue;
        public float Value
        {
            get => PlayerPrefs.GetFloat(PrefName, DefaultValue);
            set
            {
                PlayerPrefs.SetFloat(PrefName, value);
                PlayerPrefs.Save();
            }
        }
        public DBFloat(string name, float defaultValue = 0f)
        {
            PrefName = name;
            DefaultValue = defaultValue;

            if (!PlayerPrefs.HasKey(PrefName))
            {
                PlayerPrefs.SetFloat(PrefName, DefaultValue);
                PlayerPrefs.Save();
            }
        }
    }

    public static class DBVariablesHolder
    {
        //---------------------Ads Related -----------------------
        public static DBInt RemoveAds = new DBInt("removeads", 0);
        public static DBInt AdBlocked = new DBInt("AdBlocked", 0);
        public static DBString AdBlockingTime = new DBString("AdBlockingTime", DateTime.MinValue.ToString());
        public static DBString LastDate = new DBString("LastDate", DateTime.MinValue.ToString());

        //---------------------Economy Data -------------------  
        public static DBInt GoldWallet = new DBInt("GoldWallet", 0);

        //---------------------Daily Tasks ------------------------
        public static DBInt Task0 = new DBInt("Task0", 0);
        public static DBInt Task1 = new DBInt("Task1", 0);
        public static DBInt Task2 = new DBInt("Task2", 0);
        public static DBInt Task3 = new DBInt("Task3", 0);

        //---------------------Spin Wheel -------------------------
        public static DBInt SpinAvailable = new DBInt("DailySpinAvailable", 1);

        //---------------------Sound Related ----------------------
        public static DBInt Music = new DBInt("Music", 1);
        public static DBInt Sound = new DBInt("Sound", 1);

        //---------------------Store Data -------------------------


        //---------------------Game Flow --------------------------
        public static DBInt FFT = new DBInt("FFT", 0);
        public static DBInt IsGameplay = new DBInt("IsGameplay", 0);
        public static DBInt CurrentMap = new DBInt("CurrentMap", 0);
        public static DBInt CharismaLvl = new DBInt("CharismaLvl", 0);
        public static DBInt EruditionLvl = new DBInt("EruditionLvl", 0);
        public static DBInt ImprovisationLvl = new DBInt("ImprovisationLvl", 0);
        public static DBInt WitLvl = new DBInt("WitLvl", 0);
        public static DBInt CameraLvl = new DBInt("CameraLvl", 0);
        public static DBInt TripodLvl = new DBInt("TripodLvl", 0);
        public static DBInt MicrophoneLvl = new DBInt("MicrophoneLvl", 0);
        public static DBInt ClothesLvl = new DBInt("ClothesLvl", 0);
        public static DBInt HairsLvl = new DBInt("HairsLvl", 0);
        public static DBInt WatchLvl = new DBInt("WatchLvl", 0);
        public static DBInt HouseLvl = new DBInt("HouseLvl", 0);
        public static DBInt GroundLvl = new DBInt("GroundLvl", 0);
        public static DBInt VehicleLvl = new DBInt("VehicleLvl", 0);
        public static DBInt StatueLvl = new DBInt("StatueLvl", 0);
        public static DBInt BackyardLvl = new DBInt("BackyardLvl", 0);
        public static DBInt LastPlayedTime = new DBInt("LastPlayedTime", 0);
        public static DBInt StoryProgress = new DBInt("StoryProgress", 0);
        public static DBFloat BasicIncome = new DBFloat("BasicIncome", 1f);
        public static DBInt MaxLevels = new DBInt("MaxLevels", 0);
        public static DBInt SubscriberLvl = new DBInt("SubscriberLvl", 0);
    }

    public static class DBVariableDictionariesHolder
    {

        public static Dictionary<int, DBInt> TaskIndexies = new Dictionary<int, DBInt>()
        {
            { 0, DBVariablesHolder.Task0 },
            { 1, DBVariablesHolder.Task1 },
            { 2, DBVariablesHolder.Task2 },
            { 3, DBVariablesHolder.Task3 }
        };

        public static Dictionary<string, DBInt> NonConsumableProductsData = new Dictionary<string, DBInt>(StringComparer.Ordinal)
        {
            { "removeads", DBVariablesHolder.RemoveAds }
        };
    }

    public static class JsonDB
    {
        public static void Save<T>(string key, T data)
        {
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
        }

        public static T Load<T>(string key)
        {
            if (!PlayerPrefs.HasKey(key))
                return default;

            string json = PlayerPrefs.GetString(key);
            return JsonUtility.FromJson<T>(json);
        }
    }
}
