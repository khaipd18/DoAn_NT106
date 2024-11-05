using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Nametag : MonoBehaviour
{
    public string characterTag = "Player"; // Tag hoặc tên nhân vật
    private TextMeshProUGUI textUI; // Sử dụng TextMeshProUGUI nếu dùng TextMeshPro
    // Nếu dùng UI Text của Unity, thay đổi thành Text textUI;

    void Start()
    {
        // Tìm thành phần Text trên đối tượng
        textUI = GetComponentInChildren<TextMeshProUGUI>(); // Sử dụng TextMeshProUGUI nếu dùng TextMeshPro
        // Nếu dùng UI Text của Unity, thay đổi thành textUI = GetComponentInChildren<Text>();

        // Gán nội dung cho Text
        if (textUI != null)
        {
            textUI.text = characterTag;
        }
    }
}
