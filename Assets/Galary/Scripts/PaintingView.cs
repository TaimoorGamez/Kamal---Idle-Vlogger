using UnityEngine;
using UnityEngine.UI;

namespace Core.Screen
{
    public class PaintingView : MonoBehaviour
    {
        [SerializeField] RawImage PaintingImg;
        [SerializeField] Image[] Stars;

        int _currentLvlNum, _currentLvlIndex, _starsCount;
        LvlPreview _lvlPreview;

        public void InitPainting(Texture paintingTex, int lvlNum, int lvlIndex, int paintingStars, LvlPreview lvlPreview)
        {
            PaintingImg.texture = paintingTex;
            for(int s = 0; s < paintingStars; s++)
            {
                Stars[s].enabled = true;
            }
            PaintingImg.enabled = true;
            _currentLvlNum = lvlNum;
            _currentLvlIndex = lvlIndex;
            _starsCount = paintingStars;
            _lvlPreview = lvlPreview;
        }

        public void GenerateCustomLvl()
        {
            _lvlPreview.LevelDetails(PaintingImg.mainTexture, _currentLvlNum, _currentLvlIndex, _starsCount);
            _lvlPreview.gameObject.SetActive(true);
        }
    }
}
