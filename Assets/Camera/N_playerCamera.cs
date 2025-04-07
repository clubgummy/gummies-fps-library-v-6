using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;  
using TMPro;
using System;

public class N_playerCamera : MonoBehaviour
{
    [Header("player settings")]
    public Transform player;
    public Transform cameraConstraint;
    
    public float playerTurnSpeed;
    public bool canRotatePlayer;

    [Header("camera settings")]
    public Camera cam;
    public Transform trackingTarget;
    public Transform AimConstraint;
    
    public LayerMask targetLayerMask; // Assign this in the Inspector
    public Perspective Perspective;
    public GameObject first;
    public GameObject third;

    public float sensitivity = 100f;

    public float minVerticalAngle = -90;
    public float maxVerticalAngle = 90f;

    float verticalRotation = 0;
    [Header("CrossHair")]
    public CrossHair crossHair;
    public GameObject crosshairUI;
    public Sprite crosshairSprite;
    public string crossHair_name;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GetComponent<Camera>();
        AimConstraint = GameObject.Find("AimConstraint")?.transform; // Try to find it dynamically
    }



    // Update is called once per frame
    void Update()
    {
        if (canRotatePlayer) playerRotation();
        cameraRotation();
        if(AimConstraint != null)spineTracking();
        controllePerspective();

        if(Input.GetKeyDown(KeyCode.V) && Perspective == Perspective.firstPerson) Perspective = Perspective.thirdPerson;
        else if(Input.GetKeyDown(KeyCode.V) && Perspective == Perspective.thirdPerson) Perspective = Perspective.firstPerson;
        crossHair_name = crossHair.ToString();
        loadCrossHair();

        if(cameraConstraint != null) trackingTarget.position = cameraConstraint.position;
        // Gizmos.DrawSphere(AimConstraint.transform.position, 1);

        Debug.DrawRay(cam.transform.position, cam.transform.forward * 50, Color.yellow);
    }

    private void playerRotation()
    {
        float yawCam = cam.transform.rotation.eulerAngles.y;
        player.rotation = Quaternion.Slerp(player.rotation, Quaternion.Euler(0, yawCam, 0), playerTurnSpeed * Time.fixedDeltaTime);
    }

    public void cameraRotation()
    {
        // Get mouse input
        float horizontalRotation = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float verticalDelta = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        // Accumulate vertical rotation and clamp it
        verticalRotation -= verticalDelta; // Invert to match typical FPS behavior
        verticalRotation = Mathf.Clamp(verticalRotation, minVerticalAngle, maxVerticalAngle);
        // Apply rotation to the tracking target
        if(trackingTarget)trackingTarget.eulerAngles = new Vector3(verticalRotation, trackingTarget.eulerAngles.y + horizontalRotation, 0f);
    }


    public void spineTracking()
    {
        if (AimConstraint == null) return; // Safety check

        RaycastHit hit;
        // Cast from the camera's position straight ahead
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        // If the ray hits something
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayerMask))
        {
            AimConstraint.position = hit.point;
        }
        else
        {
            // Optionally place AimConstraint a fixed distance away if no collision
            AimConstraint.position = cam.transform.position + cam.transform.forward * 100f;
        }
    }


    public void controllePerspective()
    {
        switch (Perspective)
        {
            case Perspective.firstPerson:
                cameraConstraint = GameObject.Find("First Person Camera Constraint").transform;
                first.gameObject.SetActive(true);
                third.gameObject.SetActive(false);
            break;
            
            case Perspective.thirdPerson:
                cameraConstraint = GameObject.Find("Third Person Camera Constraint").transform;
                third.gameObject.SetActive(true);
                first.gameObject.SetActive(false);

            break;
        }
    } 

    public void loadCrossHair()
    {
        if (crosshairUI == null)
        {
            Debug.LogError("crosshairUI is NULL! Assign it in the Inspector.");
            return;
        }

        crossHair_name = crossHair.ToString(); // Ensure it's a valid name
        // Debug.Log("Loading Crosshair: " + crossHair_name);

        crosshairSprite = Resources.Load<Sprite>($"Crosshairs/{crossHair_name}");

        if (crosshairSprite == null)
        {
            Debug.LogError($"Crosshair '{crossHair_name}' not found! Ensure it's inside 'Resources/Player/Crosshairs/'.");
            return;
        }

        crosshairUI.GetComponent<Image>().sprite = crosshairSprite;
        // Debug.Log("Crosshair successfully loaded!");
    }


    private void OnDrawGizmos()
    {
        if (AimConstraint != null)
        {
            Gizmos.color = Color.red; 
            Gizmos.DrawSphere(AimConstraint.position, 0.2f);
        }
    }


}
