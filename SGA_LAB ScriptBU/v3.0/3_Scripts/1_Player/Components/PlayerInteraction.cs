using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

[RequireComponent(typeof(CharacterController), typeof(NavMeshAgent))]
public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private Camera playerCamera;
    [Tooltip("The distance from which the player can execute an interaction.")]
    /*IMPLEMENT: A distance of interaction for the player*/
    /*IMPLEMENT: A LayerMask used for interactable objects*/
    [SerializeField] private LayerMask groundLayer;

    [Header("Movement Settings")]
    [SerializeField] private float rotationSpeed = 10f;

    // --- Component & State Variables ---
    private NavMeshAgent navMeshAgent;
    /*IMPLEMENT: a Coroutine to followAndInteract*/
    public static event Action<WorldItem> OnCollect;

    // --- Debug Variable ---
    private Vector3 hitPosition = Vector3.zero;

    private void Awake()
    {
        if (playerCamera == null) playerCamera = Camera.main;
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

        /*IMPLEMENT: A visual helper to see where we are clicking*/
        //TIP: you can use Debug.DrawRay();

        /*IMPLEMENT: A Raycast to detect interactable objects*/
        //{
        //    // Clear the ground-click debug position since we are targeting an object.
        //    hitPosition = Vector3.zero;
        //    /*IMPLEMENT: We will want to move then interact*/
        //    return;
        //}

        if (Physics.Raycast(ray, out RaycastHit groundHit, 100f, groundLayer))
        {
            /*IMPLEMENT: We will need to stop the player from going to an object if he is*/
            Move(groundHit.point);
            // Update the debug variable with the click position.
            hitPosition = groundHit.point;
        }
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

    #region /!\ TO IMPLEMENT /!\

    ///// <summary>
    ///// Method to Start an Interaction with Objects
    ///// </summary>
    ///// <param name="target">The targetted object</param>
    ///*IMPLEMENT: a "StartInteraction" method that takes a taget GameObject*/
    //{
    //    /*IMPLEMENT: Before starting an interaction we will have to stop the previous one
    //     this will require to create another method (just below this one)*/

    //    /*IMPLEMENT: We also want to keep a ref of the FollowAndInteractRoutine()*/
    //    //TIP: To start a coroutine you will have to use the StartCoroutine(The-Coroutine-You-Want-To-Start)
    //}

    ///*We want a Method to Stop the current Interaction*/
    //{
    //    /*IMPLEMENT: we have to check if the coroutine is not null to avoid firing errors*/
    //    }   
    //        /*IMPLEMENT: We want to stop the Coroutine*/
    //        /*IMPLEMENT: We also have to nullify the cached routine*/
    //    }
    //}

    //private IEnumerator FollowAndInteractRoutine(GameObject target)
    //{
    //    /*IMPLEMENT: We want our navMeshAgent to go somewhere*/

    //    /*IMPLEMENT: We will have to wait until the distance is acceptable*/
    //    //TIP: Use if(), Vector3.Distance(,), and the interactionDistance
    //    {
    //        yield return null;
    //    }

    //    navMeshAgent.ResetPath();

    //    /*Foreach type of inferface we have to implement the interaction*/
    //    /*TIP: we are going to use TryGetComponent<Interface-Name>(out var interface)*/

    //    /*IMPLEMENT: We want objects to Take 15 Damage*/

    //    /*IMPLEMENT: We want them to be activatable with (this.gameObject);*/

    //    /*IMPLEMENT: We also want them to be collectable (passing the PlayerInventoryManager)*/
    //    //We also want to invoke our delegate using a type cast (interface as Type)
        
    //    followAndInteractCoroutine = null;
    //}

    // --- GIZMOS FOR VISUAL DEBUGGING ---

    ///*IMPLEMENT: a constant Gizmo to visually see where we click*/
    //{
    //    /*IMPLEMENT Don't draw if we haven't clicked anywhere yet.*/

    //    /*IMPLEMENT (Optional): Don't draw if the player has already reached the destination.*/
    //    // This check is simple and might not be perfectly accurate if the agent stops slightly short.
    //    // TIP: use Vector3.Distance(,)    


    //    /*IMPLEMENT: Draw a green SPHERE of 0.2f radius at the last clicked ground position.*/
    //}


    ///*IMPLEMENT: a Gizmo visible only when selected*/
    //{
    //    /*IMPLEMENT: Draw a yellow WIRE SPHERE from the character representing the interaction distance.*/
    //}

    #endregion
}
