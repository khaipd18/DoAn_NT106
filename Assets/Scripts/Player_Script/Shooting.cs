using UnityEngine;

public class Shooting : MonoBehaviour
{
    public GameObject shootingItem;      // Đối tượng bắn ra (ví dụ: đạn)
    public Transform shootingPoint;      // Vị trí xuất phát của đối tượng bắn ra
    public float bulletSpeed = 10f;      // Tốc độ của đạn
    private Player player;

    private void Start()
    {
        player = GetComponent<Player>(); // Lấy script Player từ nhân vật
    }

    private void Update()
    {
        // Kiểm tra nếu phím J được nhấn
        if (Input.GetKeyDown(KeyCode.J))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (player == null) return;

        // Tạo một đối tượng bắn mới tại vị trí và hướng của shootingPoint
        GameObject si = Instantiate(shootingItem, shootingPoint.position, Quaternion.identity);
        Rigidbody2D rb = si.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Vô hiệu hóa trọng lực để đạn bay thẳng
            rb.gravityScale = 0;

            // Kiểm tra hướng của nhân vật để bắn về đúng hướng
            float direction = player.IsFacingRight() ? 1 : -1;

            // Gọi phương thức SetDirection với tốc độ đạn
            si.GetComponent<ShootingItem>().SetDirection(direction, bulletSpeed);

            // Đặt vận tốc ban đầu nếu cần (nếu bạn không dùng Rigidbody2D)
            rb.velocity = new Vector2(bulletSpeed * direction, 0);
        }
    }
}
