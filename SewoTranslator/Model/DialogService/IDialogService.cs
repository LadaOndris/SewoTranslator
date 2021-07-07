using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SewoTranslator.Model.DialogService
{
    internal interface IDialogService
    {
        void Register<TViewModel, TView>()
            where TViewModel : IDialogRequestClose
            where TView : IDialog;
        bool ShowDialog<TViewModel>(TViewModel viewModel)
            where TViewModel : IDialogRequestClose;
    }
}
