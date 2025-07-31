using UnityEngine;
using System;

/// <summary>
/// Represents an item that exists in the game world and can be picked up.
/// Demonstrates the use of TryGetComponent for safe component access.
/// </summary>
[RequireComponent(typeof(Collider))] // Ensures this object always has a collider.
public class WorldItem : MonoBehaviour, ICollectable
{
    [Tooltip("The data asset that defines this item.")]
    [SerializeField]
    private ItemData itemData;

    public ItemData GetItemData() => itemData;

    public event Action<string,bool> OnMouseOverObject;

    /// <summary>
    /// Called when the object becomes enabled and active.
    /// This is where we register with the manager.
    /// </summary>
    private void OnEnable()
    {
        // Check if the WorldItemManager instance exists to avoid errors on game quit.
        if (WorldItemManager.Instance != null)
        {
            WorldItemManager.Instance.Register(this);
        }
    }

    /// <summary>
    /// Called when the object becomes disabled or is destroyed.
    /// This is where we unregister from the manager.
    /// </summary>
    private void OnDisable()
    {
        // Check if the WorldItemManager instance still exists.
        // This is important because on game quit, the manager might be destroyed first.
        if (WorldItemManager.Instance != null)
        {
            WorldItemManager.Instance.Unregister(this);
        }
    }

    private void Start()
    {
        //Ensure registering of Item
        if (WorldItemManager.Instance != null)
        {
            WorldItemManager.Instance.Register(this);
        }
    }

    public void Collect(PlayerInventoryManager collectorInventory)
    {
        // 1. Check if the dependencies are valid.
        if (itemData == null)
        {
            Debug.LogError($"WorldItem on {gameObject.name} is missing its ItemData!");
            return;
        }
        if (collectorInventory == null)
        {
            Debug.LogError($"Collect method was called with a null collectorInventory on {gameObject.name}!");
            return;
        }

        // 2. Add the item to the CORRECT player's inventory.
        collectorInventory.AddItem(itemData);
        Debug.Log($"{collectorInventory.gameObject.name} collected {itemData.itemName}.");

        // 3. Destroy the GameObject from the world.
        Destroy(gameObject);
    }

    private void OnMouseEnter()
    {
        OnMouseOverObject?.Invoke(itemData.itemName, true);
    }

    private void OnMouseExit()
    {
        OnMouseOverObject?.Invoke(itemData.itemName, false);
    }
}
