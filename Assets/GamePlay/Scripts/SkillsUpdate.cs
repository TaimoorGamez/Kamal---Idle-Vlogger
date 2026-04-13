using Core.DB.Variables;

namespace Core.GamePlay
{
    public class SkillsUpdate : UpdateSystem
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
            UpdateCost(0, DBVariablesHolder.CharismaLvl.Value);
            UpdateCost(1, DBVariablesHolder.EruditionLvl.Value);
            UpdateCost(2, DBVariablesHolder.ImprovisationLvl.Value);
            UpdateCost(3, DBVariablesHolder.WitLvl.Value);
        }

        public void UpdateItem(int itemIndex)
        {
            switch (itemIndex)
            {
                case -1:
                    UpdateItemProcess(itemIndex, DBVariablesHolder.CharismaLvl, 0, false);
                    break;

                case -2:
                    UpdateItemProcess(itemIndex, DBVariablesHolder.EruditionLvl, 1, false);
                    break;

                case -3:
                    UpdateItemProcess(itemIndex, DBVariablesHolder.ImprovisationLvl, 2, false);
                    break;

                case -4:
                    UpdateItemProcess(itemIndex, DBVariablesHolder.WitLvl, 3, false);
                    break;
            }
        }
    }
}
