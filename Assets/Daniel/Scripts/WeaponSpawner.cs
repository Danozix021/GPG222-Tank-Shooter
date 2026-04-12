using UnityEngine;
using Unity.Netcode;

public class WeaponPickupSpawner : NetworkBehaviour
{
    [Header("Pickup")]
    public GameObject pickupPrefab;
    public WeaponData shotgunWeapon;
    public float spawnInterval = 8f;

    [Header("Spawn Area")]
    public Vector2 areaCenter = Vector2.zero;
    public Vector2 areaSize = new Vector2(16f, 9f);

    [Header("Spawn Checking")]
    public float checkRadius = 0.5f;
    public int maxSpawnAttempts = 25;

    private NetworkObject currentPickup;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        InvokeRepeating(nameof(TrySpawnPickup), 2f, spawnInterval);
    }

    private void TrySpawnPickup()
    {
        if (!IsServer) return;

        if (currentPickup != null && currentPickup.IsSpawned)
            return;

        Vector2 spawnPos;
        bool foundValidSpot = TryGetValidSpawnPosition(out spawnPos);

        if (!foundValidSpot)
        {
            Debug.LogWarning("Could not find a valid pickup spawn position.");
            return;
        }

        GameObject pickupObj = Instantiate(pickupPrefab, new Vector3(spawnPos.x, spawnPos.y, 0f), Quaternion.identity );

        WeaponPickup pickup = pickupObj.GetComponent<WeaponPickup>();
        if (pickup != null)
        {
            pickup.weaponToGive = shotgunWeapon;
        }

        NetworkObject netObj = pickupObj.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Spawn();
            currentPickup = netObj;
        }
    }

    private bool TryGetValidSpawnPosition(out Vector2 validPosition)
    {
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            float x = Random.Range(areaCenter.x - areaSize.x / 2f, areaCenter.x + areaSize.x / 2f);
            float y = Random.Range(areaCenter.y - areaSize.y / 2f, areaCenter.y + areaSize.y / 2f);

            Vector2 testPosition = new Vector2(x, y);

            if (!IsInsideWall(testPosition))
            {
                validPosition = testPosition;
                return true;
            }
        }

        validPosition = Vector2.zero;
        return false;
    }

    private bool IsInsideWall(Vector2 position)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, checkRadius);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Wall"))
            {
                return true;
            }
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(areaCenter, areaSize);

    }
}