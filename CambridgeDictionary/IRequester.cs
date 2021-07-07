using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CambridgeDictionary
{
    internal interface IRequester
    {
        WordInfo[] RequestWordInfos(string word);
    }
}
