using UnityEngine;
using Unity.Netcode;

public class PlayerHealth : NetworkBehaviour
{
    public int maxHealth = 100;
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>();

    private bool isInvulnerable = false;

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

        if (isInvulnerable) return;

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

        Vector3 respawnPosition = new Vector3(
            Random.Range(-5f, 5f),
            Random.Range(-5f, 5f),
            0f
        );

        RespawnClientRpc(respawnPosition);

        currentHealth.Value = maxHealth;

        StartCoroutine(InvulnerabilityCoroutine());
    }

    [ClientRpc]
    private void RespawnClientRpc(Vector3 position)
    {
        //Only move the owner of this object
        if (!IsOwner) return;

        transform.position = position;
    }

    private System.Collections.IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;
        Debug.Log(gameObject.name + " is invulnerable");

        yield return new WaitForSeconds(3f);

        isInvulnerable = false;
        Debug.Log(gameObject.name + " is no longer invulnerable");
    }
}