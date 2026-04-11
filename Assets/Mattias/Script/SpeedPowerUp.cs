using UnityEngine;
using Unity.Netcode;

public class SpeedPowerUp : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return;

        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();

            if (player != null)
            {
                player.GiveSpeedBoostClientRpc();
            }

            NetworkObject.Despawn(true);
        }
    }
}