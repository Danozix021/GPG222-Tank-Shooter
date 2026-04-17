using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
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
    }

    public async void CreateRelay()
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2);

        currentJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        showCode = true;

        Debug.Log("JOIN CODE: " + currentJoinCode);

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