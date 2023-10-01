using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;
using System.IO;
using Universal.Common.Collections;



using DictsConsoleInterface;
using DictFiles;

namespace Dictionaries
{

    static internal class DictionaryEnterInfo
    {
        static public MultiDictionary<string, string> EnterWordTranslations()
        {
            MultiDictionary<string, string> wordTranslation = new MultiDictionary<string, string>();
            DictionaryConsole.DisplayText("Enter word and after enter his translation separated by commas:\n");

            string word = DictionaryConsole.GetLine();
            string[] translations = DictionaryConsole.GetLine().Split(',');

            foreach (string translation in translations)
                wordTranslation.Add(word.ToLower(), translation.ToLower());

            return wordTranslation;
        }
        static public KeyValuePair<string, string> EnterWordTranslation()
        {
            Dictionary<string, string> wordTranslation = new Dictionary<string, string>();
            DictionaryConsole.DisplayText("Enter existing word and after enter his translation:\n");

            string word = DictionaryConsole.GetLine();
            string translation = DictionaryConsole.GetLine();

            wordTranslation.Add(word, translation);

            return wordTranslation.First();
        }
        static public string EnterLine(string text)
        {
            DictionaryConsole.DisplayText($"{text}:");
            return DictionaryConsole.GetLine();
        }
        static public string EnterPathForCreate()
        {
            DictionaryConsole.DisplayText("(For Exit enter 0)\n");
            DictionaryConsole.DisplayText("Enter a path to directory: ");
            string pathToDirectory = DictionaryConsole.GetLine();

            if (pathToDirectory == "0")
                return pathToDirectory;

            DictionaryConsole.DisplayText("Enter a name of File: ");
            string fileName = DictionaryConsole.GetLine(); ;
            return pathToDirectory + "\\" + fileName + ".txt";
        }
        static public string EnterPathForOpen()
        {
            DictionaryConsole.DisplayText("Enter a path to file: \n(For Exit enter 0)\n");
            return DictionaryConsole.GetLine();
        }

    }

    public class DictionariesMain
    {
        DictionaryFile dictFile = null;

        public void Main()
        {
            int option;
            bool workFlag = true;

            while (workFlag)
            {              
                if (dictFile != null)
                    DictionaryConsole.DisplayText($"\nOpen file with path \"{dictFile.Path}\" right now.\n" +
                        $"You can do anything with it. If you need to change file, open or create new one\n");

                option = DictionaryMenu.Menu("Main Menu", DictionaryMenu.mainMenu);

                try
                {
                    switch (option)
                    {
                        case 1:
                            OpenDictionaryFile();
                            break;
                        case 2:
                            CreateDictionaryFile();
                            break;
                        case 3:
                            AddWord();
                            break;
                        case 4:
                            ReplaceChoose();
                            break;
                        case 5:
                            RemoveChoose();
                            break;
                        case 6:
                            FindTranslation();
                            break;
                        case 7:
                            ExportWordTranslation();
                            break;
                        case 8:
                            DictionaryConsole.DisplayMultiDictionaryFromFile(GivePath());
                            DictionaryConsole.WaitKey();
                            break;
                        case 9:
                            workFlag = false;
                            break;
                    }
                }
                catch(IOException ex)
                {
                    DictionaryConsole.DisplayText(ex.Message);
                    DictionaryConsole.WaitKey();
                }
                catch (Exception ex)
                {
                    DictionaryConsole.DisplayText(ex.Message);
                    DictionaryConsole.WaitKey();
                }
                Console.Clear(); // I coudn't put it my separated console class, But I couldn't
            }
        }


        // Choose option of recreating (yes, no or return)
        private int RecreateQuestion(string path)
        {
            int choice = 0;

            if (File.Exists(path))
            {
                choice = DictionaryMenu.Menu($"File with path \"{path}\" already exists. Do you want to recreate its?",
                    DictionaryMenu.yesNoMenu);
            }

            return choice;
        }

        // Check how we need recreate dicFile
        private int ReCreateDicFile(ref string path)
        {
            // Check that dicFile == null or dicFile != null
            int choice = 0;

            if (dictFile == null)
            {
                path = DictionaryEnterInfo.EnterPathForOpen();

                if (path == "0")
                    return 0;

                choice = RecreateQuestion(path);
            }

            if (choice == 1)
                dictFile = new DictionaryFile(path, FileMode.Create);
            else if (choice == 2)
                dictFile = new DictionaryFile(path, FileMode.Open);

            // If choice == 0 it means dictFile isnt empty

            return 1;
        }

        // Return path of dicFile, if it doesnt exist we make user to creater new one 
        private string GivePath()
        {
            string path = null;

            ReCreateDicFile(ref path);

            if (path == "0")
                return null;

            return dictFile.Path;
        }


        private int OpenDictionaryFile()
        {
            string path = DictionaryEnterInfo.EnterPathForOpen();

            if (path == "0")
                return 0;

            try
            {
                dictFile = new DictionaryFile(path, FileMode.Open);
            }
            catch (IOException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return 1;
        }

        private int CreateDictionaryFile()
        {
            string path = DictionaryEnterInfo.EnterPathForCreate();

            if (path == "0")
                return 0;

            int choice;
            choice = RecreateQuestion(path);

            if (choice == 1)
            {
                try
                {
                    DictionaryFile.Create(path, true);
                }
                catch (IOException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return 1;
            // If choise == 2 or choice == 3 we should break this function
        }       


        private int AddWord()
        {
            string path = null;

            try 
            {
                ReCreateDicFile(ref path);

                if (path == "0")
                    return 0;

                dictFile.Add(DictionaryEnterInfo.EnterWordTranslations());
            }
            catch (IOException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return 1;
        }


        private void ReplaceChoose()
        {
            int choice = DictionaryMenu.Menu("MENU REPLACE", DictionaryMenu.subMenuReplacing);
            try
            {
                switch (choice)
                {
                    case 1:
                        ReplaceWord();
                        break;
                    case 2:
                        ReplaceTranslation();
                        break;
                    case 3:
                        break;
                }
            }
            catch(IOException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private int ReplaceWord()
        {
            string path = null;

            try
            {
                ReCreateDicFile(ref path);

                if (path == "0")
                    return 0;

                dictFile.Replace(DictionaryEnterInfo.EnterLine("Enter word"), 
                    DictionaryEnterInfo.EnterLine("Enter new word(word for replacing)"));
            }
            catch (IOException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return 1;
        }

        private int ReplaceTranslation()
        {
            string path = null;

            try
            {
                ReCreateDicFile(ref path);

                if (path == "0")
                    return 0;

                dictFile.Replace(DictionaryEnterInfo.EnterWordTranslation(), DictionaryEnterInfo.EnterLine("Enter new translation"));
            }
            catch (IOException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return 1;
        }


        private void RemoveChoose()
        {
            int choice = DictionaryMenu.Menu("MENU REPLACE", DictionaryMenu.subMenuRemoving);
            try
            {
                switch (choice)
                {
                    case 1:
                        RemoveWord();
                        break;
                    case 2:
                        RemoveTranslation();
                        break;
                    case 3:
                        break;
                }
            }
            catch (IOException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private int RemoveWord()
        {
            string path = null;

            try
            {
                ReCreateDicFile(ref path);

                if (path == "0")
                    return 0;

                dictFile.Remove(DictionaryEnterInfo.EnterLine("Enter word for removing"));
            }
            catch (IOException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return 1;
        }

        private int RemoveTranslation()
        {
            string path = null;

            try
            {
                ReCreateDicFile(ref path);

                if (path == "0")
                    return 0;

                dictFile.Remove(DictionaryEnterInfo.EnterWordTranslation());
            }
            catch (IOException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return 1;
        }


        private List<string> FindTranslationLoop(string word, string pathToDirectory)
        {
            List<string> tempTranslations = null;

            if (!Directory.Exists(pathToDirectory))
                throw new IOException($"Directory with path \"{pathToDirectory}\" doesn't exist");

            DirectoryInfo dirInfo = new DirectoryInfo(pathToDirectory);
            DictionaryFile tempFile;

            foreach(FileInfo file in dirInfo.GetFiles())
            {
                tempFile = new DictionaryFile(file.FullName, FileMode.Open);
                tempTranslations = tempFile.Find(word);

                if (tempTranslations != null)
                    break;
            }

            return tempTranslations;
        }

        private int FindTranslation()
        {
            string word = null;
            string pathToDirectory = null;

            try
            {
                word = DictionaryEnterInfo.EnterLine("Enter word for finding its translation");
                pathToDirectory = DictionaryEnterInfo.EnterLine("Enter path to directory with dictionaries");

                DictionaryConsole.DisplayText($"All found tranlsations of word \"{word}\"");
                DictionaryConsole.DisplayText(FindTranslationLoop(word, pathToDirectory));
                DictionaryConsole.WaitKey();           
            }
            catch (IOException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return 1;
        }


        private int ExportWordTranslation()
        {
            try
            {

                DictionaryFile.Export(DictionaryEnterInfo.EnterWordTranslations(), DictionaryEnterInfo.EnterPathForOpen());
            }
            catch (IOException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return 1;
        }
       
    }
}

