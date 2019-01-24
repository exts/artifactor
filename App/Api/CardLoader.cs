using System.Collections.Generic;
using Artifactor.App.Api.Data;
using Godot;
using Newtonsoft.Json;

namespace Artifactor.App.Api
{
    public class CardLoader
    {
        public List<CardInfo> Cards => _cards;
        private List<CardInfo> _cards = new List<CardInfo>();
        
        public void LoadCardList()
        {
            var file = new File();
            if(!file.FileExists(CardSet.CardListPath)) return;
            
            // load file
            var error = file.Open(CardSet.CardListPath, (int) File.ModeFlags.Read);
            if(error == Error.Ok)
            {
                _cards = JsonConvert.DeserializeObject<List<CardInfo>>(file.GetAsText());
            }
                
            file.Close();
        }

        public Sprite LoadCardImage(string path)
        {
            var texture = new ImageTexture();
            texture.Load(path);
            
            var sprite = new Sprite();
            sprite.SetTexture(texture);
            sprite.Scale = new Vector2(0.4f, 0.4f);
            sprite.Centered = false;
            sprite.Position = new Vector2(90, 220);

            return sprite;
        }
    }
}