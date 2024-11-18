using UnityEngine;

public class ShootingItem : MonoBehaviour
{
    private float direction = 1;
    private float speed;
    public float maxDistance = 10f;
    private Vector3 startPosition;

    public int damageAmount = 10; // Mỗi viên đạn gây ra 10 sát thương

    public void SetDirection(float dir, float bulletSpeed)
    {
        direction = dir;
        speed = bulletSpeed;
        startPosition = transform.position;
    }

    private void Update()
    {
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra nếu đối tượng trúng có component Health
        Health health = collision.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damageAmount); // Giảm 10 máu khi trúng đạn
            Destroy(gameObject); // Hủy viên đạn sau khi gây sát thương
        }
    }
}
