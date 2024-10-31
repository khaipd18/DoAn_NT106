using UnityEngine;

public class ShootingItem : MonoBehaviour
{
    private float direction = 1; // Hướng di chuyển của đạn, mặc định là hướng phải
    private float speed; // Tốc độ di chuyển của đạn

    public float maxDistance = 10f; // Khoảng cách tối đa đạn có thể bay
    private Vector3 startPosition; // Vị trí ban đầu của đạn, để kiểm tra khoảng cách đã di chuyển

    // Phương thức này được gọi từ script Shooting để thiết lập hướng và tốc độ cho đạn
    public void SetDirection(float dir, float bulletSpeed)
    {
        direction = dir; // Gán hướng di chuyển cho đạn (1 là phải, -1 là trái)
        speed = bulletSpeed; // Gán tốc độ cho đạn
        startPosition = transform.position; // Lưu lại vị trí ban đầu của đạn
    }

    private void Update()
    {
        // Di chuyển đạn theo hướng đã đặt với tốc độ cố định
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

        // Kiểm tra nếu đạn đã bay xa hơn khoảng cách tối đa cho phép
        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject); // Hủy đối tượng đạn nếu vượt quá khoảng cách tối đa
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Nếu đạn va chạm với nhân vật Player, không làm gì cả và quay lại
        if (collision.CompareTag("Player"))
            return;

        // Hủy đối tượng đạn khi va chạm với các đối tượng khác
        Destroy(gameObject);
    }
}
