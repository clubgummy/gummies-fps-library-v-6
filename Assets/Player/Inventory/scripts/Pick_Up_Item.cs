using UnityEngine;

public class Pick_Up_Item : MonoBehaviour
{
    // Reference to the item that this script is attached to
    public Item item;
    
    // Reference to the player's inventory
    public Inventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        // Finds the GameObject named "Inventory" and gets its Inventory component
        inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
    }

    // This function is called when another collider stays within this object's trigger collider
    void OnTriggerStay(Collider other)
    {
        // Checks if there is an item assigned and if the player presses the pick-up key
        if (Input.GetKeyDown(inventory.pickUpItem)) pickUpItem();  // Calls the function to pick up the item
    }

    // Adds the item to the inventory and assigns it to the first available slot
    void pickUpItem()
    {
        
        
        // Iterates through inventory slots to find an empty item slot
        foreach (Inventory_Slot slot in inventory.slots)
        {
            // Checks if the slot is an item slot and is currently empty
            if (slot.type == slot_type.item_slot && slot.item == null)
            {
                // Adds the item to the inventory list
                inventory.items.Add(item);
                slot.item = item; // Assigns the item to the empty slot
                // Destroys the GameObject after the item is picked up to remove it from the scene
                Destroy(gameObject);
                break; // Exits the loop after assigning the item
            }
        }
        foreach (Item item in inventory.items)
        {
            if(item == null) inventory.items.Remove(item);
        }
        
    }

}
