using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace GameFramework.Core.GameFrameWork.Manager
{
    // Lớp Singleton quản lý kết nối Relay cho chế độ chơi nhiều người
    public class RelayManager : Singleton<RelayManager>
    {
        // Thuộc tính kiểm tra xem người chơi có phải là host không.
        public bool IsHost
        {
            get
            {
                return _IsHost;
            }
        }

        private bool _IsHost = false;  // Biến riêng tư lưu trữ trạng thái host.
        private string _joinCode;  // Mã tham gia (join code) cho người chơi kết nối.
        private string _ip;  // Địa chỉ IP của server Relay.
        private int _port;  // Cổng của server Relay.
        private byte[] _key;  // Khóa của phiên Relay.
        private byte[] _hostConnectionData;  // Dữ liệu kết nối của host.
        private byte[] _connectionData;  // Dữ liệu kết nối của client.
        private System.Guid _allocationId;  // ID của phiên Relay.
        private byte[] _allocationIdBytes;  // Mảng byte chứa Allocation ID.

        // Phương thức lấy Allocation ID dưới dạng chuỗi.
        public string GetAllocationId()
        {
            return _allocationId.ToString();
        }

        // Phương thức lấy dữ liệu kết nối dưới dạng chuỗi, nếu chưa được khởi tạo thì trả về thông báo.
        public string GetConnectionData()
        {
            return _connectionData != null ? BitConverter.ToString(_connectionData) : "Connection data not initialized.";
        }

        // Phương thức tạo một phiên Relay mới với số kết nối tối đa cho phép.
        public async Task<string> CreateRelay(int maxConnection)
        {
            try
            {
                // Tạo một allocation mới với số kết nối tối đa
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnection);

                // Lấy mã tham gia cho người chơi từ allocation ID
                _joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

                // Lọc server endpoints để lấy endpoint với kiểu kết nối "dtls" (giao thức bảo mật)
                RelayServerEndpoint dtlsEnpoint = allocation.ServerEndpoints.First(conn => conn.ConnectionType == "dtls");
                _ip = dtlsEnpoint.Host;  // Lưu IP của máy chủ Relay
                _port = dtlsEnpoint.Port;  // Lưu cổng của máy chủ Relay

                _allocationId = allocation.AllocationId;  // Lưu Allocation ID
                _allocationIdBytes = allocation.AllocationIdBytes;  // Lưu Allocation ID dưới dạng byte[]
                _connectionData = allocation.ConnectionData;  // Lưu dữ liệu kết nối

                _key = allocation.Key;  // Lưu khóa của phiên Relay

                _IsHost = true;  // Đặt trạng thái host là true

                return _joinCode;  // Trả về mã tham gia
            }
            catch (Exception ex)
            {
                Debug.LogError("Error during CreateRelay: " + ex.Message);  // Xử lý lỗi
                return null;
            }
        }

        // Phương thức tham gia vào một phiên Relay có sẵn thông qua mã tham gia (join code).
        public async Task<bool> JoinRelay(string joinCode)
        {
            try
            {
                _joinCode = joinCode;  // Gán mã tham gia

                // Tham gia vào phiên Relay bằng mã tham gia
                JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

                // Lọc server endpoints để lấy endpoint với kiểu kết nối "dtls"
                RelayServerEndpoint dtlsEnpoint = allocation.ServerEndpoints.First(conn => conn.ConnectionType == "dtls");
                _ip = dtlsEnpoint.Host;  // Lưu IP của máy chủ Relay
                _port = dtlsEnpoint.Port;  // Lưu cổng của máy chủ Relay
                _allocationId = allocation.AllocationId;  // Lưu Allocation ID
                _connectionData = allocation.ConnectionData;  // Lưu dữ liệu kết nối
                _allocationIdBytes = allocation.AllocationIdBytes;  // Lưu Allocation ID dưới dạng byte[]
                _hostConnectionData = allocation.HostConnectionData;  // Lưu dữ liệu kết nối của host

                _key = allocation.Key;  // Lưu khóa của phiên Relay

                return true;  // Trả về true khi tham gia thành công
            }
            catch (RelayServiceException ex)
            {
                Debug.LogError("JoinRelay failed: " + ex.Message);  // Xử lý lỗi
                return false;  // Trả về false nếu tham gia thất bại
            }
        }

        // Phương thức trả về thông tin kết nối của host dưới dạng tuple
        public (byte[] AllocationId, byte[] Key, byte[] ConnectionData, string _dtlsAddress, int _dtlsPort) GetHostConnectionInfo()
        {
            return (_allocationIdBytes, _key, _connectionData, _ip, _port);
        }

        // Phương thức trả về thông tin kết nối của client dưới dạng tuple
        public (byte[] AllocationId, byte[] Key, byte[] ConnectionData, byte[] HostConnectionData, string _dtlsAddress, int _dtlsPort) GetClientConnectionInfo()
        {
            return (_allocationIdBytes, _key, _connectionData, _hostConnectionData, _ip, _port);
        }
    }
}
