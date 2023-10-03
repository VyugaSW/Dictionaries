using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;
using System.IO;
using Universal.Common.Collections;

namespace DictFiles
{

    static public class DictionaryFileReadWrite
    {
        static public void WriteToFile(MultiDictionary<string, string> wordTranslations, string path)
        {
            if (!File.Exists(path))
                throw new IOException($"File with path \"{path}\" does not exist");

            using (StreamWriter streamWriter = new StreamWriter(path))
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
    }

    internal class DictionaryFile
    {
        public string Path { get; private set; }

        public DictionaryFile(string path, FileMode fileMode)
        {
            if (Regex.IsMatch(path, @"([A-Za-z]:\\)((?:.*\\)?)([\w\s]+\.\w+)"))
                throw new IOException("Path is wrong");


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

            Path = path;
        }

        // Add new key and its values
        public void Add(MultiDictionary<string,string> keyValues)
        {
            DictionaryFileReadWrite.WriteToFile(keyValues, Path);
        }

        // Replace key on new one
        public void Replace(string key, string newKey)
        {
            File.WriteAllText(Path, File.ReadAllText(Path).Replace(key, newKey));            
        }

        // Replace value of key on new value of same key
        public void Replace(KeyValuePair<string,string> keyValuePair, string newValue)
        {
            MultiDictionary<string, string> wordsFromFile = DictionaryFileReadWrite.ReadFromFile(Path);

            if (!wordsFromFile.ContainsKey(keyValuePair.Key))
                throw new Exception($"There is not key \"{keyValuePair.Key}\"");

            if (!wordsFromFile.ContainsValue(keyValuePair.Value))
                throw new Exception($"There is not value \"{keyValuePair.Value}\"");

            // Index of translation in list of values
            int valueIndex = wordsFromFile[keyValuePair.Key].IndexOf(keyValuePair.Value);

            wordsFromFile[keyValuePair.Key][valueIndex] = newValue;

            DictionaryFileReadWrite.WriteToFile(wordsFromFile, Path);
        }

        // Remove value by key from file 
        public void Remove(KeyValuePair<string, string> keyValuePair)
        {
            MultiDictionary<string, string> wordsFromFile = DictionaryFileReadWrite.ReadFromFile(Path);

            if (!wordsFromFile.ContainsKey(keyValuePair.Key))
                throw new Exception($"There is not key \"{keyValuePair.Key}\"");

            if (!wordsFromFile.ContainsValue(keyValuePair.Value))
                throw new Exception($"There is not value \"{keyValuePair.Value}\"");

            if (wordsFromFile[keyValuePair.Key].Count == 1)
                throw new Exception($"Key \"{keyValuePair.Key}\" has only one value. It cant be deleted");

            wordsFromFile[keyValuePair.Key].Remove(keyValuePair.Value);
            DictionaryFileReadWrite.WriteToFile(wordsFromFile, Path);
        }

        // Remove key from file
        public void Remove(string key)
        {
            MultiDictionary<string, string> wordsFromFile = DictionaryFileReadWrite.ReadFromFile(Path);

            if (!wordsFromFile.ContainsKey(key))
                throw new Exception($"There is not key \"{key}\"");

            wordsFromFile.Remove(key);
            DictionaryFileReadWrite.WriteToFile(wordsFromFile, Path);
        }

        // Find values of key in file
        public List<string> Find(string key)
        {
            MultiDictionary<string, string> wordsFromFile = DictionaryFileReadWrite.ReadFromFile(Path);

            if (!wordsFromFile.ContainsKey(key))
                return null;

            return wordsFromFile[wordsFromFile.Keys.ToList().Find(x => x.Equals(key))];
        }

        // Create new file. RecreaterFlag - should the file be overwritten
        static public void Create(string path, bool recreateFlag)
        {
            if (File.Exists(path) && recreateFlag == false)
                throw new IOException($"File with path \"{path}\" already exists");

            Stream fStream = File.Create(path);
            fStream.Close();
        }

        // Export key and its values in separated file
        static public void Export(MultiDictionary<string, string> keyValues, string newPath)
        {
            if (File.Exists(newPath))
                throw new IOException($"File with path \"{newPath}\" already exists");
            DictionaryFileReadWrite.WriteToFile(keyValues, newPath);
        }
    }
}
