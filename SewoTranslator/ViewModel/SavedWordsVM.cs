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
        
        public ObservableCollection<WordGroup> WordGroups { get; private set; }
            = new ObservableCollection<WordGroup>();

        private void LoadSavedWords()
        {
            savedWordsHandler.LoadSavedWordInfosFromFile();
            List<WordGroup> groups = GroupSavedWords();

            WordGroups = new ObservableCollection<WordGroup>(groups);
        }

        private List<WordGroup> GroupSavedWords()
        {
            var groups = new SortedDictionary<DateTime, WordGroup>(new DateComparer());

            foreach (SavedWord savedWord in savedWordsHandler.SavedWords)
            {
                if (!groups.ContainsKey(savedWord.DateSaved))
                {
                    groups.Add(savedWord.DateSaved, new WordGroup(savedWord.DateSaved));
                }
                groups[savedWord.DateSaved].SavedWords.Add(new EnglishDictionaryVM(savedWord));
            }

            return groups.Values.ToList();
        }
    }

    internal sealed class DateComparer : IComparer<DateTime>
    {
        public int Compare(DateTime x, DateTime y)
        {
            return DateTime.Compare(y, x);
        }
    }
}
