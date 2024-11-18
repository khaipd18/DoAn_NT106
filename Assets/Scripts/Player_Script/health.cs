using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class Health : NetworkBehaviour
{
    public const int maxHealth = 100;
    public int currentHealth = maxHealth;
    public RectTransform healthbar;

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("Dead");
            StartCoroutine(Die()); // Gọi hàm chết
        }

        healthbar.sizeDelta = new Vector2(currentHealth - 10, healthbar.sizeDelta.y);
    }

    // Coroutine xử lý logic chết
    private IEnumerator Die()
    {
        // Ghi log hoặc thêm hiệu ứng tại đây
        Debug.Log("Destroying character...");

        // Nếu cần thời gian để hiển thị hiệu ứng, animation
        yield return new WaitForSeconds(2f); // Chờ 2 giây (hoặc thay đổi tùy ý)

        // Hủy đối tượng
        if (IsServer)
        {
            GetComponent<NetworkObject>().Despawn(); // Hủy đối tượng
        }

        SceneManager.LoadScene("MainMenu");
    }
}