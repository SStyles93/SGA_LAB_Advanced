using System;
using UnityEngine;

/// <summary>
/// Represents an item that exists in the game world and can be picked up.
/// Demonstrates the use of TryGetComponent for safe component access.
/// </summary>
[RequireComponent(typeof(Collider))] // Ensures this object always has a collider.
public class WorldItem : MonoBehaviour
{
    [Tooltip("The data asset that defines this item.")]
    [SerializeField]
    private ItemData itemData;

    public ItemData GetItemData() => itemData;

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
        SetTrailColour();
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

    /// <summary>
    /// Method used to set the two colors of the WorldObject's Trail according to the ItemData
    /// </summary>
    private void SetTrailColour()
    {
        TrailRenderer trailRender = GetComponentInChildren<TrailRenderer>(true);

        if (trailRender != null)
        {
            Material materialInstance = trailRender.material;
            if (itemData.trailColors.Length >= 2)
            {
                materialInstance.SetColor("_Color00", itemData.trailColors[0]); // Start Color
                materialInstance.SetColor("_Color01", itemData.trailColors[1]); // End Color
            }
            else
            {
                Debug.LogWarning($"ItemData: \"{itemData.name}\" does not contain enough trail colors.");
            }
        }
    }
}
