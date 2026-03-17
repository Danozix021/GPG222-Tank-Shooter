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

        if (other.CompareTag("Player"))
        {
            Debug.Log("Bullet hit player: " + other.name);

            // Later: apply damage here

            DespawnBullet();
            return;
        }

        if (other.CompareTag("Wall"))
        {
            Debug.Log("Bullet hit wall: " + other.name);
            DespawnBullet();
        }

        void DespawnBullet()
        {
            if (IsServer && NetworkObject != null && NetworkObject.IsSpawned)
            {
                NetworkObject.Despawn(true);
            }
        }
    }
}