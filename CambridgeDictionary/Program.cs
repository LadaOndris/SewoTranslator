using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CambridgeDictionary
{
    class Program
    {
        static void Main(string[] args)
        {
            var cambridgeDictionary = new CambridgeRequester();
            cambridgeDictionary.RequestWordInfos("perspective");
            //WordInfo wordInfo = cambridgeDictionary.RequestWordInfo("perspective");

            //Console.WriteLine(wordInfo.Meaning);

            Console.ReadKey();
        }
    }
}
