using UnityEngine;
using Unity.Cinemachine;



public class Player_Camera : MonoBehaviour
{
    [Header("Camera settings")]
    public float playerTurnSpeed;
    public float cameraTurnSpeed; // Variable for camera turn speed
    public Camera cam;
    public Transform CameraLookAt;
    public Transform player;

    public CinemachineCamera Virtualcam;
    [System.Obsolete]
    public AxisState xAixis;
    [System.Obsolete]
    public AxisState yAixis;



    [Header("Rigging")]
    public GameObject LAT;
    Ray ray;

    public bool isAiming;
    int isAimingParam = Animator.StringToHash("isAiming");

    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        cam = FindAnyObjectByType<Camera>();
        anim = GetComponent<Animator>();    
    }
    

    void Reset() {
        Virtualcam = gameObject.AddComponent<CinemachineCamera>();
    }

    // Update is called once per frame
    [System.Obsolete]
    private void FixedUpdate() 
    {
        rotateCamera();
        rotatePlayer();
    }

    void Update()
    {
        headControll();
        isAiming = Input.GetMouseButton(1);
        // anim.SetBool(isAimingParam, isAiming);
    }

    [System.Obsolete]
    public void rotateCamera()
    {
        // Update the axis values without modifying the m_MaxSpeed
        xAixis.Update(Time.fixedDeltaTime);
        yAixis.Update(Time.fixedDeltaTime);
        

        // Scale the axis values by cameraTurnSpeed
        float xRotation = xAixis.Value * cameraTurnSpeed;
        float yRotation = yAixis.Value * cameraTurnSpeed;

        // Apply0 the scaled values to the camera rotation
        CameraLookAt.eulerAngles = new Vector3(yRotation, xRotation, 0);
    }



    public void rotatePlayer()
    {
        float yawCam = cam.transform.rotation.eulerAngles.y;
        player.rotation = Quaternion.Slerp(player.rotation, Quaternion.Euler(0, yawCam, 0), playerTurnSpeed * Time.fixedDeltaTime);
    }


    public void headControll()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition); // You can change the origin and direction of the ray as needed
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) ) LAT.transform.position = hit.point;
    }
}
