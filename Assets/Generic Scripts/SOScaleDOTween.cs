using DG.Tweening;
using UnityEngine;

namespace Core.Animations.DT
{
    [CreateAssetMenu(fileName = "Scale", menuName = "ScriptableObjects/Animations/Scale")]
    public class SOScaleDOTween : SODOTween
    {
        public override void PlayAnimation()
        {
            if(TargetObj == null)
                return;

            _tween = TargetObj.transform.DOScale(TargetAction, Duration).SetTarget(TargetObj).SetLink(TargetObj, LinkBehaviour.KillOnDestroy);
            base.PlayAnimation();
        }

        public override void ReseToDefault()
        {
            if(TargetObj == null)
                return;

            TargetObj.transform.localScale = OriginalVallue;
        }
    }
}
