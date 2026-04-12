using UnityEngine;
using Unity.Netcode;

public class Bullet : NetworkBehaviour
{
    private Rigidbody2D rb;

    public float speed;
    public float lifeTime;
    public int damage;
    public ulong shooterClientId;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnNetworkSpawn()
    {
        rb.linearVelocity = transform.up * speed;

        if (IsServer)
        {
            Invoke(nameof(DespawnBullet), lifeTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return;

        NetworkObject otherNetObj = other.GetComponentInParent<NetworkObject>();
        if (otherNetObj != null && otherNetObj.OwnerClientId == shooterClientId)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponentInParent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamageRpc(damage);
            }

            DespawnBullet();
            return;
        }

        if (other.CompareTag("Wall"))
        {
            DespawnBullet();
        }
    }

    private void DespawnBullet()
    {
        if (IsServer && NetworkObject != null && NetworkObject.IsSpawned)
        {
            NetworkObject.Despawn(true);
        }
    }
}