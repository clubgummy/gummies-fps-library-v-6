using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections;
using System.ComponentModel;

public class Inventory_Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("General")]
    public Inventory inventory;
    public GameObject player;
    public item_type sub_Type;
    public slot_type type;
    public bool hover;
    
    
    [Header("Item")]
    public GameObject image;
    public Sprite base_image;
    public Item item = null;

    public GameObject physical_item;
    public Transform parentObject; 

    [Header("Mouse")]
    public inventoryMouse mouse;

    [Header("UI")]
    public GameObject bulletUiGroup;
    public GameObject sub_Inventory;



    bool canAttatch()
    {
        bool checkSlot = type == slot_type.item_slot;
        bool checkSubType = sub_Type == mouse.item.item_Type;
        return checkSlot || checkSubType;
    }

    void Start()
    {
        Render();
        mouse = GameObject.Find("Mouse Info").GetComponent<inventoryMouse>();
        inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
        player = GameObject.Find("Player");
    }


    void Update()
    {
        Render();

        if(!mouse)mouse = GameObject.Find("Mouse Info").GetComponent<inventoryMouse>();
        if(!inventory)inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
        if(!player)player = GameObject.Find("Player");

        if(Input.GetKeyDown(inventory.dropItem) && item != null && hover) dropItem();
        
        if (physical_item != null && physical_item.GetComponent<Gunsettings>() != null) bulletUiGroup.SetActive(true);
        else bulletUiGroup.SetActive(false);
    }

   

    public void slotControll()
    {
        if(mouse.item == null) moveItem();
        else if (item == null && canAttatch()) attachItem();
        generate_item();
    }

    void Render()
    {
        if (item != null) image.GetComponent<Image>().sprite = item.Icon_ArtWork;
        else image.GetComponent<Image>().sprite = base_image;

        if (hover && item != null)
        {
            mouse.item_Name.text = $"{item.Name}";
            mouse.item_Desctiption.text = $"{item.description}"; 
            // Clear the stats text first
            mouse.item_stats.text = "";
            // Check if stats exist and then loop through them
            if (item.stats != null && item.stats.Count > 0)
            {
                foreach (itemStats stat in item.stats)
                {
                    mouse.item_stats.text += $"{stat.Name} {stat.value}\n";
                }
            }
            mouse.transform.position = Input.mousePosition;
        }
        else
        {
            mouse.item_Name.text = "";
            mouse.item_Desctiption.text = "";
            mouse.item_stats.text = "";
            mouse.transform.position = Input.mousePosition;
        }
    }

    void generate_item()
    {
        if(item != null && type==slot_type.Gear_slot && item.item_Type == sub_Type)
        {
            physical_item = Instantiate(item.Object, parentObject.transform.position, parentObject.transform.rotation, parentObject);
            if(physical_item.GetComponent<Gunsettings>() != null)  physical_item.GetComponent<Gunsettings>().usingItem = true;
            physical_item.GetComponent<SphereCollider>().enabled = false; 
            Debug.Log($"Clone {item}"); 
        }
        else if(item == null && type==slot_type.Gear_slot)
        {
            if(parentObject.childCount > 0 && parentObject) Destroy(parentObject.GetChild(0).gameObject);
            Debug.Log($"destroyed {item}");
        }
    }


    void moveItem()
    {
        mouse.item = item;
        item = null;
    }

    void attachItem()
    {
        item = mouse.item;
        mouse.item = null;
    }

    void dropItem()
    {
        inventory.items.Remove(item);
        GameObject Item_obj = Instantiate(item.Object, player.transform.position, player.transform.rotation);
        if(parentObject.childCount > 0) Destroy(parentObject.GetChild(0).gameObject);
        item = null;
    }    



    // This method is triggered when the mouse enters the UI element
    public void OnPointerEnter(PointerEventData eventData) {hover = true;}
    // This method is triggered when the mouse exits the UI element
    public void OnPointerExit(PointerEventData eventData) {hover = false;}
    
}
