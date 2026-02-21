namespace Core.Events
{
    public class RewardPowerEventsHandler : EventsHandler
    {
        public static RewardPowerEventsHandler I { get; private set; }

        private void Start()
        {
            if (I == null)
            {
                I = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        public override void BindEvent(string name, GameEvent fun)
        {
            switch (name)
            {
                case "SortUndo":
                    SimpleEventsHolder.RewardUndoEvent += fun;
                    break;
                case "SwapColor":
                    SimpleEventsHolder.RewardSwapColor += fun;
                    break;
                case "ExtraTube":
                    SimpleEventsHolder.RewardExtraTubeEvent += fun;
                    break;
            }
        }

        public override void UnBindEvent(string name, GameEvent fun)
        {
            switch (name)
            {
                case "SortUndo":
                    SimpleEventsHolder.RewardUndoEvent -= fun;
                    break;
                case "SwapColor":
                    SimpleEventsHolder.RewardSwapColor -= fun;
                    break;
                case "ExtraTube":
                    SimpleEventsHolder.RewardExtraTubeEvent -= fun;
                    break;
            }
        }

        public override void TriggerEvent(string name)
        {
            switch (name)
            {
                case "SortUndo":
                    SimpleEventsHolder.RewardUndoEvent?.Invoke();
                    break;
                case "SwapColor":
                    SimpleEventsHolder.RewardSwapColor?.Invoke();
                    break;
                case "ExtraTube":
                    SimpleEventsHolder.RewardExtraTubeEvent?.Invoke();
                    break;
            }
        }
    }
}
