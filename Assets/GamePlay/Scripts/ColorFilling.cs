using UnityEngine;
using UnityEngine.UI;

namespace Core.GamePlay.Coloring
{
    public class ColorFilling : MonoBehaviour
    {
        [SerializeField] RawImage ColoringPart;

        public Color32 DefaultColor;
        public Animation MyAnimation;

        Texture2D _partTexture;
        int _totalColoredPixles = 0;
        Color32 _whiteColor = Color.white;

        private void Start()
        {
            _partTexture = new Texture2D(ColoringPart.texture.width, ColoringPart.texture.height, TextureFormat.RGBA32, false);

            Texture2D originalTexture = (Texture2D)ColoringPart.texture;
            Color32[] originalPixels = originalTexture.GetPixels32();
            byte alphaThreshold = 50;

            for (int i = 0; i < originalPixels.Length; i++)
            {
                // If alpha > 0, make the pixel white, preserving alpha
                if (originalPixels[i].a > alphaThreshold)
                {
                    originalPixels[i] = _whiteColor;
                    _totalColoredPixles++;
                }
            }

            _partTexture.SetPixels32(originalPixels);
            _partTexture.Apply();

            ColoringPart.texture = _partTexture;
        }

        public Texture2D GetCurrenTexture()
        {
            return _partTexture;
        }

        public int GetColoredPixlesCount()
        {
            return  _totalColoredPixles;
        }
    }
}
