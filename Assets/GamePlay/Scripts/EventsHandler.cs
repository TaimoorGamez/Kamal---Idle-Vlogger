using UnityEngine;

namespace Core.Events
{
    public class EventsHandler : MonoBehaviour
    {
        public virtual void BindEvent(string name, GameEvent fun) { }
        public virtual void UnBindEvent(string name, GameEvent fun) { }
        public virtual void TriggerEvent(string name) { }
    }
}
