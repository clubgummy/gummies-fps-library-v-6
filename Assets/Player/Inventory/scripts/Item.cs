using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public class itemStats
{
    public int Stat_Id;
    public string Name;
    public int value;
}


[CreateAssetMenu (menuName = "Create New Item")]
public class Item : ScriptableObject
{
    public string Name;
    public int id;

    public bool usingItem = false;

    public string description;
    public Sprite Icon_ArtWork;
    public item_type item_Type;

    public GameObject Object;

    [SerializeField]public List<itemStats> stats = new List<itemStats>();

}
