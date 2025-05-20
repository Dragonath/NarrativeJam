using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player_Controller : MonoBehaviour
{
    // -- PUBLIC -- // 
    [Header("Components")]
    public Rigidbody2D rb;
    public BoxCollider2D boxCollider;
    public CircleCollider2D circleCollider;
    public Transform groundCheck;
    public Animator animator;
    public RuntimeAnimatorController controller1;
    public RuntimeAnimatorController controller2;


    [Header("Speeds")]
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
    public float dropdownTime;

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
    private float normalGravity;
    private bool jump;
    private bool dash;
    private bool moveDown;
    private bool facingRight;
    public bool canDash;


    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction dashAction;

    private Vector2 dashDecel;
    private Vector2 lastVelocity;
    private HoldInput jumpHold;

    [Header("Unlocks")]
    public bool dashUnlocked;
    public bool runUnlocked;
    public bool jumpUnlocked;
    public bool walkUnlocked;

    public static Player_Controller instance;
    void Awake()
    {
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
        // get reference for all components
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        circleCollider = GetComponent<CircleCollider2D>();

        // Get reference for player inputs
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        dashAction = InputSystem.actions.FindAction("Dash");

        normalGravity = rb.gravityScale;

        jumpHold = new HoldInput(inputHoldTime);
    }

    // Update is called once per frame
    void Update()
    {

        // Track player velocity in Inspector
        playerVelocity = rb.linearVelocity;
        // Store old velocity
        lastVelocity = rb.linearVelocity;

        if (isDashing || !playerHasControl || !walkUnlocked)
        {
            if (isDashing)
            {
                // Damp player speed during a dash.
                rb.linearVelocityX = Mathf.SmoothDamp(rb.linearVelocityX, 0, ref dashDecel.x, dashTime);
                rb.linearVelocityY = Mathf.SmoothDamp(rb.linearVelocityY, 0, ref dashDecel.y, dashTime);
            }

        }

        // Player input is checked 
        moveDirection = moveAction.ReadValue<Vector2>();
        if (moveDirection.y < -0.2f)
        {
            moveDown = true;
        }

        // Check if player is standing on ground
        grounded = Physics2D.BoxCast
            (groundCheck.position, boxSize, 0, -transform.up, groundCheckRadius, groundLayer);

        // reset jumps and dash if player is on ground
        if (grounded)
        {
            isJumping = false;
            jumpsLeft = maxJumps;
            canDash = true;
        }

        // Check for dash imput and if dash is on cooldown. Player cannot dash while giving no input
        if (dashAction.WasPressedThisFrame() && dashUnlocked && canDash)
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
        // Hold player's jump input for a while, this way jump input doesnt need to be pixel perfect
        // and player can press jump a bit before he touches the ground
        // and still be able to jump when player reaches the ground
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
        // we wanna skip everything if player is dashing or doesnt have control
        if (isDashing || !playerHasControl)
        {
            return;
        }

        // Execute movement
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

        FlipCharacter();

        // Lets player drop down from a platform
        if (moveDown)
        {
            RaycastHit2D col = Physics2D.BoxCast
            (groundCheck.position, boxSize, 0, -transform.up, groundCheckRadius, groundLayer);
            if (col)
            {
                if (col.transform.CompareTag("Platform"))
                {
                    StartCoroutine(DisablePlayerCollider(dropdownTime));
                    rb.AddRelativeForceY(moveDirection.y, ForceMode2D.Impulse);
                }
            }
        }

        // Dash
        if (dash && moveDirection != Vector2.zero) StartCoroutine(DashSequenceStart());

        // Jump
        if (jump || playerWantsToJump) Jump();


    }
    /// <summary>
    /// Checks the area directly in front of the player
    /// </summary>
    private void CheckWall()
    {
        float direction = (facingRight) ? 1 : -1;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * direction, wallCheckRadius, groundLayer);
        Debug.DrawRay(transform.position, Vector2.right * direction * wallCheckRadius, Color.red);

        // Fix normal's float precision errors 
        if (Mathf.Abs(hit.normal.x) > 0.988888)
        {
            hit.normal = new Vector2(Mathf.Round(hit.normal.x), hit.normal.y);
        }

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
        if (isDashing)
        {
            return;
        }

        if (isOnWall)
        {
            animator.Play("WallSlide");

            return;
        }

        if (playerVelocity == Vector2.zero && grounded)
        {
            animator.Play("Idle");
        }
        if (moveDirection.x != 0 && grounded)
        {
            animator.Play("Walk");
        }
        if (isJumping)
        {
            animator.Play("Jump");
        }
    }

    private IEnumerator DashSequenceStart()
    {
        animator.Play("Dash_Start");
        isDashing = true;
        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.zero;
        canDash = false;
        grounded = false;
        yield return new WaitForSeconds(dashTime);
        animator.Play("Dash_End");
    }

    public void DashSequenceMove()
    {
        boxCollider.enabled = false;
        circleCollider.enabled = true;
        // Rotate player to face the moving direction
        Vector2 v = moveDirection;
        if (Mathf.Abs(v.y) > 0.1)
        {
            var angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        rb.AddForce(moveDirection * dashSpeed, ForceMode2D.Impulse);

    }

    public void DashSequenceEnd()
    {
        boxCollider.enabled = true;
        circleCollider.enabled = false;
        isDashing = false;
        if (moveDirection.x < -0.01)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        } else
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }

        rb.gravityScale = normalGravity;
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
            jumpsLeft--;
            jumpHold.StopHold();
        }
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

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        StartCoroutine(FlashRed());
        if (currentHealth <= 0)
        {
            GameManager.instance.PlayerDeath();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null) return;
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {

            if (lastVelocity.y < -1)
            {
                PreserveMomentumOnLanding();
            }
        }
    }

    IEnumerator FlashRed()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        GetComponent<SpriteRenderer>().color = Color.white;
        yield return new WaitForSeconds(0.1f);
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void PreserveMomentumOnLanding()
    {
        rb.linearVelocityX = lastVelocity.x;
    }

    private void OnDrawGizmos()
    {
        int direction = (facingRight) ? 1 : -1;
        Debug.DrawRay(transform.position, Vector2.right * direction * wallCheckRadius, Color.red);
        Gizmos.DrawWireCube(groundCheck.position, boxSize);
    }

    public IEnumerator ToggleAnimator(float time)
    {
        yield return new WaitForSeconds(time);
        animator.runtimeAnimatorController = controller2;
    }

    public void StartAnimatorChange()
    {
        StartCoroutine(ToggleAnimator(0.2f));
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
