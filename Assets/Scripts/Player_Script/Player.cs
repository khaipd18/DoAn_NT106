using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Public
    public Collider2D standingCollider, crouchingCollider;
    public Transform groundCheckCollider;
    public Transform overheadCheckCollider;
    public LayerMask groundLayer;
    public Transform wallCheckCollider;
    public LayerMask wallLayer;

    //Private
    [SerializeField]public float speed = 2;
    [SerializeField] public float jumpPower = 500;
    bool facingRight = true;
    bool isRunning;
    bool isGrounded = true;
    bool jump;
    float runSpeedModifier = 2f;
    const float groundCheckRadius = 0.2f;
    const float overheadCheckRadius = 0.2f;
    const float wallCheckRadius = 0.2f;

    Rigidbody2D rb;
    float horizontalValue;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //store horizontal val
        horizontalValue = Input.GetAxisRaw("Horizontal");

        //If LShift is clicked enable isRunning
        if (Input.GetKeyDown(KeyCode.LeftShift))
            isRunning = true;
        //If LShift is released disable isRunning
        if (Input.GetKeyUp(KeyCode.LeftShift))
            isRunning = false;
        //If press jump = jump
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump"))
            jump = true;

        //else, no
        else if (Input.GetButtonUp("Jump"))
            jump=false;
    }

    private void FixedUpdate()
    {
        GroundCheck();
        Move(horizontalValue, jump);
    }

    void GroundCheck()
    {
        bool wasGrounded = isGrounded;
        isGrounded = false;
        //Check if the GroundCheckObject is colliding with other
        //2D Colliders that are in the "Ground" Layer
        //If yes (isGrounded true) else (isGrounded false)
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckCollider.position, groundCheckRadius, groundLayer);
        if (colliders.Length > 0)
            isGrounded = true;
    }

    void Move(float dir, bool jumpFlag)
    {
        #region jump
        if (isGrounded && jumpFlag)
        {
            isGrounded = false;
            jumpFlag = false;

            rb.AddForce(new Vector2(0f, jumpPower));
            
        }
        #endregion
        #region move and run
        //Move and speed of the movement
        float xVal = dir * speed * 100 * Time.fixedDeltaTime;
        //Running speed mod
        if (isRunning)
            xVal *= runSpeedModifier;

        Vector2 targetVelocity = new Vector2(xVal,rb.velocity.y);
        rb.velocity = targetVelocity;

        //flipping sprite with movement
        //If looking right and clicked left (flip to the left)
        if (facingRight && dir < 0)
        {
            transform.localScale = new Vector3(-6, 6, 1);
            facingRight = false;
        }
        //If looking left and clicked right (flip to the right)
        else if (!facingRight && dir > 0)
        {
            transform.localScale = new Vector3(6, 6, 1);
            facingRight = true;
        }
        #endregion

    }
}
