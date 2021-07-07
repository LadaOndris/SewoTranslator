using CambridgeDictionary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SewoTranslator.Model
{
    [JsonObject]
    public sealed class SavedWord
    {
        public SavedWord(string word, WordInfo[] wordInfos, List<string> translations)
        {
            Word = word;
            WordInfos = wordInfos;
            Translations = translations;
        }
        
        [JsonProperty]
        public String Word { get; }

        [JsonProperty]
        public WordInfo[] WordInfos { get; }

        [JsonProperty]
        public List<string> Translations { get; }

        public string TranslationsToString()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < Translations.Count; i++)
            {
                sb.Append(Translations[i]);
                if (i != Translations.Count - 1)
                    sb.Append(", ");
            }
            return sb.ToString();
        }
    }
}
