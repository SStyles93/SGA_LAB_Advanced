// A simple class to hold the data we want to save.
// In a real project, this would be more complex.
using System.Collections.Generic;
using UnityEngine;

//The data we want to save independently from the way we save it
public class SaveData
{
    public Vector3 playerPosition;
    public List<string> inventoryItemIDs;
}

/// <summary>
/// DEPENDENCY INVERSION PRINCIPLE (DIP)
/// This interface defines the contract for a data service.
/// It decouples high-level modules (like a GameManager) from the low-level
/// details of how data is saved (e.g., JSON, binary, PlayerPrefs).
/// </summary>
public interface IDataService
{
    /// <summary>
    /// Saves the Data
    /// </summary>
    /// <param name="data">The data</param>
    /// <param name="fileName">File name</param>
    /// <param name="persistentData">Is data saved in persistent path or project</param>
    void Save(SaveData data, string fileName, bool persistentData = false);
    /// <summary>
    /// Loads the game Data
    /// </summary>
    /// <param name="fileName">File name</param>
    /// <param name="persistentData">Is data saved in persistent path or project</param>
    /// <returns>The saved data</returns>
    SaveData Load(string fileName, bool persistentData = false);
}
