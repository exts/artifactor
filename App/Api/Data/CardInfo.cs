using System;
using System.Text.RegularExpressions;

namespace Artifactor.App.Api.Data
{
    public class CardInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }

        public string SavePath()
        {
            return $"{Api.CardSet.CachePath}/{Id}.png";
        }

        public string GetDescBBCode()
        {
            return Description.Trim(' ').Length <= 0 ? string.Empty : ParseDescBBCode();
        }
        
        private string ParseDescBBCode()
        {
            var desc = Regex.Replace(Description, @"<span[^>]+>(.*?)</span>", "[b]$1[/b]", RegexOptions.IgnoreCase);
            desc = desc.Replace("\n", "");
            desc = Regex.Replace(desc, @"(?:<br([^>]+)?>)+?", "<br>", RegexOptions.IgnoreCase);
            desc = Regex.Replace(desc, @"<br([^>]+)?>", "\n", RegexOptions.IgnoreCase);
            
            return desc;
        }
    }
}