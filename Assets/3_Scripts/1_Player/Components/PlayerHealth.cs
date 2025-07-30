using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float currentHealth = 75;
    public float maxHealth = 100;

    /// <summary>
    /// Method used to heal the player
    /// </summary>
    /// <param name="amount"></param>
    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        Debug.Log($"Player healed {amount}. Health is now {currentHealth}/{maxHealth}");
    }

    /// <summary>
    /// Method used to damage the player
    /// </summary>
    /// <param name="amount"></param>
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;
        Debug.Log($"Player took {amount} damage. Health is now {currentHealth}/{maxHealth}");
    }
}
