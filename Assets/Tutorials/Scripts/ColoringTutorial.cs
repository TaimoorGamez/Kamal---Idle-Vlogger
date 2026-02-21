using UnityEngine;
using Core.Events;

namespace Core.GamePlay.Coloring
{
    public class ColoringTutorial : MonoBehaviour
    {
        [SerializeField] GameObject TutorialHand;


        private void OnEnable()
        {
            SimpleEventsHolder.StartColoringEvent += StartColoring;
            SimpleEventsHolder.ColorSelectedEvent += ColorSelected;
        }

        private void OnDisable()
        {
            SimpleEventsHolder.StartColoringEvent -= StartColoring;
            SimpleEventsHolder.ColorSelectedEvent -= ColorSelected;
        }

        void StartColoring()
        {
            Invoke(nameof(OnTutorialHand),1);
        }

        void OnTutorialHand()
        {
            TutorialHand.SetActive(true);
        }

        void ColorSelected()
        {
            TutorialHand.SetActive(false);
        }
    }
}
