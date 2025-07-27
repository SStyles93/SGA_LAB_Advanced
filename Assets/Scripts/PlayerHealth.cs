using System; // Required for Action
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int currentHealth = 75;
    public int maxHealth = 100;

    // Observer pattern
    public static event Action<int,int> OnHealthChanged;

    //// Old version (Delegates)
    //public delegate void HealthChangedDelegate(int currentHealth, int maxHealth);
    //Here, the "event" access modifier forces this class to be the only possible Invoker
    //public static event HealthChangedDelegate HealthChanged;
    

    private void Start()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// Method used to heal the player
    /// </summary>
    /// <param name="amount"></param>
    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        // Announce the change after healing.
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log($"Player healed {amount}. Health is now {currentHealth}/{maxHealth}");
    }
    /// <summary>
    /// Method used to damage the player
    /// </summary>
    /// <param name="amount"></param>
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        // Announce that the health has changed, providing the new values.
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log($"Player took {amount} damage. Health is now {currentHealth}/{maxHealth}");
    }
}
