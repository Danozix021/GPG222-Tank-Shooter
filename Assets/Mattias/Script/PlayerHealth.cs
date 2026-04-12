using UnityEngine;
using Unity.Netcode;

public class PlayerHealth : NetworkBehaviour
{
    public int maxHealth = 100;
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>();

    private void Start()
    {
        Die();
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void TakeDamageRpc(int damage)
    {
        if (!IsServer) return;

        currentHealth.Value -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage");

        if (currentHealth.Value <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " died");

        Vector3 respawnPosition = GetSpawnPointPosition();

        
        currentHealth.Value = maxHealth;

        
        RespawnClientRpc(respawnPosition);
    }

    private Vector3 GetSpawnPointPosition()
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points found! Respawning at (0,0,0)");
            return Vector3.zero;
        }

        
        int index = (int)(OwnerClientId % (ulong)spawnPoints.Length);
        return spawnPoints[index].transform.position;
    }

    [ClientRpc]
    private void RespawnClientRpc(Vector3 respawnPosition)
    {
        transform.position = respawnPosition;
    }
}