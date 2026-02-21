namespace Core.GamePlay
{
    public class SimpleColorAssigner : ColorAssigner
    {
        protected override void Start()
        {
            base.Start();
            _propBlock.SetColor("_Color", SkinColor);
            MySkin.SetPropertyBlock(_propBlock);
        }
    }
}
