using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(CharacterController), typeof(NavMeshAgent))]
public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private Camera playerCamera;
    [Tooltip("The distance from which the player can execute an interaction.")]
    [SerializeField] private float interactionDistance = 2f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private LayerMask groundLayer;

    [Header("Movement Settings")]
    [SerializeField] private float rotationSpeed = 10f;

    // --- Component & State Variables ---
    private NavMeshAgent navMeshAgent;
    private Coroutine followAndInteractCoroutine;

    // --- Debug Variable ---
    private Vector3 hitPosition = Vector3.zero;

    private void Awake()
    {
        if(playerCamera == null) playerCamera = Camera.main;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
    }

    void Update()
    {
        FaceMovementDirection();
    }

    public void HandleLeftClick()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.blue, 2.0f);

        // --- Step 1: Fire a raycast to identify an interactable target ---
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, interactableLayer))
        {
            // Clear the ground-click debug position since we are targeting an object.
            hitPosition = Vector3.zero;
            StartInteraction(hit.collider.gameObject);
            return;
        }

        // --- Step 2: If no interactable was hit, check for ground to move ---
        if (Physics.Raycast(ray, out RaycastHit groundHit, 100f, groundLayer))
        {
            StopInteraction();
            Move(groundHit.point);
            // Update the debug variable with the click position.
            hitPosition = groundHit.point;
        }
    }

    private void StartInteraction(GameObject target)
    {
        StopInteraction();
        followAndInteractCoroutine = StartCoroutine(FollowAndInteractRoutine(target));
    }

    private void StopInteraction()
    {
        if (followAndInteractCoroutine != null)
        {
            StopCoroutine(followAndInteractCoroutine);
            followAndInteractCoroutine = null;
        }
    }

    private IEnumerator FollowAndInteractRoutine(GameObject target)
    {
        navMeshAgent.SetDestination(target.transform.position);

        while (Vector3.Distance(transform.position, target.transform.position) > interactionDistance)
        {
            yield return null;
        }

        navMeshAgent.ResetPath();

        if (target.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(15);
        }
        else if (target.TryGetComponent<IActivatable>(out var activatable))
        {
            activatable.Activate(this.gameObject);
        }
        else if (target.TryGetComponent<ICollectable>(out var collectable))
        {
            collectable.Collect(transform.GetComponent<InventoryManager>());
        }
        else
        {
            Debug.LogWarning($"Player is in range of {target.name}, but it has no recognized interaction interface!");
        }

        followAndInteractCoroutine = null;
    }

    public void Move(Vector3 destination)
    {
        navMeshAgent.SetDestination(destination);
    }

    private void FaceMovementDirection()
    {
        if (navMeshAgent.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 direction = navMeshAgent.velocity.normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    // --- GIZMOS FOR VISUAL DEBUGGING ---

    private void OnDrawGizmos()
    {
        // Don't draw if we haven't clicked anywhere yet.
        if (hitPosition == Vector3.zero) return;

        // Optional: Don't draw if the player has already reached the destination.
        // This check is simple and might not be perfectly accurate if the agent stops slightly short.
        if (Vector3.Distance(transform.position, hitPosition) < 0.5f) return;

        // Draw a green sphere at the last clicked ground position.
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(hitPosition, 0.2f);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a yellow wire sphere representing the interaction distance.
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
