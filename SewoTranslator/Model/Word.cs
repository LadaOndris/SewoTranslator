using SewoTranslator.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SewoTranslator.Model
{
    public class Word : ViewModelBase, IEquatable<Word>
    {
        private string _original;
        private string _enPron;
        private string _usPron;
        private List<string> _translations;

        public Word(string original,
                    List<string> translations = default(List<string>),
                    string enPron = default(string),
                    string usPron = default(string))
        {
            Original = original;
            Translations = translations;
            EnPron = enPron;
            UsPron = usPron;

            CleanedOriginal = original.RemoveDiacritics();
        }

        public string Original
        {
            get { return _original; }
            set
            {
                _original = value;
                OnPropertyChanged("Original");
            }
        }
        public string CleanedOriginal { get; private set; }
        public string EnPron
        {
            get { return _enPron; }
            set
            {
                _enPron = value;
                OnPropertyChanged("EnPron");
            }
        }
        public string UsPron
        {
            get { return _usPron; }
            set
            {
                _usPron = value;
                OnPropertyChanged("UsPron");
            }
        }
        public List<string> Translations
        {
            get { return _translations; }
            set
            {
                _translations = value;
                OnPropertyChanged("Translations");
            }
        }
        
        public bool Equals(Word other)
        {
            if (other == null) return false;
            return _original.Equals(other.Original, StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            return _original;
        }
    }
}
