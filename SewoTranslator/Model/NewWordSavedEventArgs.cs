using SewoTranslator.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SewoTranslator.Model
{
    public sealed class NewWordSavedEventArgs : EventArgs
    {
        public NewWordSavedEventArgs(SavedWord savedWord)
        {
            SavedWordVM = savedWord;
        }

        public SavedWord SavedWordVM { get; }
    }
}
