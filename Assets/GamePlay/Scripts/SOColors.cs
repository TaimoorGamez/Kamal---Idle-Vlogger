using UnityEngine;

namespace Core.GamePlay.Coloring
{
    [CreateAssetMenu(fileName = "lvl ", menuName = "ScriptableObjects/GamePlay/Colors")]
    public class SOColors : ScriptableObject
    {
        public Color32[] Colors;
    }
}
