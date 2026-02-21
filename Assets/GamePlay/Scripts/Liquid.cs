using DG.Tweening;
using UnityEngine;

namespace Core.GamePlay.WaterSort
{
    public class Liquid : MonoBehaviour
    {
        [SerializeField] Renderer MySkin;

        MaterialPropertyBlock _propBlock;
        float _transparencyChangeDuration = 1;

        private void Start()
        {
            _propBlock = new MaterialPropertyBlock();
        }

        public void SetHidenColour()
        {
            _propBlock.SetFloat("_HidenRange", 1);
            MySkin.SetPropertyBlock(_propBlock);
        }

        public void SetColor(Color currentColor)
        {
            _propBlock.SetColor("_BaseColor", currentColor);
            _propBlock.SetFloat("_TransparencyRange", 1);
            MySkin.SetPropertyBlock(_propBlock);
        }
        public void ChangeColor(Color currentColor)
        {
            _propBlock.SetColor("_BaseColor", currentColor);
            _propBlock.SetFloat("_TransparencyRange", 0);
            MySkin.SetPropertyBlock(_propBlock);
            DOTween.To(() => 0f, x =>
            {
                _propBlock.SetFloat("_TransparencyRange", x);
                MySkin.SetPropertyBlock(_propBlock);
            }, 1f, 1);
        }

        public void SetGlow(bool state)
        {
            if (state)
            {
                _propBlock.SetInteger("_Glow", 1);
            }
            else
            {
                _propBlock.SetInteger("_Glow", 0);
            }
            MySkin.SetPropertyBlock(_propBlock);
        }

        public void HideColor()
        {
            _propBlock.SetFloat("_TransparencyRange", 0);
            MySkin.SetPropertyBlock(_propBlock);
        }

        public void SmoothlyAddColor(Color currentColor, float duration)
        {
            _propBlock.SetColor("_BaseColor", currentColor);
            _propBlock.SetFloat("_TransparencyRange", 0);
            MySkin.SetPropertyBlock(_propBlock);

            DOTween.To(() => 0f, x =>
            {
                _propBlock.SetFloat("_TransparencyRange", x);
                MySkin.SetPropertyBlock(_propBlock);
            }, 1f, duration);
        }


        public void RevelColour()
        {
            DOTween.To(() => 1f, x =>
            {
                _propBlock.SetFloat("_HidenRange", x);
                MySkin.SetPropertyBlock(_propBlock);
            }, 0f, _transparencyChangeDuration);
        }
    }
}