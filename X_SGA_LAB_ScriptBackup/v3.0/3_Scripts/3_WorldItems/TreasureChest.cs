using System;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 50;
    [SerializeField] private int currentHealth;
    public static bool isOpen = false;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void Activate(GameObject activator)
    {
        if (currentHealth > 0) return;
        isOpen = !isOpen;
    }
}
