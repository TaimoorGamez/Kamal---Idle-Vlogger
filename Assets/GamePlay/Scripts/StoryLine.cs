using TMPro;
using UnityEngine;

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

        protected int CurrentMsgIndex = 0;
    }
}
