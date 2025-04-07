using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

// Enum for mouse click buttons used for shooting
public enum mouseClick
{
    LeftClick = KeyCode.Mouse0,
    RightClick = KeyCode.Mouse1,
    middleClick = KeyCode.Mouse2,
}

// Main class for handling gun settings and behavior
public class Gunsettings : MonoBehaviour
{
    [Header("General")]
    public mouseClick shootButton; // Mouse button used to shoot
    public KeyCode reloadButton = KeyCode.R; // Key used to reload the gun
    public KeyCode Holster = KeyCode.X;
    public bool usingItem = false;  // Indicates if the gun is currently in use

    [Header("Gun Settings")]
    public GunData gunData;
    public GameObject muzzlePos; // Position where bullets are fired from
    [Space]
    public float fireRate = 2;    // Current fire rate counter
    public float currentAmmo  = 0;    // Current bullets left in magazine
    public float reloadTime;

    public List<GameObject> bullets = new List<GameObject>(); // Initialize list

    [Header("Gun Animation")]
    public Animator animator; // Reference to player's animator
    public bool shootingGun = false; // Is the gun currently shooting
    public bool AimingGun = false;      // Is the gun currently aiming
    public bool isReloading = false;

    [Header("Bullet Settings")]
    public GameObject bulletO;       // Bullet prefab
    public float bulletSpeed;        // Bullet's speed when fired
    
    [Header("UI")]
    public TMP_Text bulletCount;
    

    void Start()
    {
        // Initialize magazine size to setMagSize minus 1
        currentAmmo = gunData.maxAmmo;
    }

    // Check if the gun is ready to shoot (has ammo and is being used)
    bool canShoot()
    {
        bool checkMag = currentAmmo > 0; // Check if there's ammo
        bool InUse = usingItem;      // Check if the gun is in use
        return checkMag && InUse;
    }

    private void Update() 
    {
        
        if(bulletCount)bulletCount.text = $"{currentAmmo}/{gunData.maxAmmo}";
        else bulletCount = GameObject.Find("bulletCount").GetComponent<TMP_Text>();

        ControllAnimation();
        ShootGun();

        // Reload magazine if reload button pressed
        if(Input.GetKeyDown(reloadButton) && usingItem) StartCoroutine(Reload());
    
        // Call to destroy bullets that have expired
        destroyBullet();
        // Debug.DrawRay(muzzlePos.transform.position, muzzlePos.transform.forward * 50f, Color.red);
    }


    void ShootGun()
    {
        // Switch between semi-automatic and fully automatic fire modes
        switch (gunData.fireType)
        {
            case FireType.Semi_Automatic:
                // Shoot once per click if conditions are met
                if (canShoot() && Input.GetKeyDown((KeyCode)shootButton)) 
                {
                    shootingGun = true;
                    cloneBullet();
                }
                
                if(Input.GetKeyDown(Holster) && shootingGun == true) shootingGun = false;
            break;
            case FireType.Fully_Automatic:
                // Continuously shoot if conditions are met and fire rate allows
                if (canShoot() && Input.GetKey((KeyCode)shootButton)) 
                {
                    shootingGun = true;
                    fireRate -= Time.deltaTime; 
                    if(fireRate <=0) cloneBullet();               
                }
                else shootingGun = false;
            break;
        }
    }



    // Function for handling the shooting logic
    public void cloneBullet()
    {
        // Decrease magazine size by one per shot
        currentAmmo -= 1;

        GameObject bulletClone = Instantiate(bulletO, muzzlePos.transform.position, muzzlePos.transform.rotation);

        // Add the bullet to the list for tracking
        bullets.Add(bulletClone);
        bulletClone.GetComponent<Rigidbody>().AddForce(muzzlePos.transform.forward * bulletSpeed, ForceMode.Impulse);
        

        fireRate = gunData.setFireRate; // sets fire rate back to what it was originally
        AudioSource.PlayClipAtPoint(gunData.gunShotSound, muzzlePos.transform.position);        
    } 

    // Function to destroy bullets after their duration expires
    public void destroyBullet()
    {
        if (bullets.Count > 0)
        {
            // Iterate backwards through bullet list to safely remove expired bullets
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                GameObject bullet = bullets[i];
                // Reduce bullet duration, destroy bullet if duration is up
                if (bullets[i].GetComponent<Bullet_Data>().Duration > 0) bullets[i].GetComponent<Bullet_Data>().Duration -= Time.deltaTime;

                RaycastHit hit;
                if (Physics.SphereCast(bullets[i].transform.position, 1, bullets[i].transform.forward, out hit, 1f, gunData.bulletcollide) || bullets[i].GetComponent<Bullet_Data>().Duration <= 0)
                {
                    bullets.RemoveAt(i);
                    Destroy(bullet);
                }  
            }   
        }

    }

    public void ControllAnimation()
    {
        // Find and cache the animator if not already done
        if (!animator) animator = GameObject.Find("Rigs").GetComponent<Animator>();
     
        // Set animation states
        if (animator && isReloading == false) animator.SetBool("Shooting", shootingGun);
        if (animator && isReloading == false) animator.SetBool("Aiming", AimingGun);
        if (animator) animator.SetBool("Reload", isReloading);
    }

    IEnumerator Reload ()
    {
    isReloading = true;
    Debug.Log("Reloading...");

    animator.SetBool("Reload", true);

    yield return new WaitForSeconds(reloadTime);

    animator.SetBool("Reload", false);

    currentAmmo = gunData.maxAmmo;
    isReloading = false;
    }

}

