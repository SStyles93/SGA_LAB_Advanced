using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the player's inventory. This script demonstrates safe and efficient
/// coding practices, including dependency management and access modifiers.
/// </summary>
public class PlayerInventoryManager : MonoBehaviour /*IMPLEMENT: the saveable interface HERE*/
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

    [Tooltip("A list of all possible recipes the player can use.")]
    [SerializeField] private List<CraftingRecipe> availableRecipes;

    public GameObject GetInventoryPannel() => inventoryPannel;
    public List<ItemData> GetInventory() => inventory;
    public List<CraftingRecipe> GetAvailableRecipes() => availableRecipes;

    /*IMPLEMENT: We want a delegate(Action) to notify the change in inventory*/
    //TIP: Use static and event. It will be called "OnInventoryChanged"

    /// <summary>
    /// Toggles the visibility of the Inventory
    /// </summary>
    public void ToggleInventoryVisibility()
    {
        bool visibleState = inventoryPannel.activeSelf;
        visibleState = !visibleState;
        inventoryPannel.SetActive(visibleState);
        savePannel.SetActive(visibleState);
        /*IMPLEMENT: We want to call it when we open the inventory to be sure we are up to date when it shows*/
    }

    #region /!\ IMPLEMENT /!\ Add/Remove Item
    ///// <summary>
    ///// Adds an item to the inventory and updates the UI.
    ///// </summary>
    ///// <param name="itemToAdd">The ScriptableObject data of the item to add.</param>
    ///*IMPLEMENT: A method to add the item to the inventory or availableRecipes lists*/
    ////TIP: the method will have to be PUBLIC has NO return type and takes and ItemData as PARAMETER
    //{
    //    /*IMPLEMENT: Check for null*/

    //    /*IMPLEMENT a Declaration pattern*/
    //    //TIP: Use if() and the declaration pattern with the CraftingRecipe type
    //    {
    //        if (!availableRecipes.Contains(recipe))
    //            availableRecipes.Add(recipe);
    //    }
    //    /*Otherwise it is an "standard item*/
    //    {
    //        if (inventory.Count > 10)
    //        {
    //            Debug.Log("Inventory is full");
    //            return;
    //        }

    //        /*IMPLEMENT: the addition of item to the inventory list*/
    //        Debug.Log($"Added {/*IMPLEMENT: it's name*/} to inventory.");

    //        /*IMPLEMENT: Call the delegate*/

    //    }
    //    // Update the status text to show what was added.
    //    UpdateStatus(itemToAdd);
    //}


    ///// <summary>
    ///// Removes an item from the inventory.
    ///// </summary>
    ///*IMPLEMENT: We want to be able to remove the items too*/
    //{
    //    /*IMPLEMENT: Check for null*/

    //    /*IMPLEMENT: Remove the item from the list*/
    //    //TIP: you can use the "listName".Remove() in an if() to check if removal was done or not :)
    //    {
    //        Debug.Log($"Removed {itemToRemove.itemName} from inventory.");

    //        /*IMPLEMENT: We have to notify the removal of the object now*/
    //    }
    //}
    #endregion

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
            ///*UNCOMMENT when previous steps are done*/
            //RemoveItem(itemToUse);
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
        string stackableMessage = item.isStackable ? "It is stackable." : "It is not stackable.";

        statusText.text = $"Acquired: {item.itemName}. {stackableMessage}";

        // --- Good Practice: switch statement ---
        // A switch statement is cleaner and often more performant than a long
        // if-else if chain when checking a variable against a set of discrete values,
        // like an enum.
        switch (item.itemType)
        {
            case ItemType.Ingredient:
                Debug.Log("This is an ingredient. It can be used for crafting.");
                break; // 'break' exits the switch statement.

            case ItemType.Potion:
                Debug.Log("This is a potion. It can be consumed for an effect.");
                break;

            case ItemType.Key:
                Debug.Log("This is a key. It can be used to unlock something.");
                break;
            case ItemType.Recipe:
                Debug.Log("This is a recipe. It can be used to craft something.");
                break;

            // The 'default' case runs if none of the other cases match.
            // It's good practice to include it to handle unexpected values.
            default:
                Debug.Log("This is a generic item with no special category.");
                break;
        }
    }

    #region /!\ TO IMPLEMENT /!\ ISaveable Implementation 

    //public Dictionary<string, string> CaptureState()
    //{
    //    // Get a list of all the ItemIDs from the current inventory.
    //    // Bonus : The LINQ Select method is a clean, modern way to do this.
    //    /*IMPLEMENT: list of strings*/ itemIDs = /*IMPLEMENT: we want the item.ItemIDs*/

    //    //We are adding the list of recipes to the items list (since recipes are also items)
    //    itemIDs.AddRange(/*IMPLEMENT: Again we can use LINQ to get the list of recipes*/);

    //    // Join the list of IDs into a single string, separated by commas.
    //    // This is a robust way to store a list of strings in our key-value pair system.
    //    // Example: "item001,item003,item001,item002"
    //    // TIP: Use string.Join();


    //    string inventoryStateString = /* IMPLEMENT */

    //    // Return the state in the required dictionary format.
    //    //IMPLEMENT: the return of dictionary
    //}

    ///// <summary>
    ///// Restores the inventory's state from the loaded data.
    ///// </summary>
    ///// <param name="state">The dictionary containing the saved inventory string.</param>
    //public void RestoreState(Dictionary<string, string> state)
    //{
    //    // Check if the saved data contains an "inventory" key.
    //    if (state.TryGetValue(/*IMEPLEMENT: "nameOfStoredValue*/, out string savedInventoryString))
    //    {
    //        // Clear the current inventory and recipe list before loading the new one.
    //        /*IMPLEMENT: we want to clear inventory and availableRecipes*/

    //        // If the saved string is empty, there's nothing to load.
    //        if (string.IsNullOrEmpty(savedInventoryString))
    //        {
    //            OnInventoryChanged?.Invoke(); // Still invoke to update the UI to be empty.
    //            return;
    //        }

    //        // Split the single string back into a list of individual IDs.
    //        //TIP: use .Split().ToList()
    //        /*IMPLEMENT: a List of strings called itemIDs*/

    //        // --- Find all ItemData assets in the project ---
    //        // This is the most complex part. We need a way to map an ID back to an asset.
    //        // In production we probably would use Unity's Adressables
    //        var allItems = Resources.FindObjectsOfTypeAll<ItemData>().ToDictionary(item => item.ItemID);

    //        // Re-populate the inventory list using the loaded IDs.
    //        foreach (string id in itemIDs)
    //        {
    //            if (allItems.TryGetValue(id, out ItemData itemAsset))
    //            {
    //                if( itemAsset is CraftingRecipe recipe) availableRecipes.Add(recipe);
    //                else inventory.Add(itemAsset);
    //            }
    //            else
    //            {
    //                Debug.LogWarning($"Could not find ItemData asset with ID: {id}");
    //            }
    //        }

    //        Debug.Log($"Inventory loaded with {inventory.Count} items and {availableRecipes.Count} available recipes.");

    //        // After loading the inventory, broadcast the change to update the UI.
    //        /*IMPLEMENT: Invoke the delegate to update UI*/
    //    }
    //}
    #endregion

}
