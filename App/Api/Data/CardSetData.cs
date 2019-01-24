using System.Collections.Generic;
using Newtonsoft.Json;

namespace Artifactor.App.Api.Data
{
    public class CardSetData
    {
        [JsonProperty("version")]
        public int Version;

        [JsonProperty("set_info")]
        public SetInfo SetInfo;

        [JsonProperty("card_list")]
        public List<CardList> CardList;
    }
}