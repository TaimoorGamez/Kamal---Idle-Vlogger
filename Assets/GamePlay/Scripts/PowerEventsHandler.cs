namespace Core.Events
{
    public class PowerEventsHandler : EventsHandler
    {
        public static PowerEventsHandler I { get; private set; }

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
            switch(name)
            {
                case "SortUndo":
                    SimpleEventsHolder.UndoEvent += fun;
                    break;
                case "SwapColor":
                    SimpleEventsHolder.SwapColorsEvent += fun;
                    break;
                case "ExtraTube":
                    SimpleEventsHolder.ExtraTubeEvent += fun;
                    break;
            }
        }

        public override void UnBindEvent(string name, GameEvent fun)
        {
            switch(name)
            {
                case "SortUndo":
                    SimpleEventsHolder.UndoEvent -= fun;
                    break;
                case "SwapColor":
                    SimpleEventsHolder.SwapColorsEvent -= fun;
                    break;
                case "ExtraTube":
                    SimpleEventsHolder.ExtraTubeEvent -= fun;
                    break;
            }
        }

        public override void TriggerEvent(string name)
        {
            switch(name)
            {
                case "SortUndo":
                    SimpleEventsHolder.UndoEvent?.Invoke();
                    break;
                case "SwapColor":
                    SimpleEventsHolder.SwapColorsEvent?.Invoke();
                    break;
                case "ExtraTube":
                    SimpleEventsHolder.ExtraTubeEvent?.Invoke();
                    break;
            }
        }
    }
}