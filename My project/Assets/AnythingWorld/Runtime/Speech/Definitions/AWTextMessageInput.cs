using System;
using AnythingWorld.Utilities;

namespace AnythingWorld.Speech
{
    [Serializable]
    public class AWTextMessageInput
    {
        public AWQueryInput queryInput;
        public AWDFAgent agentId;
        public string apiKey = AnythingSettings.ApiKey;
        public AWTextMessageInput(string message)
        {
            queryInput = new AWQueryInput
            {
                text = new AWTextInput
                {
                    text = message
                }
            };
            agentId = new AWDFAgent();
        }
    }
    // this is the req.body expected

    // {"queryInput":
    // 	{"text":
    // 		{"text":"what can you do",
    // 		"languageCode":"en-US"
    // 		}
    // 	},
    // 	"agentId": "agent id string"
    // }

}
