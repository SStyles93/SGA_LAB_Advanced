using UnityEngine;

public class IngredientStation : MonoBehaviour, IActivatable
{
    [Tooltip("The item currently placed on this station.")]
    [SerializeField] // Exposed for debugging
    private ItemData currentItem = null;

    public ItemData CurrentItem => currentItem;


    public void Activate()
    {

    }

    /// <summary>
    /// Places an item on this station.
    /// </summary>
    public void PlaceItem(ItemData item)
    {
        currentItem = item;
        Debug.Log($"Placed {item.itemName} on station {gameObject.name}.");
        // In a real game, you would update a visual model here.
    }

    /// <summary>
    /// Removes and returns the item from this station.
    /// </summary>
    public ItemData ClearStation()
    {
        ItemData item = currentItem;
        currentItem = null;
        // Update visuals to be empty.
        return item;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = (currentItem != null) ? Color.cyan : Color.gray;
        Gizmos.DrawWireCube(transform.position + Vector3.up * 0.5f, Vector3.one);
        if (currentItem != null)
        {
#if UNITY_EDITOR
            UnityEditor.Handles.Label(transform.position + Vector3.up, currentItem.itemName);
#endif
        }
    }
}
