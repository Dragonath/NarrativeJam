using System.Collections;
using System.Threading;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Player_Controller : MonoBehaviour
{
    // Components
    public Rigidbody2D playerRB;
    public BoxCollider2D playerCollider;
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
    public float slopeCheckRadius;
    public float pushValue;
    public float jumpsLeft;
    public float slopeAngle;
    public float maxSlopeAngle;
    public float minSlopeAngle;
    public float fallMultiplier;
    
    // Booleans
    public bool _debug;
    public bool grounded;
    public bool canDash;
    public bool dashing;
    public bool playerHasControl;
    private bool dead;
    public bool isOnSlope;
    public bool ascendingSlope;
    public bool descendingSlope;

    // Unlocks
    public bool dashUnlocked;
    public bool runUnlocked;
    public bool jumpUnlocked;
    public bool walkUnlocked;

    // LayerMasks
    public LayerMask groundLayer;
    public LayerMask enemyLayer;

    public Vector2 boxSize;
    private Vector2 lastVelocity;
    private Vector2 moveDirection;
    private bool jump;
    private bool playerWantsToJump;
    private bool dash;
    private bool moveDown; 
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
        playerCollider = GetComponent<BoxCollider2D>();
        groundCheck = transform.Find("GroundCheck");

        playerCollider.enabled = true;
        playerHasControl = true;

        // probably move somewhere else. 
        dashCooldown = new Cooldown(4f);
    }


    // Update is called once per frame
    void Update()
    {
        // for tracking player speed in inspector
        PlayerSpeedX = playerRB.linearVelocity;

        // Check if player is grounded, if yes reset remaining jumps
        grounded = Physics2D.BoxCast(groundCheck.position, boxSize, 0, -transform.up, groundCheckRadius, groundLayer);
        if(grounded)
        {
            jumpsLeft = maxJumpCount;
        }

        // Get pressed inputs if player has controls and is not dashing 
        if (playerHasControl && !dashing)
        {
            GetInputs();
            if(moveDirection != Vector2.zero)
            {
                playerRB.bodyType = RigidbodyType2D.Dynamic;
            }
        }

        // Check when dashing should be over
        if(dashing)
        {
            dashTime += Time.deltaTime;
            if(dashTime >= maxDashTime)
            {
                dashing = false;
                dashTime = 0;
                playerRB.gravityScale = 9;
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
        
        if(moveDirection.y < 0.2f)
        {
            moveDown = true;
        }
        if (jumpAction.WasPressedThisFrame())
        {
            jump = true;
            StartCoroutine(HoldJump(0.2f));
        }
        if (dashAction.WasPressedThisFrame())
        {
            dash = true;
        }

    }

    private void FixedUpdate()
    {

        MoveCharacter();

        // Reset Variables
        if (!dashing) moveDirection = Vector2.zero;
        moveDown = false;
        jump = false;
         

        // Store old velocity
        lastVelocity = playerRB.linearVelocity;
    }

    private void MoveCharacter()
    {
        // Limit all controls for dash duration
        if(dashing)
        {
            return;
        }

        if (isOnSlope)
        {
            if(moveDirection.x == 0)
            {
                playerRB.bodyType = RigidbodyType2D.Kinematic;
                playerRB.linearVelocity = Vector2.zero;
            }
            AdjustVelocityToSlope(playerRB.linearVelocity);
            
            float yClimb = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * playerSpeed;
            if (descendingSlope) yClimb *= -1;
            float xClimb = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * playerSpeed * Mathf.Sign(moveDirection.x);
            Vector2 climb = new Vector2(xClimb, yClimb);
            playerRB.AddRelativeForce(climb * playerSpeed, ForceMode2D.Force);
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

        if(moveDown)
        {
            Collider2D col = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
            if (col != null)
            {
                if(col.CompareTag("Platform"))
                {
                    StartCoroutine(DisablePlayerCollider(0.5f));
                    playerRB.AddRelativeForceY(moveDirection.y * pushValue, ForceMode2D.Impulse);
                }
            }
        }

        // Keep players speed within maximum speed
        if(playerRB.linearVelocityX > maxSpeed)
        {
            playerRB.linearVelocityX = maxSpeed;   
        }
        else if(playerRB.linearVelocityX < -maxSpeed)
        {
            playerRB.linearVelocityX = -maxSpeed;
        }
        
        // Jump if jump key was pressed and player is on ground. 
        if ((jump || playerWantsToJump) && jumpsLeft > 0)
        {
            playerRB.linearVelocityY = 0; 
            playerRB.AddRelativeForceY(jumpForce, ForceMode2D.Impulse);
            jumpsLeft--;
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
        }
        else if (!faceRight && moveDirection.x == 1)
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            faceRight = true;
        }

        if(playerRB.linearVelocityY < 0.1f && !isOnSlope)
        {
            playerRB.linearVelocity += Vector2.up * Physics2D.gravity.y * fallMultiplier * Time.deltaTime;
        }

    }

    //private void CheckSlope()
    //{
    //    // Check Front
    //    float direction = (faceRight) ? 1 : -1;
    //    RaycastHit2D hit = Physics2D.Raycast(slopeCheckFront.position, Vector2.right * direction, slopeCheckRadius, groundLayer);
    //    if (hit)
    //    {
    //        Vector2 normal = hit.normal;
    //        slopeAngle = Vector2.Angle(normal, Vector2.up);
    //        if (slopeAngle > minSlopeAngle && slopeAngle < maxSlopeAngle)
    //        {
    //            ascendingSlope = true;
    //            descendingSlope = false;
    //            isOnSlope = true;
    //            return;
    //        }
    //    }
    //    // Check rear
    //    hit = Physics2D.Raycast(slopeCheckRear.position, Vector2.down, slopeCheckRadius, groundLayer);
    //    if(hit)
    //    {

    //        Vector2 normal = hit.normal;
    //        slopeAngle = Vector2.Angle(normal, Vector2.up); 
    //        if(slopeAngle > minSlopeAngle && slopeAngle < maxSlopeAngle)
    //        {
    //            ascendingSlope = false;
    //            descendingSlope = true;
    //            isOnSlope = true;
    //            return;
    //        }
    //    }
    //    ascendingSlope = false;
    //    descendingSlope = false;
    //    isOnSlope = false;
    //}

    private Vector2 AdjustVelocityToSlope(Vector2 velocity) 
    {

        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, slopeCheckRadius, groundLayer);
        if(hit)
        {
            var SlopeRotation = Quaternion.FromToRotation(Vector2.up, hit.normal);
            var adjustedVelocity = SlopeRotation * velocity;
            if(adjustedVelocity.y < 0 )
            {
                return adjustedVelocity;
            }
        }
        return velocity;
    
    }
    

    private void Dash()
    {
        dashing = true;
        playerRB.AddRelativeForce(moveDirection * dashSpeed, ForceMode2D.Impulse);
        dashCooldown.StartCooldown();
        canDash = false;

        
    }

    private IEnumerator DisablePlayerCollider(float time)
    {
        playerCollider.enabled = false;
        yield return new WaitForSeconds(time);
        playerCollider.enabled = true;
    }

    public void Knock(float knockback)
    {
        playerRB.linearVelocity = Vector2.zero;

        playerRB.AddRelativeForceX(-knockback, ForceMode2D.Impulse);
        playerRB.AddRelativeForceY(knockback, ForceMode2D.Impulse);
    }

    public void PreserveMomentumOnLanding()
    {
        Debug.Log("Movement preserved");
        playerRB.linearVelocityX = lastVelocity.x;
    }

    private IEnumerator HoldJump(float time)
    {
        playerWantsToJump = true;
        yield return new WaitForSeconds(time);
        playerWantsToJump = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Landing");
        if (collision == null) return;
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) 
        {
            
            if(lastVelocity.y < -1)
            {
                PreserveMomentumOnLanding();
            }
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