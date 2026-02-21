using System;
using UnityEngine;
using System.Collections.Generic;

namespace Core.Store
{
    public class ItemData
    {
        public int ItemId, TotalVideos, TotalCards;

        string _itemName;

        public ItemData(string name, int id, int totalVideos, int totalCards)
        {
            ItemId = id;
            TotalVideos = totalVideos;
            _itemName = name;
            TotalCards = totalCards;
        }

        public bool IsPurchased
        {
            get => PlayerPrefs.GetInt(_itemName + ItemId, 0) == 1;
            set => PlayerPrefs.SetInt(_itemName + ItemId, value ? 1 : 0);
        }

        public int WatchedVideos
        {
            get => PlayerPrefs.GetInt(_itemName + "Videos" + ItemId, 0);
            set => PlayerPrefs.SetInt(_itemName + "Videos" + ItemId, Mathf.Clamp(value, 0, TotalVideos));
        }

        public int AvailableCards
        {
            get => PlayerPrefs.GetInt(_itemName + "Cards" + ItemId, 0);
            set => PlayerPrefs.SetInt(_itemName + "Cards" + ItemId, Mathf.Clamp(value, 0, TotalCards));
        }
    }

    public static class StorageData
    {
        public static string FlameThrowersKey = "FlameThrowers", CapsKey = "Caps", SpraysKey = "Sprays";

        public static Dictionary<string, Dictionary<int, GameObject>> StoreItemsContainer = new Dictionary<string, Dictionary<int, GameObject>>(StringComparer.Ordinal);

        public static Dictionary<string, Dictionary<int, ItemData>> AllItems = new Dictionary<string, Dictionary<int, ItemData>>(System.StringComparer.Ordinal)
        {
            {
                "FlameThrowers",
                new Dictionary<int, ItemData>()
                {
                    { 0, new ItemData("Flame", 0, 0, 0) },
                    { 1, new ItemData("Flame", 1, 3, 8) },
                    { 2, new ItemData("Flame", 2, 3, 8) },
                    { 3, new ItemData("Flame", 3, 3, 8) },
                }
            },
            {
                "Caps",
                new Dictionary<int, ItemData>()
                {
                    { 0, new ItemData("Cap", 0, 0, 0) },
                    { 1, new ItemData("Cap", 1, 2, 6) },
                    { 2, new ItemData("Cap", 2, 2, 6) },
                    { 3, new ItemData("Cap", 3, 3, 8) },
                    { 4, new ItemData("Cap", 4, 3, 8) },
                    { 5, new ItemData("Cap", 5, 3, 8) },
                }
            },
            {
                "Sprays",
                new Dictionary<int, ItemData>()
                {
                    { 0, new ItemData("Spray", 0, 0, 0) },
                    { 1, new ItemData("Spray", 1, 2, 6) },
                    { 2, new ItemData("Spray", 2, 2, 6) },
                    { 3, new ItemData("Spray", 3, 2, 6) },
                    { 4, new ItemData("Spray", 4, 3, 8) },
                    { 5, new ItemData("Spray", 5, 3, 8) },
                    { 6, new ItemData("Spray", 6, 3, 8) },
                    { 7, new ItemData("Spray", 7, 3, 8) },
                }
            }
        };
    } 
}

