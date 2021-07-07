using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SewoTranslator.Model
{
    public sealed class UnfoundWordsManager
    {
        string filePath = Path.Combine(Environment.GetFolderPath(
                     Environment.SpecialFolder.ApplicationData), @"SewoTranslator\unfound_words");
        string directoryPath = Path.Combine(Environment.GetFolderPath(
                     Environment.SpecialFolder.ApplicationData), "SewoTranslator");

        public void SaveWord(string word)
        {
            word += Environment.NewLine;

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            File.AppendAllText(filePath, word);
        }

        public string UnfoundWords()
        {
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
            return null;
        }

        public void SaveAll(string text)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            File.WriteAllText(filePath, text);
        }

        /// <summary>
        ///     Reads a file and returns the last written line. (the last word)
        ///     If no word exists, null is returned.
        /// </summary>
        /// <returns></returns>
        public string GetLastWord()
        {
            return File.ReadLines(filePath).LastOrDefault();
        }
    }
}
