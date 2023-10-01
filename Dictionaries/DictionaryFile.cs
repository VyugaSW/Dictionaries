using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;
using System.IO;
using Universal.Common.Collections;

namespace DictionariesFiles
{

    static internal class DictionaryFileReadWrite
    {
        static public void WriteToFile(MultiDictionary<string, string> wordTranslations, string path)
        {
            if (!File.Exists(path))
                throw new IOException($"File with path \"{path}\" does not exist");

            using (StreamWriter streamWriter = File.CreateText(path))
            {
                IEnumerable<string> keys = wordTranslations.Keys;
                List<string> values;

                foreach (var key in keys)
                {
                    values = wordTranslations[key];

                    streamWriter.Write(key + " - ");
                    foreach (string value in values)
                        streamWriter.Write(value + ", ");
                    streamWriter.WriteLine();
                }
            }
        }
        static public MultiDictionary<string, string> ReadFromFile(string path)
        {
            if (!File.Exists(path))
                throw new IOException($"File with path \"{path}\" does not exist");

            MultiDictionary<string, string> mulDictionary = new MultiDictionary<string, string>();
            string[] keyValues;
            string buffer;

            using (StreamReader streamReader = File.OpenText(path))
            {
                while ((buffer = streamReader.ReadLine()) != null)
                {
                    keyValues = buffer.Split(" -,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 1; i < keyValues.Length; i++)
                        mulDictionary.Add(keyValues[0], keyValues[i]);
                }
            }

            return mulDictionary;
        }

        static public void Export(MultiDictionary<string, string> wordTranslations, string newPath)
        {
            if (File.Exists(newPath))
                throw new IOException($"File with path \"{newPath}\" already exists");
            WriteToFile(wordTranslations, newPath);
        }
    }

    internal class DictionaryFile
    {
        string _path;

        public DictionaryFile(string path, FileMode fileMode)
        {
            if (Regex.IsMatch(path, @"([A-Za-z]:\\)((?:.*\\)?)([\w\s]+\.\w+)"))
                throw new FormatException("Path is wrong");

            if (!File.Exists(path) && (fileMode != FileMode.Create 
                    && fileMode != FileMode.CreateNew && fileMode != FileMode.OpenOrCreate && fileMode != FileMode.Append))
                throw new IOException($"File with path \"{path}\" does not exist");

            else if (!File.Exists(path) && (fileMode == FileMode.CreateNew || fileMode == FileMode.Create))
                Create(path, false);

            else if (File.Exists(path) && fileMode == FileMode.Create)
                Create(path, true);
            
            // FileMpde.Append is same FileModeOpenOrCreate,
            // cause we always put cursor in end of file, when write new information in its
            else if(fileMode == FileMode.OpenOrCreate || fileMode == FileMode.Append)
            {
                if (!File.Exists(path))
                    Create(path, false);

                // If the file exists, work with the file is possible and will be continued
            }

            // FileMode.Open wasnt written, cause we dont have to create file,
            // we only have to know exist it or no

            else if(fileMode == FileMode.Truncate)
                Create(path, true); // Clear file with recreating

            _path = path;
        }


        public void Add(MultiDictionary<string,string> wordTranslations)
        {
            DictionaryFileReadWrite.WriteToFile(wordTranslations, _path);
        }

        public void Replace(string wordKey, string newWordKey)
        {
            File.WriteAllText(_path, File.ReadAllText(_path).Replace(wordKey, newWordKey));            
        }
        public void Replace(KeyValuePair<string,string> wordTranslation, string newValue)
        {
            MultiDictionary<string, string> wordsFromFile = DictionaryFileReadWrite.ReadFromFile(_path);

            if (!wordsFromFile.ContainsKey(wordTranslation.Key))
                throw new Exception($"There is not key \"{wordTranslation.Key}\"");

            if (!wordsFromFile.ContainsValue(wordTranslation.Value))
                throw new Exception($"There is not value \"{wordTranslation.Value}\"");

            // Index of translation in list of values
            int valueIndex = wordsFromFile[wordTranslation.Key].IndexOf(wordTranslation.Value);

            wordsFromFile[wordTranslation.Key][valueIndex] = newValue;
            DictionaryFileReadWrite.WriteToFile(wordsFromFile, _path);
        }

        public void Remove(KeyValuePair<string, string> wordTranslation)
        {
            MultiDictionary<string, string> wordsFromFile = DictionaryFileReadWrite.ReadFromFile(_path);

            if (!wordsFromFile.ContainsKey(wordTranslation.Key))
                throw new Exception($"There is not key \"{wordTranslation.Key}\"");

            if (!wordsFromFile.ContainsValue(wordTranslation.Value))
                throw new Exception($"There is not value \"{wordTranslation.Value}\"");

            // In main class
            if (wordsFromFile[wordTranslation.Key].Count == 1)
                throw new Exception($"Word \"{wordTranslation.Key}\" has only one translation. It cant be deleted");
            // ----

            wordsFromFile[wordTranslation.Key].Remove(wordTranslation.Value);
            DictionaryFileReadWrite.WriteToFile(wordsFromFile, _path);
        }
        public void Remove(string wordKey)
        {
            MultiDictionary<string, string> wordsFromFile = DictionaryFileReadWrite.ReadFromFile(_path);

            if (!wordsFromFile.ContainsKey(wordKey))
                throw new Exception($"There is not key \"{wordKey}\"");

            wordsFromFile.Remove(wordKey);
            DictionaryFileReadWrite.WriteToFile(wordsFromFile, _path);
        }

        public List<string> Find(string wordKey)
        {
            MultiDictionary<string, string> wordsFromFile = DictionaryFileReadWrite.ReadFromFile(_path);

            if (!wordsFromFile.ContainsKey(wordKey))
                throw new Exception($"There is not key \"{wordKey}\"");

            return wordsFromFile[wordsFromFile.Keys.ToList().Find(x => x.Equals(wordKey))];
        }

        public void Create(string path, bool recreateFlag)
        {

            if (File.Exists(path) && recreateFlag == false)
                throw new IOException($"File with path \"{path}\" already exists");

            Stream fStream = File.Create(path);
            fStream.Close();
        }
    }
}
