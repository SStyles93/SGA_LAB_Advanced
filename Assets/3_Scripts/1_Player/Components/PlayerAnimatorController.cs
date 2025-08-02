using UnityEngine;
using UnityEngine.AI;

public class PlayerAnimatorController : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private NavMeshAgent navMeshAgent;
    [SerializeField]
    private PlayerActions actions;

    int MovementSpeed = 0;
    int PickUpTrigger = 0;

    void OnEnable()
    {
        PlayerInteraction.OnCollect += PlayCollectAnimation;
    }
    void OnDisable()
    {
        PlayerInteraction.OnCollect -= PlayCollectAnimation;
    }

    void Awake()
    {
        if (navMeshAgent == null && TryGetComponent<NavMeshAgent>(out var agent))
        {
            navMeshAgent = agent;
        }
        if (animator == null && TryGetComponent<Animator>(out var anim))
        {
            animator = anim;
        }
        if (actions == null && TryGetComponent<PlayerActions>(out var action))
        {
            actions = action;
        }
    }

    void Start()
    {
        MovementSpeed = Animator.StringToHash("MovementSpeed");
        PickUpTrigger = Animator.StringToHash("PickUp");
    }

    void Update()
    {
        if (animator != null && navMeshAgent != null)
        {
            animator.SetFloat(MovementSpeed, navMeshAgent.velocity.magnitude);
        }
    }

    void PlayCollectAnimation()
    {
        if (animator != null) animator.SetTrigger(PickUpTrigger);
        if (actions != null) actions.SetIsCollecting(true);
    }

    //Is called at the end of the PickUp Action
    void EndPickUp()
    {
        if (actions != null) actions.SetIsCollecting(false);
    }
}
