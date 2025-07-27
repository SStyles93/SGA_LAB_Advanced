// C#
using UnityEngine;

/// <summary>
/// A concrete implementation of UsableItemData.
/// This object can be substituted anywhere a UsableItemData is expected.
/// </summary>
[CreateAssetMenu(fileName = "New Healing Potion", menuName = "Alchemist's Inventory/Healing Potion")]
public class HealingPotionData : UsableItemData
{
    [Tooltip("The amount of health to restore when used.")]
    public int healthToRestore;

    /// <summary>
    /// The concrete implementation of the Use method for this specific item.
    /// </summary>
    public override void Use(GameObject user)
    {
        // Find the health component on the user and heal them.
        if (user.TryGetComponent<PlayerHealth>(out var playerHealth))
        {
            playerHealth.Heal(healthToRestore);
            Debug.Log($"{user.name} used {itemName} and restored {healthToRestore} health.");
        }
    }
}
