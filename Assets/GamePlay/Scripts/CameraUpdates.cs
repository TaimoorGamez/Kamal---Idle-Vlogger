using Core.DB.Variables;
using Core.Economy;
using static UnityEditor.Progress;

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

        public override int GetAvailableUpdates()
        {
            int count = 0;

            int cameraLvl = DBVariablesHolder.CameraLvl.Value;
            if (!AnyRestriction(cameraLvl) && CashCurrency.Amount >= GetCost(cameraLvl + 1))
                count++;

            int tripodLvl = DBVariablesHolder.TripodLvl.Value;
            if (!AnyRestriction(tripodLvl) && CashCurrency.Amount >= GetCost(tripodLvl + 1))
                count++;

            int microLvl = DBVariablesHolder.MicrophoneLvl.Value;
            if (!AnyRestriction(microLvl) && CashCurrency.Amount >= GetCost(microLvl + 1))
                count++;

            return count;
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
