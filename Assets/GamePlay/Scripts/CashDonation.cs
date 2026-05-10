using TMPro;
using UnityEngine;
using DG.Tweening;
using Core.Economy;
using UnityEngine.UI;
using Core.DB.Variables;

namespace Core.GamePlay
{
    public class CashDonation : MonoBehaviour
    {
        [SerializeField] Image FillImage;
        [SerializeField] TextMeshProUGUI AmountText;

        RectTransform _parent, _rect, _textRect;        
        Tween _moveTween, _fillTween, _scaleTween, _amountTween;
        int _rewardMultipler = 10;
        float _SpawnScaleDuration = 0.25f, _lifeTime = 5f, _moveDuration = 2, _textDisplayDuration = 0.65f, _donationAmount = 0, _textPadding = 125;

        void Awake()
        {
            _rect = GetComponent<RectTransform>();
            _textRect = AmountText.GetComponent<RectTransform>();
            _parent = _rect.parent.GetComponent<RectTransform>();
        }

        void OnEnable()
        {
            _scaleTween = _rect.DOScale(Vector3.one, _SpawnScaleDuration).From(Vector3.zero).OnComplete(StartMoving);
            _donationAmount = DBVariablesHolder.BasicIncome.Value * _rewardMultipler;
            AmountText.text = $"+{GameManager.Instance.FormatMoney(_donationAmount)}";
            AmountText.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            _scaleTween.Kill();
            _fillTween.Kill();
            _moveTween.Kill();
            _amountTween.Kill();
            AmountText.gameObject.SetActive(false);
        }

        public void StartMoving()
        {
            MoveRandom();
            _fillTween = FillImage.DOFillAmount(1, _lifeTime).From(0).SetEase(Ease.Linear).OnComplete(DisappearNow);
        }

        void DisappearNow()
        {
            _scaleTween.Kill();
            _fillTween.Kill();
            _moveTween.Kill();
            _scaleTween = _rect.DOScale(Vector3.zero, _SpawnScaleDuration).SetEase(Ease.InBack).OnComplete(()=> gameObject.SetActive(false));
        }

        void MoveRandom()
        {
            float x = Random.Range(-_parent.rect.width / 2f, _parent.rect.width / 2f);
            float y = Random.Range(-_parent.rect.height / 3f, _parent.rect.height / 3f);
            _moveTween = _rect.DOAnchorPos(new Vector2(x, y), _moveDuration).SetEase(Ease.InOutSine).OnComplete(MoveRandom);
        }

        public void OnClickDonation()
        {
            CashCurrency.Amount += _donationAmount;
            _moveTween.Kill();
            AmountText.gameObject.SetActive(true);
            _amountTween = _textRect.DOAnchorPosY(_textPadding, _textDisplayDuration).From(Vector2.zero).SetEase(Ease.OutSine).OnComplete(DisappearNow);
        }
    }
}