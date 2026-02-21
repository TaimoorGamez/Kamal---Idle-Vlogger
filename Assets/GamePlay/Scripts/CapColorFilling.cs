using DG.Tweening;

namespace Core.GamePlay
{
    public class CapColorFilling : ColorAssigner
    {
        protected override void Start()
        {
            base.Start();
            _propBlock.SetColor("_BaseColor", SkinColor);
            MySkin.SetPropertyBlock(_propBlock);
            DOTween.To(() => 0f, value =>
            {
                _propBlock.SetFloat("_ColorRange", value);
                MySkin.SetPropertyBlock(_propBlock);
            }, 1, 1).SetLink(gameObject);
        }
    }
}