using UnityEngine;
using UnityEngine.UI;

namespace Core.CustomUI
{
    [AddComponentMenu("UI/Effects/UIGradient")]
    public class UIGradient : BaseMeshEffect
    {
        public enum GradientType
        {
            Vertical,
            Horizontal,
            DiagonalLeftToRight,
            DiagonalRightToLeft,
            FromCenterToBoundaries
        }

        public enum BlendMode
        {
            Normal,
            Additive,
            Multiply
        }

        [SerializeField] private Gradient gradient = new Gradient();
        [SerializeField] private GradientType gradientType = GradientType.Vertical;
        [SerializeField] private BlendMode blendMode = BlendMode.Normal;

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive() || vh.currentVertCount == 0 || graphic == null)
                return;

            Rect rect = graphic.rectTransform.rect;

            // Safety: prevent divide-by-zero & invalid rect
            if (rect.width <= 0f || rect.height <= 0f)
                return;

            UIVertex vertex = new UIVertex();

            for (int i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref vertex, i);

                float t = GetNormalizedPosition(vertex.position, rect);
                t = Safe01(t);

                vertex.color = ApplyBlendMode(vertex.color, gradient.Evaluate(t));
                vh.SetUIVertex(vertex, i);
            }
        }

        // ================= SAFE HELPERS =================

        private float Safe01(float value)
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
                return 0f;

            return Mathf.Clamp01(value);
        }

        private float GetNormalizedPosition(Vector3 position, Rect rect)
        {
            float value = 0f;

            switch (gradientType)
            {
                case GradientType.Vertical:
                    value = (position.y - rect.yMin) / rect.height;
                    break;

                case GradientType.Horizontal:
                    value = (position.x - rect.xMin) / rect.width;
                    break;

                case GradientType.DiagonalLeftToRight:
                    value =
                        ((position.x - rect.xMin) + (position.y - rect.yMin)) /
                        (rect.width + rect.height);
                    break;

                case GradientType.DiagonalRightToLeft:
                    value =
                        ((rect.xMax - position.x) + (position.y - rect.yMin)) /
                        (rect.width + rect.height);
                    break;

                case GradientType.FromCenterToBoundaries:
                    Vector2 center = rect.center;
                    float maxDistance =
                        Mathf.Sqrt(rect.width * rect.width + rect.height * rect.height) * 0.5f;

                    if (maxDistance <= 0f)
                        return 0f;

                    value = Vector2.Distance(position, center) / maxDistance;
                    break;
            }

            return value;
        }

        private Color ApplyBlendMode(Color originalColor, Color gradientColor)
        {
            switch (blendMode)
            {
                case BlendMode.Additive:
                    return originalColor + gradientColor;

                case BlendMode.Multiply:
                    return originalColor * gradientColor;

                case BlendMode.Normal:
                default:
                    return gradientColor;
            }
        }

        // ================= PUBLIC API =================

        public Gradient Gradient
        {
            get => gradient;
            set
            {
                gradient = value;
                graphic?.SetVerticesDirty();
            }
        }

        public GradientType GradientDirection
        {
            get => gradientType;
            set
            {
                gradientType = value;
                graphic?.SetVerticesDirty();
            }
        }

        public BlendMode BlendModeType
        {
            get => blendMode;
            set
            {
                blendMode = value;
                graphic?.SetVerticesDirty();
            }
        }
    }
}
