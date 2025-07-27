using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Linq;

/// <summary>
/// Manages the overall game state, including the advanced hierarchical save/load system.
/// This system recursively traverses the scene to save both generic (Transform, active state)
/// and specific (ISaveable) data for every relevant object.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// A static singleton instance of the GameManager for easy access from other scripts.
    /// </summary>
    public static GameManager Instance { get; private set; }

    [Header("System Dependencies")]
    [Tooltip("Reference to the player's InventoryManager.")]
    [SerializeField] private GameObject m_Player;


    // --- DEPENDENCY INVERSION (DIP) ---
    // The GameManager depends on the IDataService interface, not the concrete
    // JsonDataService. This decouples the GameManager from the specific save/load implementation.
    private IDataService dataService;

    [Space(10)]
    [SerializeField] private string SAVE_FILE_NAME = "game_save.json";

    private void Awake()
    {
        // Singleton pattern to ensure only one GameManager exists.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Here, we create the concrete instance of our data service.
        // In a larger project, this might be handled by a "dependency injector".
        dataService = new JsonDataService();
    }

    /// <summary>
    /// Initiates the process to save the entire scene's state to a file.
    /// </summary>
    public void SaveGame()
    {
        SceneSaveData saveData = new SceneSaveData();
        Scene scene = SceneManager.GetActiveScene();
        List<GameObject> rootObjects = new List<GameObject>();
        scene.GetRootGameObjects(rootObjects);

        foreach (var rootObject in rootObjects)
        {
            // Don't save the GameManager itself.
            if (rootObject.GetComponent<GameManager>() != null) continue;

            saveData.rootObjects.Add(CaptureGameObjectData(rootObject));
        }

        dataService.Save(saveData, SAVE_FILE_NAME);
        Debug.Log("Full Scene Saved.");
    }

    /// <summary>
    /// Recursively captures the state of a GameObject and all of its children.
    /// </summary>
    /// <param name="go">The GameObject to capture.</param>
    /// <returns>A populated GameObjectSaveData object representing the GameObject's state.</returns>
    private GameObjectSaveData CaptureGameObjectData(GameObject go)
    {
        GameObjectSaveData data = new GameObjectSaveData();

        // --- Capture Generic Data ---
        //(??) is a Null-Coalescing Operator
        data.uniqueID = go.GetComponent<SaveableEntity>()?.UniqueId ?? string.Empty;
        data.name = go.name;
        data.isActiveSelf = go.activeSelf;

        // Transform
        data.position = go.transform.position;
        data.rotation = go.transform.rotation;
        data.localScale = go.transform.localScale;

        // --- Capture Specific ISaveable Data ---
        ISaveable saveable = go.GetComponent<ISaveable>();
        if (saveable != null)
        {
            var specificData = saveable.CaptureState();
            // Convert dictionary to our serializable list format.
            data.specificSaveData = specificData.Select(kvp => new SaveDataItem { key = kvp.Key, value = kvp.Value }).ToList();
        }

        // --- Recurse for Children ---
        foreach (Transform child in go.transform)
        {
            data.children.Add(CaptureGameObjectData(child.gameObject));
        }

        return data;
    }

    /// <summary>
    /// Initiates the process to load the entire scene's state from a file.
    /// </summary>
    public void LoadGame()
    {
        SceneSaveData saveData = dataService.Load(SAVE_FILE_NAME);
        if (saveData == null) { Debug.Log("No save data found."); return; }

        // Create a dictionary of all saveable entities in the scene for quick lookup.
        var allEntities = FindObjectsByType<SaveableEntity>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID) .ToDictionary(e => e.UniqueId);

        foreach (var rootObjectData in saveData.rootObjects)
        {
            RestoreGameObjectData(rootObjectData, allEntities);
        }
        Debug.Log("Full Scene Loaded.");
    }

    /// <summary>
    /// Recursively restores the state of a GameObject and all of its children from the save data.
    /// </summary>
    /// <param name="data">The saved data for the GameObject to restore.</param>
    /// <param name="allEntities">A dictionary of all SaveableEntity components in the scene for fast lookups.</param>
    private void RestoreGameObjectData(GameObjectSaveData data, Dictionary<string, SaveableEntity> allEntities)
    {
        // Find the corresponding GameObject in the scene using its unique ID.
        if (string.IsNullOrEmpty(data.uniqueID) || !allEntities.TryGetValue(data.uniqueID, out SaveableEntity entity))
        {
            // This object might have been created dynamically and doesn't exist in the current scene.
            // A full system would handle instantiating it here. For now, we'll skip it.
            return;
        }

        GameObject go = entity.gameObject;

        // --- Restore Generic Data ---
        go.SetActive(data.isActiveSelf);
        go.transform.position = data.position;
        go.transform.rotation = data.rotation;
        go.transform.localScale = data.localScale;

        // --- Restore Specific ISaveable Data ---
        ISaveable saveable = go.GetComponent<ISaveable>();
        if (saveable != null && data.specificSaveData != null)
        {
            // Convert our list back into a dictionary.
            var specificData = data.specificSaveData.ToDictionary(item => item.key, item => item.value);
            saveable.RestoreState(specificData);
        }

        // --- Recurse for Children ---
        foreach (var childData in data.children)
        {
            RestoreGameObjectData(childData, allEntities);
        }
    }
}
