using UnityEngine;
using System.Collections;

namespace Core.ToastMsg
{
    public class ToastScreen : MonoBehaviour
    {
        [SerializeField] ToastManager CurrenToastManager;

        Coroutine _selfHideRotine;

        private void OnEnable()
        {
            _selfHideRotine = StartCoroutine(SelfDestruct());
        }

        IEnumerator SelfDestruct()
        {
            while (CurrenToastManager.HiddingDelay > 0)
            {
                CurrenToastManager.HiddingDelay -= Time.deltaTime;
                yield return null;
            }
            CurrenToastManager.OldMsgNum = -1;
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            if (_selfHideRotine != null)
            {
                StopCoroutine(_selfHideRotine);
                _selfHideRotine = null;
            }
        }
    }
}
