using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class inventoryMouse : MonoBehaviour
{
    [Header("inventory_mouseScript")]
    public GameObject mouse_obj; 
    public Sprite base_image;
    public Color color;
    public Item item;
    
    public TMP_Text item_Name;
    public TMP_Text item_Desctiption;
    public TMP_Text item_stats;


    // Start is called before the first frame update
    void Start()
    {
        item_Name.text =" ";
        item_Desctiption.text = " "; 
        item_stats.text = " ";

    }

    void Update()
    {
        calculateMouse();
        Render();
    }

    void Render()
    {
        if (item == null)
        {
            mouse_obj.GetComponent<Image>().sprite = null;
            color.a = 0;
            mouse_obj.GetComponent<Image>().color = color;
        }
        else 
        {
            mouse_obj.GetComponentInParent<Image>().sprite = item.Icon_ArtWork;
            color.a = 255;
            mouse_obj. GetComponent<Image>().color = color;
        }
    }



    void calculateMouse()
    {
        Vector3 mouse = Input.mousePosition;
        mouse_obj.transform.position = new Vector3(mouse.x, mouse.y, -5);
    }
}
