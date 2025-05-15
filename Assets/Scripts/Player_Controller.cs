using System.Collections;
using System.Net;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class Player_Controller : MonoBehaviour
{
    // -- PUBLIC -- // 

    [Header("Components")]
    public Rigidbody2D rb;
    public BoxCollider2D boxCollider;
    public Transform groundCheck;
    public Animator animator;

    [Header("Speeds")]
    // Track player velocity
    public Vector2 playerVelocity;

    public float maxSpeed;
    public float moveSpeed;
    public float jumpSpeed;
    public float dashSpeed;
    public float wallSlidingSpeed;

    [Header("Times")]
    public float dashTime;
    public float inputHoldTime;
    public float wallJumpTime;

    [Header("Air Friction Multiplier")]
    public float airFriction;

    [Header("Player stat")]
    public int maxHealth;
    public int currentHealth;
    public float maxJumps;

    [Header("Radiuses")]
    public float groundCheckRadius;
    public float wallCheckRadius;

    [Header("Booleans")]
    public bool grounded;
    public bool isOnSlope;
    public bool isOnWall;
    public bool isDashing;
    public bool isJumping;
    public bool DEBUG;
    public bool playerHasControl;
    public bool playerWantsToJump;

    [Header("Layer Mask")]
    public LayerMask groundLayer;

    [Header("Vectors")]
    public Vector2 boxSize;
    public Vector2 moveDirection;

    // -- PRIVATE -- // 
    private float jumpsLeft;
    private int wallJumpDirection;
    private float wallJumpCounter;
    private bool jump;
    private bool dash;
    private bool moveDown;
    private bool facingRight;


    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction dashAction;

    private Vector2 lastVelocity;
    private Cooldown dashCooldown;
    private HoldInput jumpHold;

    [Header("Unlocks")]
    public bool dashUnlocked;
    public bool runUnlocked;
    public bool jumpUnlocked;
    public bool walkUnlocked;

    public static Player_Controller instance;
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
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        dashAction = InputSystem.actions.FindAction("Dash");

        dashCooldown = new Cooldown(2f);
        jumpHold = new HoldInput(inputHoldTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDashing || !playerHasControl || !walkUnlocked)
        {
            return;
        }

        if (DEBUG)
        {

        }

        // Track player velocity in Inspector
        playerVelocity = rb.linearVelocity;
        // Store old velocity
        lastVelocity = rb.linearVelocity;
        animator.SetFloat("Velocity_Y", rb.linearVelocityY);

        // Player input is checked 
        moveDirection = moveAction.ReadValue<Vector2>();
        if (moveDirection.y < -0.2f)
        {
            moveDown = true;
        }

        // Check if player is standing on ground
        grounded = Physics2D.BoxCast
            (groundCheck.position, boxSize, 0, -transform.up, groundCheckRadius, groundLayer);

        // reset jumps if player is on ground
        if (grounded)
        {
            isJumping = false;
            jumpsLeft = maxJumps;
        }

        // Check for dash imput and if dash is on cooldown. Player cannot dash while giving no input
        if (dashAction.WasPressedThisFrame() && dashUnlocked)
        {

            dash = true;
        }

        // check for jump input
        if (jumpAction.WasPressedThisFrame() && jumpUnlocked)
        {
            jump = true;
            playerWantsToJump = true;
            jumpHold.StartHold();
        }

        if (!jumpHold.isOnHold)
        {
            playerWantsToJump = false;
        }

        CheckWall();
        SetAnimation();
        jump = false;
    }

    private void FixedUpdate()
    {
        if (isDashing || !playerHasControl)
        {
            return;
        }

        MovePlayer();
        // Reset inputs if necessary
        jump = false;
        dash = false;
        moveDown = false;

    }

    private void MovePlayer()
    {
        // Apply movement
        if (grounded)
        {
            rb.AddForceX(moveDirection.x * moveSpeed, ForceMode2D.Impulse);
        }
        else
        {
            rb.AddForceX(moveDirection.x * moveSpeed * airFriction, ForceMode2D.Impulse);
        }
        //  players speed within maximum speed
        if (rb.linearVelocityX > maxSpeed)
        {
            rb.linearVelocityX = maxSpeed;
        }
        else if (rb.linearVelocityX < -maxSpeed)
        {
            rb.linearVelocityX = -maxSpeed;
        }
        // Flip character model
        FlipCharacter();

        if (moveDown)
        {
            RaycastHit2D col = Physics2D.BoxCast
            (groundCheck.position, boxSize, 0, -transform.up, groundCheckRadius, groundLayer);
            if (col)
            {
                if (col.transform.CompareTag("Platform"))
                {
                    StartCoroutine(DisablePlayerCollider(0.5f));
                    rb.AddRelativeForceY(moveDirection.y * 5, ForceMode2D.Impulse);
                }
            }
        }

        // Dash
        if (dash && moveDirection != Vector2.zero) StartCoroutine(Dash());

        // Jump
        if (jump || playerWantsToJump) Jump();


    }

    private void CheckWall()
    {
        float direction = (facingRight) ? 1 : -1;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * direction, wallCheckRadius, groundLayer);
        Debug.DrawRay(transform.position, Vector2.right * direction * wallCheckRadius, Color.red);

        if (hit.normal.x == -direction && !grounded && moveDirection.x == direction)
        {
            // wall slide
            wallJumpCounter = wallJumpTime;
            wallJumpDirection = (int)hit.normal.x;
            rb.linearVelocityY = Mathf.Clamp(rb.linearVelocityY, -wallSlidingSpeed, float.MaxValue);
            isOnWall = true;
            jumpsLeft = maxJumps;
        }
        else
        {
            wallJumpCounter -= Time.deltaTime;
            if (wallJumpCounter < 0)
            {
                isOnWall = false;
            }
        }
    }

    private void SetAnimation()
    {
        if (playerVelocity == Vector2.zero && grounded)
        {
            animator.Play("Idle");
            Debug.Log("Idle Animation");

        }
        if (moveDirection.x != 0 && grounded)
        {
            animator.Play("Walk");
            Debug.Log("Walk Animation");
        }
        if(isJumping)
        {
            animator.Play("Jump");
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        var t_gravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.zero;
        rb.AddRelativeForce(moveDirection * dashSpeed, ForceMode2D.Impulse);
        dashCooldown.StartCooldown();
        yield return new WaitForSeconds(dashTime);
        isDashing = false;
        rb.gravityScale = t_gravity;

    }

    private void FlipCharacter()
    {
        // Rotate player depending where players is trying to go.
        if (facingRight && moveDirection.x == -1)
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
            facingRight = false;
        }
        else if (!facingRight && moveDirection.x == 1)
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            facingRight = true;
        }
    }

    private void Jump()
    {
        Debug.Log("Jump");
        if (jumpsLeft > 0)
        {
            isJumping = true;
            rb.linearVelocityY = 0;
            switch (isOnWall)
            {
                case true:
                    rb.AddForce(new Vector2(jumpSpeed * wallJumpDirection, jumpSpeed), ForceMode2D.Impulse);
                    wallJumpCounter = 0;
                    break;
                case false:
                    rb.AddForceY(jumpSpeed, ForceMode2D.Impulse);
                    break;
            }

        }
        jumpsLeft--;
        jumpHold.StopHold();
    }
    private IEnumerator DisablePlayerCollider(float time)
    {
        boxCollider.enabled = false;
        yield return new WaitForSeconds(time);
        boxCollider.enabled = true;
        moveDown = false;
    }

    public void Knock(float knockback)
    {
        rb.linearVelocity = Vector2.zero;

        rb.AddRelativeForceX(-knockback, ForceMode2D.Impulse);
        rb.AddRelativeForceY(knockback, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Landing");
        if (collision == null) return;
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {

            if (lastVelocity.y < -1)
            {
                PreserveMomentumOnLanding();
            }
        }
    }

    public void PreserveMomentumOnLanding()
    {
        Debug.Log("Movement preserved");
        rb.linearVelocityX = lastVelocity.x;
    }

    private void OnDrawGizmos()
    {
        int direction = (facingRight) ? 1 : -1;
        Debug.DrawRay(transform.position, Vector2.right * direction * wallCheckRadius, Color.red);
        Gizmos.DrawWireCube(groundCheck.position, boxSize);
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
    private float _nextAbilityTime = 0;

    public bool isOnCooldown => Time.time < _nextAbilityTime;
    public void StartCooldown() => _nextAbilityTime = Time.time + cooldownTime;
}

public static class Vector2Extensiom
{
    public static Vector2 Rotate(this Vector2 vector, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = vector.x;
        float ty = vector.y;

        vector.x = (cos * tx) - (sin * tx);
        vector.y = (sin * tx) + (cos * tx);

        return vector;
    }
}

public class HoldInput
{
    public HoldInput(float t)
    {
        holdTime = t;
    }
    private float holdTime;
    private float _nextInputTime = 0;

    public bool isOnHold => Time.time < _nextInputTime;
    public void StartHold() => _nextInputTime = Time.time + holdTime;
    public void StopHold() => _nextInputTime = 0;
}
