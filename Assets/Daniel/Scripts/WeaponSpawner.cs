using UnityEngine;
using Unity.Netcode;

public class WeaponSpawner : NetworkBehaviour
{
    [Header("Pickup Prefabs")]
    public GameObject[] pickupPrefabs;
    public float spawnInterval = 8f;

    [Header("Spawn Area")]
    public Vector2 areaCenter = Vector2.zero;
    public Vector2 areaSize = new Vector2(16f, 9f);

    [Header("Spawn Checking")]
    public float checkRadius = 0.5f;
    public int maxSpawnAttempts = 20;

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

        if (pickupPrefabs == null || pickupPrefabs.Length == 0)
        {
            Debug.LogError("No pickup prefabs assigned to WeaponPickupSpawner.");
            return;
        }

        Vector2 spawnPos;

        if (!TryGetValidSpawnPosition(out spawnPos))
        {
            Debug.LogWarning("Could not find a valid pickup spawn position.");
            return;
        }

        GameObject chosenPrefab = pickupPrefabs[Random.Range(0, pickupPrefabs.Length)];

        GameObject pickupObj = Instantiate(
            chosenPrefab,
            new Vector3(spawnPos.x, spawnPos.y, 0f),
            Quaternion.identity
        );

        NetworkObject netObj = pickupObj.GetComponent<NetworkObject>();

        if (netObj == null)
        {
            Debug.LogError("Chosen pickup prefab is missing NetworkObject.");
            Destroy(pickupObj);
            return;
        }

        netObj.Spawn();
        currentPickup = netObj;

        Debug.Log("Spawned pickup prefab: " + chosenPrefab.name);
    }

    private bool TryGetValidSpawnPosition(out Vector2 validPosition)
    {
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            float x = Random.Range(areaCenter.x - areaSize.x / 2f, areaCenter.x + areaSize.x / 2f);
            float y = Random.Range(areaCenter.y - areaSize.y / 2f, areaCenter.y + areaSize.y / 2f);

            Vector2 testPosition = new Vector2(x, y);

            if (!IsBlocked(testPosition))
            {
                validPosition = testPosition;
                return true;
            }
        }

        validPosition = Vector2.zero;
        return false;
    }

    private bool IsBlocked(Vector2 position)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, checkRadius);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Wall") || hit.CompareTag("Player"))
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

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(areaCenter, checkRadius);
    }
}