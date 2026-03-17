using UnityEngine;
using Unity.Netcode;

public class Bullet : NetworkBehaviour
{
    public float speed = 10f;
    public float lifeTime = 2f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnNetworkSpawn()
    {
        rb.linearVelocity = transform.up * speed;

        if (IsServer)
        {
            DestroyBulletAfterTime();
        }
    }

    private async void DestroyBulletAfterTime()
    {
        await System.Threading.Tasks.Task.Delay((int)(lifeTime * 1000));

        if (this != null && IsServer && NetworkObject != null && NetworkObject.IsSpawned)
        {
            NetworkObject.Despawn(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return;

        // Check if bullet hit a player
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamageServerRpc(25); // Deal 25 damage
            }

            DespawnBullet();
            return;
        }

        // Check if bullet hit a wall
        if (other.CompareTag("Wall"))
        {
            DespawnBullet();
        }

        // Local function to despawn bullet
        void DespawnBullet()
        {
            if (IsServer && NetworkObject != null && NetworkObject.IsSpawned)
            {
                NetworkObject.Despawn(true);
            }
        }
    }
}