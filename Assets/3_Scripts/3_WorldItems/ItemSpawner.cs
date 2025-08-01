// C#
using UnityEngine;
using System.Collections;

/// <summary>
/// A singleton manager that can spawn items and move them along a parabolic arc
/// from a starting point to a destination.
/// </summary>
public class ItemSpawner : MonoBehaviour
{
    public static ItemSpawner Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Spawns an item and moves it along a curved path.
    /// </summary>
    /// <param name="itemData">The data of the item to spawn, containing the prefab.</param>
    /// <param name="startPoint">The world position where the arc should begin.</param>
    /// <param name="endPoint">The world position where the arc should end.</pawn>
    /// <param name="arcHeight">The height of the arc at its peak, relative to the midpoint.</param>
    /// <param name="duration">The time in seconds the item should take to travel the arc.</param>
    public void SpawnItemWithArc(ItemData itemData, Vector3 startPoint, Vector3 endPoint, float arcHeight = 2.0f, float duration = 0.5f)
    {
        if (itemData == null || itemData.prefab == null)
        {
            Debug.LogError("Cannot spawn item: ItemData or its prefab is null.");
            return;
        }

        // Instantiate the item prefab at the starting point.
        GameObject itemObject = Instantiate(itemData.prefab, startPoint, Quaternion.identity);

        // Start the coroutine that will move the object along the calculated path.
        StartCoroutine(MoveAlongArcRoutine(itemObject.transform, startPoint, endPoint, arcHeight, duration));
    }

    /// <summary>
    /// A coroutine that animates a Transform along a parabolic arc.
    /// </summary>
    private IEnumerator MoveAlongArcRoutine(Transform objectToMove, Vector3 start, Vector3 end, float height, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // If the object was destroyed mid-flight (e.g., by another system), stop the coroutine.
            if (objectToMove == null)
            {
                yield break;
            }

            // --- Calculate the position in the arc ---

            // 1. Calculate the linear position (a straight line from start to end).
            // The 't' value is the normalized progress (0 to 1) of the animation.
            float t = elapsedTime / duration;
            Vector3 linearPosition = Vector3.Lerp(start, end, t);

            // 2. Calculate the arc height adjustment.
            // This uses a parabolic equation: 4 * h * (t - t^2)
            // This formula gives 0 at t=0 and t=1, and gives 'h' at t=0.5.
            float arc = 4 * height * (t - (t * t));
            Vector3 arcPosition = linearPosition + new Vector3(0, arc, 0);

            // 3. Apply the final position.
            objectToMove.position = arcPosition;

            // --- Update time and wait for the next frame ---
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // --- Finalize ---
        // Ensure the object ends up at the exact end position.
        if (objectToMove != null)
        {
            objectToMove.position = end;
        }
    }
}
