using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum gunHand
{
    LeftHand,
    RightHand,
}

[System.Serializable]
public class Mag
{
    public GameObject Mag1;
    public float setTimeToDestroyMag;
    public float timeToDestroyMag;
}

// This class handles the logic for reloading a gun by managing magazine attachment and detachment
public class Reload : MonoBehaviour
{
   
    [Header("General")]
    public Transform transformParent;
    public GameObject magHand;
  
    [Header("Gun settings")]
    public GameObject Gun;
    public GameObject Mag;
    public GameObject newMag;
    public List<Mag> mags;

    void Update()
    {
         // If we have a valid parent transform that has at least one child...
        if (transformParent != null && transformParent.childCount > 0) Gun = transformParent.GetChild(0).gameObject;
        // If we have a gun, try to get its Gunsettings component,
        // so we can read which Mag prefab it uses.
        if (Gun != null)
        {
            var gunSettings = Gun.GetComponent<Gunsettings>();
            if (gunSettings != null) Mag = Gun.transform.GetChild(0).gameObject;
        }
        // If our list has any mags to destroy, call the method.
        if (mags.Count > 0) destroyMag();
    }

    // Attaches a new mag to the gun by instantiating the Mag prefab as a child of the Gun.
    public void attachMagToGun()
    {
        Mag.SetActive(true);
        // If newMag still exists for some reason, destroy it first.
        if (newMag != null) Destroy(newMag);
        Debug.Log("mag attatched");

    }


    // Detaches the mag from the gun (instantiates a mag prefab in the world).
    public void detachMagFromGun()
    {
        Mag.SetActive(false);
        // Spawn a mag at the Gun's transform, but unparent it so it falls.
        newMag = Instantiate(Mag, Gun.transform);
        newMag.transform.SetParent(null);

        var col = newMag.GetComponent<SphereCollider>();
        if (col != null) col.enabled = true;

        var rb = newMag.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        // Create a tracking entry for this mag so we can destroy it after some time.
        Mag bMag = new Mag
        {
            Mag1 = newMag,
            setTimeToDestroyMag = 5f,
            timeToDestroyMag = 5f
        };

        mags.Add(bMag);
        newMag = null;
        Debug.Log("mag detatched");

    }

    // Instantiates a mag in the hand position (for picking up from a "bag" or inventory).
    public void pickUpMagFromBag()
    {
        if (magHand == null)
        {
            Debug.LogWarning("magHand is not assigned.");
            return;
        }

        newMag = Instantiate(Mag, magHand.transform);

        // If you intend (0,0,0) in local coordinates of the magHand, use localPosition instead:
        newMag.transform.localPosition = Vector3.zero;
        Debug.Log("picked up mag");

    }


    // Counts down the lifetime of each detached mag and destroys it once time hits 0.
    void destroyMag()
    {
        // Iterate backwards so removing an item won't break the loop index.
        for (int i = mags.Count - 1; i >= 0; i--)
        {
            mags[i].timeToDestroyMag -= Time.deltaTime;
            if (mags[i].timeToDestroyMag <= 0f)
            {
                // Destroy the mag GameObject in the scene
                Destroy(mags[i].Mag1);

                // Remove that entry from the list
                mags.RemoveAt(i);
                Debug.Log("mag destroyed");

            }
        }
    }





}
