using UnityEngine;

/// <summary>
/// This class demonstrates implementing multiple, small interfaces.
/// A treasure chest can be activated (opened) and also damaged (broken).
/// It doesn't need to implement ICollectable, so it isn't forced to.
/// </summary>
public class TreasureChest : MonoBehaviour, IActivatable, IDamageable
{
    [SerializeField] private int health = 30;
    private bool isOpen = false;

    public void Activate()
    {
        if (isOpen)
        {
            Debug.Log("The chest is already open.");
            return;
        }
        isOpen = true;
        Debug.Log("The chest creaks open! You find some loot.");
        // In a real game, you would spawn items here.
    }

    public void TakeDamage(int amount)
    {
        if (isOpen) return;

        health -= amount;
        Debug.Log($"The chest takes {amount} damage. Health is now {health}.");
        if (health <= 0)
        {
            isOpen = true;
            Debug.Log("The chest shatters into pieces! You find some loot.");
        }
    }
}
