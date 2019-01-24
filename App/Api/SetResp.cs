using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Artifactor.App.Api
{
    public class SetResp
    {
        [JsonProperty("url")]
        public string Url;
        
        [JsonProperty("cdn_root")]
        public string CdnRoot;
        
        [JsonProperty("expire_time")]
        public int ExpireTime;
    }
}