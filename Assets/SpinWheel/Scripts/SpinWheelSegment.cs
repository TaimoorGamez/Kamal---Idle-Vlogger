using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.SpinWheel
{
    public class SpinWheelSegment : MonoBehaviour
    {
        [SerializeField] Image IconImage, SegmentImage;
        [SerializeField] TextMeshProUGUI AmountText;

        public void Initialize(Sprite icon, int amount, Color segmentColor, float fillAmount)
        {
            SegmentImage.fillAmount = fillAmount;
            IconImage.sprite = icon;
            AmountText.text = amount.ToString();
            ChangeGradient(segmentColor);
        }

        public void ChangeGradient(Color segmentColor)
        {
            SegmentImage.color = segmentColor;
        }
    }
}
