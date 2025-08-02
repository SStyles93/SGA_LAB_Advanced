using UnityEngine;
using UnityEngine.AI;

public class PlayerAnimatorController : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private NavMeshAgent navMeshAgent;

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
        if (navMeshAgent == null) navMeshAgent = GetComponentInParent<NavMeshAgent>();
        if (animator != null) animator = GetComponent<Animator>();
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

    void PlayCollectAnimation(bool state)
    {
        if (!state) return;
        if (animator != null) animator.SetTrigger(PickUpTrigger);
    }

    //Is called at the end of the PickUp Action
    void EndPickUp()
    {
        PlayerInteraction.OnCollect?.Invoke(false);
    }
}
