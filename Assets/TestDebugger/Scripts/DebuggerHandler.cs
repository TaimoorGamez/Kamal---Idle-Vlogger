using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.TestDebugging
{
    public class DebuggerHandler : MonoBehaviour
    {
        [SerializeField] ScrollRect TextScrol;
        [SerializeField] RectTransform DebugParent;
        [SerializeField] TextMeshProUGUI DebuggerTextPrefab;
        [SerializeField] Button TestDebuggerBtn;
        [SerializeField] GameObject TestDebuggerPanel;

        void OnEnable()
        {
            Application.logMessageReceived += HandleLogs;

            TestDebuggerBtn.onClick.AddListener(TestDebuggerToggle);
        }

        void OnDisable()
        {
            Application.logMessageReceived -= HandleLogs;

            TestDebuggerBtn.onClick.RemoveListener(TestDebuggerToggle);
        }

        void HandleLogs(string logString, string stackTrace, LogType logtype)
        {
            TextMeshProUGUI newLogText = Instantiate(DebuggerTextPrefab, DebugParent);
            newLogText.text = logString;
            LayoutRebuilder.ForceRebuildLayoutImmediate(DebugParent);
            TextScrol.normalizedPosition = new Vector2(0, 0);
        }

        void TestDebuggerToggle()
        {
            TestDebuggerPanel.SetActive(!TestDebuggerPanel.activeSelf);
        }
    }
}
