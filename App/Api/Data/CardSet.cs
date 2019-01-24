using System.Collections.Generic;
using Newtonsoft.Json;

namespace Artifactor.App.Api.Data
{
    public class CardSet
    {
        [JsonProperty("card_set")]
        public CardSetData Data;
    }
}