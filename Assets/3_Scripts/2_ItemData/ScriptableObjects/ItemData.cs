using UnityEngine;

/// <summary>
/// This is the base ScriptableObject for all items in the game.
/// It contains the core data that every item must have.
/// </summary>
/*IMPLEMENT: We want to create the menu in the UnityEditor to create an ItemData*/
//TIP: Use the CreateAssetMenu Attribute
//TIP: Give it a file name ex: fileName = "name"
//TIP: And a menu name ex: menuName = "menu/submenu/subsubmenu/aso..."
public class ItemData : ScriptableObject
{
    // By using [Header], we can create categories in the Inspector,
    // making it much easier to read and manage.
    /*IMPLEMENT: A Header for the "Core Item Information"*/

    // [Tooltip] provides a helpful description when a user hovers over the field name.
    /*IMPLEMENT: a Tooltip for this itemName*/
    public string itemName;

    // [TextArea(minLines,maxLines)] allows for a multi-line string field in the Inspector,
    // which is much better for writing descriptions.
    /*UNCOMMENT*///[Tooltip("The description of the item, shown when the player inspects it.")]
    /*IMPLEMENT: a TextArea with min 3 lines and max 5*/
    public string description;

    // [Range] constrains a numerical value between a minimum and maximum.
    // This prevents data entry errors, like setting a negative value.
    /*IMPLEMENT: A range from 0 to 999*/
    public int value;


    //IMPLEMENT: A Space of 15 pixels
    /*IMPLEMENT: a Tooltip for the Icon*/
    /*IMPLEMENT: an icon (use Sprite)*/

    /*IMPLEMENT: a Tooltip for the Prefab*/
    /*IMPLEMENT: a GameObject for the prefab (worldItem)*/



    // [Space] adds a visual gap in the Inspector, which helps to separate
    // different groups of variables for better clarity.
    /*IMPLEMENT: A space of 15 pixels*/

    /*IMPLEMENT: a header with "Item Properties"*/

    /*IMPLEMENT: Add item types, to do so, we will want to CREATE a new script "ItemType.cs" that holds these informations*/

    public bool isStackable = true;


    // This is a PRIVATE variable. By default, it is not visible in the Inspector.
    public string internalId = System.Guid.NewGuid().ToString();
    /* ^ CHANGE*/

    // However, by using [SerializeField], we can expose a private variable
    // to the Inspector. This is good practice because it allows other scripts
    // to access this variable only through controlled public methods,
    // while still allowing designers to set its value in the Editor if needed.
    /*IMPLEMENT: a SerializedField for the items ID*/
    //TIP: this must be a string

    /// <summary>
    /// Public accessor for the private itemID.
    /// This follows the principle of encapsulation.
    /// </summary>
    /*IMPLEMENT: a public accessor*/
    //TIP: Use "=>"

    // This method is automatically called by Unity when the script is first created
    // or when the "Reset" command is used in the Inspector.
    // We use it to assign a unique ID automatically.
    private void Reset()
    {
        // We ensure the ID is unique by combining the asset's name and a GUID.
        //itemID = $"{this.name}_{System.Guid.NewGuid()}";
    }

    // This method is called whenever a value is changed in the Inspector.
    // It's useful for validating data in real-time.
    private void OnValidate()
    {
        //if (string.IsNullOrEmpty(itemID))
        //{
        //    // If the ID is ever cleared by mistake, this will regenerate it.
        //    Reset();
        //}
    }
}
