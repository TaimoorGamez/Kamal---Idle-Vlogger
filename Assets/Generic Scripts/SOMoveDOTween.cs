using DG.Tweening;
using UnityEngine;

namespace Core.Animations.DT
{
    [CreateAssetMenu(fileName = "MoveLT", menuName = "ScriptableObjects/Animations/Move")]
    public class SOMoveDOTween : SODOTween
    {
        public override void PlayAnimation()
        {
            if (TargetObj == null)
                return;

            _tween = TargetObj.transform.DOLocalMove(TargetAction, Duration).SetTarget(TargetObj).SetLink(TargetObj, LinkBehaviour.KillOnDestroy);
            base.PlayAnimation();
        }

        public override void ReseToDefault()
        {
            if (TargetObj == null)
                return;

            TargetObj.transform.localPosition = OriginalVallue;
        }
    }
}
