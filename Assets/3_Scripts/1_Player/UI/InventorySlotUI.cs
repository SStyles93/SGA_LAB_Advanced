using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script goes on the Inventory Slot UI prefab. It holds a reference to the inventoryManager
/// </summary>
public class InventorySlotUI : MonoBehaviour
{
    private ItemData item;
    private PlayerInventoryManager inventoryManager;
    private InventoryUI inventoryUI;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="newItem"></param>
    /// <param name="manager"></param>
    /// <param name="ui"></param>
    public void Setup(ItemData newItem, PlayerInventoryManager manager, InventoryUI ui)
    {
        item = newItem;
        inventoryManager = manager;
        inventoryUI = ui;

        if (transform.TryGetComponent(out Image icon))
        {
            //icon.sprite = item.icon;
            icon.enabled = true;
        }
    }
}
