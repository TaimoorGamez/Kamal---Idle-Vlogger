using TMPro;
using UnityEngine;
using DG.Tweening;
using Core.Events;
using Core.Economy;
using System.Collections;

namespace Core.Screen
{
    public class CurrencyBarController : MonoBehaviour
    {
        [SerializeField] GameObject CurrencyIcon, CurrencyShine;
        [SerializeField] TextMeshProUGUI CurrencyTxt;
        [SerializeField] RectTransform InitLocation, SubmitLocation;

        int _minCurrenyCount = 8, _maxCurrencyCount = 13, _currentAmount;
        float _circleRadious = 1, _moveDuration = 0.5f;
        GameObject[] _currencyIconArray = new GameObject[0];
        Coroutine _depositRotine, _transactionRotine;
        bool _isDeposting = false, _doingTransaction;

        private void OnEnable()
        {
            CurrencyShine.SetActive(false);
            _currentAmount = CurrenciesHolder.CashCurrency.Amount;
            CurrencyTxt.text = _currentAmount.ToString();
            SingleIntegerEventsHolder.DepositEvent += DepositCash;
            SingleIntegerEventsHolder.TransactionEvent += TransactCash;
        }

        private void OnDisable()
        {
            SingleIntegerEventsHolder.DepositEvent -= DepositCash;
            SingleIntegerEventsHolder.TransactionEvent -= TransactCash;
            DOTween.Kill(this);
            DepositEnd();
            TransactionEnd();
        }

        void DepositCash(int cashAmount)
        {
            if (!_isDeposting)
            {
                _isDeposting = true;
                _depositRotine = StartCoroutine(DepositAnimation(cashAmount));
            }
        }
        IEnumerator DepositAnimation(int cashAmount)
        {
            int cashCount = Random.Range(_minCurrenyCount, _maxCurrencyCount);
            _currencyIconArray = new GameObject[cashCount];
            int amountSegment = cashAmount / cashCount;
            float angleStep = 360f / cashCount;

            for (int c = 0; c < cashCount; c++)
            {
                float angle = c * angleStep;
                float radians = angle * Mathf.Deg2Rad;

                float x = Mathf.Cos(radians) * _circleRadious;
                float y = Mathf.Sin(radians) * _circleRadious;

                Vector3 position = new Vector3(x, y, 0) + InitLocation.position;

                GameObject newAmount = Instantiate(CurrencyIcon, transform);
                newAmount.transform.position = position;
                _currencyIconArray[c] = newAmount;
            }

            yield return new WaitForSeconds(0.1f);

            for (int m = 0; m < cashCount; m++)
            {
                int index = m;
                GameObject icon = _currencyIconArray[index];
                if (icon == null) continue;

                RectTransform cashTransform = icon.GetComponent<RectTransform>();
                icon.SetActive(true);

                cashTransform
                    .DOAnchorPos(SubmitLocation.anchoredPosition, _moveDuration)
                    .SetEase(Ease.InBack)
                    .SetTarget(cashTransform)
                    .SetAutoKill(true)
                    .OnComplete(() =>
                    {
                        if (icon != null)
                            icon.SetActive(false);

                        CurrencyShine.SetActive(true);
                        _currentAmount += amountSegment;
                        CurrencyTxt.text = _currentAmount.ToString();
                        SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(8);
                    });

                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(0.1f);
            CurrencyTxt.text = CurrenciesHolder.CashCurrency.Amount.ToString();
            CurrencyShine.SetActive(false);

            DepositEnd();
        }
        void DepositEnd()
        {
            for (int c = 0; c < _currencyIconArray.Length; c++)
            {
                GameObject icon = _currencyIconArray[c];
                if (icon == null) continue;

                RectTransform rt = icon.GetComponent<RectTransform>();
                if (rt != null)
                    DOTween.Kill(rt);

                Destroy(icon);
            }

            if (_depositRotine != null)
            {
                StopCoroutine(_depositRotine);
                _depositRotine = null;
            }

            _isDeposting = false;
        }


        void TransactCash(int cashAmount)
        {
            if (!_doingTransaction)
            {
                //Debug.Log("line 123");
                _doingTransaction = true;
                _transactionRotine = StartCoroutine(TransactionAnimation(cashAmount));
            }
        }

        IEnumerator TransactionAnimation(int cashAmount)
        {
            int cashCount = Random.Range(_minCurrenyCount, _maxCurrencyCount);
            int amountSegment = cashAmount / cashCount;
            for (int t = 0; t < cashCount; t++)
            {
                SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(8);
                CurrencyShine.SetActive(true);
                _currentAmount -= amountSegment;
                CurrencyTxt.text = _currentAmount.ToString();
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(0.1f);
            CurrencyTxt.text = CurrenciesHolder.CashCurrency.Amount.ToString();
            CurrencyShine.SetActive(false);
            TransactionEnd();
        }

        void TransactionEnd()
        {
            if (_transactionRotine != null)
            {
                StopCoroutine(_transactionRotine);
            }
            _doingTransaction = false;
        }
    }
}
