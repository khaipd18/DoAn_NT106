using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
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
            // Thêm logic chết tại đây (như kích hoạt animation, hủy object, v.v.)
        }

        healthbar.sizeDelta = new Vector2(currentHealth-10, healthbar.sizeDelta.y);
    }
}
