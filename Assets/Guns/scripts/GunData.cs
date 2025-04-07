using System;
using UnityEngine;

[CreateAssetMenu(menuName = "gun data")]
public class GunData : ScriptableObject
{
    // Name of the gun
    public string GunName;
    // Type/category of the gun
    public GunType gunType;
    // Fire mode (e.g., Semi or Fully Automatic)
    public FireType fireType;
    public LayerMask bulletcollide;
    // Set fire rate (time between shots)
    public float setFireRate = 2; 
    // Maximum magazine size
    public float maxAmmo = 0;

    [Range(0, 1)]public float gunShotVolume;
    public AudioClip gunShotSound;

    
}
