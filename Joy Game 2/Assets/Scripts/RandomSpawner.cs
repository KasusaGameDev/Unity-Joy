using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    [Header("Spawn Positions")]
    public List<Transform> spawnPositions = new List<Transform>();

    [Header("Prefabs")]
    public List<GameObject> prefabs = new List<GameObject>();

    [Header("Spawn On Start")]
    public bool spawnOnStart = true;

    private void Start()
    {
        if (spawnOnStart)
        {
            SpawnAll();
        }
    }

    public void SpawnAll()
    {
        if (spawnPositions.Count == 0 || prefabs.Count == 0)
        {
            Debug.LogWarning("SpawnPositions atau Prefabs kosong.");
            return;
        }

        foreach (Transform spawnPoint in spawnPositions)
        {
            GameObject randomPrefab = prefabs[Random.Range(0, prefabs.Count)];
            Instantiate(randomPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
