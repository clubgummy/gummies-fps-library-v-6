using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{   
    [Header("General")]
    public GameObject canvas;
    public bool toggle;

    [Header("Key Binds")]
    public KeyCode toggleInventory = KeyCode.I;
    public KeyCode pickUpItem = KeyCode.E;
    public KeyCode dropItem = KeyCode.Q;
    

    [Header("Data")]
    public List<Inventory_Slot> slots;
    public List<Item> items;
    
    private void Awake() 
    {
        update_lists();
    }

    void Update()
    {
        if(Input.GetKeyDown(toggleInventory) && toggle == false) toggle = true;
        else if(Input.GetKeyDown(toggleInventory) && toggle == true) toggle = false;

        if (toggle == true)
        {
            canvas.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else 
        {
            canvas.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void update_lists()
    {
        if (canvas != null)
        {
            Inventory_Slot[] renderers = canvas.GetComponentsInChildren<Inventory_Slot>();
            if(slots.Count <= renderers.Length)slots.AddRange(renderers);
        }

        foreach (Inventory_Slot slot in slots)
        {
            if(slot.item != null)
            {
                items.Add(slot.item);
            }
        }
    }


}
