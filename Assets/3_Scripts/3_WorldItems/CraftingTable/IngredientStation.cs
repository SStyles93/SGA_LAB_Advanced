using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class IngredientStation : MonoBehaviour
{
    [Header("Dependencies")]
    [Tooltip("Reference to the main Inventory UI panel.")]
    [SerializeField] private InventoryUI inventoryUI;
    [Tooltip("Reference to the player's inventory manager.")]
    [SerializeField] private PlayerInventoryManager inventoryManager;

    [Header("Data")]
    [Tooltip("The item currently placed on this station.")]
    [SerializeField] private ItemData currentItem = null; // Exposed for debugging

    [Header("Station Parts")]
    [SerializeField] private GameObject worldItemPosition = null;
    [SerializeField] private GameObject currentWorldItem = null; // Exposed for debugging

    public ItemData CurrentItem => currentItem;

    private void Start()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Activate(other.gameObject);
        }
    }

    public void Activate(GameObject activator)
    {
        InventoryUI playerUI = activator.GetComponentInChildren<InventoryUI>(true);
        PlayerInventoryManager playerInventory = activator.GetComponent<PlayerInventoryManager>();

        // Check for dependencies first to avoid errors.
        if (playerUI == null || playerInventory == null)
        {
            Debug.LogError($"Activator {activator.name} is missing an InventoryUI or InventoryManager component!");
            return;
        }

        if (currentItem == null)
        {
            // If the station is EMPTY, open the inventory in selection mode.
            // We tell the UI which station is asking for an item.
            playerUI.OpenForSelection(this);
        }
        else
        {
            // If the station is FULL, return the item to the player.
            ReturnItemToPlayer(playerInventory);
        }
    }

    /// <summary>
    /// Places an item on this station.
    /// </summary>
    public void PlaceItem(ItemData item, PlayerInventoryManager placerInventory = null)
    {
        // We should only accept items that are ingredients.
        if (item.itemType == ItemType.Ingredient)
        {
            currentItem = item;
            Debug.Log($"Placed {item.itemName} on station {gameObject.name}.");

            // Update visual model.
            if(currentWorldItem != null) { Destroy(currentWorldItem); currentWorldItem = null; }
            currentWorldItem = Instantiate(currentItem.prefab, worldItemPosition.transform.position, Quaternion.identity, worldItemPosition.transform);
            currentWorldItem.GetComponent<WorldItem>().enabled = false;
        }
        else
        {
            Debug.LogWarning($"{item.name} is not an ingredient and cannot be placed here.");
            // If a non-ingredient was somehow selected, give it back to the player.
            placerInventory.AddItem(item);
        }
    }

    /// <summary>
    /// Adds the current item back to the player's inventory and clears the station.
    /// </summary>
    private void ReturnItemToPlayer(PlayerInventoryManager playerInventory)
    {
        if (currentItem == null) return;

        playerInventory.AddItem(currentItem);
        Debug.Log($"Returned {currentItem.itemName} to {playerInventory.gameObject.name}.");
        currentItem = null;
        Destroy(currentWorldItem);
    }

    /// <summary>
    /// Removes and returns the item from this station.
    /// </summary>
    public ItemData ClearStation()
    {
        ItemData item = currentItem;
        currentItem = null;
        Destroy(currentWorldItem);
        return item;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = (currentItem != null) ? Color.cyan : Color.gray;
        Gizmos.DrawWireCube(transform.position, Vector3.one);
        if (currentItem != null)
        {
#if UNITY_EDITOR
            UnityEditor.Handles.Label(transform.position + Vector3.up, currentItem.itemName);
#endif
        }
    }
}
