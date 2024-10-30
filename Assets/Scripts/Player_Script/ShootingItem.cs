using UnityEngine;

public class ShootingItem : MonoBehaviour
{
    private float direction = 1; // Mặc định hướng phải
    private float speed; // Thay đổi ở đây

    // Phương thức này được gọi từ Shooting để đặt hướng và tốc độ cho đạn
    public void SetDirection(float dir, float bulletSpeed)
    {
        direction = dir;
        speed = bulletSpeed; // Gán tốc độ
    }

    private void Update()
    {
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            return;

        Destroy(gameObject);
    }
}
