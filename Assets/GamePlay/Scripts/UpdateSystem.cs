using TMPro;
using UnityEngine;
using DG.Tweening;
using Core.Economy;
using Core.DB.Variables;

namespace Core.GamePlay
{
    public class UpdateSystem : MonoBehaviour
    {
        [SerializeField] protected Transform Body;
        [SerializeField] protected float[] Increments;
        [SerializeField] protected TextMeshProUGUI[] UpdatePriceTxts, UpdateItemLvlTxt, AvailableUpdatesTxt;

        int _startingCost = 1;
        float _costMultiplier = 1.45f, _sizeTween = 0.25f;
        
        protected PriceHandler[] _priceData;

        protected virtual void OnEnable()
        {
            Body.DOScale(Vector3.one, _sizeTween).From(Vector3.zero).SetEase(Ease.OutBack);
            _priceData = new PriceHandler[Increments.Length];
            UpdatePriceForAll();
        }

        protected virtual void UpdatePriceForAll()
        { }

        public virtual void UpdateItem(int itemIndex)
        { }

        protected virtual void UpdateCost(int item, int lvl) 
        {
            if (!AnyRestriction())
            {
                UpdateItemLvlTxt[item].text = $"Level: {lvl}";
                int cost = GetCost(lvl);
                int nextLvls = 1;
                if (DBVariablesHolder.MaxLevels.Value > 0) 
                {
                    int nextCost = GetCost(lvl+1);
                    float availableCash = CashCurrency.Amount-cost;
                    while (availableCash > nextCost) 
                    {
                        availableCash -= nextCost;
                        cost += nextCost;
                        lvl++;
                        nextLvls++;
                        nextCost = GetCost(lvl+1);
                    }
                }
                _priceData[item].Levels = nextLvls;
                _priceData[item].Cost = cost;
                UpdatePriceTxts[item].text = GetCost(lvl).ToString();
                AvailableUpdatesTxt[item].text = $"+{nextLvls} Level";
            }
            else
            {
                // If there is any restriction, we can set the text to "Max" or something similar
            }
        }

        protected virtual int GetCost(int level)
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
}
