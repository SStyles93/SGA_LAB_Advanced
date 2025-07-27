using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

/// <summary>
/// A high-level manager for the game. It handles saving, loading, and holds
/// references to other core systems. This demonstrates the Dependency Inversion Principle.
/// </summary>
public class GameManager : MonoBehaviour
{
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

    // These methods can be called by UI buttons.
    public void SaveGame()
    {
        SaveData saveData = new SaveData
        {
            // Get the player's actual position.
            playerPosition = m_Player.transform.position,
            // Ask the inventory manager for its data.
            inventoryItemIDs = m_Player.GetComponent<InventoryManager>()?.GetItemIDsForSaving()
        };

        dataService.Save(saveData, SAVE_FILE_NAME);
        Debug.Log("Game Saved!");
    }

    public void LoadGame()
    {
        SaveData saveData = dataService.Load(SAVE_FILE_NAME);
        if (saveData != null)
        {
            // Pass the loaded data to the relevant systems.
            m_Player.transform.position = saveData.playerPosition;
            m_Player.GetComponent<NavMeshAgent>().destination = saveData.playerPosition;
            m_Player.GetComponent<InventoryManager>()?.LoadInventoryFromIDs(saveData.inventoryItemIDs);
            Debug.Log("Game Loaded!");
        }
    }
}
