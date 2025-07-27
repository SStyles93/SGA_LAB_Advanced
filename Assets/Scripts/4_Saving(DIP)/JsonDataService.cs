using UnityEngine;
using System.IO;

/// <summary>
/// A concrete implementation of IDataService that uses Unity's built-in
/// JsonUtility to serialize and deserialize the scene data.
/// </summary>
public class JsonDataService : IDataService
{
    /// <summary>
    /// Serializes the SceneSaveData object to a JSON file.
    /// </summary>
    public void Save(SceneSaveData data, string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        try
        {
            // Use JsonUtility to convert the entire object tree into a JSON string.
            // The 'true' argument makes the JSON file human-readable (pretty print).
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);
            Debug.Log($"Successfully saved data to {path}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save data to {path}. Error: {e.Message}");
        }
    }

    /// <summary>
    /// Deserializes a JSON file back into a SceneSaveData object.
    /// </summary>
    public SceneSaveData Load(string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (!File.Exists(path))
        {
            // This is not an error, just a first-time run.
            Debug.Log($"Save file not found at {path}.");
            return null;
        }

        try
        {
            string json = File.ReadAllText(path);
            // Use JsonUtility to parse the JSON string and reconstruct the SceneSaveData object tree.
            SceneSaveData data = JsonUtility.FromJson<SceneSaveData>(json);
            Debug.Log($"Successfully loaded data from {path}");
            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load data from {path}. Error: {e.Message}");
            return null;
        }
    }
}
