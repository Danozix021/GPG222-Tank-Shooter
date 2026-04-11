using UnityEngine;
using Unity.Netcode;

public class PowerUpSpawner : NetworkBehaviour
{
    public GameObject powerUpPrefab;

    public float minSpawnTime = 5f;
    public float maxSpawnTime = 10f;

    private void Start()
    {
        if (IsServer)
        {
            StartCoroutine(SpawnLoop());
        }
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
        Vector3 randomPos = new Vector3(
            Random.Range(-8f, 8f),
            Random.Range(-4f, 4f),
            0f
        );

        GameObject powerUp = Instantiate(powerUpPrefab, randomPos, Quaternion.identity);
        powerUp.GetComponent<NetworkObject>().Spawn();
    }
}