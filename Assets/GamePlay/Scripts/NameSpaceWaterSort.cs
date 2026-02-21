using UnityEngine;

namespace Core.GamePlay.WaterSort
{
    public class UndoData
    {
        public TubeHandler SenderTube, GetterTube;
        public int LiquidLayers;
    }

    public class CapAnimation : MonoBehaviour
    {
        public virtual void PlayCapAnimation(Color currentColor)
        {

        }
    }
}
