using UnityEngine;
using Unity.Netcode;

public class PlayerHealth : NetworkBehaviour
{
    public int maxHealth = 100;
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>();

    
    public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>();

    private void Awake()
    {
        currentHealth.Value = maxHealth;
    }

    private void Start()
    {
        //Subscribe to position changes
        networkPosition.OnValueChanged += OnPositionChanged;
    }

    private void OnPositionChanged(Vector3 oldPos, Vector3 newPos)
    {
        transform.position = newPos;
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(int damage)
    {
        if (!IsServer) return;

        currentHealth.Value -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage! Current Health: {currentHealth.Value}");

        if (currentHealth.Value <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} died!");

        //Choose a random respawn position
        Vector3 respawnPosition = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0f);

        //Update the NetworkVariable to sync with all clients
        networkPosition.Value = respawnPosition;

        //Reset health
        currentHealth.Value = maxHealth;
    }
}