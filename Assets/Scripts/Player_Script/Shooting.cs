using UnityEngine;
using Unity.Netcode;

public class Shooting : NetworkBehaviour
{
    public GameObject shootingItem;      // Đối tượng bắn ra (ví dụ: đạn)
    public Transform shootingPoint;      // Vị trí xuất phát của đối tượng bắn ra
    public float bulletSpeed = 10f;      // Tốc độ của đạn
    private Player player;               // Tham chiếu đến script Player để lấy thông tin hướng của nhân vật

    private void Start()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        if (!IsOwner) return; // Chỉ cho phép chủ sở hữu thực hiện lệnh bắn

        // Kiểm tra nếu phím J được nhấn để thực hiện bắn
        if (Input.GetKeyDown(KeyCode.J))
        {
            RequestShootServerRpc(); // Gửi yêu cầu bắn tới server
        }
    }

    [ServerRpc]
    private void RequestShootServerRpc()
    {
        Shoot(); // Gọi hàm Shoot trên server
    }

    private void Shoot()
    {
        if (player == null) return;

        // Tạo đạn mới tại vị trí và hướng của shootingPoint
        GameObject si = Instantiate(shootingItem, shootingPoint.position, Quaternion.identity);
        NetworkObject networkObject = si.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Spawn(true); // Đồng bộ đối tượng đạn mới với tất cả client
        }

        ShootingItem shootingItemScript = si.GetComponent<ShootingItem>();
        if (shootingItemScript != null)
        {
            float direction = player.IsFacingRight() ? 1 : -1;
            shootingItemScript.SetDirection(direction, bulletSpeed);
        }

        // Gọi Client RPC để đồng bộ đạn với tất cả client
        ShootClientRpc(shootingPoint.position, player.IsFacingRight() ? 1 : -1);
    }

    [ClientRpc]
    private void ShootClientRpc(Vector2 position, float direction)
    {
        // Chỉ thực hiện với client, server đã thực hiện bắn rồi
        if (IsServer) return;

        // Tạo đạn mới cho client
        GameObject si = Instantiate(shootingItem, position, Quaternion.identity);
        ShootingItem shootingItemScript = si.GetComponent<ShootingItem>();
        if (shootingItemScript != null)
        {
            shootingItemScript.SetDirection(direction, bulletSpeed);
        }
    }
}
