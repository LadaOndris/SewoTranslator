using CambridgeDictionary;
using Newtonsoft.Json;
using SewoTranslator.Command;
using SewoTranslator.Model;
using SewoTranslator.Properties;
using SewoTranslator.View;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace SewoTranslator.ViewModel
{
    public sealed class MainVM : ViewModelBase
    {
        private UnfoundWordsManager uwm = new UnfoundWordsManager();
        private SavedWordsHandler savedWordsHandler;
        private FileReader reader = new FileReader();
        private List<Word> enWords;
        private List<Word> czWords;
        private List<Word> words;
        private Word selectedWord;
        private ICommand changeLang;
        private ICommand clearText;
        private ICommand hide;
        private ICommand showCurrentWordInCambridgeDictCommand;
        private ICommand showInCambridgeDict;
        private ICommand saveWord;
        private ICommand removeWord;
        private Language language; 
        private string text;
        private bool isWindowVisible = true;

        public MainVM(SavedWordsHandler savedWordsHandler)
        {
            this.savedWordsHandler = savedWordsHandler;
            enWords = reader.ReadAll(Language.English);
            Words = enWords;
            czWords = reader.ReadAll(Language.Czech);
            Language = Language.English;
        }

        public event Action<object> Scroll;
        public event Action TextBoxFocused;

        private bool IsTextBoxFocused
        {
            set
            {
                if (value == true)
                    TextBoxFocused?.Invoke();
            }
        }
        private Language Language
        {
            get { return language; }
            set
            {
                language = value;
                OnPropertyChanged(nameof(Lang));
            }
        }
        public ICommand ChangeLang
        {
            get
            {
                if (changeLang == null)
                {
                    changeLang = new RelayCommand(ReverseLanguage);
                }
                return changeLang;
            }
        }
        public ICommand ClearText
        {
            get
            {
                if (clearText == null)
                {
                    clearText = new RelayCommand(DeleteText);
                }
                return clearText;
            }
        }
        public ICommand Hide
        {
            get
            {
                if (hide == null)
                {
                    hide = new RelayCommand(HideWindow);
                }
                return hide;

            }
        }
        public ICommand ShowCurrentWordInCambridgeDictCommand
        {
            get => showCurrentWordInCambridgeDictCommand ?? 
                (showCurrentWordInCambridgeDictCommand = new RelayCommand(ShowCurrentWordInDictionary));
        }

        public ICommand ShowInCambridgeDictCommand
        {
            get => showInCambridgeDict ?? (showInCambridgeDict = new RelayCommand<Word>(ShowEnglishDictionary));
        }
        public ICommand SaveWordCommand
        {
            get => saveWord ?? (saveWord = new RelayCommand<Word>(savedWordsHandler.SaveWord, savedWordsHandler.CanSaveWord));
        }
        
        public ICommand RemoveWordCommand
        {
            get => removeWord ?? (removeWord = new RelayCommand<Word>(savedWordsHandler.RemoveFromSaved, savedWordsHandler.CanRemoveWord));
        }

        public string Lang
        {
            get { return language == Language.English ? "EN" : "CZ"; }
        }
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                OnPropertyChanged(nameof(Text));

                ScrollToWord(value, true);
            }
        }
        public bool IsWindowVisible
        {
            get { return isWindowVisible; }
            set
            {
                isWindowVisible = value;
                OnPropertyChanged(nameof(IsWindowVisible));
                if (value == true)
                    IsTextBoxFocused = true;
            }
        }
        public List<Word> Words
        {
            get { return words; }
            set
            {
                words = value;
                OnPropertyChanged(nameof(Words));
            }
        }

        public SavedWordsHandler SavedWordsHandler { get { return savedWordsHandler; } }


        private void ShowCurrentWordInDictionary()
        {
            ShowEnglishDictionary(selectedWord);
        }

        private void ShowEnglishDictionary(Word word)
        {
            if (language == Language.English)
            {
                SavedWord savedWord = savedWordsHandler.GetSavedWordVM(word);

                EnglishDictionaryVM vm = new EnglishDictionaryVM(savedWord);
                EnglishDictionaryWindow window = new EnglishDictionaryWindow();
                window.DataContext = vm;
                window.ShowDialog();
            }
        }


        private void ReverseLanguage()
        {
            Words = language == Language.English ? czWords : enWords;
            Language = language == Language.English ? Language.Czech : Language.English;

            ScrollToWord(Text, false);
        }

        private void ChangeLanguage(Language lang)
        {
            Words = lang == Language.English ? enWords : czWords;
            Language = lang == Language.English ? Language.English : Language.Czech;
        }

        private void DeleteText()
        {
            SaveIfNotFound(Text);
            Text = string.Empty;
            IsTextBoxFocused = true;
        }

        private void HideWindow()
        {
            SaveIfNotFound(Text);
            Text = string.Empty;
            IsWindowVisible = false;
        }
        
        private void ScrollToWord(string word, bool canLangChange)
        {
            word = word.RemoveDiacritics();

            if (!string.IsNullOrWhiteSpace(word))
            {
                int length1, length2;
                int k1 = Find(enWords, word, out length1);
                int k2 = Find(czWords, word, out length2);

                if (canLangChange)
                {
                    if (length1 < length2)
                    {
                        if (k2 != 0)
                        {
                            ChangeLanguage(Language.Czech);
                            selectedWord = words[k2];
                        }
                    }
                    else
                    {
                        if (k1 != 0)
                        {
                            ChangeLanguage(Language.English);
                            selectedWord = words[k1];
                        }
                    }
                }
                else
                {
                    int k = Language == Language.English ? k1 : k2;
                    if (k != 0)
                    {
                        selectedWord = words[k];
                    }
                }
            }
            else
            {
                selectedWord = words[0];
            }
            ScrollToWord(selectedWord);
        }

        private void ScrollToWord(Word word)
        {
            ScrollIntoView(words[words.Count - 1]);
            ScrollIntoView(word);
        }

        private int Find(List<Word> words, string word, out int length)
        {
            int k = 0;
            length = 0;

            for (int i = 0; i < word.Length; i++)
            {
                string w = word.Substring(0, i + 1);
                bool found = false;
                for (int j = k; j < words.Count; j++)
                {
                    if (words[j].CleanedOriginal.StartsWith(w, StringComparison.InvariantCultureIgnoreCase))
                    {
                        k = j;
                        length = w.Length;
                        found = true;
                        break;
                    }
                }
                if (!found) break;
            }
            return k;
        }

        private void SaveIfNotFound(string wordStr)
        {
            if (string.IsNullOrEmpty(wordStr))
                return;

            if (Settings.Default.HandleUnfoundWords == true)
            {
                Word word = new Word(wordStr);

                if (!czWords.Contains(word) && !enWords.Contains(word))
                {
                    if (uwm.GetLastWord() != wordStr)
                    {
                        uwm.SaveWord(wordStr);
                    }
                }
            }
        }

        private void ScrollIntoView(Word word)
        {
            Scroll?.Invoke(word);
        }

        public void HandleInput()
        {
            string text = null;

            try
            {
                text = Clipboard.GetText().Trim();
            }
            catch 
            {
                MessageBox.Show("Could not copy text to the clipboard.");
            }

            if (!string.IsNullOrEmpty(text))
            {
                IsWindowVisible = true;

                Text = text;

                SaveIfNotFound(text);
            }
        }

        public void ReverseVisibility()
        {
            if (!string.IsNullOrEmpty(Text))
            {
                SaveIfNotFound(Text);
                Text = string.Empty;
            }
            IsWindowVisible = !IsWindowVisible;
        }
    }
}
