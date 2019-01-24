using System.Collections.Generic;
using Newtonsoft.Json;

namespace Artifactor.App.Api.Data
{
    public class CardList
    {
        [JsonProperty("card_id")]
        public int CardId;

        [JsonProperty("card_type")]
        public string CardType;

        [JsonProperty("card_name")]
        public Dictionary<string, string> CardName = new Dictionary<string, string>();

        [JsonProperty("card_text")]
        public Dictionary<string, string> CardText = new Dictionary<string, string>();

        [JsonProperty("large_image")]
        public Dictionary<string, string> Images = new Dictionary<string, string>();
    }
}