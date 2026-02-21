using UnityEngine;

namespace Core.Screen
{
    public class BottomHandler : UiScreens
    {
        [SerializeField] GameObject[] ActiveLayers, MainMenuPanels;
        [SerializeField] UiScreens[] MainMenuScreens;

        int _currentLayerIndex = 0;

        public void ChangeActiveLayer(int index)
        {
            if (_currentLayerIndex == index)
                return;

            ActiveLayers[_currentLayerIndex].SetActive(false);
            MainMenuScreens[_currentLayerIndex].OnClose();
            ActiveLayers[index].SetActive(true);
            if(index != 0)
            {
                MainMenuPanels[index ].SetActive(true);
            }
           _currentLayerIndex = index;
        }
    }
}