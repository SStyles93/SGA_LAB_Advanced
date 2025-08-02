using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    [SerializeField] private PlayerInteraction playerInteraction;
    [SerializeField] private PlayerInventoryManager inventoryManager;

    bool isCollecting = false;

    private void Awake()
    {
        playerInteraction = GetComponent<PlayerInteraction>();
        inventoryManager = GetComponent<PlayerInventoryManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
        
        // On I pressed, Open/Close InventoryPannel
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryManager.ToggleInventoryVisibility();
        }

        if (inventoryManager.GetInventoryPannel().activeSelf) return;
        if (isCollecting) return;
        
        // On left-click, try to interact or move.
        if (Input.GetMouseButtonDown(0))
        {
            playerInteraction.HandleLeftClick();
        }
    }

    public void SetIsCollecting(bool values)
    {
        isCollecting = values;
    }
}
