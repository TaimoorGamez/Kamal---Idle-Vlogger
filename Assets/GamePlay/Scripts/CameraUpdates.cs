using Core.DB.Variables;

namespace Core.GamePlay
{
    public class CameraUpdates : UpdateSystem
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
            UpdateCost(0, DBVariablesHolder.CameraLvl.Value);
            UpdateCost(1, DBVariablesHolder.TripodLvl.Value);
            UpdateCost(2, DBVariablesHolder.MicrophoneLvl.Value);
        }

        public void UpdateItem(int itemIndex)
        {
            switch (itemIndex)
            {
                case 3:
                    UpdateItemProcess(itemIndex, DBVariablesHolder.CameraLvl, 0, true);
                    break;

                case 4:
                    UpdateItemProcess(itemIndex, DBVariablesHolder.TripodLvl, 1, true);
                    break;

                case 5:
                    UpdateItemProcess(itemIndex, DBVariablesHolder.MicrophoneLvl, 2, true);
                    break;
            }
        }
    }
}
