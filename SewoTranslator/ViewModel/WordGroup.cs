using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SewoTranslator.ViewModel
{
    internal sealed class WordGroup : ViewModelBase
    {
        public WordGroup(DateTime date)
        {
            Date = date;
        }
        
        public DateTime Date { get; }

        public ObservableCollection<EnglishDictionaryVM> SavedWords { get; }
           = new ObservableCollection<EnglishDictionaryVM>();
    }
}
