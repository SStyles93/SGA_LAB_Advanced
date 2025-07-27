using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class demonstrates implementing multiple, small interfaces.
/// A treasure chest can be activated (opened) and also damaged (broken).
/// It doesn't need to implement ICollectable, so it isn't forced to.
/// </summary>
public class TreasureChest : MonoBehaviour, IActivatable, IDamageable, ISaveable 
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 50;
    private int currentHealth;
    private bool isOpen = false;

    public event Action<int, int> OnHealthChanged;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void Activate(GameObject activator)
    {
        if (isOpen)
        {
            Debug.Log("The chest is already open.");
            return;
        }
        isOpen = true;
        Debug.Log($"The chest creaks open! {activator.name} finds some loot.");
        
        //Hide healthbar
        OnHealthChanged?.Invoke(0, maxHealth);
        
        // In a real game, you would spawn items here.
    }

    public void TakeDamage(int amount)
    {
        if (isOpen) return;

        currentHealth -= amount;
        Debug.Log($"The chest takes {amount} damage. Health is now {currentHealth}.");
        if (currentHealth <= 0)
        {
            isOpen = true;
            Debug.Log("The chest shatters into pieces! You find some loot.");
        }
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    #region ISaveable Implementation

    public Dictionary<string, string> CaptureState()
    {
        var state = new Dictionary<string, string>
        {
            // Convert all values to strings for serialization.
            { "isOpen", isOpen.ToString() },
            { "currentHealth", currentHealth.ToString() }
        };
        return state;
    }

    public void RestoreState(Dictionary<string, string> state)
    {
        if (state.TryGetValue("isOpen", out string isOpenStr))
        {
            // Parse the string back to its original type.
            bool.TryParse(isOpenStr, out isOpen);
        }
        if (state.TryGetValue("health", out string healthStr))
        {
            // Parse the string back to its original type.
            int.TryParse(healthStr, out currentHealth);
        }
    }

    #endregion
}
