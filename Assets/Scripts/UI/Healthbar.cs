// C#
using UnityEngine;

/// <summary>
/// Manages a world-space health bar made of two quads.
/// It controls the fill amount and color of the health bar.
/// </summary>
public class WorldHealthBar : MonoBehaviour
{
    private Camera mainCamera;

    [Header("Visual Components")]
    [Tooltip("The child transform representing the fill of the health bar.")]
    [SerializeField] private Transform healthFillTransform;

    [Tooltip("The renderer for the health fill quad, used to change its color.")]
    [SerializeField] private Renderer healthFillRenderer;

    [SerializeField] private bool looksAtCamera = true;

    // A MaterialPropertyBlock is used to override material properties for a single
    // renderer instance without creating a new material, which is great for performance.
    private MaterialPropertyBlock propertyBlock;

    private void Awake()
    {
        if(looksAtCamera)
        mainCamera = Camera.main;

        // Initialize the property block. We only need to do this once.
        propertyBlock = new MaterialPropertyBlock();
    }

    private void LateUpdate()
    {
        if (mainCamera == null || !looksAtCamera) return;

        // Make this object's forward direction point towards the camera.
        // We use the negative of the camera's forward vector to ensure it's oriented correctly.
        transform.forward = -mainCamera.transform.forward;
    }

    /// <summary>
    /// Updates the health bar's fill amount by scaling the child quad.
    /// </summary>
    /// <param name="normalizedHealth">The health value from 0.0 to 1.0.</param>
    public void UpdateHealth(float normalizedHealth)
    {
        if (healthFillTransform == null) return;

        // Clamp the value to ensure it's always between 0 and 1.
        normalizedHealth = Mathf.Clamp01(normalizedHealth);

        // Scale the local X-axis of the fill quad to represent the health percentage.
        // A scale of (1, 1, 1) is full health, (0.5, 1, 1) is half health.
        healthFillTransform.localScale = new Vector3(normalizedHealth, 1f, 1f);
    }

    /// <summary>
    /// Sets a unique color for this specific health bar instance.
    /// </summary>
    /// <param name="color">The color to apply to the health fill.</param>
    public void SetColor(Color color)
    {
        if (healthFillRenderer == null) return;

        // 1. Get the current properties from the renderer into our block.
        healthFillRenderer.GetPropertyBlock(propertyBlock);
        // 2. Set the "_Color" property in our block.
        propertyBlock.SetColor("_Color", color);
        // 3. Apply the modified block back to the renderer.
        healthFillRenderer.SetPropertyBlock(propertyBlock);
    }
}
