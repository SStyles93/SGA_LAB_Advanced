using System.IO;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A concrete implementation of the IDataService interface that uses JSON.
/// This is a low-level module focused on one specific task.
/// </summary>
public class JsonDataService : IDataService
{
    public void Save(SaveData data, string fileName, bool persistentData)
    {
        string dataPath = persistentData ? Application.persistentDataPath : Application.dataPath;

        string path = Path.Combine(dataPath, fileName);
        string json = JsonUtility.ToJson(data, true); // 'true' for pretty print
        File.WriteAllText(path, json);
        Debug.Log($"Successfully saved data to {path}");
    }

    public SaveData Load(string fileName, bool persistentData)
    {
        string dataPath = persistentData ? Application.persistentDataPath : Application.dataPath;

        string path = Path.Combine(dataPath, fileName);
        if (!File.Exists(path))
        {
            Debug.LogError($"Cannot load file at {path}. File does not exist!");
            return null;
        }

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        Debug.Log($"Successfully loaded data from {path}");
        return data;
    }
}
