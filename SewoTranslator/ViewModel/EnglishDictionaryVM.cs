using CambridgeDictionary;
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
    internal sealed class EnglishDictionaryVM : ViewModelBase
    {
        private WordInfo _currentWordInfo;
        private ICommand _previousWordInfoCommand;
        private ICommand _nextWordInfoCommand;
        private ICommand _hideCommand;

        private int currentWordInfoIndex = 0;

        public EnglishDictionaryVM(SavedWord word)
        {
            this.Word = word;
            UpdateWordInfo();
        }

        public SavedWord Word { get; }

        public WordInfo WordInfo
        {
            get => _currentWordInfo;
            private set
            {
                _currentWordInfo = value;
                OnPropertyChanged(nameof(WordInfo));
            }
        }

        public int WordInfosCount => Word.WordInfos.Length;

        public int CurrentPage => currentWordInfoIndex + 1;

        public ICommand PreviousWordInfoCommand
        {
            get => _previousWordInfoCommand ?? (_previousWordInfoCommand = new RelayCommand(SetPreviousWordInfo));
        }

        public ICommand NextWordInfoCommand
        {
            get => _nextWordInfoCommand ?? (_nextWordInfoCommand = new RelayCommand(SetNextWordInfo));
        }

        public ICommand HideCommand
        {
            get => _hideCommand ?? (_hideCommand = new RelayCommand<IClosable>(CloseWindow));
        }

        private void SetPreviousWordInfo()
        {
            if (currentWordInfoIndex == 0)
                currentWordInfoIndex = Word.WordInfos.Length - 1;
            else
                currentWordInfoIndex--;
            UpdateWordInfo();
        }

        private void SetNextWordInfo()
        {
            if (currentWordInfoIndex == Word.WordInfos.Length - 1)
                currentWordInfoIndex = 0;
            else
                currentWordInfoIndex++;
            UpdateWordInfo();
        }

        private void UpdateWordInfo()
        {
            WordInfo = Word.WordInfos[currentWordInfoIndex];
            OnPropertyChanged(nameof(CurrentPage));
        }

        private void CloseWindow(IClosable closable)
        {
            closable.Close();
        }

    }
}