using System;
using UnityEngine;

/// <summary>
/// Represents an item that exists in the game world and can be picked up.
/// Demonstrates the use of TryGetComponent for safe component access.
/// </summary>
[RequireComponent(typeof(Collider))] // Ensures this object always has a collider.
public class WorldItem : MonoBehaviour /*IMPLEMENT: Collectible interface*/
{
    [Tooltip("The data asset that defines this item.")]
    [SerializeField]
    private ItemData itemData;

    public ItemData GetItemData() => itemData;

    public event Action<string, bool> OnMouseOverObject;

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

    private void Awake()
    {
        // Ensure the collider is a trigger so it doesn't block the player.
        GetComponent<Collider>().isTrigger = true;
    }

    // This method is called by Unity when another collider enters this object's trigger.
    private void OnTriggerEnter(Collider other)
    {
        // --- Good Practice: TryGetComponent ---
        // TryGetComponent is the safest way to get a component. It returns true
        // and assigns the component to the 'out' variable if it's found.
        // If not found, it returns false and does nothing. This prevents
        // "NullReferenceException" errors if the colliding object isn't the player.
        if (other.TryGetComponent<PlayerInventoryManager>(out var inventoryManager))
        {
            // If the component was found, we can safely use it.
            //inventoryManager.AddItem(itemData);

            // The item has been collected, so we destroy the world object.
            Destroy(gameObject);
        }
    }

    public void Collect(PlayerInventoryManager collectorInventory)
    {
        /*IMPLEMENT: 1. Check if the dependencies are valid.*/
        //   Debug.LogError($"WorldItem on {gameObject.name} is missing its ItemData!");
        //   Debug.LogError($"Collect method was called with a null collectorInventory on {gameObject.name}!");

        /*IMPLEMENT 2. Add the item to the player's inventory.*/

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
