using UnityEngine;

namespace SaintsField.Samples.Scripts.IssueAndTesting.Issue.Issue8
{
    public class OnValueChanged : OnValueChangedBase
    {
        [OnValueChanged(nameof(OnValueChangedCallback))]
        public int v;
    }
}
