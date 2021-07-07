using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CambridgeDictionary
{
    internal sealed class CambridgeRequester : IRequester
    {
        private readonly string webAddress = "https://dictionary.cambridge.org/dictionary/english/";

        
        // TO DO: 
        // format word to satisfy url format
        public WordInfo[] RequestWordInfos(string word = "perspective")
        {
            List<WordInfo> wordInfos = new List<WordInfo>();

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(webAddress + word);

            HtmlNode[] senseBlocks = doc.DocumentNode.SelectNodes("//div[contains(@class,'sense-block')]").ToArray();
            if (senseBlocks == null)
                return wordInfos.ToArray();

            foreach (HtmlNode senseBlock in senseBlocks)
            {
                HtmlNode senseBody = senseBlock.SelectSingleNode(".//div[contains(@class,'sense-body')]");
                if (senseBody == null)
                    continue;
                IEnumerable<HtmlNode> defBlocks = senseBody.ChildNodes.Where(child => child.HasClass("def-block"));
                if (defBlocks == null)
                    continue;

                string guideWord = LoadGuideWord(senseBlock);
                string[] extraExamples = LoadExtraExamples(senseBlock);

                bool firstElement = true;
                foreach (HtmlNode defBlock in defBlocks)
                {
                    string definition = LoadDefinition(defBlock);
                    string[] examples = LoadExamples(defBlock);
                    if (firstElement)
                    {
                        examples.Concat(extraExamples);
                        firstElement = false;
                    }
                    WordInfo wordInfo = new WordInfo(word, string.Empty, guideWord, definition, examples);
                    wordInfos.Add(wordInfo);
                }
            }
            return wordInfos.ToArray();
        }

        private string LoadGuideWord(HtmlNode senseBlock)
        {
            if (!senseBlock.HasClass("sense-block"))
                throw new ArgumentException("Given HtmlNode is not a senseblock. Make sure the HtmlNode has a class 'sense-block'");
            HtmlNode guideWord = senseBlock.SelectSingleNode(".//span[contains(@class,'guideword')]/span");
            return ReturnInnerTextOrEmptyString(guideWord);
        }

        private string LoadDefinition(HtmlNode defBlock)
        {
            if (!defBlock.HasClass("def-block"))
                throw new ArgumentException("Given HtmlNode is not a defBlock. Make sure the HtmlNode has a class 'def-block'");
            HtmlNode definition = defBlock.SelectSingleNode(".//p[contains(@class,'def-head')]/b[contains(@class,'def')]");
            return ReturnInnerTextOrEmptyString(definition);
        }

        private string[] LoadExamples(HtmlNode defBlock)
        {
            if (!defBlock.HasClass("def-block"))
                throw new ArgumentException("Given HtmlNode is not a defBlock. Make sure the HtmlNode has a class 'def-block'");
            HtmlNodeCollection examplesCollection = defBlock.SelectNodes(".//span[contains(@class,'def-body')]/div[contains(@class,'examp')]");
            if (examplesCollection == null)
                return new string[] { };

            string[] examples = new string[examplesCollection.Count];
            for (int i = 0; i < examplesCollection.Count; i++)
            {
                examples[i] = ((HtmlNode)examplesCollection[i]).InnerText;
            }
            return examples;
        }

        private string[] LoadExtraExamples(HtmlNode senseBlock)
        {
            if (!senseBlock.HasClass("sense-block"))
                throw new ArgumentException("Given HtmlNode is not a senseblock. Make sure the HtmlNode has a class 'sense-block'");

            HtmlNodeCollection examplesCollection = senseBlock.SelectNodes(".//div[contains(@class,'extraexamps')]/ul/li[contains(@class,'eg')]");
            if (examplesCollection == null)
                return new string[] { };

            string[] examples = new string[examplesCollection.Count];
            for (int i = 0; i < examplesCollection.Count; i++)
            {
                examples[i] = ((HtmlNode)examplesCollection[i]).InnerText;
            }
            return examples;
        }
        
        private string ReturnInnerTextOrEmptyString(HtmlNode html)
        {
            if (html == null)
                return string.Empty;
            return html.InnerText;
        }
    }
}
