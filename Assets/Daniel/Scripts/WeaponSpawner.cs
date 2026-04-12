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

    private NetworkObject currentPickup;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        Debug.Log("Spawner started on server");
        InvokeRepeating(nameof(TrySpawnPickup), 2f, spawnInterval);
    }

    private void TrySpawnPickup()
    {
        if (!IsServer) return;

        Debug.Log("TrySpawnPickup called");

        if (pickupPrefab == null)
        {
            Debug.LogError("pickupPrefab is NULL");
            return;
        }

        if (currentPickup != null && currentPickup.IsSpawned)
        {
            Debug.Log("Pickup already exists");
            return;
        }

        Vector2 randomPos = GetRandomPositionInArea();

        GameObject pickupObj = Instantiate(
            pickupPrefab,
            new Vector3(randomPos.x, randomPos.y, 0f),
            Quaternion.identity
        );

        WeaponPickup pickup = pickupObj.GetComponent<WeaponPickup>();
        if (pickup != null)
        {
            pickup.weaponToGive = shotgunWeapon;
        }
        else
        {
            Debug.LogError("WeaponPickup script missing on pickup prefab");
        }

        NetworkObject netObj = pickupObj.GetComponent<NetworkObject>();
        if (netObj == null)
        {
            Debug.LogError("NetworkObject missing on pickup prefab");
            Destroy(pickupObj);
            return;
        }

        netObj.Spawn();
        currentPickup = netObj;

        Debug.Log("Pickup spawned at: " + randomPos);
    }

    private Vector2 GetRandomPositionInArea()
    {
        float x = Random.Range(areaCenter.x - areaSize.x / 2f, areaCenter.x + areaSize.x / 2f);
        float y = Random.Range(areaCenter.y - areaSize.y / 2f, areaCenter.y + areaSize.y / 2f);
        return new Vector2(x, y);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(areaCenter, areaSize);
    }
}