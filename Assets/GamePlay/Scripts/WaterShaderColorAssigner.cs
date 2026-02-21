using UnityEngine;

namespace Core.GamePlay
{
    public class WaterShaderColorAssigner : ColorAssigner
    {
        [SerializeField] int ColorCounter = 4;

        protected override void Start()
        {
            base.Start();
            for (int c =1; c <= ColorCounter; c++)
            {
                _propBlock.SetColor("_Color" + c.ToString(), SkinColor);
            }
            MySkin.SetPropertyBlock(_propBlock);
        }
    }
}
