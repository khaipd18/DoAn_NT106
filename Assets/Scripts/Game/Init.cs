using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    // Class Init được sử dụng để khởi tạo các dịch vụ của Unity và xử lý đăng nhập người chơi.
    public class Init : MonoBehaviour
    {
        // Start is called before the first frame update
        async void Start()
        {
            await UnityServices.InitializeAsync();   // Khởi tạo các dịch vụ Unity (Authentication, Relay, Lobby)

            if (UnityServices.State == ServicesInitializationState.Initialized) // Kiểm tra xem các dịch vụ đã được khởi tạo thành công chưa
            {

                AuthenticationService.Instance.SignedIn += OnSignIn;

                await AuthenticationService.Instance.SignInAnonymouslyAsync(); // Đăng nhập người chơi ẩn danh (anonymous)


                if (AuthenticationService.Instance.IsSignedIn)
                {
                    string username = PlayerPrefs.GetString(key: "Username");
                    if (username == "")
                    {
                        username = "Player";
                        PlayerPrefs.SetString("Username", username);
                    }

                    SceneManager.LoadSceneAsync("MainMenu");  // Chuyển sang cảnh "MainMenu" sau khi đăng nhập thành công
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnSignIn() // Hàm xử lý khi người chơi đăng nhập thành công
        {
            Debug.Log(message: $"Player ID: {AuthenticationService.Instance.PlayerId}");
            Debug.Log(message: $"Token: {AuthenticationService.Instance.AccessToken}");
        }
    }
}
