using System;
using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;

namespace Artifactor.App.Api
{
    public class Expiration
    {
        public int UnixTimestamp => _unixTimestamp;

        private int _unixTimestamp;
        private const string ExpirationPath = "user://expiration.json";

        public void Load()
        {
            var file = new File();
            if(!file.FileExists(ExpirationPath)) return;
            
            // load file
            var error = file.Open(ExpirationPath, (int) File.ModeFlags.Read);
            if(error == Error.Ok)
            {
                var data = JsonConvert.DeserializeObject<Dictionary<string, int>>(file.GetAsText());
                _unixTimestamp = data["expiration"];
            }
                
            file.Close();
        }

        public bool NeedsToBeUpdated()
        {
            Load();
            
            var epochTicks = new DateTime(1970, 1, 1).Ticks;
            var now = (DateTime.UtcNow.Ticks - epochTicks) / TimeSpan.TicksPerSecond;
            
            GD.Print($"{now} - {_unixTimestamp}");
            return now > _unixTimestamp;
        }

        public void Update(int expiration)
        {
            var file = new File();

            var error = file.Open(ExpirationPath, (int) File.ModeFlags.Write);
            if(error == Error.Ok)
            {
                file.StoreString(JsonConvert.SerializeObject(new Dictionary<string, int>
                {
                    {"expiration", expiration}
                }));

                _unixTimestamp = expiration;
            }
            
            file.Close();
        }
    }
}