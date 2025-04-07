using Unity.VisualScripting;
using UnityEngine;

public class ControllRig : MonoBehaviour
{

    [Header("Animation References")]
    [Tooltip("Main animator for controlling character animations.")]
    public Animator animator;
    [Tooltip("Animator specifically used for gun animations.")]
    public Animator gunAnimator;

    [Header("Game Objects")]
    [Tooltip("Reference to the player's main GameObject.")]
    public GameObject player;
    [Tooltip("Reference to the item the player is currently holding.")]
    public GameObject Item;
    [Tooltip("Parent object that holds the player's current item.")]
    public GameObject parent;
    public CharacterController controller;

    [Header("Status Booleans")]
    [Tooltip("Tells whether the player is currently shooting.")]
    public bool Shooting;
    private bool _hasAnimator;
    [SerializeField] bool crouch;

    [Header("Internal Strings")]
    [Tooltip("Used to switch between different gun animation states.")]
    string toggle;

    [Header("Player Input")]
    public float jumpTimeoutDelta;
    public float fallTimeoutDelta;
    

    [Tooltip("Stores horizontal (x) and vertical (y) input values.")]
    Vector2 input;

    
    
    [Header("Animation ID References")]
    [Tooltip("Integer ID for the speed parameter.")]
    [SerializeField]private int _animIDSpeed;
    [Tooltip("Integer ID for the grounded parameter.")]
    private int _animIDGrounded;
    [Tooltip("Integer ID for the jump parameter.")]
    private int _animIDJump;
    [Tooltip("Integer ID for the free-fall parameter.")]
    private int _animIDFreeFall;
    [Tooltip("Integer ID for the motion speed parameter.")]
    private int _animIDMotionSpeed;

    [Header("Sound")]
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;
    public AudioClip[] FootstepAudioClips;
    public AudioClip LandingAudioClip;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        _hasAnimator = TryGetComponent(out animator);
    }

    // Update is called once per frame
    void Update()
    {   
        _hasAnimator = TryGetComponent(out animator);
        movementController();  
        jumpingController();
        GunController();

    }


    void GunController()
    {
        // bool Aiming = gunAnimator.GetBool("Aiming");
        if (parent.transform.childCount == 1) Item = parent.transform.GetChild(0).gameObject;
        if(Item != null && Item.GetComponent<Gunsettings>()) Shooting = Item.GetComponent<Gunsettings>().shootingGun;

        if (Item != null && Item.GetComponent<Gunsettings>() != null) 
        {
            if(Item.GetComponent<Gunsettings>().isReloading == false)
            {
                if(Shooting) toggle = "(S)";
                else toggle = "(H)";
                gunAnimator.Play(Item.GetComponent<Gunsettings>().gunData.gunType.ToSafeString() + toggle);
            }
            // Debug.Log(Item.GetComponent<Gunsettings>().gunData.gunType.ToSafeString() + toggle);
        }
        else if (Item == null) gunAnimator.Play("UnArmed");
        
        
    }

    void movementController()
    {        
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");

        if(player == null) player = GameObject.Find("Player");
        
        float horizontalInput = Input.GetAxis("Horizontal");  // Get horizontal input (A/D or Left/Right Arrow)
        float verticalInput = Input.GetAxis("Vertical");      // Get vertical input (W/S or Up/Down Arrow)
        float inputMagnitude = player.GetComponent<Player_Controller>().inputMagnitude; 
        float animationBlend = player.GetComponent<Player_Controller>().animationBlend;
        
        if(Input.GetKey(player.GetComponent<Player_Controller>().crouchKey) && !Input.GetKey(player.GetComponent<Player_Controller>().sprintKey)) crouch = true;
        else crouch = false;

        input.x = horizontalInput;
        input.y = verticalInput;

        // animator.SetFloat("horizontal", input.x);
        // animator.SetFloat("vertical", input.y);
        animator.SetFloat(_animIDSpeed, animationBlend);
        animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        animator.SetBool("Crouch", crouch);
    }

   public void jumpingController()
    {
        // IDs for our jump-related animator bools
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump     = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");

        // These booleans/values come from the Player_Controller script
        bool Grounded = player.GetComponent<Player_Controller>().on_Ground;
        bool Jumped   = player.GetComponent<Player_Controller>().has_jumped;
        float FallTimeout = player.GetComponent<Player_Controller>().FallTimeout;
        float JumpTimeout = player.GetComponent<Player_Controller>().JumpTimeout;

        animator.SetBool(_animIDGrounded, Grounded);

        // If the player is grounded...
        if (Grounded)
        {
            // Reset the fall timeout timer
            fallTimeoutDelta = FallTimeout;

            if (_hasAnimator)
            {
                animator.SetBool(_animIDJump, false);
                animator.SetBool(_animIDFreeFall, false);
            }

            // If we detect a jump event from Player_Controller, set Jump true
            if (Jumped && _hasAnimator) animator.SetBool(_animIDJump, true);

            // Countdown jump timeout
            if (jumpTimeoutDelta >= 0.0f) jumpTimeoutDelta -= Time.deltaTime;
        }
        else
        {
            // If not grounded, reset the jump timeout
            jumpTimeoutDelta = JumpTimeout;

            if (fallTimeoutDelta >= 0.0f)
            {
                fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                if (_hasAnimator) animator.SetBool(_animIDFreeFall, true);
            }
        }
    }

    private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(controller.center), FootstepAudioVolume);
                }
            }
        }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(controller.center), FootstepAudioVolume);
        }
    }

   
    
}
