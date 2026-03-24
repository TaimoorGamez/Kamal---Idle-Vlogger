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
            UpdateCost(0, DBVariablesHolder.CameraLvl.Value);
            UpdateCost(1, DBVariablesHolder.TripodLvl.Value);
            UpdateCost(2, DBVariablesHolder.MicrophoneLvl.Value);
        }

        public void UpdateItem(int itemIndex)
        {
            switch (itemIndex)
            {
                case 0:
                    UpdateItemProcess(itemIndex, DBVariablesHolder.CameraLvl, 3, true);
                    break;

                case 1:
                    UpdateItemProcess(itemIndex, DBVariablesHolder.TripodLvl, 4, true);
                    break;

                case 2:
                    UpdateItemProcess(itemIndex, DBVariablesHolder.MicrophoneLvl, 5, true);
                    break;
            }
        }
    }
}
