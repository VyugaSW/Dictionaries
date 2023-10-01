using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using Universal.Common.Collections;

using DictFiles;

namespace DictsConsoleInterface
{
    static internal class DictionaryConsole
    {
        static public string GetLine()
        {
            return Console.ReadLine();
        }

        static public void DisplayText(string text)
        {
            Console.WriteLine(text);
        }

        static public void DisplayText(List<string> text)
        {
            foreach(string str in text)
                Console.Write(str + ", ");
        }

        static public void WaitKey()
        {
            Console.WriteLine("Press any button to continue...");
            Console.ReadKey();   
        }

        static public void DisplayMultiDictionaryFromFile(string path)
        {
            MultiDictionary<string, string> multDictionary = DictionaryFileReadWrite.ReadFromFile(path);

            IEnumerable<string> keys = multDictionary.Keys;
            List<string> values;

            foreach (var key in keys)
            {
                values = multDictionary[key];

                Console.Write(key + " - ");
                foreach (string value in values)
                    Console.Write(value + ", ");
                Console.WriteLine();
            }

        }

    }


    static internal class DictionaryMenu
    {
        static public List<string> mainMenu = new List<string> {
            "1| - Open dictionary\n",
            "2| - Create dictionary\n",
            "3| - Add new words and translations\n",
            "4| - Replace word or translation\n",
            "5| - Remove word or translation\n",
            "6| - Find translations\n",
            "7| - Export word and its translations\n",
            "8| - Show all words\n",
            "9| - Exit\n"
        };

        static public List<string> subMenuRemoving = new List<string> {
            "1| - Remove word\n",
            "2| - Remove translation\n",
            "3| - Return\n"
        };

        static public List<string> subMenuReplacing = new List<string> {
            "1| - Replace word\n",
            "2| - Replace translation by word\n",
            "3| - Return\n"
        };

       /* static public List<string> subMenuFinding = new List<string> {
            "1| - Find translation in all directory\n",
            "2| - Find translation in definited file\n",
            "3| - Return\n"
        };*/

        static public List<string> yesNoMenu = new List<string>
        {
            "1| - Yes\n",
            "2| - No\n",
            "3| - Return\n"
        };


        static private void DisplayMenu(List<string> items, int row, int col, int index)
        {

            Console.SetCursorPosition(col, row);
            for (int i = 0; i < items.Count; i++)
            {
                if (i == index)
                {
                    Console.BackgroundColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.Write(items[i]);
                Console.ResetColor();
            }
            Console.WriteLine();
        }

        static public int Menu(string nameMenu, List<string> items)
        {
            Console.WriteLine(nameMenu +'\n');

            int row = Console.CursorTop;
            int col = Console.CursorLeft;
            int index = 0;

            while (true)
            {
                DisplayMenu(items, row, col, index);
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.DownArrow:
                        if (index < items.Count - 1)
                            index++;
                        break;
                    case ConsoleKey.UpArrow:
                        if (index > 0)
                            index--;
                        break;
                    case ConsoleKey.Enter:
                        return index + 1; // For comfort
                }
            }

        }

    }
}
