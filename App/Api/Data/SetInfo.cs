using Godot.Collections;
using Newtonsoft.Json;

namespace Artifactor.App.Api.Data
{
    public class SetInfo
    {
        [JsonProperty("set_id")]
        public int SetId;
        
        [JsonProperty("name")]
        public Dictionary<string, string> Name = new Dictionary<string, string>();
    }
}