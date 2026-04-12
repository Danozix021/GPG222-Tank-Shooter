using UnityEngine;
using Unity.Netcode;

public class PowerUpSpawner : NetworkBehaviour
{
    public GameObject powerUpPrefab;

    public float minSpawnTime = 5f;
    public float maxSpawnTime = 10f;

    public float spawnCheckRadius = 0.4f;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        StartCoroutine(SpawnLoop());
    }

    private System.Collections.IEnumerator SpawnLoop()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            SpawnPowerUp();
        }
    }

    private void SpawnPowerUp()
    {
        Vector3 spawnPos;
        int tries = 0;

        do
        {
            spawnPos = new Vector3(
                Random.Range(-8f, 8f),
                Random.Range(-4f, 4f),
                0f
            );

            tries++;

        }
        while (IsInsideWall(spawnPos) && tries < 20);

        GameObject powerUp = Instantiate(powerUpPrefab, spawnPos, Quaternion.identity);
        powerUp.GetComponent<NetworkObject>().Spawn();

        Debug.Log("PowerUp spawned at: " + spawnPos);
    }

    private bool IsInsideWall(Vector3 pos)
    {
        Collider2D hit = Physics2D.OverlapCircle(pos, spawnCheckRadius);

        if (hit != null && hit.CompareTag("Wall"))
        {
            return true;
        }

        return false;
    }
}