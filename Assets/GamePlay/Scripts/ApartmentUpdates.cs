using Core.DB.Variables;

namespace Core.GamePlay
{
    public class ApartmentUpdates : UpdateSystem
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
            UpdateCost(0, DBVariablesHolder.HouseLvl.Value);
            UpdateCost(1, DBVariablesHolder.VehicleLvl.Value);
            UpdateCost(2, DBVariablesHolder.StatueLvl.Value);
            UpdateCost(3, DBVariablesHolder.BackyardLvl.Value);
            UpdateCost(4, DBVariablesHolder.GroundLvl.Value);
        }

        public void UpdateItem(int itemIndex)
        {
            switch (itemIndex)
            {
                case 0:
                    UpdateItemProcess(itemIndex,DBVariablesHolder.HouseLvl);
                    break;

                case 1:
                    UpdateItemProcess(itemIndex, DBVariablesHolder.VehicleLvl);
                    break;

                case 2:
                    UpdateItemProcess(itemIndex, DBVariablesHolder.StatueLvl);
                    break;

                case 3:
                    UpdateItemProcess(itemIndex, DBVariablesHolder.BackyardLvl);
                    break;

                case 4:
                    UpdateItemProcess(itemIndex, DBVariablesHolder.GroundLvl);
                    break;
            }
        }
    }
}
