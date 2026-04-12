using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class SpeedPowerUp : NetworkBehaviour
{
    public float lifeTime = 10f;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        StartCoroutine(AutoDespawn());
    }

    private IEnumerator AutoDespawn()
    {
        yield return new WaitForSeconds(lifeTime);

        if (NetworkObject != null && NetworkObject.IsSpawned)
        {
            NetworkObject.Despawn(true);
            Debug.Log("Powerup auto-despawned");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return;

        if (!other.CompareTag("Player")) return;

        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if (player != null)
        {
            player.GiveSpeedBoostClientRpc();
        }

        NetworkObject.Despawn(true);
    }
}