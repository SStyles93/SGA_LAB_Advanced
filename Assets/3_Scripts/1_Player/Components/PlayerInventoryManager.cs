using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the player's inventory. This script demonstrates safe and efficient
/// coding practices, including dependency management and access modifiers.
/// </summary>
public class PlayerInventoryManager : MonoBehaviour
{
    // --- Good Practice: Cached References & [SerializeField] ---
    // Instead of using Find() or GetComponent() repeatedly in Update(), we assign
    // references once in the Inspector or in Start(). This is far more performant.
    // By making the field private and using [SerializeField], we follow the principle
    // of encapsulation while still allowing designers to link objects in the editor.
    [Header("Dependencies")]
    [Tooltip("The UI Text element used to display status messages.")]
    [SerializeField] private TMPro.TMP_Text statusText;

    [Tooltip("The pannel used to display the Inventory")]
    [SerializeField] private GameObject inventoryPannel = null;
    [SerializeField] private GameObject savePannel = null;

    // The player's inventory is a private list. No other script can directly
    // modify this list, which prevents bugs. They must use public methods like AddItem().
    [Tooltip("A list of all Items in the inventory.")]
    [SerializeField] private List<ItemData> inventory = new List<ItemData>();

    public GameObject GetInventoryPannel() => inventoryPannel;
    public List<ItemData> GetInventory() => inventory;

    /// <summary>
    /// Toggles the visibility of the Inventory
    /// </summary>
    public void ToggleInventoryVisibility()
    {
        bool visibleState = inventoryPannel.activeSelf;
        visibleState = !visibleState;
        inventoryPannel.SetActive(visibleState);
        savePannel.SetActive(visibleState);
    }

    public void AddItem(ItemData item)
    {
        inventory.Add(item);
    }

    /// <summary>
    /// Updates the status text with information about the last added item.
    /// </summary>
    private void UpdateStatus(ItemData item)
    {
        if (statusText == null)
        {
            Debug.LogWarning("Status Text dependency is not set in the Inspector!");
            return;
        }

        // --- Good Practice: Ternary Operator ---
        // A ternary operator is a concise way to write a simple if-else statement.
        // Here, we check if the item is stackable and set the message accordingly.
        // The format is: condition ? value_if_true : value_if_false;
        /*REFACTOR:*/
        if (item.isStackable) statusText.text = $"Acquired: " + item.itemName + ". It is stackable.";
        else statusText.text = $"Acquired: " + item.itemName + ". It is not stackable.";

        // --- Good Practice: switch statement ---
        // A switch statement is cleaner and often more performant than a long
        // if-else if chain when checking a variable against a set of discrete values,
        // like an enum.
        /*UNCOMMENT && REFACTOR:*/
        //if (item.itemType == ItemType.Ingredient)
        //{
        //    Debug.Log("This is an ingredient. It can be used for crafting.");
        //}
        //else if (item.itemType == ItemType.Potion)
        //{
        //    Debug.Log("This is a potion. It can be consumed for an effect.");
        //}
        //else if (item.itemType == ItemType.Key)
        //{
        //    Debug.Log("This is a key. It can be used to unlock something.");
        //}
        //else if (item.itemType == ItemType.Recipe)
        //{
        //    Debug.Log("This is a recipe. It can be used to craft something.");
        //}
        //else
        //{
        //    Debug.Log("This is a generic item with no special category.");
        //}
    }
}
