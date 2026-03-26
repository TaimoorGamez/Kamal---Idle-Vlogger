using TMPro;
using System;
using UnityEngine;
using DG.Tweening;
using Core.Events;
using Core.Economy;
using UnityEngine.UI;
using Core.DB.Variables;

namespace Core.GamePlay
{
    public class UpdateSystem : MonoBehaviour
    {
        [SerializeField] protected Transform Body;
        [SerializeField] protected float[] Increments;
        [SerializeField] protected Image[] UpdateFillBars, TimerFillBar;
        [SerializeField] protected TextMeshProUGUI[] UpdatePriceTxts, UpdateItemLvlTxt, AvailableUpdatesTxt, UpdateTimerTxt;
        [SerializeField] protected GameObject[] UpdatePanels, ChangeWaitingPanels, MovingWarningPanels;
        [SerializeField] protected string[] ItemsNames;

        int _startingCost = 1;
        float _costMultiplier = 1.45f, _sizeTween = 0.25f;

        protected PriceHandler[] _priceData;
        protected UpgradeStateData[] _upgradeStates;
        protected Tween[] _timerTweens;

        protected virtual void OnEnable()
        {
            SimpleEventsHolder.UpdatePriceTxt += UpdatePriceForAll;
            Body.DOScale(Vector3.one, _sizeTween).From(Vector3.zero).SetEase(Ease.OutBack);
            _priceData = new PriceHandler[Increments.Length];
            _upgradeStates = new UpgradeStateData[Increments.Length];
            _timerTweens = new Tween[Increments.Length];
            UpdatePriceForAll();
        }

        protected virtual void OnDisable()
        {
            SimpleEventsHolder.UpdatePriceTxt -= UpdatePriceForAll;
            for (int i = 0; i < _upgradeStates.Length; i++)
            {
                if (_upgradeStates[i].IsUpdating)
                {
                    JsonDB.Save($"{ItemsNames[i]}_UpgradeState", _upgradeStates[i]);
                }
            }

            if (_timerTweens != null)
            {
                for (int i = 0; i < _timerTweens.Length; i++)
                {
                    if (_timerTweens[i] != null && _timerTweens[i].IsActive())
                    {
                        _timerTweens[i].Kill();
                        _timerTweens[i] = null;
                    }
                }
            }
        }

        protected virtual void UpdatePriceForAll()
        {
            for (int i = 0; i < _upgradeStates.Length; i++)
            {
                if (PlayerPrefs.HasKey($"{ItemsNames[i]}_UpgradeState"))
                {
                    _upgradeStates[i] = JsonDB.Load<UpgradeStateData>($"{ItemsNames[i]}_UpgradeState");
                }
                else
                {
                    _upgradeStates[i] = new UpgradeStateData { IsUpdating = false, UpdateStartTime = "" };
                }
            }
        }

        public virtual void UpdateItemProcess(int itemIndex, DBInt lvlData, int eventIndex, bool canUpdateVisuals)
        {
            int lvl = lvlData.Value;
            int cost = GetCost(lvl);
            if (CashCurrency.Amount >= _priceData[itemIndex].Cost)
            {
                CashCurrency.Amount -= _priceData[itemIndex].Cost;
                DBVariablesHolder.BasicIncome.Value += Increments[itemIndex] * _priceData[itemIndex].Levels;
                if (lvlData.Value % GameManager.Instance.SpriteChangeCount == 0 && canUpdateVisuals)
                {
                    _upgradeStates[itemIndex].IsUpdating = true;
                    _upgradeStates[itemIndex].UpdateStartTime = DateTime.Now.ToString();
                    _upgradeStates[itemIndex].Levels = _priceData[itemIndex].Levels;
                    DoubleIntegerEventHolder.UpdateItemEvent?.Invoke(eventIndex, _priceData[itemIndex].Levels);
                    UpdatePanels[itemIndex].SetActive(false); 
                    ChangeWaitingPanels[itemIndex].SetActive(true);
                    float delay = GameManager.Instance.UpdateDelay;
                    float currentTime = delay;
                    _timerTweens[itemIndex] = DOTween.To(() => currentTime, x => currentTime = x, 0, delay)
                    .OnUpdate(() =>
                    {
                        int minutes = Mathf.FloorToInt(currentTime / 60);
                        int seconds = Mathf.FloorToInt(currentTime % 60);

                        UpdateTimerTxt[itemIndex].text = $"{minutes:00}:{seconds:00}";
                        TimerFillBar[itemIndex].fillAmount = 1 - (currentTime / delay);
                    })
                    .OnComplete(() =>
                    {
                        UpdatePanels[itemIndex].SetActive(true);
                        ChangeWaitingPanels[itemIndex].SetActive(false);
                    });
                }
                else
                {
                    lvlData.Value += _priceData[itemIndex].Levels;
                }
                if (DBVariablesHolder.MaxLevels.Value > 0)
                {
                    UpdatePriceForAll();
                }
                else
                {
                    UpdateCost(itemIndex, DBVariablesHolder.HouseLvl.Value);
                }
            }
            else
            {
                SingleIntegerEventsHolder.ShowToastEvent?.Invoke(0);
            }
        }



        protected virtual void UpdateCost(int item, int lvl)
        {
            if (!AnyRestriction() && !_upgradeStates[item].IsUpdating)
            {
                UpdateItemLvlTxt[item].text = $"Level: {lvl}";
                UpdateFillBars[item].fillAmount = (float)(lvl % GameManager.Instance.SpriteChangeCount) / GameManager.Instance.SpriteChangeCount;
                int cost = GetCost(lvl);
                int nextLvls = 1;
                if (DBVariablesHolder.MaxLevels.Value > 0)
                {
                    int targetLvl = GetNextMilestone(lvl);
                    int nextCost = GetCost(lvl + 1);
                    float availableCash = CashCurrency.Amount - cost;
                    while (availableCash > nextCost && lvl < targetLvl - 1)
                    {
                        availableCash -= nextCost;
                        cost += nextCost;
                        lvl++;
                        nextLvls++;
                        nextCost = GetCost(lvl + 1);
                    }
                }
                _priceData[item].Levels = nextLvls;
                _priceData[item].Cost = cost;
                UpdatePriceTxts[item].text = GetCost(lvl).ToString();
                AvailableUpdatesTxt[item].text = $"+{nextLvls} Level";
            }
            else if (_upgradeStates[item].IsUpdating)
            {
                UpdatePanels[item].SetActive(false);
                ChangeWaitingPanels[item].SetActive(true);
                TimeSpan timePassed = DateTime.Now - DateTime.Parse(_upgradeStates[item].UpdateStartTime);
                float updateDelay = GameManager.Instance.UpdateDelay;
                float remainingTime = updateDelay - (float)timePassed.TotalSeconds;
                if (remainingTime > 0)
                {
                    float currentTime = remainingTime;
                    _timerTweens[item] = DOTween.To(() => currentTime, x => currentTime = x, 0, remainingTime).OnUpdate(() =>
                    {
                        int minutes = Mathf.FloorToInt(remainingTime / 60);
                        int seconds = Mathf.FloorToInt(remainingTime % 60);
                        UpdateTimerTxt[item].text = $"{minutes:00}:{seconds:00}";
                        TimerFillBar[item].fillAmount = 1 - (remainingTime / updateDelay);
                    })
                    .OnComplete(() =>
                    {
                        UpdatePanels[item].SetActive(true);
                        ChangeWaitingPanels[item].SetActive(false);
                    });
                }
            }
            else
            {
                UpdatePanels[item].SetActive(false);
                MovingWarningPanels[item].SetActive(true);
            }
        }

        protected int GetNextMilestone(int level)
        {
            int mileStonePoint = GameManager.Instance.SpriteChangeCount;
            return ((level / mileStonePoint) + 1) * mileStonePoint;
        }

        protected int GetCost(int level)
        {
            return (int)(_startingCost * Mathf.Pow(_costMultiplier, level));
        }

        bool AnyRestriction()
        {
            return false;
        }
    }

    public struct PriceHandler
    {
        public int Levels, Cost;
    }

    public struct UpgradeStateData
    {
        public bool IsUpdating;
        public string UpdateStartTime;
        public int Levels;
    }
}
