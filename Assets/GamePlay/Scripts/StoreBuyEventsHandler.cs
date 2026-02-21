namespace Core.Events {
    public class StoreBuyEventsHandler : EventsHandler
    {
        public static StoreBuyEventsHandler I { get; private set; }

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
                case "Caps":
                    SimpleEventsHolder.BuyCaps += fun;
                    break;

                case "Sprays":
                    SimpleEventsHolder.BuySprays += fun;
                    break;

                case "FlameThrowers":
                    SimpleEventsHolder.BuyFlames += fun;
                    break;
            }
        }

        public override void UnBindEvent(string name, GameEvent fun)
        {
            switch (name)
            {
                case "Caps":
                    SimpleEventsHolder.BuyCaps -= fun;
                    break;

                case "Sprays":
                    SimpleEventsHolder.BuySprays -= fun;
                    break;

                case "FlameThrowers":
                    SimpleEventsHolder.BuyFlames -= fun;
                    break;
            }
        }

        public override void TriggerEvent(string name)
        {
            switch (name)
            {
                case "Caps":
                    SimpleEventsHolder.BuyCaps?.Invoke();
                    break;

                case "Sprays":
                    SimpleEventsHolder.BuySprays?.Invoke();
                    break;

                case "FlameThrowers":
                    SimpleEventsHolder.BuyFlames?.Invoke();
                    break;
            }
        }
    }
}

