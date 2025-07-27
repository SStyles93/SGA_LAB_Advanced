using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script goes on the Inventory Slot UI prefab. It holds a reference to the inventoryManager
/// </summary>
public class InventorySlotUI : MonoBehaviour
{
    private ItemData item;
    private InventoryManager inventoryManager;

    /// <summary>
    /// Sets up the slot with the necessary data.
    /// </summary>
    /// <param name="newItem">The item this slot will represent.</param>
    /// <param name="manager">A reference to the inventory manager.</param>
    public void Setup(ItemData newItem, InventoryManager manager)
    {
        item = newItem;
        inventoryManager = manager;

        if (transform.TryGetComponent(out Image icon))
        {
            icon.sprite = item.icon;
            icon.enabled = true;
        }
    }

    /// <summary>
    /// Method used by button for when the object is clicked in UI Inventory
    /// </summary>
    public void OnSlotClicked()
    {
        if (item == null || inventoryManager == null) return;

        // Check if the item is a UsableItemData (like a potion).
        if (item is UsableItemData)
        {
            // If it's usable, call the UseItem method.
            inventoryManager.UseItem(item);
        }
        else
        {
            RemoveItem();
        }
    }

    private void RemoveItem()
    {
        // Ensure we have an item and a manager reference before trying to do anything.
        if (item == null || inventoryManager == null)
        {
            return;
        }

        // Tell the InventoryManager to remove the specific item this slot represents.
        inventoryManager.RemoveItem(item);
    }
}
