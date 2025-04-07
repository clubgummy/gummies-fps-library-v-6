using UnityEngine;

// Ensures this script's GameObject has a GravityController component
[RequireComponent(typeof(GravityController))]
public class Player_Controller : MonoBehaviour
{
    [Header("References")]
    public GravityController gravityController;
    public SlideController slideController;
    public CharacterController controller;
    public Transform cameraTransform;
    public Transform Armature;

    [Header("Inputs")]
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.C;


    [Header("Movement")]
    public float speed;
    [SerializeField] float walkSpeed = 3;
    [SerializeField] float SprintSpeed = 6;
    [SerializeField] float crouchSpeed = 2;
    [SerializeField] float SpeedChangeRate = 10.0f;



    Vector3 movement;

    public bool isSprinting()
    {
        bool checkSprintKey = Input.GetKey(sprintKey);
        bool checkCrouchKey = Input.GetKey(crouchKey);
        return checkSprintKey && !checkCrouchKey;
    }

    public bool Idle(float horizontalInput, float verticalInput)
    {
        return horizontalInput == 0 && verticalInput == 0;
    }

    [Header("Jump")]
    public float jumpForce;
    public float jumpHeightPeak;
    public float playerVelocity;

    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.50f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;

    public bool has_jumped = false;
    public bool jumpHeightMaxed = false;
    public bool on_Ground = false;

    public Transform GroundCollider;
    public float sphereCast_Radius;
    public float max_Distance = 0.5f;
    public LayerMask groundDetection;

    [Header("Animation")]
    [SerializeField] float inputMagnitudeMultiplyer;
    public float animationBlend;
    public float inputMagnitude;
    
    void Start() {}

    void Update()
    {
        if(!slideController.sliding)
        {
            Move(controller);
            Jumping(gravityController, controller, jumpForce);
        }
        else
        {
            speed = 8;
            animationBlend = speed;
        }
    }

    public void Move(CharacterController controller)
    {
        if (controller != null)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            speed = Idle(horizontalInput, verticalInput) ? 0 : (isSprinting() ? SprintSpeed : (Input.GetKey(crouchKey) ? crouchSpeed : walkSpeed));

            if(Input.GetKey(crouchKey)) enterCrouch();
            else exitCrouch();

            movement = new Vector3(horizontalInput, 0, verticalInput) * speed * Time.deltaTime;
            if(cameraTransform) movement = cameraTransform.forward * movement.z + cameraTransform.right * movement.x;
            movement.y = 0;
            controller.Move(movement);

            Armature.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                       
            inputMagnitude = movement.magnitude * inputMagnitudeMultiplyer;
            controller.Move(movement);
        }

        animationBlend = Mathf.Lerp(animationBlend, speed, Time.deltaTime * SpeedChangeRate);
        if (animationBlend < 0.01f) animationBlend = 0f;
    }

    public void enterCrouch()
    {
        controller.center = new Vector3(0,0.5f,0);
        controller.height = 1;
    }

    public void exitCrouch()
    {
        controller.center = new Vector3(0, 1f,0);
        controller.height = 2;
    }

    public void Jumping(GravityController gravityController, CharacterController controller, float verticalSpeed)
    {
        RaycastHit hit;
        Vector3 p1 = GroundCollider.position;
        on_Ground = Physics.SphereCast(p1, sphereCast_Radius, Vector3.down, out hit, max_Distance, groundDetection);

        playerVelocity = gravityController.velocity;

        if (playerVelocity >= jumpHeightPeak) jumpHeightMaxed = true;

        if (has_jumped)
        {
            if (!jumpHeightMaxed) gravityController.Apply_InversGravity(controller, verticalSpeed);
            else gravityController.ApplyGravity(controller, verticalSpeed);
        }

        if (on_Ground)
        {
            if (Input.GetKeyDown(jumpKey)) has_jumped = true;
            if (jumpHeightMaxed)
            {
                gravityController.velocity = 0f;
                jumpHeightMaxed = false;
                has_jumped = false;
            }

            jumpHeightPeak = verticalSpeed / -gravityController.gravity.y;
        }

        if (!has_jumped && !on_Ground) gravityController.ApplyGravity(controller, verticalSpeed);
    }

    private void OnDrawGizmos()
    {
        if (GroundCollider != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(GroundCollider.position, sphereCast_Radius);

            Gizmos.color = Color.red;
            Gizmos.DrawRay(GroundCollider.position, Vector3.down * max_Distance);
        }
    }
}
