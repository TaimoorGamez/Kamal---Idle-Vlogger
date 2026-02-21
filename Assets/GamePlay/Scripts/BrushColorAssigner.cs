using Core.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Core.GamePlay.Coloring
{
    public class BrushColorAssigner : MonoBehaviour
    {
        [SerializeField] Image BurshImage;


        private void OnEnable()
        {
            SimpleEventsHolder.ColorSelectedEvent += ColorSelected;
        }

        private void OnDisable()
        {
            SimpleEventsHolder.ColorSelectedEvent -= ColorSelected;
        }

        void ColorSelected()
        {
            BurshImage.color = LevelsManager.I.CurrentColor;
        }
    }
}
