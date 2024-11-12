﻿using System;
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

        public bool IsHost 
        { get { 
                return _IsHost;
            }
        }

        private bool _IsHost = false;
        private string _joinCode;
        private string _ip;
        private int _port;
        private byte[] _key;
        private byte[] _hostConnectionData;
        private byte[] _connectionData;
        private System.Guid _allocationId;
        private byte[] _allocationIdBytes;

        public string GetAllocationId()
        {
            return _allocationId.ToString();
        }

        public string GetConnectionData()
        {
            return _connectionData != null ? BitConverter.ToString(_connectionData) : "Connection data not initialized.";
        }




        public async Task<string> CreateRelay(int maxConnection)
        {
            try
            {
                // Tạo một phiên (allocation) mới trên máy chủ Relay với số kết nối tối đa cho phép
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnection);

                // Lấy mã tham gia (join code) cho phép người chơi kết nối tới máy chủ Relay này thông qua allocation ID
                _joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

                // Lọc danh sách ServerEndpoints để lấy endpoint có kiểu kết nối là "dtls" (giao thức bảo mật)
                RelayServerEndpoint dtlsEnpoint = allocation.ServerEndpoints.First(conn => conn.ConnectionType == "dtls");
                _ip = dtlsEnpoint.Host;  // Lưu địa chỉ IP của máy chủ Relay
                _port = dtlsEnpoint.Port;  // Lưu cổng của máy chủ Relay

                _allocationId = allocation.AllocationId; // ID của phiên relay
                _allocationIdBytes = allocation.AllocationIdBytes;
                _connectionData = allocation.ConnectionData; // Dữ liệu kết nối của phiên relay
                

                _key = allocation.Key;

                _IsHost = true;

                return _joinCode;
            } catch (Exception ex)
            {
                Debug.LogError("Error during CreateRelay: " + ex.Message);
                return null;
            }

            

        }

        // Phương thức tham gia vào một phiên relay có sẵn thông qua mã tham gia
        public async Task<bool> JoinRelay(string joinCode)
        {
            try
            {
                _joinCode = joinCode;
                JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

                RelayServerEndpoint dtlsEnpoint = allocation.ServerEndpoints.First(conn => conn.ConnectionType == "dtls");
                _ip = dtlsEnpoint.Host;
                _port = dtlsEnpoint.Port;
                _allocationId = allocation.AllocationId;
                _connectionData = allocation.ConnectionData;
                _allocationIdBytes = allocation.AllocationIdBytes;
                _hostConnectionData = allocation.HostConnectionData;

                _key = allocation.Key;

                return true;
            }
            catch (RelayServiceException ex)
            {
                Debug.LogError("JoinRelay failed: " + ex.Message);
                return false;
            }


        }

        public (byte[] AllocationId, byte[] Key, byte[] ConnectionData, string _dtlsAddress, int _dtlsPort) GetHostConnectionInfo()
        {
            return (_allocationIdBytes, _key, _connectionData, _ip, _port);
        }

        public (byte[] AllocationId, byte[] Key, byte[] ConnectionData, byte[] HostConnectionData,string _dtlsAddress, int _dtlsPort) GetClientConnectionInfo()
        {
            return (_allocationIdBytes, _key, _connectionData,_hostConnectionData, _ip, _port);
        }
    }
}