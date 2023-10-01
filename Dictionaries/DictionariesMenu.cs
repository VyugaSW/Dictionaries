using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionariesMenu
{
    static internal class DictionariesMenu
    {
        static public List<string> mainMenuItems = new List<string> {
            "1| - Open dictionary\n",
            "2| - Create dictionary\n",
            "3| - Add new words and translations\n",
            "4| - Replace word or translation\n",
            "5| - Remove word of translation\n",
            "6| - Find translations\n",
            "7| - Export word and its translations\n"
        };

        static public List<string> subMenuCreating = new List<string> {
            "1| - Create new dictionary\n",
            "2| - Recreate existing dictionary\n"
        };

        static public List<string> subMenuReplacing = new List<string> {
            "1| - Replace word\n",
            "2| - Replace translation by word\n"
        };

        static public List<string> subMenuFinding = new List<string> {
            "1| - Find translation in all files\n",
            "2| - Find translation in definited file\n"
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
                Console.WriteLine(items[i]);
                Console.ResetColor();
            }
            Console.WriteLine();
        }

        static public int MenuNavigation(string nameMenu, List<string> items)
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
