using System;

namespace AnythingWorld.Speech
{
    [Serializable]
    public class AWQueryResult
    {
        public string action = "none";
        public AWParameterResult parameters;
    }
}
