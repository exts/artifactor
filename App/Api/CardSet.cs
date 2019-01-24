using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Artifactor.App.Api.Data;
using Artifactor.App.Scripts;
using Godot;
using Newtonsoft.Json;
using Directory = Godot.Directory;
using File = Godot.File;

namespace Artifactor.App.Api
{
    public class CardSet
    {
        public const string CachePath = "user://cache";
        public const string CardListPath = "user://cardlist.json";
        
        private const string ApiUrl = "https://playartifact.com/cardset";
        private List<string> _setIds = new List<string>
        {
            "00", "01"
        };

        public List<string> GetSetUrls()
        {
            var setIds = new List<string>();
            foreach(var setId in _setIds)
            {
                setIds.Add($"{ApiUrl}/{setId}/");
            }

            return setIds;
        }

        public static string FormatApiUrl(SetResp setResp)
        {
            var uri = new Uri(setResp.CdnRoot);
            var urlFull = new Uri(uri, setResp.Url);

            return urlFull.AbsoluteUri;
        }

        public async Task<List<SetResp>> FetchSetApiUrl()
        {
            var setUrls = GetSetUrls();
            var setResp = new List<SetResp>();
            
            foreach(var setUrl in setUrls)
            {
                // force stop
                if(DataSyncScene.ForceStop) break;
                
                using(var client = new ApiWebClient())
                {
                    var response = await client.DownloadStringTaskAsync(setUrl);
                    setResp.Add(JsonConvert.DeserializeObject<SetResp>(response));
                }
            }

            return setResp;
        }

        public static async Task<Data.CardSet> FetchCardSetData(string url)
        {
            using(var client = new ApiWebClient())
            {
                var response = await client.DownloadStringTaskAsync(url);
                return JsonConvert.DeserializeObject<Data.CardSet>(response);
            }
        }

        public static async Task DownloadCardImages(List<CardInfo> cards, ProgressBar progressBar)
        {
            foreach(var card in cards)
            {
                // force stop
                if(DataSyncScene.ForceStop) break;
                
                using(var client = new ApiWebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.2; WOW64)   AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.97 Safari/537.11 AlexaToolbar/alxg-3.1");
                    var uri = new Uri(card.Image);
                    var data = await client.DownloadDataTaskAsync(uri);
                    StoreCardImage(card, data);
                    
                    // update progress bar
                    progressBar.Value += 1;
                }
            }
        }

        public static void StoreCardData(List<CardInfo> cards)
        {
            if(cards.Count <= 0) return;

            var json = JsonConvert.SerializeObject(cards);
            
            using(var file = new File())
            {
                using(var dir = new Directory())
                {
                    // delete save file if it exists
                    if(file.FileExists(CardListPath))
                    {
                        dir.Remove(CardListPath);
                    }
                }
                
                // attempt to save file
                var error = file.Open(CardListPath, (int) File.ModeFlags.Write);
                if(error != Error.Ok)
                {
                    file.Close();
                    throw new Exception($"There was a problem trying to save {CardListPath}");
                }
                
                file.StoreString(json);
                file.Close();
            }
        }
        
        public static void StoreCardImage(CardInfo card, byte[] data)
        {
            using(var file = new File())
            {
                using(var dir = new Directory())
                {
                    // delete save file if it exists
                    if(file.FileExists(card.SavePath()))
                    {
                        dir.Remove(card.SavePath());
                    }
                    
                    // create directory if it doesn't exist
                    if(!dir.DirExists(CachePath))
                    {
                        dir.MakeDir(CachePath);
                    }
                }
                
                // attempt to save file
                var error = file.Open(card.SavePath(), (int) File.ModeFlags.Write);
                if(error == Error.Ok)
                {
                    file.StoreBuffer(data);
                }
                else
                {
                    GD.Print($"Error {(int)error} - Problem opening save path");
                }
                
                file.Close();
            }
        }
    }
}