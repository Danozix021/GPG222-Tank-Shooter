using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class Shoot : NetworkBehaviour
{
    [Header("Weapons")]
    public WeaponData defaultWeapon;
    public WeaponData shotgunWeapon;

    [HideInInspector] public WeaponData currentWeapon;

    [Header("References")]
    public Transform firePoint;

    private float nextFireTime = 0f;
    private Coroutine weaponResetCoroutine;

    void Start()
    {
        currentWeapon = defaultWeapon;
    }

    void Update()
    {
        if (!IsOwner) return;
        if (currentWeapon == null || firePoint == null) return;

        bool wantsToShoot = currentWeapon.fullAuto
            ? Input.GetMouseButton(0)
            : Input.GetMouseButtonDown(0);

        if (!wantsToShoot) return;

        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + (1f / currentWeapon.fireRate);
            ShootRpc();
        }
    }

    [Rpc(SendTo.Server)]
    private void ShootRpc()
    {
        if (currentWeapon == null || currentWeapon.bulletPrefab == null || firePoint == null)
            return;

        for (int i = 0; i < currentWeapon.bulletsPerShot; i++)
        {
            float spreadOffset = Random.Range(-currentWeapon.spreadAngle, currentWeapon.spreadAngle);
            Quaternion bulletRotation = firePoint.rotation * Quaternion.Euler(0f, 0f, spreadOffset);

            GameObject bulletObj = Instantiate(currentWeapon.bulletPrefab, firePoint.position, bulletRotation);

            Bullet bullet = bulletObj.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet.speed = currentWeapon.bulletSpeed;
                bullet.lifeTime = currentWeapon.bulletLifeTime;
                bullet.damage = currentWeapon.damage;
                bullet.shooterClientId = OwnerClientId;
            }

            NetworkObject netObj = bulletObj.GetComponent<NetworkObject>();
            if (netObj != null)
            {
                netObj.Spawn();
            }
        }
    }

    [Rpc(SendTo.Server)]
    public void ApplyTemporaryWeaponRpc(string weaponName, float duration)
    {
        WeaponData newWeapon = GetWeaponByName(weaponName);
        if (newWeapon == null) return;

        ApplyWeaponClientRpc(weaponName, duration);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ApplyWeaponClientRpc(string weaponName, float duration)
    {
        WeaponData newWeapon = GetWeaponByName(weaponName);
        if (newWeapon == null) return;

        currentWeapon = newWeapon;

        if (weaponResetCoroutine != null)
        {
            StopCoroutine(weaponResetCoroutine);
        }

        weaponResetCoroutine = StartCoroutine(ResetWeaponAfterTime(duration));
    }

    private IEnumerator ResetWeaponAfterTime(float duration)
    {
        yield return new WaitForSeconds(duration);
        currentWeapon = defaultWeapon;
    }

    private WeaponData GetWeaponByName(string weaponName)
    {
        if (defaultWeapon != null && defaultWeapon.weaponName == weaponName)
            return defaultWeapon;

        if (shotgunWeapon != null && shotgunWeapon.weaponName == weaponName)
            return shotgunWeapon;

        return null;
    }
}