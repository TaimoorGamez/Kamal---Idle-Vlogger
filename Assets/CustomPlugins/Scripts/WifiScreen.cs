using DG.Tweening;

namespace Core.Screen
{
    public class WifiScreen : UiScreens
    {
        private void OnEnable()
        {
            OnOpen();
        }

        public override void OnOpen()
        {
            Body.DOScale(1,_transitionDuration).SetEase(Ease.OutBack);
        }

        public override void OnClose()
        {
            Body.DOScale(0, _transitionDuration).SetEase(Ease.InBack).OnComplete(() => gameObject.SetActive(false));
        }
    }
}
