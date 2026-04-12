using Core.DB.Variables;

namespace Core.GamePlay
{
    public class AppearanceUpdate : UpdateSystem
    {
        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void UpdatePriceForAll()
        {
            UpdateCost(0, DBVariablesHolder.ClothesLvl.Value);
            UpdateCost(1, DBVariablesHolder.HairsLvl.Value);
            UpdateCost(2, DBVariablesHolder.WatchLvl.Value);
        }

        public void UpdateItem(int itemIndex)
        {
            switch (itemIndex)
            {
                case 0:
                    UpdateItemProcess(itemIndex, DBVariablesHolder.ClothesLvl, 0, true);
                    break;

                case 1:
                    UpdateItemProcess(itemIndex, DBVariablesHolder.HairsLvl, 1, true);
                    break;

                case 2:
                    UpdateItemProcess(itemIndex, DBVariablesHolder.WatchLvl, 2, true);
                    break;
            }
        }
    }
}
