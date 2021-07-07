using CambridgeDictionary;
using Newtonsoft.Json;
using SewoTranslator.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SewoTranslator.ViewModel
{
    internal sealed class SavedWordsVM : ViewModelBase
    {
        private readonly SavedWordsHandler savedWordsHandler;
    
        public SavedWordsVM(SavedWordsHandler savedWordsHandler)
        {
            this.savedWordsHandler = savedWordsHandler;
            LoadSavedWords();
        }
        
        public ObservableCollection<EnglishDictionaryVM> SavedWords { get; } 
            = new ObservableCollection<EnglishDictionaryVM>();

        private void LoadSavedWords()
        {
            savedWordsHandler.LoadSavedWordInfosFromFile();

            foreach (SavedWord savedWordVM in savedWordsHandler.SavedWords)
            {
                SavedWords.Add(new EnglishDictionaryVM(savedWordVM));
            }
        }
        
    }
}
