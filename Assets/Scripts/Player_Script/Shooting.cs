using UnityEngine;

public class Shooting : MonoBehaviour
{
    public GameObject shootingItem;      // Đối tượng bắn ra (ví dụ: đạn)
    public Transform shootingPoint;      // Vị trí xuất phát của đối tượng bắn ra
    public float bulletSpeed = 10f;      // Tốc độ của đạn
    private Player player;               // Tham chiếu đến script Player để lấy thông tin hướng của nhân vật

    private void Start()
    {
        // Lấy tham chiếu đến script Player để kiểm tra hướng của nhân vật
        player = GetComponent<Player>();
    }

    private void Update()
    {
        // Kiểm tra nếu phím J được nhấn để thực hiện bắn
        if (Input.GetKeyDown(KeyCode.J))
        {
            Shoot(); // Gọi hàm Shoot để bắn
        }
    }

    void Shoot()
    {
        // Kiểm tra xem có tham chiếu đến Player không
        if (player == null) return;

        // Tạo một đối tượng bắn mới tại vị trí và hướng của shootingPoint
        GameObject si = Instantiate(shootingItem, shootingPoint.position, Quaternion.identity);
        ShootingItem shootingItemScript = si.GetComponent<ShootingItem>(); // Lấy script ShootingItem của đối tượng đạn

        if (shootingItemScript != null)
        {
            // Xác định hướng bắn dựa vào hướng mà nhân vật đang đối mặt
            float direction = player.IsFacingRight() ? 1 : -1;

            // Gọi phương thức SetDirection với tốc độ đạn và hướng
            shootingItemScript.SetDirection(direction, bulletSpeed);
        }
    }
}
