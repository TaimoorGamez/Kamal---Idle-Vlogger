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
            SimpleEventsHolder.UpdatePriceTxt += UpdateData;
            Body.DOScale(Vector3.one, _sizeTween).From(Vector3.zero).SetEase(Ease.OutBack);
            _priceData = new PriceHandler[ItemsNames.Length];
            _upgradeStates = new UpgradeStateData[ItemsNames.Length];
            _timerTweens = new Tween[ItemsNames.Length];
            UpdateData();
        }

        protected virtual void OnDisable()
        {
            SimpleEventsHolder.UpdatePriceTxt -= UpdateData;

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

        void UpdateData()
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
            UpdatePriceForAll();
        }

        protected virtual void UpdatePriceForAll()
        {
        }

        public virtual void UpdateItemProcess(int eventIndex, DBInt lvlData, int itemIndex, bool canUpdateVisuals)
        {
            int lvl = lvlData.Value;
            int cost = GetCost(lvl);
            if (CashCurrency.Amount >= _priceData[itemIndex].Cost)
            {
                CashCurrency.Amount -= _priceData[itemIndex].Cost;
                DBVariablesHolder.BasicIncome.Value += Increments[itemIndex] * _priceData[itemIndex].Levels;
                lvlData.Value += _priceData[itemIndex].Levels;
                if (lvlData.Value % GameManager.Instance.SpriteChangeCount == 0 && canUpdateVisuals)
                {
                    _upgradeStates[itemIndex].IsUpdating = true;
                    _upgradeStates[itemIndex].UpdateStartTime = DateTime.Now.ToString();
                    _upgradeStates[itemIndex].Levels = _priceData[itemIndex].Levels;
                    JsonDB.Save($"{ItemsNames[itemIndex]}_UpgradeState", _upgradeStates[itemIndex]);
                    SingleIntegerEventsHolder.UpdateItemEvent?.Invoke(eventIndex);
                }
                UpdatePriceForAll();
            }
            else
            {
                SingleIntegerEventsHolder.ShowToastEvent?.Invoke(0);
            }
        }

        protected virtual void UpdateCost(int item, int lvl)
        {
            if (!_upgradeStates[item].IsUpdating)
            {
                int totalLevels = 1, nextLvl = lvl + 1;
                int cost = GetCost(nextLvl);
                if (DBVariablesHolder.MaxLevels.Value > 0)
                {
                    int targetLvl = GetNextMilestone(lvl), maxLvls = nextLvl + 1, maxCost = cost + GetCost(maxLvls);
                    float availableCash = CashCurrency.Amount;
                    while (availableCash >= maxCost && maxLvls <= targetLvl)
                    {
                        cost = maxCost;
                        nextLvl = maxLvls;
                        totalLevels++;
                        maxLvls++;
                        maxCost = cost + GetCost(maxLvls);
                    }
                }
                _priceData[item].Levels = totalLevels;
                _priceData[item].Cost = cost;
                UpdatePriceTxts[item].text = GameManager.Instance.FormatMoney(cost);
                AvailableUpdatesTxt[item].text = $"+{totalLevels} Level";
                UpdateItemLvlTxt[item].text = $"Level: {nextLvl}";
                int count = GameManager.Instance.SpriteChangeCount;
                UpdateFillBars[item].fillAmount = (float)((nextLvl % count == 0) ? count : nextLvl % count) / count;
                UpdatePanels[item].SetActive(true);
                ChangeWaitingPanels[item].SetActive(false);
            }
            else
            {
                if (AnyRestriction(lvl))
                {
                    UpdatePanels[item].SetActive(false);
                    MovingWarningPanels[item].SetActive(true);
                    DBVariablesHolder.MapProgress.Value++;
                    SimpleEventsHolder.UpdateMapProgress?.Invoke();
                }
                else if (_upgradeStates[item].IsUpdating)
                {
                    UpdatePanels[item].SetActive(false);
                    ChangeWaitingPanels[item].SetActive(true);
                    TimeSpan timePassed = DateTime.Now - DateTime.Parse(_upgradeStates[item].UpdateStartTime);
                    float updateDelay = GameManager.Instance.UpdateDelay;
                    float remainingTime = updateDelay - (float)timePassed.TotalSeconds;
                    if (remainingTime > 0 && _timerTweens[item] == null)
                    {
                        float currentTime = remainingTime;
                        _timerTweens[item] = DOTween.To(() => currentTime, x => currentTime = x, 0, remainingTime)
                        .OnUpdate(() =>
                        {
                            int minutes = Mathf.FloorToInt(currentTime / 60);
                            int seconds = Mathf.FloorToInt(currentTime % 60);
                            UpdateTimerTxt[item].text = $"{minutes:00}:{seconds:00}";
                            TimerFillBar[item].fillAmount = 1 - (currentTime / updateDelay);
                        })
                        .OnComplete(() =>
                        {
                            int nextLvl = lvl + 1, count = GameManager.Instance.SpriteChangeCount;
                            UpdateItemLvlTxt[item].text = $"Level: {nextLvl}";
                            UpdateFillBars[item].fillAmount = (float)((nextLvl % count == 0) ? count : nextLvl % count) / count;
                            _upgradeStates[item].IsUpdating = false;
                            JsonDB.Save($"{ItemsNames[item]}_UpgradeState", _upgradeStates[item]);
                            UpdateData();
                        });
                    } 
                }
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

        bool AnyRestriction(int lvl)
        {
            return lvl % GameManager.Instance.MapChangeCount == 0;
        }
    }

    public struct PriceHandler
    {
        public int Levels, Cost;
    }

    public class UpgradeStateData
    {
        public bool IsUpdating;
        public string UpdateStartTime;
        public int Levels;
    }
}
