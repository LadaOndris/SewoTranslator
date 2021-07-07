using SewoTranslator.Model;
using SewoTranslator.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SewoTranslator.View
{
    /// <summary>
    /// Interaction logic for EnglishDictionaryWindow.xaml
    /// </summary>
    public partial class EnglishDictionaryWindow : Window, IClosable
    {
        public EnglishDictionaryWindow()
        {
            InitializeComponent();
        }


        private new void MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void WindowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
            else if (e.Key == Key.Right)
                ExecuteNextTranslationCommand();
            else if (e.Key == Key.Left)
                ExecutePreviousTranslationCommand();
        }

        private void ExecuteNextTranslationCommand()
        {
            var vm = (EnglishDictionaryVM)this.DataContext;
            vm.NextWordInfoCommand.Execute(null);
        }

        private void ExecutePreviousTranslationCommand()
        {
            var vm = (EnglishDictionaryVM)this.DataContext;
            vm.PreviousWordInfoCommand.Execute(null);
        }

    }
}
