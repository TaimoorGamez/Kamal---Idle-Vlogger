using UnityEngine;
using DG.Tweening;

namespace Core.GamePlay
{
    public class CurtainController : MonoBehaviour
    {
        [SerializeField] Transform RightCurtain, LeftCurtain;

        float _duration = 0.5f;

        private void OnEnable()
        {
            RightCurtain.localScale = Vector3.one;
            LeftCurtain.localScale = Vector3.one;
        }

        public void CloseCurtains()
        {
            RightCurtain.DOScaleX(0f, _duration).SetEase(Ease.Linear);
            LeftCurtain.DOScaleX(0f, _duration).SetEase(Ease.Linear).OnComplete(()=> gameObject.SetActive(false));
        }
    }
}
