using UnityEngine;

public class ShootingItem : MonoBehaviour
{
    private float direction = 1; // Mặc định hướng phải
    private float speed; // Thay đổi ở đây

    public float maxDistance = 10f; // Khoảng cách tối đa đạn có thể bay
    private Vector3 startPosition; // Vị trí ban đầu của đạn

    // Phương thức này được gọi từ Shooting để đặt hướng và tốc độ cho đạn
    public void SetDirection(float dir, float bulletSpeed)
    {
        direction = dir;
        speed = bulletSpeed; // Gán tốc độ
        startPosition = transform.position; // Lưu lại vị trí ban đầu của đạn
    }

    private void Update()
    {
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

        // Kiểm tra nếu đạn đã vượt quá khoảng cách tối đa
        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            return;

        Destroy(gameObject);
    }
}
