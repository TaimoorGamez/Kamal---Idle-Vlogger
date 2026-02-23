using TMPro;
using UnityEngine;
using DG.Tweening;

namespace Core.Screen
{
    public class SplashScreen : MonoBehaviour
    {
        [SerializeField] Transform FillImage;
        [SerializeField] TextMeshProUGUI LoadingText;

        float _loadingTime = 2;
        string _loadingTxt = "Loading...     ";
     

        private void Start()
        {
            FillImage.DOScaleX(1f, _loadingTime).SetEase(Ease.Linear).OnUpdate(() =>
            {
                float currentX = FillImage.localScale.x;
                int percent = (int)(currentX * 100f);
                LoadingText.text = _loadingTxt + percent + "%";
            }).OnComplete(() =>
            {
                Destroy(gameObject, 0.1f);
            });
        }
    }
}
