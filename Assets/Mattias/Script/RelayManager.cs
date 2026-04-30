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

    private void Awake()
    {
        Instance = this;
    }

    async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
    }

    //HOST MIGRATION
    private void OnClientDisconnect(ulong clientId)
    {
        //If HOST disconnects
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
        //Shutdown current session
        NetworkManager.Singleton.Shutdown();

        //Small delay to ensure cleanup
        await Task.Delay(1000);

        //New host creates a NEW relay session
        await CreateRelay();
    }

    //RELAY
    public async Task CreateRelay()
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2);

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

    void Update()
    {
        if (!NetworkManager.Singleton.IsListening) return;

        int playerCount = NetworkManager.Singleton.ConnectedClients.Count;

        if (playerCount >= 2)
        {
            showCode = false;
        }
    }
}