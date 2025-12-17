using UnityEngine;
using System.Collections;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Zombie Settings")]
    public GameObject zombiePrefab;
    public int zombiesPerWave = 5;
    public float spawnInterval = 30f;

    [Header("Spawn Requirement")]
    public float spawnRange = 25f; // Player must be inside this distance
    private Transform player;

    [Header("Spawn Zones")]
    public Transform[] spawnZones;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (PlayerIsClose())
            {
                SpawnWave();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    bool PlayerIsClose()
    {
        if (player == null) return false;
        float distance = Vector3.Distance(player.position, transform.position);
        return distance <= spawnRange;
    }

    void SpawnWave()
    {
        for (int i = 0; i < zombiesPerWave; i++)
        {
            SpawnZombie();
        }
    }

    void SpawnZombie()
    {
        Transform zone = spawnZones[Random.Range(0, spawnZones.Length)];
        Vector3 spawnPos = zone.position;

        if (zone.childCount > 0)
            spawnPos = zone.GetChild(Random.Range(0, zone.childCount)).position;

        Instantiate(zombiePrefab, spawnPos, Quaternion.identity);
    }
}
