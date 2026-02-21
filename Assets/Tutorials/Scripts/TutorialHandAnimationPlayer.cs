using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Core.Animations.DT
{
    public class TutorialHandAnimationPlayer : MonoBehaviour
    {
        [SerializeField] GameObject HandObj;
        [SerializeField] Image ParticleObj;
        [SerializeField] SODOTween ScaleDown, ScaleUp;
        [SerializeField] float Delay;

        Coroutine _animationsRotine;
        bool _whileEnable;

        private void OnEnable()
        {
            _whileEnable = true;
            ScaleDown.TargetObj = HandObj;
            ScaleUp.TargetObj = HandObj;
            _animationsRotine = StartCoroutine(PlayAnimation());
        }
        private void OnDisable()
        {
            _whileEnable = false;
            if (_animationsRotine != null)
            {
                StopCoroutine(_animationsRotine);
            }
        }

        IEnumerator PlayAnimation()
        {
            float customTime = 0.01f;
            while (_whileEnable)
            {
                ScaleDown.PlayAnimation();
                yield return new WaitForSeconds(Delay);
                ScaleUp.PlayAnimation();
                float elapseTime = 0;
                Color currentColor = ParticleObj.color;
                currentColor.a = 0;
                ParticleObj.color = currentColor;
                ParticleObj.rectTransform.localScale = Vector2.zero;
                ParticleObj.gameObject.SetActive(true);
                while (elapseTime < Delay)
                {
                    ParticleObj.rectTransform.localScale = Vector2.Lerp(Vector2.zero, Vector2.one, elapseTime / Delay);

                    // Fade alpha from 0 to 1
                    currentColor.a = Mathf.Lerp(0f, 1f, elapseTime / Delay);
                    ParticleObj.color = currentColor;
                    yield return customTime;
                    elapseTime += customTime;
                }
                ParticleObj.gameObject.SetActive(false);
            }
        }

    }
}
