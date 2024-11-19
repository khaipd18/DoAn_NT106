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

    private bool isRunning;
    private float horizontalValue;
    private Rigidbody2D rb;

    // Network variables
    private NetworkVariable<Vector2> networkPosition = new NetworkVariable<Vector2>(Vector2.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<bool> networkFacingRight = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<bool> networkIsGrounded = new NetworkVariable<bool>(true);

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!IsOwner) return;

        // Get input for movement
        horizontalValue = Input.GetAxisRaw("Horizontal");
        if (Input.GetKeyDown(KeyCode.LeftShift)) isRunning = true;
        if (Input.GetKeyUp(KeyCode.LeftShift)) isRunning = false;

        // Check for jump input
        GroundCheck();
        if (networkIsGrounded.Value && (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump")))
        {
            RequestJumpServerRpc();
        }

        // Move locally and send movement request to server
        Move(horizontalValue, isRunning);
        RequestMoveServerRpc(horizontalValue, isRunning);
    }

    private void FixedUpdate()
    {
        if (IsServer)
        {
            // Server handles physics and synchronization
            Move(horizontalValue, isRunning);
            networkPosition.Value = rb.position;
            networkFacingRight.Value = IsFacingRight();
            networkIsGrounded.Value = isGrounded();
        }
        else
        {
            // Client predicts movement and corrects position if needed
            if (Vector2.Distance(rb.position, networkPosition.Value) > 0.1f)
            {
                rb.position = Vector2.Lerp(rb.position, networkPosition.Value, 0.1f);
            }

            // Flip character based on server direction
            if (networkFacingRight.Value != IsFacingRight())
            {
                Flip();
            }
        }
    }

    [ServerRpc]
    private void RequestMoveServerRpc(float dir, bool running)
    {
        horizontalValue = dir;
        isRunning = running;
    }

    [ServerRpc]
    private void RequestJumpServerRpc()
    {
        if (isGrounded())
        {
            rb.AddForce(new Vector2(0f, jumpPower));
            networkIsGrounded.Value = false; // Update ground state
        }
    }

    private void Move(float dir, bool running)
    {
        float xVal = dir * speed * 100 * Time.fixedDeltaTime;
        Vector2 targetVelocity = new Vector2(xVal, rb.velocity.y);
        rb.velocity = targetVelocity;

        if (IsFacingRight() && dir < 0)
        {
            Flip();
        }
        else if (!IsFacingRight() && dir > 0)
        {
            Flip();
        }
    }

    private void GroundCheck()
    {
        if (IsServer)
        {
            networkIsGrounded.Value = isGrounded();
        }
    }

    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheckCollider.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        // Flip character
        networkFacingRight.Value = !networkFacingRight.Value;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        // Flip nametag
        if (nametag != null)
        {
            Vector3 nametagScale = nametag.localScale;
            nametagScale.x = Mathf.Abs(nametagScale.x);
            nametag.localScale = nametagScale;
        }
    }

    public bool IsFacingRight()
    {
        return networkFacingRight.Value;
    }
}
