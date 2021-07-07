using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace SewoTranslator.Model
{
    public sealed class FileReader
    {
        private Assembly assembly = Assembly.GetExecutingAssembly();

        private IEnumerable<string> GetFiles(string startwith)
        {
            foreach (string s in assembly.GetManifestResourceNames())
            {
                if (s.StartsWith(startwith) && s.EndsWith(".txt"))
                {
                    yield return s;
                }
            }
        }

        private IEnumerable<string> ReadLines(string file)
        {
            using (Stream stream = assembly.GetManifestResourceStream(file))
            using (BufferedStream bs = new BufferedStream(stream))
            using (StreamReader reader = new StreamReader(bs))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        public List<Word> ReadAll(Language lang)
        {
            var words = new List<Word>();
            string startswith = lang == Language.English ?
                "SewoTranslator.Res.raw.a" : "SewoTranslator.Res.raw.b";

            //Stopwatch sw = Stopwatch.StartNew();
            foreach (string file in GetFiles(startswith))
            {
                List<string> lines = new List<string>(ReadLines(file));

                for (int i = 0; i < lines.Count; i++)
                {
                    string[] line = lines[i].Split('%');
                    int enPos = line[0].IndexOf('{');
                    int usPos = line[0].IndexOf('}');

                    List<string> trans = new List<string>();
                    for (int j = 1; j < line.Length; j++) trans.Add(line[j]);

                    if (enPos == -1 && usPos == -1)
                    {
                        words.Add(new Word(line[0], trans));
                    }
                    else if (enPos == -1)
                    {
                        words.Add(new Word(line[0].Substring(0, usPos), trans, usPron: " [" + line[0].Substring(usPos + 1) + "]"));
                    }
                    else if (usPos == -1)
                    {
                        words.Add(new Word(line[0].Substring(0, enPos), trans, enPron: " [" + line[0].Substring(enPos + 1) + "]"));
                    }
                    else
                    {
                        words.Add(new Word(line[0].Substring(0, enPos), trans,
                              enPron: " [" + line[0].Substring(enPos + 1, usPos - enPos - 1) + "]",
                              usPron: " [" + line[0].Substring(usPos + 1) + "]"));
                    }
                }
            }
            //sw.Stop();
            //Console.WriteLine(sw.Elapsed.TotalMilliseconds);

            return new List<Word>(words.OrderBy(word => word.Original));
        }
    }
}
