using TMPro;
using Core.Store;
using UnityEngine;
using Core.Events;
using DG.Tweening;
using Core.DB.Variables;
using System.Collections;
using Core.Plugins.Firebase;
using System.Collections.Generic;

namespace Core.Screen
{
    public class StorageRoomView : MonoBehaviour
    {
        [SerializeField] ItemView[] RoomItems;
        [SerializeField] RectTransform Content, Viewport;
        [SerializeField] GameObject[] Buttons;
        [SerializeField] Transform ItemHolder;
        [SerializeField] TextMeshProUGUI VideoText;
        [SerializeField] string ItemName;

        int _selectedItem = -1;
        Coroutine _activationRotine;
        GameObject _currentItem;
        Dictionary<int, GameObject> _itemContainer;

        private void OnEnable()
        {
            StoreBuyEventsHandler.I.BindEvent(ItemName, PurchaseByAd);
            _selectedItem = DBVariableDictionariesHolder.StoreActiveItems[ItemName].Value;
            _activationRotine = StartCoroutine(ActiveStorageRoom());
            UpdateItemStatus(_selectedItem);
            FirebaseHandler.I?.LogEvent($"ST_Open_{ItemName}");
        }

        private void OnDisable()
        {
            StoreBuyEventsHandler.I.UnBindEvent(ItemName, PurchaseByAd);
            DOTween.Kill(this);
            Content.DOKill();
            StopActiveRotines();
            if (_currentItem != null)
            {
                _currentItem.transform.DOKill();
                Destroy(_currentItem);
            }
            for (int i = 0; i < RoomItems.Length; i++)
            {
                RoomItems[i].DOKill();
                RoomItems[i].transform.localScale = Vector3.zero; 
                RoomItems[i].gameObject.SetActive(false);
            }
        }

        IEnumerator ActiveStorageRoom()
        {
            _itemContainer = StorageData.StoreItemsContainer[ItemName];

            for (int i = 0; i < RoomItems.Length; i++)
            {
                RoomItems[i].gameObject.SetActive(true);
                yield return new WaitForSeconds(0.1f);
            }
            ChangeActiveItem();
            InitItem(DBVariableDictionariesHolder.StoreActiveItems[ItemName].Value);
            yield return new WaitForSeconds(0.5f);
            float targetY = Content.rect.height - Viewport.rect.height;
            if (targetY > 200)
            {
                Content.DOAnchorPosY(targetY, 0.25f).SetEase(Ease.OutBack);
            }
            StopActiveRotines();
        }

        void StopActiveRotines()
        {
            if (_activationRotine != null)
            {
                StopCoroutine(_activationRotine);
                _activationRotine = null;
            }
        }

        void InitItem(int item)
        {
            if (_currentItem != null)
            {
                Destroy(_currentItem);
            }
            _currentItem = Instantiate(_itemContainer[item], ItemHolder);
        }

        public void UpdateItemStatus(int selectedItem)
        {
            _selectedItem = selectedItem;
            for (int i = 0; i < RoomItems.Length; i++)
            {
                if (i == selectedItem)
                {
                    RoomItems[i].SelectItem();
                    InitItem(i);
                    if (i == DBVariableDictionariesHolder.StoreActiveItems[ItemName].Value)
                    {
                        ChangeButton(0);
                    }
                    else if (StorageData.AllItems[ItemName][i].IsPurchased)
                    {
                        ChangeButton(1);
                    }
                    else
                    {
                        ChangeButton(2);
                        VideoText.text = StorageData.AllItems[ItemName][i].WatchedVideos + "/" + StorageData.AllItems[ItemName][i].TotalVideos;
                    }
                }
                else if (i == DBVariableDictionariesHolder.StoreActiveItems[ItemName].Value)
                {
                    RoomItems[i].ActiveSelectItem();
                }
                else
                {
                    RoomItems[i].UnSelectItem();
                }
            }
        }

        void ChangeButton(int activeButton)
        {
            for (int b = 0; b < Buttons.Length; b++)
            {
                Buttons[b].SetActive(false);
            }
            Buttons[activeButton].SetActive(true);
        }

        public void ChangeActiveItem()
        {
            RoomItems[DBVariableDictionariesHolder.StoreActiveItems[ItemName].Value].UnSelectItem();
            if (_selectedItem != -1)
            {
                DBVariableDictionariesHolder.StoreActiveItems[ItemName].Value = _selectedItem;
                RoomItems[DBVariableDictionariesHolder.StoreActiveItems[ItemName].Value].ActiveSelectItem();
                ChangeButton(0);
            }
        }

        void PurchaseByAd()
        {
            StorageData.AllItems[ItemName][_selectedItem].WatchedVideos += 1;
            VideoText.text = StorageData.AllItems[ItemName][_selectedItem].WatchedVideos + "/" + StorageData.AllItems[ItemName][_selectedItem].TotalVideos;
            if (StorageData.AllItems[ItemName][_selectedItem].WatchedVideos >= StorageData.AllItems[ItemName][_selectedItem].TotalVideos)
            {
                StorageData.AllItems[ItemName][_selectedItem].IsPurchased = true;
                UpdateItemStatus(_selectedItem);
                FirebaseHandler.I?.LogEvent($"ST_{ItemName}_item_{_selectedItem}");
            }
        }
    }
}