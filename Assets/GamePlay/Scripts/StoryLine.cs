using TMPro;
using UnityEngine;
using Core.Events;

namespace Core.GamePlay
{
    public class StoryLine : MonoBehaviour
    {
        [SerializeField] protected CurtainController CurrentCurtainController;
        [SerializeField] protected GameObject MsgObj;
        [SerializeField] protected Transform MsgBubble;
        [SerializeField] protected TextMeshProUGUI MsgTxt, NameTxt;
        [SerializeField] protected string[] Messages, MessengerName;
        [SerializeField] protected float[] BubbleRotation;

        protected int _currentMsgIndex = 0;

        protected virtual void ShowMsg()
        {
            MsgTxt.text = Messages[_currentMsgIndex];
            NameTxt.text = MessengerName[_currentMsgIndex];
            MsgBubble.rotation = Quaternion.Euler(0, BubbleRotation[_currentMsgIndex], 0);
        }

        public virtual void NextMsg()
        {
            _currentMsgIndex++;
            if (_currentMsgIndex < Messages.Length)
            {
                ShowMsg();
            }
            else
            {
                MsgObj.SetActive(false);
                gameObject.SetActive(false);
                SimpleEventsHolder.StoryPartComplete?.Invoke();
            }
        }
    }
}
