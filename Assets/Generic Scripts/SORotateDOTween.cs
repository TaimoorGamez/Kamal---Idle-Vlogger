using DG.Tweening;
using UnityEngine;

namespace Core.Animations.DT
{
    [CreateAssetMenu(fileName = "RotateLT", menuName = "ScriptableObjects/Animations/Rotation")]
    public class SORotateDOTween : SODOTween
    {
        [SerializeField] RotateMode RotMode;

        public override void PlayAnimation()
        {
            if (TargetObj == null)
                return;

            _tween = TargetObj.transform.DORotate(TargetAction, Duration, RotMode).SetTarget(TargetObj).SetLink(TargetObj, LinkBehaviour.KillOnDestroy);
            base.PlayAnimation();
        }
        public override void ReseToDefault()
        {
            if (TargetObj == null)
                return;

            TargetObj.transform.eulerAngles = OriginalVallue;
        }
    }
}
