using UnityEngine;
using DG.Tweening;

namespace Core.Animations
{ 

}

namespace Core.Animations.DT
{
    public class SODOTween : ScriptableObject
    {
        public GameObject TargetObj { get; set; }

        [SerializeField] protected float Duration;
        [SerializeField] protected Ease CurrentEaseType;
        [SerializeField] bool IsLoop = false, CanChangeStateOnComlete = false, StateOnComplete = true, ResetOnComplete = false;
        [SerializeField] protected Vector3 TargetAction, OriginalVallue;

        protected Tween _tween;

        public virtual void PlayAnimation()
        {
            _tween.SetEase(CurrentEaseType);

            if (IsLoop)
            {
                _tween.Loops();
            }

            _tween.OnComplete(() => {
                if (CanChangeStateOnComlete)
                { TargetObj.SetActive(StateOnComplete); }
                if (ResetOnComplete)
                {
                    ReseToDefault();
                }
                });
        }

        public virtual void CancleAnimation()
        {
            if (_tween != null)
            {
                _tween.Kill();
                _tween = null;
            }
        }

        public virtual void ReseToDefault()
        {

        }
    }
}

