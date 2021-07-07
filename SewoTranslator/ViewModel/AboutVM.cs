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
    public sealed class AboutVM : ViewModelBase
    {
        private ICommand close;

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

        private void CloseWindow(IClosable window)
        {
            window.Close();
        }
    }
}
