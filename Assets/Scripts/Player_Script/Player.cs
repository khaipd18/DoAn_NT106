using UnityEngine;

public class Player : MonoBehaviour
{
    // Public variables
    public Collider2D standingCollider, crouchingCollider;
    public Transform groundCheckCollider;
    public Transform overheadCheckCollider;
    public Transform wallCheckCollider;
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public float speed = 2;
    public float jumpPower = 500;
    public Transform nametag; // Thêm Transform cho nametag

    // Private variables
    private bool facingRight = true;
    private bool isRunning;
    private bool isGrounded = true;
    private bool jump;
    private float horizontalValue;
    private const float groundCheckRadius = 0.2f;
    private const float overheadCheckRadius = 0.2f;
    private const float wallCheckRadius = 0.2f;
    private float runSpeedModifier = 2f;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Store horizontal input
        horizontalValue = Input.GetAxisRaw("Horizontal");

        // Enable/disable running mode based on Shift key
        if (Input.GetKeyDown(KeyCode.LeftShift))
            isRunning = true;
        if (Input.GetKeyUp(KeyCode.LeftShift))
            isRunning = false;

        // Handle jump input
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump"))
            jump = true;
        else if (Input.GetButtonUp("Jump"))
            jump = false;
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

        // Check if touching ground layer
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckCollider.position, groundCheckRadius, groundLayer);
        if (colliders.Length > 0)
            isGrounded = true;
    }

    void Move(float dir, bool jumpFlag)
    {
        // Handle jumping
        if (isGrounded && jumpFlag)
        {
            isGrounded = false;
            jumpFlag = false;
            rb.AddForce(new Vector2(0f, jumpPower));
        }

        // Handle movement and running speed
        float xVal = dir * speed * 100 * Time.fixedDeltaTime;
        if (isRunning)
            xVal *= runSpeedModifier;

        Vector2 targetVelocity = new Vector2(xVal, rb.velocity.y);
        rb.velocity = targetVelocity;

        // Flip player based on direction
        if (facingRight && dir < 0)
        {
            Flip();
        }
        else if (!facingRight && dir > 0)
        {
            Flip();
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        // Đảm bảo nametag không bị lật
        if (nametag != null)
        {
            Vector3 nametagScale = nametag.localScale;
            nametagScale.x = Mathf.Abs(nametagScale.x); // Đảm bảo x luôn dương
            nametag.localScale = nametagScale;
        }
    }

    public bool IsFacingRight()
    {
        return facingRight;
    }
}
