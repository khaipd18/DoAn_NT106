using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 1;
    private bool facingRight = true;

    private Rigidbody2D rb;
    private float horizontalValue;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        horizontalValue = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate()
    {
        Move(horizontalValue);
    }

    void Move(float dir)
    {
        float xVal = dir * speed * 100 * Time.deltaTime;
        Vector2 targetVelocity = new Vector2(xVal, rb.velocity.y);
        rb.velocity = targetVelocity;

        // Thay đổi hướng dựa trên đầu vào
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
    }

    public bool IsFacingRight()
    {
        return facingRight;
    }
}
