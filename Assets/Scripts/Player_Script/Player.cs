using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    public Collider2D standingCollider, crouchingCollider;
    public Transform groundCheckCollider;
    public Transform overheadCheckCollider;
    public Transform wallCheckCollider;
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public float speed = 2f;
    public float jumpPower = 500f;
    public Transform nametag;

    private Rigidbody2D rb;
    private float horizontalValue;
    private bool isRunning;

    // Network variables
    private NetworkVariable<Vector2> networkPosition = new NetworkVariable<Vector2>(Vector2.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<bool> networkFacingRight = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<bool> networkIsGrounded = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!IsOwner) return;

        // Input handling
        horizontalValue = Input.GetAxisRaw("Horizontal");
        isRunning = Input.GetKey(KeyCode.LeftShift);

        // Jump input
        if (networkIsGrounded.Value && (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump")))
        {
            RequestJumpServerRpc();
        }

        // Send movement request to the server
        RequestMoveServerRpc(horizontalValue, isRunning);
    }

    private void FixedUpdate()
    {
        if (IsServer)
        {
            // Server handles movement and synchronization
            Move(horizontalValue, isRunning);
            networkPosition.Value = rb.position;
            networkFacingRight.Value = IsFacingRight();
            networkIsGrounded.Value = IsGrounded();
        }
        else
        {
            // Client-side prediction and correction
            if (Vector2.Distance(rb.position, networkPosition.Value) > 0.1f)
            {
                rb.position = Vector2.Lerp(rb.position, networkPosition.Value, 0.1f);
            }

            // Update facing direction from server
            if (networkFacingRight.Value != IsFacingRight())
            {
                FlipLocal();
            }
        }
    }

    [ServerRpc]
    private void RequestMoveServerRpc(float dir, bool running)
    {
        horizontalValue = dir;
        isRunning = running;

        // Server processes movement
        Move(horizontalValue, isRunning);
    }

    [ServerRpc]
    private void RequestJumpServerRpc()
    {
        if (IsGrounded())
        {
            rb.AddForce(new Vector2(0f, jumpPower));
            networkIsGrounded.Value = false;
        }
    }

    private void Move(float dir, bool running)
    {
        float moveSpeed = speed * (running ? 2f : 1f); // Adjust speed if running
        float xVelocity = dir * moveSpeed;
        rb.velocity = new Vector2(xVelocity, rb.velocity.y);

        // Flip the character if needed
        if (dir > 0 && !IsFacingRight())
        {
            FlipLocal();
        }
        else if (dir < 0 && IsFacingRight())
        {
            FlipLocal();
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheckCollider.position, 0.2f, groundLayer);
    }

    private void FlipLocal()
    {
        // Update local direction
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        // Flip nametag if it exists
        if (nametag != null)
        {
            Vector3 nametagScale = nametag.localScale;
            nametagScale.x = Mathf.Abs(nametagScale.x);
            nametag.localScale = nametagScale;
        }
    }

    public bool IsFacingRight()
    {
        return transform.localScale.x > 0;
    }
}
