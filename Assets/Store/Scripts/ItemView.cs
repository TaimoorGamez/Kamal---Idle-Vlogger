using UnityEngine;
using DG.Tweening;
using Core.Screen;
using UnityEngine.UI;

namespace Core.Store
{
    public class ItemView : MonoBehaviour
    {
        [SerializeField] protected StorageRoomView MyStorageRoom;
        [SerializeField] protected Image SelectionObj;
        [SerializeField] protected Color32 SelectColor, UnselectColor;
        [SerializeField] protected int ItemIndex;

        [SerializeField] Color32 SelectedImg, ActiveImg;

        Color32 _unSelectImg = Color.white;
        float _tweenTime = 0.25f;

        public virtual void UnSelectItem()
        {
            SelectionObj.DOColor(_unSelectImg, _tweenTime);
        }

        public virtual void SelectItem()
        {
            SelectionObj.DOColor(SelectedImg, _tweenTime);
        }

        public virtual void ActiveSelectItem()
        {
            SelectionObj.DOColor(ActiveImg, _tweenTime);
        }
    }
}
