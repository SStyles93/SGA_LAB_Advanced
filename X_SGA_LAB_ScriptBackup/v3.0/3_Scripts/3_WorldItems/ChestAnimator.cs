using UnityEngine;

public class ChestAnimator : MonoBehaviour
{
    [SerializeField] Animator animator;

    //Hashes of the animation parameters
    int isOpenHash = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isOpenHash = Animator.StringToHash("isOpen");
    }

    public void Update()
    {
        if (TreasureChest.isOpen)
        {
            animator.SetBool(isOpenHash, true);
        }
        else
        {
            animator.SetBool(isOpenHash, false);
        }
    }
}
