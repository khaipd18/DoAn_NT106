using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour // Kế thừa từ NetworkBehaviour thay vì MonoBehaviour
{
    public Collider2D standingCollider, crouchingCollider;
    public Transform groundCheckCollider;
    public Transform overheadCheckCollider;
    public Transform wallCheckCollider;
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public float speed = 2;
    public float jumpPower = 500;
    public Transform nametag;

    private bool isRunning;
    private bool isGrounded = true;
    private float horizontalValue;
    private Rigidbody2D rb;

    // Network variables for position and facing direction
    private NetworkVariable<Vector2> networkPosition = new NetworkVariable<Vector2>(Vector2.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<bool> networkFacingRight = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!IsOwner) return; // Chỉ thực thi điều khiển cho chủ sở hữu của đối tượng này

        horizontalValue = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.LeftShift))
            isRunning = true;
        if (Input.GetKeyUp(KeyCode.LeftShift))
            isRunning = false;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump"))
        {
            GroundCheck();
            if (isGrounded)
            {
                RequestJumpServerRpc();
            }
        }

        RequestMoveServerRpc(horizontalValue, isRunning); // Gửi lệnh di chuyển tới server
    }

    private void FixedUpdate()
    {
        if (IsServer)
        {
            // Server cập nhật vị trí và hướng
            Move(horizontalValue, isRunning);
            networkPosition.Value = rb.position;
            networkFacingRight.Value = networkFacingRight.Value;  // Đồng bộ hướng với tất cả client
        }
        else
        {
            // Client đồng bộ hóa vị trí và hướng từ server
            rb.position = networkPosition.Value;
            if (networkFacingRight.Value != networkFacingRight.Value)
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
        if (isGrounded)
        {
            rb.AddForce(new Vector2(0f, jumpPower));
            isGrounded = false;
        }
    }

    private void Move(float dir, bool running)
    {
        float xVal = dir * speed * 100 * Time.fixedDeltaTime;
        if (running)
            xVal *= 2f;

        Vector2 targetVelocity = new Vector2(xVal, rb.velocity.y);
        rb.velocity = targetVelocity;

        if (networkFacingRight.Value && dir < 0)
        {
            Flip();
        }
        else if (!networkFacingRight.Value && dir > 0)
        {
            Flip();
        }
    }

    private void GroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckCollider.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        networkFacingRight.Value = !networkFacingRight.Value; // Lật hướng trên server
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        if (nametag != null)
        {
            Vector3 nametagScale = nametag.localScale;
            nametagScale.x = Mathf.Abs(nametagScale.x);
            nametag.localScale = nametagScale;
        }
    }

    public bool IsFacingRight()
    {
        return networkFacingRight.Value; // Trả về trạng thái hướng từ NetworkVariable
    }
}
