using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CraftingTable : MonoBehaviour /*IMPLEMENT the Activatable inferface*/
{
    [Header("Configuration")]
    [Tooltip("The list of ingredient stations connected to this table.")]
    [SerializeField]
    private List<IngredientStation> ingredientStations;

    // We need a reference to the player's inventory to give them the crafted item.
    [SerializeField] private PlayerInventoryManager playerInventory;

    ///// <summary>
    ///// This method will be called when the player activated the table
    ///// </summary>
    ///*IMPLEMENT the interfaces method*/
    //{
    //    /*IMPLEMENT: A Log to show the name of the player that activated it: "Table Activated by TheNAMEofTheGuy" */
    //    /*IMPLEMENT: We want to keep a ref of the PlayerInventoryManager: */
    //    /*IMPLEMENT: Here we are going to call the Craft() method */
    //}

    /// <summary>
    /// This is the primary method to be called by a UI button or player interaction.
    /// It attempts to craft an item based on the current ingredients.
    /// </summary>

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Craft();
        }
    }

    private void Craft()
    {
        // 1. Gather all ingredients from the connected stations.
        List<ItemData> currentIngredients = new List<ItemData>();
        foreach (var station in ingredientStations)
        {
            if (station.CurrentItem != null)
            {
                currentIngredients.Add(station.CurrentItem);
            }
        }

        if (currentIngredients.Count == 0)
        {
            Debug.Log("No ingredients on the table!");
            return;
        }

        // 2. Find a matching recipe.
        CraftingRecipe matchedRecipe = FindMatchingRecipe(currentIngredients);

        // 3. If a recipe is found, perform the craft.
        if (matchedRecipe != null)
        {
            Debug.Log($"Recipe match found: {matchedRecipe.name}! Crafting {matchedRecipe.outputItem.itemName}.");

            // Consume ingredients from the stations.
            foreach (var station in ingredientStations)
            {
                station.ClearStation();
            }

            // Add the crafted item to the player's inventory.
            playerInventory.AddItem(matchedRecipe.outputItem);
        }
        else
        {
            Debug.Log("No valid recipe found for the current ingredients.");
        }
    }

    /// <summary>
    /// Checks the provided ingredients against all available recipes.
    /// </summary>
    /// <returns>The matching CraftingRecipe, or null if no match is found.</returns>
    private CraftingRecipe FindMatchingRecipe(List<ItemData> ingredients)
    {
        foreach (var recipe in playerInventory.GetAvailableRecipes())
        {
            // We check if the recipe can be crafted with the ingredients.
            // We also ensure the number of ingredients matches to prevent crafting with extra items on the table.
            if (recipe.ingredients.Count == ingredients.Count && recipe.CanCraft(ingredients))
            {
                return recipe;
            }
        }
        return null;
    }
}
