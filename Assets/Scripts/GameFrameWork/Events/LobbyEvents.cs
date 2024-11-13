using Unity.Services.Lobbies.Models;

namespace GameFramework.Events 
{
    // Lớp static chứa các sự kiện liên quan đến Lobby
    public static class LobbyEvents
    {
        // Định nghĩa một delegate, kiểu hàm này nhận một đối tượng Lobby và không trả về giá trị gì
        public delegate void LobbyUpdated(Lobby lobby);

        // Biến static, loại delegate sẽ lưu trữ các phương thức sẽ được gọi khi sự kiện 'OnLobbyUpdated' xảy ra
        public static LobbyUpdated OnLobbyUpdated;
    }
}
