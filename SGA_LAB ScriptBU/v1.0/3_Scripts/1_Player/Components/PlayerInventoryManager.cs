using System;
using System.Collections.Generic;
using System.Linq;
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

    ///*UNCOMMENT*/
    //[Tooltip("A list of all possible recipes the player can use.")]
    //[SerializeField] private List<CraftingRecipe> availableRecipes;

    public GameObject GetInventoryPannel() => inventoryPannel;
    public List<ItemData> GetInventory() => inventory;
    ///*UNCOMMENT*/
    //public List<CraftingRecipe> GetAvailableRecipes() => availableRecipes;
    
    public static event Action OnInventoryChanged;

    /// <summary>
    /// Toggles the visibility of the Inventory
    /// </summary>
    public void ToggleInventoryVisibility()
    {
        bool visibleState = inventoryPannel.activeSelf;
        visibleState = !visibleState;
        inventoryPannel.SetActive(visibleState);
        savePannel.SetActive(visibleState);
        OnInventoryChanged?.Invoke();
    }

    /// <summary>
    /// Adds an item to the inventory and updates the UI.
    /// </summary>
    /// <param name="itemToAdd">The ScriptableObject data of the item to add.</param>
    public void AddItem(ItemData itemToAdd)
    {
        if (itemToAdd == null) return;

        if (itemToAdd is CraftingRecipe recipe)
        {
            if (!availableRecipes.Contains(recipe))
                availableRecipes.Add(recipe);
        }
        else
        {
            if (inventory.Count > 10)
            {
                Debug.Log("Inventory is full");
                return;
            }

            inventory.Add(itemToAdd);
            Debug.Log($"Added {itemToAdd.itemName} to inventory.");

            OnInventoryChanged?.Invoke();

            // Update the status text to show what was added.
        }
        UpdateStatus(itemToAdd);
    }

    /// <summary>
    /// Removes an item from the inventory.
    /// </summary>
    public void RemoveItem(ItemData itemToRemove)
    {
        if (itemToRemove == null) return;

        if (inventory.Remove(itemToRemove))
        {
            Debug.Log($"Removed {itemToRemove.itemName} from inventory.");
            // Also broadcast the event on removal.
            OnInventoryChanged?.Invoke();
        }
    }

    /// <summary>
    /// Uses an item from the inventory, triggering its effect and removing it.
    /// </summary>
    public void UseItem(ItemData itemToUse)
    {
        // Check if the item is actually a "UsableItemData"
        // here, the (name "is" Type name) is called Pattern Matching introduced in C# 7.0
        if (itemToUse is UsableItemData usableItem)
        {
            // Call the item's specific Use() method.
            // We pass 'gameObject' which is the Player
            usableItem.Use(this.gameObject);

            // After using the item, remove it from the inventory.
            RemoveItem(itemToUse);
        }
        else
        {
            Debug.LogWarning($"{itemToUse.name} is not a usable item.");
        }
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
        if(item.isStackable) statusText.text = $"Acquired: " + item.itemName + ". It is stackable.";
        else statusText.text = $"Acquired: " + item.itemName + ". It is not stackable.";

        // --- Good Practice: switch statement ---
        // A switch statement is cleaner and often more performant than a long
        // if-else if chain when checking a variable against a set of discrete values,
        // like an enum.
        /*REFACTOR:*/
        if(item.itemType == ItemType.Ingredient)
        {
            Debug.Log("This is an ingredient. It can be used for crafting.");
        }
        else if (item.itemType == ItemType.Potion)
        {
            Debug.Log("This is a potion. It can be consumed for an effect.");
        }
        else if (item.itemType == ItemType.Key)
        {
            Debug.Log("This is a key. It can be used to unlock something.");
        }
        else if (item.itemType == ItemType.Recipe)
        {
            Debug.Log("This is a recipe. It can be used to craft something.");
        }
        else
        {
            Debug.Log("This is a generic item with no special category.");
        }
    }
}
