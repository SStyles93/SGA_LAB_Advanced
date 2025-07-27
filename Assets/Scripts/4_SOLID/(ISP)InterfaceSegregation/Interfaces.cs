/// <summary>
/// This interface defines an object that can be activated.
/// A lever or a button would implement this.
/// </summary>
public interface IActivatable
{
    void Activate();
}

/// <summary>
/// This interface defines an object that can be collected by the player.
/// </summary>
public interface ICollectable
{
    /// <summary>
    /// Collects the item and adds it to the specified inventory.
    /// </summary>
    /// <param name="collectorInventory">The InventoryManager of the character who is collecting the item.</param>
    void Collect(InventoryManager collectorInventory);
}

/// <summary>
/// This interface defines an object that can take damage.
/// A breakable crate or an enemy would implement this.
/// </summary>
public interface IDamageable
{
    void TakeDamage(int amount);
}
