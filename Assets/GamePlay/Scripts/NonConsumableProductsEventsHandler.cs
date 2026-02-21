namespace Core.Events
{
    public class NonConsumableProductsEventsHandler : EventsHandler
    {
        public static NonConsumableProductsEventsHandler I { get; private set; }

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
                case "removeads":
                    SimpleEventsHolder.RemoveAds += fun;
                    break;
            }
        }

        public override void UnBindEvent(string name, GameEvent fun)
        {
            switch (name)
            {
                case "removeads":
                    SimpleEventsHolder.RemoveAds -= fun;
                    break;
            }
        }

        public override void TriggerEvent(string name)
        {
            switch (name)
            {
                case "removeads":
                    SimpleEventsHolder.RemoveAds?.Invoke();
                    break;
            }
        }
    }
}
