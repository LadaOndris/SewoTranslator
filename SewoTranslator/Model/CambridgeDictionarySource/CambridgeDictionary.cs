using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CambridgeDictionary
{
    internal sealed class CambridgeDictionaryRequester : IRequester
    {
        private readonly string webAddress = "http://dictionary.cambridge.org/dictionary/english/";

        
        // TO DO: 
        // format word to satisfy url format
        public WordInfo[] RequestWordInfos(string word = "perspective")
        {
            List<WordInfo> wordInfos = new List<WordInfo>();
            
            string address = webAddress + word;
            string html = LoadHtml(address);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            var entries = doc.DocumentNode.SelectNodes("//div").Where(node => node.HasClass("entry-body__el"));

            foreach (HtmlNode entry in entries)
            {
                string wordClass = LoadWordClass(entry);

                HtmlNodeCollection senseBlocks = entry.SelectNodes(".//div[contains(@class,'pos-body')]/div[contains(@class,'dsense')]");
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
                        WordInfo wordInfo = new WordInfo(word, wordClass, guideWord, definition, examples);
                        wordInfos.Add(wordInfo);
                    }
                }
            }
            
            return wordInfos.ToArray();
        }

        private string LoadHtml(string address)
        {
           ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using (WebClient client = new WebClient())
            {
                var uri = new Uri(address, UriKind.Absolute);
                try
                {
                    return client.DownloadString(uri);
                }
                catch (WebException e)
                {
                    return string.Empty;
                }
            }
        }

        private string LoadWordClass(HtmlNode entry)
        {
            if (!entry.HasClass("entry-body__el"))
                throw new ArgumentException("Given HtmlNode is not an entry. Make sure the HtmlNode has a class 'entry-body__el'");
            HtmlNode posHeader = entry.SelectSingleNode(".//div[contains(@class,'pos-header')]");
            HtmlNode wordClass = posHeader.SelectSingleNode("//div[contains(@class, 'posgram')]");
            
            return ReturnInnerTextOrEmptyString(wordClass);
        }

        private string LoadGuideWord(HtmlNode senseBlock)
        {
            if (!senseBlock.HasClass("dsense"))
                throw new ArgumentException("Given HtmlNode is not a senseblock. Make sure the HtmlNode has a class 'dsense'");
            HtmlNode guideWord = senseBlock.SelectSingleNode(".//span[contains(@class,'guideword')]/span");
            return ReturnInnerTextOrEmptyString(guideWord);
        }

        private string LoadDefinition(HtmlNode defBlock)
        {
            if (!defBlock.HasClass("def-block"))
                throw new ArgumentException("Given HtmlNode is not a defBlock. Make sure the HtmlNode has a class 'def-block'");
            HtmlNode definition = defBlock.SelectSingleNode(".//div[contains(@class, 'ddef_h')]/div[contains(@class, 'ddef_d')]");
            return ReturnInnerTextOrEmptyString(definition);
        }

        private string[] LoadExamples(HtmlNode defBlock)
        {
            if (!defBlock.HasClass("def-block"))
                throw new ArgumentException("Given HtmlNode is not a defBlock. Make sure the HtmlNode has a class 'def-block'");
            HtmlNodeCollection examplesCollection = defBlock.SelectNodes(".//div[contains(@class,'def-body')]/div[contains(@class,'examp')]");
            if (examplesCollection == null)
                return new string[] { };

            string[] examples = new string[examplesCollection.Count];
            for (int i = 0; i < examplesCollection.Count; i++)
            {
                examples[i] = ((HtmlNode)examplesCollection[i]).InnerText.Trim();
            }
            return examples;
        }

        private string[] LoadExtraExamples(HtmlNode senseBlock)
        {
            if (!senseBlock.HasClass("dsense"))
                throw new ArgumentException("Given HtmlNode is not a senseblock. Make sure the HtmlNode has a class 'dsense'");

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
