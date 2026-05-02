using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public class RelayManager : MonoBehaviour
{
    public static RelayManager Instance;

    public UnityTransport transport;

    public string currentJoinCode = "";
    public bool showCode = false;

    private async void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        await InitializeUnityServices();
    }

    private async Task InitializeUnityServices()
    {
        if (UnityServices.State == ServicesInitializationState.Initialized)
            return;

        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    private void Start()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
        }
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (clientId == 0 && NetworkManager.Singleton.IsServer)
        {
            Debug.Log("HOST LEFT - starting migration...");

            List<ulong> remainingClients = new List<ulong>(NetworkManager.Singleton.ConnectedClientsIds);

            if (remainingClients.Count > 0)
            {
                ulong newHostId = remainingClients[0];
                Debug.Log("NEW HOST WILL BE: " + newHostId);
                PromoteNewHost(newHostId);
            }
        }
    }

    private async void PromoteNewHost(ulong newHostId)
    {
        NetworkManager.Singleton.Shutdown();
        await Task.Delay(1000);
        await CreateRelay();
    }

    public async Task CreateRelay()
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);

        currentJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        showCode = true;

        Debug.Log("NEW JOIN CODE: " + currentJoinCode);

        transport.SetHostRelayData(
            allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData
        );

        NetworkManager.Singleton.StartHost();
    }

    public async void JoinRelay(string joinCode)
    {
        JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

        transport.SetClientRelayData(
            allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData,
            allocation.HostConnectionData
        );

        NetworkManager.Singleton.StartClient();
    }

    private void Update()
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening) return;

        int playerCount = NetworkManager.Singleton.ConnectedClients.Count;

        if (playerCount >= 2)
        {
            showCode = false;
        }
    }
}