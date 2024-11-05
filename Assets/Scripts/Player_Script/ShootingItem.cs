using UnityEngine;

public class ShootingItem : MonoBehaviour
{
    private float direction = 1;
    private float speed;
    public float maxDistance = 10f;
    private Vector3 startPosition;

    public int damageAmount = 25; // Mỗi viên đạn gây ra 25 sát thương

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
        if (collision.CompareTag("Player"))
        {
            HealthBar healthBar = collision.GetComponentInChildren<HealthBar>();
            if (healthBar != null)
            {
                healthBar.LoseHealth(damageAmount); // Mỗi lần trúng đạn sẽ trừ 25 máu
            }
            Destroy(gameObject);
            return;
        }

        Destroy(gameObject);
    }
}
