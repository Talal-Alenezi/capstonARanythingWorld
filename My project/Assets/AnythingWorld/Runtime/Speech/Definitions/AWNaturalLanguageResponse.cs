using System;
using System.Collections.Generic;


namespace AnythingWorld.Speech
{
    [Serializable]
    public class AWNaturalLanguageResponse
    {
        #region Fields
        public string text;
        public string query;
        public AWQueryResult queryResult;
        public List<AWThingResponseParams> @params;
        #endregion


        public List<string> ReturnCreatureParams()
        {
            var animalStrings = new List<string>();
            animalStrings.Add(queryResult.parameters.fields.generic_creature_1.stringValue);
            animalStrings.Add(queryResult.parameters.fields.generic_creature_2.stringValue);
            return animalStrings;

        }

    }
}
