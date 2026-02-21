using UnityEngine;
using DG.Tweening;
using Core.Events;

namespace Core.GamePlay.WaterSort
{
    public class FlowerAnimation : CapAnimation
    {
        [SerializeField] Renderer MySkin;
        [SerializeField] Texture MyTexture;

        float _transparencyChangeDuration = 2.5f, _targetTransparency = 1;
        MaterialPropertyBlock _propBlock;

        private void OnEnable()
        {
            SimpleEventsHolder.StartColoringEvent += HideNow;
        }

        private void OnDisable()
        {
            SimpleEventsHolder.StartColoringEvent -= HideNow;
        }

        public override void PlayCapAnimation(Color currentColor)
        {
            _propBlock = new MaterialPropertyBlock();
            _propBlock.SetColor("_BaseColor", currentColor);
            _propBlock.SetTexture("_MainTex", MyTexture);
            _propBlock.SetFloat("_ColorRange", 0);
            MySkin.SetPropertyBlock(_propBlock); 
            transform.DOLocalMoveY(0,0.2f).SetLink(gameObject).OnComplete(() =>
            {
                SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(9);
                DOTween.To(() => 0f, value =>
                {
                    _propBlock.SetFloat("_ColorRange", value);
                    MySkin.SetPropertyBlock(_propBlock);
                }, _targetTransparency, _transparencyChangeDuration).SetLink(gameObject);
            });
        }

        void HideNow()
        {
            gameObject.SetActive(false);
        }

    }
}
