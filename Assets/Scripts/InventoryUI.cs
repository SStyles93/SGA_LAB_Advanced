using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the visual representation of the player's inventory.
/// It listens to the InventoryManager's OnInventoryChanged event to know when to update.
/// </summary>
public class InventoryUI : MonoBehaviour
{
    //[Header("UI References")]
    //[Tooltip("The parent object that will hold all the inventory slot UI elements.")]
    //[SerializeField] private Transform itemsParent;

    [Tooltip("The UI prefab for a single inventory slot.")]
    [SerializeField] private GameObject inventorySlotPrefab;

    // We need a reference to the data source, but only to read from it.
    [SerializeField] private InventoryManager inventoryManager;

    // --- Subscribing and Unsubscribing to the Event ---

    private void OnEnable()
    {
        // 3. Subscribe the RedrawUI method to the event.
        // Now, whenever OnInventoryChanged is invoked, RedrawUI will be called automatically.
        InventoryManager.OnInventoryChanged += RedrawUI;
    }

    private void OnDisable()
    {
        // 4. Unsubscribe when the object is disabled or destroyed.
        // This is crucial to prevent memory leaks and errors if the UI object is destroyed
        // before the InventoryManager.
        InventoryManager.OnInventoryChanged -= RedrawUI;
    }

    private void Start()
    {
        // Initial drawing of the UI when the game starts.
        RedrawUI();
    }

    /// <summary>
    /// This method is called by the event. It redraws the entire inventory UI.
    /// </summary>
    private void RedrawUI()
    {
        Debug.Log("Redrawing Inventory UI...");

        // Clear all existing UI slots to prevent duplicates.
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Get the current list of items from the manager.
        List<ItemData> currentItems = inventoryManager.GetInventory();

        // Create a new UI slot for each item in the inventory.
        foreach (ItemData item in currentItems)
        {
            GameObject slotInstance = Instantiate(inventorySlotPrefab, transform);

            // Get the InventorySlotUI component from the newly created slot.
            var slotUI = slotInstance.GetComponent<InventorySlotUI>();
            if (slotUI != null)
            {
                // Pass the item data and a reference to the manager to the slot.
                slotUI.Setup(item, inventoryManager);
            }
            else
            {
                Debug.LogWarning("InventorySlot prefab is missing an InventorySlotUI component!");
            }
        }
    }
}
