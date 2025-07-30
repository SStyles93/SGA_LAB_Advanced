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
    [SerializeField] private int currentHealth;
    private bool isOpen = false;

    public event Action<int, int> OnHealthChanged;

    public static event Action<bool> OnChestOpen;

    private void OnEnable()
    {
        OnChestOpen += OpenChest;
    }

    private void OnDisable()
    {
        OnChestOpen -= OpenChest;
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void Activate(GameObject activator)
    {
        if (currentHealth > 0) return;
        OnChestOpen?.Invoke(isOpen = !isOpen);
    }

    private void OpenChest(bool value)
    {
        isOpen = value;
    }

    public void TakeDamage(int amount)
    {
        if(currentHealth <= 0) return;

        currentHealth -= amount;
        if (currentHealth <= 0) currentHealth = 0;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log($"The chest takes {amount} damage. Health is now {currentHealth}.");
    }

    #region ISaveable Implementation

    public Dictionary<string, string> CaptureState()
    {
        var state = new Dictionary<string, string>
        {
            // Convert all values to strings for serialization.
            { "isOpen", isOpen.ToString() },
            //{ "currentHealth", currentHealth.ToString() }
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
        //if (state.TryGetValue("currentHealth", out string healthStr))
        //{
        //    // Parse the string back to its original type.
        //    int.TryParse(healthStr, out currentHealth);
        //}
    }

    #endregion
}
