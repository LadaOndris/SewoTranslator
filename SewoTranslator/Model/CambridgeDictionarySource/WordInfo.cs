using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CambridgeDictionary
{
    public class WordInfo
    {
        public static readonly WordInfo None = new WordInfo(string.Empty, string.Empty, string.Empty, string.Empty, new string[] { });

        public WordInfo(string word, string wordClass, string guideWord, string meaning, string[] examples)
        {
            Word = word;
            WordClass = wordClass;
            GuideWord = guideWord;
            Meaning = meaning;
            Examples = examples;
        }

        public string Word { get; }
        public string WordClass { get; }
        public string GuideWord { get; }
        public string Meaning { get; }
        public string[] Examples { get; }
    }
}
