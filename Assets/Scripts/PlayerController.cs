using System.Collections;
using System.Net;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Util;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public BoxCollider2D boxCollider;
    public Transform groundCheck;
    public Animator animator;

    // Track player velocity
    public Vector2 playerVelocity;
    private Vector2 lastVelocity;

    public float maxSpeed;
    public float moveSpeed;
    public float jumpSpeed;
    public float dashSpeed;
    public float dashTime;
    public float groundCheckRadius;
    public float wallCheckRadius;
    public float slopeAngle;
    public float minSlopeAngle;
    public float maxSlopeAngle;
    public float airFriction;
    public float holdTime;
    public float wallSlidingSpeed;
    public float wallJumpCounter;
    public float wallJumpTime;


    private float jumpsLeft;
    private int wallJumpDirection;
    public float maxJumps;

    private bool jump;
    private bool dash;
    private bool facingRight;

    public bool playerWantsToJump;
    public bool isDashing;
    public bool grounded;
    public bool isOnSlope;
    public bool isOnWall;

    public bool DEBUG;

    public Vector2 boxSize;
    public Vector2 moveDirection;
    public Vector2 slopeDirection;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction dashAction;

    public LayerMask groundLayer;

    private Cooldown jumpCooldown;
    private Cooldown dashCooldown;
    private HoldInput jumpHold;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        dashAction = InputSystem.actions.FindAction("Dash");

        jumpCooldown = new Cooldown(0.1f);
        dashCooldown = new Cooldown(2f);
        jumpHold = new HoldInput(holdTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDashing)
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

        // Check if player is standing on ground
        grounded = Physics2D.BoxCast
            (groundCheck.position, boxSize, 0, -transform.up, groundCheckRadius, groundLayer);

        // reset jumps if player is on ground
        if (grounded)
        {
            animator.Play("Idle");
            jumpsLeft = maxJumps;
        }

        // Check for dash imput and if dash is on cooldown. Player cannot dash while giving no input
        if (dashAction.WasPressedThisFrame())
        {

            dash = true;
        }

        // check for jump input
        if (jumpAction.WasPressedThisFrame())
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



    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

        MovePlayer();
        // Reset inputs if necessary
        jump = false;
        dash = false;

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
            if(wallJumpCounter < 0)
            {
                isOnWall = false;
            }
        }
    }


    //private void CheckSlope()
    //{
    //    // Check front
    //    float direction = (facingRight) ? 1 : -1;
    //    RaycastHit2D hit = Physics2D.Raycast(slopeCheckFront.position, Vector2.right * direction, slopeCheckRadius, groundLayer);
    //    if(hit)
    //    {
    //        slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
    //        if (slopeAngle > minSlopeAngle && slopeAngle < maxSlopeAngle)
    //        {
    //            slopeDirection = Vector2.Perpendicular(hit.normal) * -direction;
    //            ascending = true;
    //            descending = false;
    //            isOnSlope = true;
    //            return;
    //        }
    //    }

    //    // Check behind
    //    hit = Physics2D.Raycast(slopeCheckBack.position, Vector2.down, slopeCheckRadius, groundLayer);
    //    if(hit)
    //    {
    //        slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
    //        if (slopeAngle > minSlopeAngle && slopeAngle < maxSlopeAngle)
    //        {
    //            slopeDirection = Vector2.Perpendicular(hit.normal) * -direction;
    //            if (slopeDirection.y > 0) slopeDirection.y = 0;
    //            ascending = false;
    //            descending = true;
    //            isOnSlope = true;
    //            return;
    //        }
    //    }

    //    ascending = false;
    //    descending = false;
    //    isOnSlope = false;
    //}

    private IEnumerator Dash()
    {
        Debug.Log("Dashierino");
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
        if (jumpsLeft > 0)
        {
            animator.Play("Jump");
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
}


namespace Util
{
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
}