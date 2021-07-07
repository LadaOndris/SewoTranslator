using SewoTranslator.Command;
using SewoTranslator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SewoTranslator.ViewModel
{
    public sealed class UnfoundWordsVM : ViewModelBase
    {
        private ICommand close;
        private string unfoundWords;

        public UnfoundWordsVM(string unfoundWords)
        {
            UnfoundWords = unfoundWords;
        }

        public ICommand Close
        {
            get
            {
                if (close == null)
                {
                    close = new RelayCommand<IClosable>(CloseWindow);
                }
                return close;
            }
        }
        
        public string UnfoundWords
        {
            get
            {
                return unfoundWords;
            }
            set
            {
                unfoundWords = value;

                OnPropertyChanged("UnfoundWords");
            }
        }

        private void CloseWindow(IClosable window)
        {
            new UnfoundWordsManager().SaveAll(UnfoundWords);
            window.Close();
        }
    }
}
