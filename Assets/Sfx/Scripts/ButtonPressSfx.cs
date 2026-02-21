using Core.Events;
using UnityEngine;

namespace Core.Sfx
{
    public class ButtonPressSfx : MonoBehaviour
    {
       
        public void OnBtnPress()
        {
            SimpleEventsHolder.BtnPressSfxEvent?.Invoke();
        }
    }
}
