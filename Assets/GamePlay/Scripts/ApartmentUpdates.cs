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
            base.UpdatePriceForAll();
            UpdateCost(0, DBVariablesHolder.HouseLvl.Value);
            UpdateCost(1, DBVariablesHolder.VehicleLvl.Value);
            UpdateCost(2, DBVariablesHolder.BackyardLvl.Value);
            UpdateCost(3, DBVariablesHolder.GroundLvl.Value);
            UpdateCost(4, DBVariablesHolder.StatueLvl.Value);
        }

        public void UpdateItem(int itemIndex)
        {
            switch (itemIndex)
            {
                case 6:
                    UpdateItemProcess(itemIndex,DBVariablesHolder.HouseLvl, 0, true);
                    break;

                case 7:
                    UpdateItemProcess(itemIndex, DBVariablesHolder.VehicleLvl, 1, true);
                    break;

                case 8:
                    UpdateItemProcess(itemIndex, DBVariablesHolder.BackyardLvl, 2, true);
                    break;

                case 9:
                    UpdateItemProcess(itemIndex, DBVariablesHolder.GroundLvl, 3, true);
                    break;

                case 10:
                    UpdateItemProcess(itemIndex, DBVariablesHolder.StatueLvl, 4, true);
                    break;
            }
        }
    }
}
