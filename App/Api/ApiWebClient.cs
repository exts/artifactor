using System;
using System.Net;

namespace Artifactor.App.Api
{
    public class ApiWebClient : System.Net.WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            var req = base.GetWebRequest(address) as HttpWebRequest;
            req.AutomaticDecompression = DecompressionMethods.GZip;
            return req;
        }
    }
}