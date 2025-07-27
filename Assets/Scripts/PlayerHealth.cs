using System;
using System.Collections; // Required for Action
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float currentHealth = 75;
    public float maxHealth = 100;

    private Coroutine healingCoroutine; // A reference to the active healing coroutine

    // Observer pattern
    public static event Action<float, float> OnHealthChanged;

    //// Old version (Delegates)
    //public delegate void HealthChangedDelegate(int currentHealth, int maxHealth);
    //Here, the "event" access modifier forces this class to be the only possible Invoker
    //public static event HealthChangedDelegate HealthChanged;
    

    private void Start()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// This is the new public method that starts the healing process based on potion data.
    /// </summary>
    public void StartHealing(HealingPotionData potion)
    {
        if (potion.isHealOverTime)
        {
            // If a healing effect is already running, stop it before starting a new one.
            // This prevents stacking and just refreshes the effect.
            if (healingCoroutine != null)
            {
                StopCoroutine(healingCoroutine);
            }
            healingCoroutine = StartCoroutine(HealOverTimeRoutine(potion.healthToRestore, potion.duration));
        }
        else
        {
            // If it's an instant heal, just apply it directly.
            Heal(potion.healthToRestore);
        }
    }

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
        // Announce the change after healing.
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log($"Player healed {amount}. Health is now {currentHealth}/{maxHealth}");
    }

    /// <summary>
    /// Method used to damage the player
    /// </summary>
    /// <param name="amount"></param>
    public void TakeDamage(float amount)
    {
        // If the player takes damage, it should probably stop any active healing.
        if (healingCoroutine != null)
        {
            StopCoroutine(healingCoroutine);
            healingCoroutine = null;
            Debug.Log("Healing effect was interrupted by damage!");
        }

        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log($"Player took {amount} damage. Health is now {currentHealth}/{maxHealth}");
    }

    /// <summary>
    /// A coroutine that smoothly heals the player over a given duration.
    /// </summary>
    private IEnumerator HealOverTimeRoutine(float totalHealAmount, float duration)
    {
        float amountHealed = 0;
        float healPerSecond = totalHealAmount / duration;

        Debug.Log($"Starting heal-over-time: {totalHealAmount} health over {duration} seconds.");

        while (amountHealed < totalHealAmount)
        {
            // Calculate the healing for this frame.
            float healThisFrame = healPerSecond * Time.deltaTime;
            amountHealed += healThisFrame;

            // Apply the healing and update health.
            currentHealth += healThisFrame;
            if (currentHealth > maxHealth) currentHealth = maxHealth;

            // Announce the change to update the UI smoothly.
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            // Stop healing if health is already full.
            if (currentHealth >= maxHealth)
            {
                Debug.Log("Healing stopped because health is full.");
                healingCoroutine = null; // Clear the reference
                yield break; // Exit the coroutine
            }

            // Wait for the next frame.
            yield return null;
        }

        // Ensure the final health value is accurate after the loop.
        // This corrects any minor floating point inaccuracies.
        // (This part is optional but good practice).

        Debug.Log("Heal-over-time effect finished.");
        healingCoroutine = null; // Clear the reference
    }
}
