using UnityEngine;
using Unity.Netcode;

public class WeaponPickup : NetworkBehaviour
{
    public WeaponData weaponToGive;
    public float duration = 5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return;

        Shoot playerShoot = other.GetComponentInParent<Shoot>();
        if (playerShoot == null) return;

        playerShoot.ApplyTemporaryWeaponRpc(weaponToGive.weaponName, duration);

        if (NetworkObject != null && NetworkObject.IsSpawned)
        {
            NetworkObject.Despawn(true);
        }
    }
}