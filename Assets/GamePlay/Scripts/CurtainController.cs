using UnityEngine;
using DG.Tweening;

namespace Core.GamePlay
{
    public class CurtainController : MonoBehaviour
    {
        [SerializeField] Transform RightCurtain, LeftCurtain;

        float _duration = 1f;

        private void OnEnable()
        {
            RightCurtain.localScale = Vector3.one;
            LeftCurtain.localScale = Vector3.one;
            CloseCurtains();
        }

        void CloseCurtains()
        {
            RightCurtain.DOScaleX(0f, _duration).SetEase(Ease.Linear);
            LeftCurtain.DOScaleX(0f, _duration).SetEase(Ease.Linear).OnComplete(()=> gameObject.SetActive(false));
        }
    }
}
