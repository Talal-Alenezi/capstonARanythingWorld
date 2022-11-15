using System;
using AnythingWorld.Utilities;


namespace AnythingWorld.Speech
{
    [Serializable]
    public class AWDFAgent
    {
        public string stringValue = AnythingSettings.Instance.dialogFlowAgentId;
    }
}
