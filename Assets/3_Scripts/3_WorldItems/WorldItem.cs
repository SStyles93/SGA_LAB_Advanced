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

    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.CompareTag("Player"))
        {
            other.GetComponent<PlayerInventoryManager>().AddItem(this.itemData);
            Destroy(this.gameObject);
        }
    }
}
