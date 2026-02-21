using UnityEngine;

namespace Core.GamePlay
{
    public class ChangeColorWithTexture : ColorAssigner
    {
        [SerializeField] Texture CurrentTexture;

        protected override void Start()
        {
            base.Start();
            _propBlock.SetColor("_Color", SkinColor);
            _propBlock.SetTexture("_MainTex", CurrentTexture);
            MySkin.SetPropertyBlock(_propBlock);
        }
    }
}
