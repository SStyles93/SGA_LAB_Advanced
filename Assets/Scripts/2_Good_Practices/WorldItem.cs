using UnityEngine;

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


    public void Collect(InventoryManager collectorInventory)
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
    //private void Awake()
    //{
    //    // Ensure the collider is a trigger so it doesn't block the player.
    //    GetComponent<Collider>().isTrigger = true;
    //}

    //// This method is called by Unity when another collider enters this object's trigger.
    //private void OnTriggerEnter(Collider other)
    //{
    //    // --- Good Practice: TryGetComponent ---
    //    // TryGetComponent is the safest way to get a component. It returns true
    //    // and assigns the component to the 'out' variable if it's found.
    //    // If not found, it returns false and does nothing. This prevents
    //    // "NullReferenceException" errors if the colliding object isn't the player.
    //    if (other.TryGetComponent<InventoryManager>(out var inventoryManager))
    //    {
    //        // If the component was found, we can safely use it.
    //        inventoryManager.AddItem(itemData);

    //        // The item has been collected, so we destroy the world object.
    //        Destroy(gameObject);
    //    }
    //}
}
