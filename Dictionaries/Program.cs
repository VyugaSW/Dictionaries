using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using Universal.Common.Collections;

using DictionariesFiles;

using DictionariesMenu;

internal class Program
{
    static void Main(string[] args)
    {
        DictionaryFile DicFile = new DictionaryFile(@"D:\\Test\English-Russia.txt", FileMode.Open);
        MultiDictionary<string, string> AuthorList = new MultiDictionary<string, string>();


    }
}
