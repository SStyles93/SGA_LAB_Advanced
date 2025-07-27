using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the saved state of a single GameObject in the scene.
/// This is the core node of our hierarchical save data.
/// </summary>
[System.Serializable]
public class GameObjectSaveData
{
    // --- Generic Data (for all objects) ---
    public string uniqueID;
    public string name;
    public bool isActiveSelf;

    // Transform data
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 localScale;

    // --- Specific Data (for ISaveable objects) ---
    // We need a way to serialize a dictionary. JsonUtility can't do this directly,
    // so we use a list of key-value pairs instead.
    public List<SaveDataItem> specificSaveData;

    // --- Hierarchy Data ---
    public List<GameObjectSaveData> children;

    public GameObjectSaveData()
    {
        specificSaveData = new List<SaveDataItem>();
        children = new List<GameObjectSaveData>();
    }
}

/// <summary>
/// A serializable key-value pair to work around JsonUtility's limitations with dictionaries.
/// We will store ISaveable data in this format.
/// </summary>
[System.Serializable]
public class SaveDataItem
{
    public string key;
    public string value; // We serialize all values to string and parse them on load.
}

/// <summary>
/// The root container for the entire scene's save data.
/// </summary>
[System.Serializable]
public class SceneSaveData
{
    // A list of all the root-level objects in the scene.
    public List<GameObjectSaveData> rootObjects;

    public SceneSaveData()
    {
        rootObjects = new List<GameObjectSaveData>();
    }
}
