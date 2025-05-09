using System.Collections;
using System.Threading;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
    // Components
    public Rigidbody2D playerRB;
    public Collider2D playerCollider;
    public Transform groundCheck;
    
    // Numeric variables
    public int maxHealth;
    public int currentHealth;
    public int maxJumpCount;
    public float playerSpeed;
    public float maxSpeed;
    public float jumpForce;
    public float dashSpeed;
    public float dashTime;
    public float maxDashTime;
    public Cooldown dashCooldown;
    public float airFrictionMultiplier;
    public float groundCheckRadius;


    // Booleans
    public bool _debug;
    public bool grounded;
    public bool canDash;
    public bool dashing;
    public bool playerHasControl;
    private bool dead;

    // Unlocks
    public bool dashUnlocked;
    public bool runUnlocked;
    public bool jumpUnlocked;
    public bool walkUnlocked;

    // LayerMasks
    public LayerMask groundLayer;
    public LayerMask enemyLayer;

    private Vector2 moveDirection;
    private bool jump;
    private bool dash;
    private bool faceRight = true;

    // Input Actions
    InputAction moveAction;
    InputAction jumpAction;
    InputAction dashAction;

    // Reference to players velocity on X-axis so we can track it in inspector
    public Vector2 PlayerSpeedX;

    public static Player_Controller instance; // Static instance of the Player_Controller class

    void Awake()
    {
        // Check if an instance of SoundManager already exists
        if (instance == null)
        {
            instance = this; // Assign this instance to the static instance
            DontDestroyOnLoad(gameObject); // Prevent this object from being destroyed on scene load
        }
        else
        {
            Destroy(gameObject); // Destroy this object if another instance already exists
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get references for Input actions
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        dashAction = InputSystem.actions.FindAction("Dash");

        // Get references to required compontent and make sure player's collider is enabled. 
        playerRB = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        groundCheck = transform.Find("GroundCheck");
        playerCollider.enabled = true;
        playerHasControl = true;

        // probably move somewhere else. 
        dashCooldown = new Cooldown(4f);
    }


    // Update is called once per frame
    void Update()
    {
        PlayerSpeedX = playerRB.linearVelocity;

        // Get pressed inputs if player has controls and is not dashing 
        if (playerHasControl && !dashing)
        {
            GetInputs();
        }

        if (_debug)
        {
            Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckRadius);
        }
        // Check when dashing should be over
        if(dashing)
        {
            dashTime += Time.deltaTime;
            if(dashTime >= maxDashTime)
            {
                dashing = false;
                dashTime = 0;
                playerRB.gravityScale = 1;
            }
        }
        
        // Check if next dash is available 
        if (!canDash)
        {
            if (!dashCooldown.isOnCooldown) 
            {
                canDash = true;
            }
        }
    }

    private void GetInputs()
    {
        moveDirection = moveAction.ReadValue<Vector2>();
        if (jumpAction.IsPressed())
        {
            jump = true;
        }
        if (dashAction.WasPressedThisFrame())
        {
            dash = true;
        }

    }

    private void FixedUpdate()
    {
        CheckGround();
        MoveCharacter();

        // Reset Variables
        if (!dashing)
        {
            moveDirection = Vector2.zero;
        }

        jump = false;
        dash = false;
    }

    private void MoveCharacter()
    {
        // Limit all controls for dash duration
        if(dashing)
        {
            return;
        }
        // normal movement if player is on ground. On air we want to limit controls
        if(grounded)
        {
        playerRB.AddRelativeForceX(
            moveDirection.x * playerSpeed, ForceMode2D.Impulse);
        }
        else
        {
            playerRB.AddRelativeForceX(
    moveDirection.x * playerSpeed * airFrictionMultiplier, ForceMode2D.Impulse);
        }

        // Keep players speed within maximum speed
        if(playerRB.linearVelocityX > maxSpeed)
        {
            playerRB.linearVelocityX =  maxSpeed;   
        }
        else if(playerRB.linearVelocityX < -maxSpeed)
        {
            playerRB.linearVelocityX = -maxSpeed;
        }
        
        // Jump if jump key was pressed and player is on ground. 
        if (jump && grounded)
        {
            playerRB.linearVelocityY = 0; 
            playerRB.AddRelativeForceY(jumpForce, ForceMode2D.Impulse);
            grounded = false;
        }

        // Dash if dash key was pressed, player is pressing directional key (left or right) and player can dash. Player cannot dash while standing still.
        if (dash && moveDirection != Vector2.zero && canDash)
        {
            Dash();
        }

        // Rotate player depending where players is trying to go.
        if (faceRight && moveDirection.x == -1)
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
            faceRight = false;
            Debug.Log("Flipped character to face left");
        }
        else if (!faceRight && moveDirection.x == 1)
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            faceRight = true;
            Debug.Log("Flipped character to face right");
        }

    }

    private void CheckGround()
    {
        if (grounded) return;

        if (Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer))
        {
            grounded = true;
            Debug.Log("Landed");

        }
    }  
    private void Dash()
    {
        dashing = true;
        playerRB.AddRelativeForceX(moveDirection.x * dashSpeed, ForceMode2D.Impulse);
        dashCooldown.StartCooldown();
        canDash = false;
        playerRB.gravityScale = 0;
        playerRB.linearVelocityY = 0;
    }

    private void FlipTransform()
    {
        if(faceRight && moveDirection.x == -1)
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
            faceRight = false;
        }
        else if (!faceRight && moveDirection.x == 1) 
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            faceRight = true;
        }
    }
}
    
public class Cooldown
{ 
    /// <param name="n">Numeric value of the cooldown</param>
    public Cooldown(float n)
    {
        cooldownTime = n;
    }
    [SerializeField] private float cooldownTime;
    private float _nextAbilityTime;

    public bool isOnCooldown => Time.time < _nextAbilityTime;
    public void StartCooldown() => _nextAbilityTime = Time.time + cooldownTime;
}