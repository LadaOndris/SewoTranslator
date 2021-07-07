using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CambridgeDictionary
{
    interface IDictionary
    {
        WordInfo RequestWordInfo(string word);
    }
}
