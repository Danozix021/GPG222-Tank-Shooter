using UnityEngine;
using Unity.Netcode;
public class Shoot : NetworkBehaviour
{

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    void Update()
    {
        if (!IsOwner) return;


        if (Input.GetMouseButtonDown(0))
        {
            ShootServerRpc();
        }
    }


    [ServerRpc]
    private void ShootServerRpc()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<NetworkObject>().Spawn();
    }
}
