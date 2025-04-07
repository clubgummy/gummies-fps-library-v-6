using UnityEngine;

public class SlideController : MonoBehaviour
{
    [Header("References")]
    public GravityController gravityController;
    public CharacterController controller;
    public Player_Controller pController;
    public Transform orientation;
    public Transform cameraTransfrom;
    public GameObject Armature;

    [Header("Sliding")]
    public float slideForce = 0;
    public float slideDuration = 0;
    [HideInInspector]public float sd = 0;
    public bool sliding  = false;

    [Header("Inputs")]
    float verticalInput;
    float horizontalInput;

 

  
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");

        if(Input.GetKey(pController.sprintKey) && Input.GetKeyDown(pController.crouchKey) && !pController.Idle(horizontalInput, verticalInput) && pController.on_Ground && !pController.has_jumped) StartSlide();
        if(Input.GetKeyUp(pController.sprintKey) && Input.GetKeyUp(pController.crouchKey) && sliding ) StopSlide();
    }


    private void FixedUpdate()
    {
        if(sliding) SlidingMovement(verticalInput, horizontalInput);
    }

    void StartSlide()
    {
        sliding = true;
        controller.Move(Vector3.down * 5);
        sd = slideDuration;
    }


    void SlidingMovement(float verticalInput, float horizontalInput)
    {
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (cameraTransfrom) 
        {
            Vector3 camForward = cameraTransfrom.forward;
            camForward.y = 0;
            camForward.Normalize();

            inputDir = (camForward * inputDir.z) + (cameraTransfrom.right * inputDir.x);
        }
        controller.Move(inputDir * slideForce);
        sd  -= Time.deltaTime;

        if (!pController.has_jumped && !pController.on_Ground) gravityController.ApplyGravity(controller, pController.jumpForce);
        if(sd <= 0) StopSlide();
    }


    void StopSlide()
    {
        sliding = false;
    }

}
