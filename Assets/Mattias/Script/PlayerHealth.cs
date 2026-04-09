using UnityEngine;
using Unity.Netcode;

public class PlayerHealth : NetworkBehaviour
{
    public int maxHealth = 100;
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>();

    private void Start()
    {
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(int damage)
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

        // Simple respawn
        Vector3 respawnPosition = new Vector3(
            Random.Range(-5f, 5f),
            Random.Range(-5f, 5f),
            0f
        );

        // Move player (NetworkTransform will sync this)
        transform.position = respawnPosition;

        // Reset health
        currentHealth.Value = maxHealth;
    }
}