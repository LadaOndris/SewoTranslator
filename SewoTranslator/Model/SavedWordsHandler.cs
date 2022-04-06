using CambridgeDictionary;
using Newtonsoft.Json;
using SewoTranslator.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SewoTranslator.Model
{
    public sealed class SavedWordsHandler
    {
        private readonly string savedWordsFilePath;
        private List<SavedWord> savedWords;

        public SavedWordsHandler(string savedWordsFilePath)
        {
            this.savedWordsFilePath = savedWordsFilePath;
        }

        public event EventHandler<NewWordSavedEventArgs> NewWordSaved;
        public event EventHandler<WordRemovedEventArgs> WordRemoved;

        public List<SavedWord> SavedWords
        {
            get => savedWords;
        }

        public WordInfo[] RequestWordInfos(string word)
        {
            var dict = new CambridgeDictionaryRequester();
            var wordInfos = dict.RequestWordInfos(word);
            if (wordInfos.Length == 0)
                wordInfos = new WordInfo[] { WordInfo.None };
            return wordInfos;
        }

        public void SaveWord(Word word)
        {
            var savedWordVM = GetSavedWordVM(word);
            savedWords.Add(savedWordVM);
            SaveWordInfosToFile(savedWords);
            NewWordSaved?.Invoke(this, new NewWordSavedEventArgs(savedWordVM));
        }

        public bool CanSaveWord(Word word)
        {
            if (word == null)
                return false;
            foreach (var savedWord in savedWords)
            {
                if (string.Equals(savedWord.Word, word.Original, StringComparison.Ordinal))
                    return false;
            }
            return true;
        }

        public void RemoveFromSaved(Word word)
        {
            SavedWord removed = null;
            foreach (SavedWord savedWord in savedWords)
            {
                if (string.Equals(savedWord.Word, word.Original))
                {
                    removed = savedWord;
                    savedWords.Remove(savedWord);
                    break;
                }
            }
            if (removed != null)
            {
                SaveWordInfosToFile(savedWords);
                WordRemoved?.Invoke(this, new WordRemovedEventArgs(removed));
            }
        }

        public bool CanRemoveWord(Word word)
        {
            if (word == null)
                return false;
            foreach (var savedWord in savedWords)
            {
                if (string.Equals(savedWord.Word, word.Original, StringComparison.Ordinal))
                    return true;
            }
            return false;
        }

        public void LoadSavedWordInfosFromFile()
        {
            if (!File.Exists(savedWordsFilePath))
                savedWords = new List<SavedWord>();
            try
            {
                string json = File.ReadAllText(savedWordsFilePath);
                var listOfWordInfos = JsonConvert.DeserializeObject<List<SavedWord>>(json);
                savedWords = listOfWordInfos;
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to read saved words.");
                savedWords = new List<SavedWord>();
            }
        }

        private void SaveWordInfosToFile(List<SavedWord> wordInfos)
        {
            try
            {
                string json = JsonConvert.SerializeObject(wordInfos);
                File.WriteAllText(savedWordsFilePath, json);
            }
            catch (Exception e)
            {
                MessageBox.Show("Could not save words.");
            }
        }

        public SavedWord GetSavedWordVM(Word word)
        {
            WordInfo[] wordInfos = RequestWordInfos(word.Original);
            return new SavedWord(word.Original, wordInfos, word.Translations, DateTime.Now.Date);
        }
        
    }
}
