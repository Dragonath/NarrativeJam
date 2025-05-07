using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerControls : MonoBehaviour
{

    public GameObject player;
    public Animator animator;
    public Rigidbody2D playerRB;
    public Collider2D playerCollider;

    public Transform groundCheck;

    public LayerMask groundLayers;
    public LayerMask enemyLayers;

    public GameManager gameManager;

    public float groundCheckRadius = 0.59f;
    public bool grounded;
    public bool canTakeDamage;
    public int maxJumps;
    private int jumpsLeft;
    private bool dead;

    //Player Statistics
    public float playerSpeed;
    public float jumpForce;
    public float dashForce;
    public bool canDash;
    public float dashCD;

    //Used for translating Controls to Movement (Update -> FixedUpdate) 
    private float movDirTemp;
    private bool jumpTemp;
    private bool dashTemp;
    private bool facingRight = true;




    private void Start()
    {
        gameManager = GameManager.instance;
        gameManager.player = gameObject;
        gameManager.playerAlive = true;
        playerRB.bodyType = RigidbodyType2D.Kinematic;
        playerCollider.enabled = true;
        dead = false;
        canTakeDamage = true;
        jumpsLeft = maxJumps;

        SetControl(true);


    }


    private void Update()
    {
        if (gameManager.playerInput)
        {
            Controls();
        }
    }

    // Gets input from player
    private void Controls()
    {
        movDirTemp = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            jumpTemp = true;
        }
        if (Input.GetButtonDown("Fire1"))
        {
            // Attacks if implemented (?)
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            dashTemp = true;
        }
    }

    private void FixedUpdate()
    {
        CheckGround();
        Movement();
        ResetTemp();
    }

    /*
    --------------- UTILS ---------------
    */


    //Getter and Setter for player controls
    public bool GetControl() { return gameManager.playerInput; }

    public void SetControl(bool state)
    {
        gameManager.playerInput = state;

        if (!state)
        {
            playerRB.linearVelocity = new Vector2(0, 0);
        }
    }

    //Getter for Player Dead
    public bool getDead() { return dead; }

    // Checks if the player is standing on the ground
    private void CheckGround()
    {
        bool wasGrounded = grounded;
        grounded = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius, groundLayers);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                grounded = true;
                if (!wasGrounded)
                {
                    Debug.Log("Landed");
                    animator.SetTrigger("Landed");
                    jumpsLeft = maxJumps;
                }
            }
        }
    }

    // Kills the player if they can take damage
    public void Die()
    {
        if (canTakeDamage == true && !dead)
        {
            dead = true;
            SetControl(false);
            animator.SetTrigger("Dead");
            // SoundManager.PlaySound("death");
            gameManager.playerAlive = false;

            if (playerRB.IsSleeping())
            {
                playerCollider.enabled = false;
                playerRB.bodyType = RigidbodyType2D.Kinematic;
            }
            else
            {
                // Check if player is moving/in the air - Slower to respond but ensures that player doesn't stay hovering in the air
                StartCoroutine(DeadAndStoppedMoving());
            }
        }
    }

    public IEnumerator DeadAndStoppedMoving()
    {
        //Wait till player's rigidbody is sleeping and then turn off collider and stop rigidbody

        while (!playerRB.IsSleeping())
        {
            yield return null;
        }

        playerCollider.enabled = false;
        playerRB.bodyType = RigidbodyType2D.Kinematic;
        yield return null;
    }


    /*
    --------------- MOVEMENT ---------------
    */

    // Moves player according to inputs
    private void Movement()
    {

        if (facingRight)
        {
            transform.Translate(new Vector3(movDirTemp * playerSpeed * Time.deltaTime, 0, 0));
        }
        else
        {
            transform.Translate(new Vector3(movDirTemp * playerSpeed * Time.deltaTime * -1f, 0, 0));
        }

        if (jumpTemp && jumpsLeft > 0) { Jump(); }

    }

    //Makes the player Jump
    private void Jump()
    {
        animator.SetTrigger("Jump");
        //SoundManager.PlaySound("Jump");
        playerRB.linearVelocity = Vector2.up * jumpForce;
        jumpsLeft--;
    }

    //Resets temporary variables
    private void ResetTemp()
    {
        movDirTemp = 0;
        jumpTemp = false;
        dashTemp = false;
    }

    /*
   --------------- ATTACKS ---------------
   */

    // Sets Cooldown for player abilities
    public IEnumerator Cooldown(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (duration == dashCD)
        {
            canDash = true;
        }
    }


    // Flips the player model to face the other way
    private void Flip()
    {
        if (facingRight && movDirTemp < 0)
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
            facingRight = false;
        }
        else if (!facingRight && movDirTemp > 0)
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            facingRight = true;
        }
    }

}