using SewoTranslator.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SewoTranslator.Model
{
    public sealed class WordRemovedEventArgs : EventArgs
    {
        public WordRemovedEventArgs(SavedWord savedWord)
        {
            SavedWordVM = savedWord;
        }

        public SavedWord SavedWordVM { get; }
    }
}
