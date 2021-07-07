using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.IO;

public class SaveLoad : MonoBehaviour
{
    public static string path = Application.persistentDataPath + "/saves/";
    public static string fileExt = ".yyy";

    public static void Save<T>(T objectToSave, string key)
    {
        Directory.CreateDirectory(path);
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream fileStream = new FileStream(path + key + fileExt, FileMode.Create))
        {
            formatter.Serialize(fileStream, objectToSave);
        }
    }

    public static T Load<T>(string key)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        T returnValue = default(T);
        using (FileStream fileStream = new FileStream(path + key + fileExt, FileMode.Open))
        {
            returnValue = (T)formatter.Deserialize(fileStream);
        }

        return returnValue;
    }

    public static bool SaveExists(string key)
    {
        string _path = path + key + fileExt;
        return File.Exists(_path);
    }

    public static void DeleteAllSaves()
    {
        DirectoryInfo directory = new DirectoryInfo(path);
        directory.Delete();
        Directory.CreateDirectory(path);
    }
}
