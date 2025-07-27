using UnityEngine;

/// <summary>
/// This script acts as a bridge between an IDamageable entity (like a chest)
/// and its world-space health bar. It subscribes to the entity's health change
/// event and updates the UI accordingly.
/// </summary>
public class DamageableEntityUIConnector : MonoBehaviour
{
    [Header("Dependencies")]
    [Tooltip("The WorldHealthBar prefab instance to control.")]
    [SerializeField] private WorldHealthBar healthBar;

    [Tooltip("The color to apply to this specific health bar.")]
    [SerializeField] private Color healthBarColor = Color.red;

    private void Awake()
    {
        // Try to get the IDamageable component from this GameObject.
        // We use the interface type here for maximum flexibility.
        var damageable = GetComponent<IDamageable>();

        if (damageable != null && healthBar != null)
        {
            // Subscribe our local method to the health change event.
            damageable.OnHealthChanged += OnHealthChanged;

            // Set the unique color for the health bar.
            healthBar.SetColor(healthBarColor);
        }
        else
        {
            Debug.LogWarning("DamageableEntityUIConnector is missing a reference to a health bar or is on an object without an IDamageable component.");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks when the object is destroyed.
        var damageable = GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.OnHealthChanged -= OnHealthChanged;
        }
    }

    /// <summary>
    /// This method is called by the event from the IDamageable entity.
    /// </summary>
    private void OnHealthChanged(int currentHealth, int maxHealth)
    {
        if (healthBar == null) return;

        // If health is zero or the object is inactive, hide the health bar.
        if (currentHealth <= 0 || !gameObject.activeInHierarchy)
        {
            healthBar.gameObject.SetActive(false);
        }
        else
        {
            healthBar.gameObject.SetActive(true);
            // Calculate the normalized health and update the UI.
            float normalizedHealth = (float)currentHealth / maxHealth;
            healthBar.UpdateHealth(normalizedHealth);
        }
    }
}
