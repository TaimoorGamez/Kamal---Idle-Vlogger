using TMPro;
using DG.Tweening;
using UnityEngine;

namespace Core.GamePlay
{
    public class UpdateSystem : MonoBehaviour
    {
        [SerializeField] protected Transform Body;
        [SerializeField] protected float[] Increments;
        [SerializeField] protected TextMeshProUGUI[] UpdatePriceTxts, UpdateItemLvlTxt;

        int _startingCost = 1;
        float _costMultiplier = 1.45f, _sizeTween = 0.25f;

        protected virtual void OnEnable()
        {
            Body.DOScale(Vector3.one, _sizeTween).From(Vector3.zero).SetEase(Ease.OutBack);
        }

        public virtual void UpdateItem(int itemIndex)
        { }

        protected virtual void UpdateCost(int item, int lvl) 
        {
            if (!AnyRestriction())
            {
                UpdateItemLvlTxt[item].text = $"Level: {lvl}";
                UpdatePriceTxts[item].text = GetCost(lvl).ToString();
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
}
