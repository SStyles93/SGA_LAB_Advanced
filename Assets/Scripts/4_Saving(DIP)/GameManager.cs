using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private IDataService dataService;
    private const string SAVE_FILE_NAME = "scene_save.json";
    private Dictionary<string, SaveableEntity> allEntities;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        dataService = new JsonDataService();
    }

    public void SaveGame()
    {
        var sceneSaveData = new SceneSaveData();

        // Find all root GameObjects in the current scene.
        var rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        // Start the recursive capture from the root objects.
        foreach (var go in rootObjects)
        {
            // We only start capturing from roots that have a SaveableEntity.
            // The recursive function will handle children that might not have one.
            if (go.GetComponent<SaveableEntity>() != null)
            {
                sceneSaveData.rootObjects.Add(CaptureStateRecursive(go));
            }
        }

        dataService.Save(sceneSaveData, SAVE_FILE_NAME);
        Debug.Log($"Game Saved. Saved {sceneSaveData.rootObjects.Count} root object hierarchies.");
    }

    public void LoadGame()
    {
        var sceneSaveData = dataService.Load(SAVE_FILE_NAME);
        if (sceneSaveData == null)
        {
            Debug.Log("No save data found.");
            return;
        }

        // Create a dictionary of all saveable entities for fast lookups.
        allEntities = FindObjectsByType<SaveableEntity>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).ToDictionary(e => e.UniqueId);

        // Start the recursive restore process.
        foreach (var rootObjectData in sceneSaveData.rootObjects)
        {
            RestoreStateRecursive(rootObjectData);
        }

        Debug.Log("Game Loaded.");
    }

    /// <summary>
    /// Recursively captures the state of a GameObject and all its children.
    /// </summary>
    private GameObjectSaveData CaptureStateRecursive(GameObject go)
    {
        var data = new GameObjectSaveData
        {
            uniqueID = go.GetComponent<SaveableEntity>().UniqueId,
            name = go.name,
            isActive = go.activeSelf,
            position = new Vector3Data(go.transform.position),
            rotation = new QuaternionData(go.transform.rotation),
            scale = new Vector3Data(go.transform.localScale)
        };

        // Capture data from all ISaveable components on this object.
        foreach (var saveable in go.GetComponents<ISaveable>())
        {
            string componentTypeName = saveable.GetType().ToString();
            data.componentSaveData[componentTypeName] = saveable.CaptureState();
        }

        // Recurse into children.
        foreach (Transform child in go.transform)
        {
            // IMPORTANT: We only save children that have a SaveableEntity.
            // This prevents saving parts of complex prefabs you don't care about.
            if (child.GetComponent<SaveableEntity>() != null)
            {
                data.children.Add(CaptureStateRecursive(child.gameObject));
            }
        }
        return data;
    }

    /// <summary>
    /// Recursively restores the state of a GameObject and its children from save data.
    /// </summary>
    private void RestoreStateRecursive(GameObjectSaveData data)
    {
        if (!allEntities.TryGetValue(data.uniqueID, out SaveableEntity entity))
        {
            // This object was in the save file but is no longer in the scene.
            // In a real game, you might want to re-instantiate it from a prefab here.
            return;
        }

        // Restore generic transform and active state.
        entity.gameObject.SetActive(data.isActive);
        entity.transform.position = data.position.ToVector3();
        entity.transform.rotation = data.rotation.ToQuaternion();
        entity.transform.localScale = data.scale.ToVector3();

        // Restore specific component data.
        foreach (var saveable in entity.GetComponents<ISaveable>())
        {
            string componentTypeName = saveable.GetType().ToString();
            if (data.componentSaveData.TryGetValue(componentTypeName, out var componentState))
            {
                saveable.RestoreState(componentState);
            }
        }

        // Recurse into children.
        foreach (var childData in data.children)
        {
            RestoreStateRecursive(childData);
        }
    }
}
