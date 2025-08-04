using UnityEngine;
using System;

/// <summary>
/// Represents an item that exists in the game world and can be picked up.
/// Demonstrates the use of TryGetComponent for safe component access.
/// </summary>
[RequireComponent(typeof(Collider))] // Ensures this object always has a collider.
public class WorldItem : MonoBehaviour /*UNCOMMENT*//*,ICollectable*/
{
    [Tooltip("The data asset that defines this item.")]
    [SerializeField]
    private ItemData itemData;

    public ItemData GetItemData() => itemData;

    private void Start()
    {

        SetTrailColour();
    }

    #region /!\ TO DELETE /!\
    private void OnTriggerEnter(Collider other)
    {
        if(other != null && other.CompareTag("Player"))
        {
            other.GetComponent<PlayerInventoryManager>().AddItem(this.itemData);
            Destroy(this.gameObject);
        }
    }
    #endregion

    /*UNCOMMENT*/
    //public void Collect(PlayerInventoryManager collectorInventory)
    //{
    //    // 1. Check if the dependencies are valid.
    //    if (itemData == null)
    //    {
    //        Debug.LogError($"WorldItem on {gameObject.name} is missing its ItemData!");
    //        return;
    //    }
    //    if (collectorInventory == null)
    //    {
    //        Debug.LogError($"Collect method was called with a null collectorInventory on {gameObject.name}!");
    //        return;
    //    }

    //    // 2. Add the item to the CORRECT player's inventory.
    //    collectorInventory.AddItem(itemData);
    //    Debug.Log($"{collectorInventory.gameObject.name} collected {itemData.itemName}.");
    //}


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
