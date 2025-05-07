using System.Collections;
using System.Threading;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
    public Rigidbody2D playerRB;
    public Collider2D playerCollider;
    public Transform groundCheck;

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

    // LayerMasks
    public LayerMask groundLayer;
    public LayerMask enemyLayer;

    private Vector2 moveDirection;
    private bool jump;
    private bool dash;
    private bool faceRight = true;

    InputAction moveAction;
    InputAction jumpAction;
    InputAction dashAction;

    public Vector2 PlayerSpeedX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        dashAction = InputSystem.actions.FindAction("Dash");

        playerRB = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        groundCheck = transform.Find("GroundCheck");
        playerCollider.enabled = true;
        playerHasControl = true;
        dashCooldown = new Cooldown(4f); 

        if (_debug)
        {
            Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckRadius);
        }
    }


    // Update is called once per frame
    void Update()
    {

        PlayerSpeedX = playerRB.linearVelocity;
        if (playerHasControl && !dashing)
        {
            GetInputs();
        }

        if (_debug)
        {
            Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckRadius);
        }

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

        if(!canDash)
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
        if(dashing)
        {
            return;
        }
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

        if(playerRB.linearVelocityX > maxSpeed)
        {
            playerRB.linearVelocityX =  maxSpeed;   
        }
        else if(playerRB.linearVelocityX < -maxSpeed)
        {
            playerRB.linearVelocityX = -maxSpeed;
        }

        if (jump && grounded)
        {
            playerRB.AddRelativeForceY(jumpForce, ForceMode2D.Impulse);
            grounded = false;
        }

        if (dash && moveDirection != Vector2.zero && canDash)
        {
            Dash();
        }
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
        // transform.Translate(new Vector3(moveDirection.x * dashSpeed * Time.deltaTime, 0, 0));
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