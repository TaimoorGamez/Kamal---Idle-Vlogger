using UnityEngine;
using System.Collections;

namespace Core.Animations.DT
{
    public class PlaySODOTweens : MonoBehaviour
    {
        [SerializeField] GameObject[] TargetObjs;
        [SerializeField] SODOTween[] TweenAnimations;
        [SerializeField] float[] Delays;
        [SerializeField] bool PlayOnEnable = false, PlayLoop = false, ResetOnDisable = true;
        [SerializeField] float StartDelay = 0;

        Coroutine _animationsRotine;

        private void OnEnable()
        {
            if (PlayOnEnable)
            {
                PlayAnimation();
            }
        }

        public void PlayAnimation()
        {
            _animationsRotine = StartCoroutine(PlayAllAnimations());
        }

        IEnumerator PlayAllAnimations()
        {
            for (int a = 0; a < TweenAnimations.Length; a++)
            {
                TweenAnimations[a].TargetObj = TargetObjs[a];
                TweenAnimations[a].PlayAnimation();
                yield return new WaitForSeconds(Delays[a]);
            }
            yield return new WaitForSeconds(StartDelay);
            AnimationsComplete();
        }

        void AnimationsComplete()
        {
            if (_animationsRotine != null)
            {
                StopCoroutine(_animationsRotine);
                _animationsRotine = null;
            }

            if (PlayLoop)
            {
                PlayAnimation();
            }
        }
        public void StopAnimation()
        {
            // Stop coroutine
            if (_animationsRotine != null)
            {
                StopCoroutine(_animationsRotine);
                _animationsRotine = null;
            }

            // Stop all tweens
            for (int i = 0; i < TweenAnimations.Length; i++)
            {
                TweenAnimations[i].CancleAnimation();   // Add this in SODOTween
            }
        }

        private void OnDisable()
        {
            if (ResetOnDisable)
            {
                for (int a = 0; a < TweenAnimations.Length; a++)
                {
                    TweenAnimations[a].ReseToDefault();
                }
            }
        }
    }
}
