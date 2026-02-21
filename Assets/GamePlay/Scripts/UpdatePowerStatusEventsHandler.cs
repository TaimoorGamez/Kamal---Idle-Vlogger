namespace Core.Events
{
    public class UpdatePowerStatusEventsHandler : EventsHandler
    {

        public static UpdatePowerStatusEventsHandler I { get; private set; }

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
                    SimpleEventsHolder.UpdateUndoStatusEvent += fun;
                    break;
                case "SwapColor":
                    SimpleEventsHolder.UpdateSwapStatusEvent += fun;
                    break;
                case "ExtraTube":
                    SimpleEventsHolder.UpdateExtraTubeStatusEvent += fun;
                    break;
            }
        }

        public override void UnBindEvent(string name, GameEvent fun)
        {
            switch (name)
            {
                case "SortUndo":
                    SimpleEventsHolder.UpdateUndoStatusEvent -= fun;
                    break;
                case "SwapColor":
                    SimpleEventsHolder.UpdateSwapStatusEvent -= fun;
                    break;
                case "ExtraTube":
                    SimpleEventsHolder.UpdateExtraTubeStatusEvent -= fun;
                    break;
            }
        }

        public override void TriggerEvent(string name)
        {
            switch (name)
            {
                case "SortUndo":
                    SimpleEventsHolder.UpdateUndoStatusEvent?.Invoke();
                    break;
                case "SwapColor":
                    SimpleEventsHolder.UpdateSwapStatusEvent?.Invoke();
                    break;
                case "ExtraTube":
                    SimpleEventsHolder.UpdateExtraTubeStatusEvent?.Invoke();
                    break;
            }
        }
    }
}