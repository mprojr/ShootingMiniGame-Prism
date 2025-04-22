using System.Linq;
using UnityEngine;

/// <summary>
/// Spawns normal wall-dots and bug variants at random, then drops a boss on a chosen wave.
/// </summary>
public class SpawnManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject wallDotPrefab;
    public GameObject tankBugPrefab;
    public GameObject fastBugPrefab;
    public GameObject healthBugPrefab;
    public GameObject bossBugPrefab;

    [Header("Spawn Zones")]
    [Tooltip("Assign your ISpawnZone implementations here or leave empty to auto-find.")]
    public ISpawnZone[] spawnZones;

    [Header("Variant Probabilities")]
    [Range(0f, 1f)] public float tankProbability = 0.1f;
    [Range(0f, 1f)] public float fastProbability = 0.1f;
    [Range(0f, 1f)] public float healProbability = 0.05f;

    [Header("Timing & Waves")]
    public float spawnInterval = 1f;
    public int bossWave = 5;
    public Transform bossSpawnPoint;

    private float timer;
    private int currentWave = 1;

    void Start()
    {
        if (spawnZones == null || spawnZones.Length == 0)
            spawnZones = Object
                .FindObjectsOfType<MonoBehaviour>()
                .OfType<ISpawnZone>()
                .ToArray();

        timer = spawnInterval;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            if (currentWave == bossWave && bossBugPrefab != null && bossSpawnPoint != null)
            {
                Instantiate(bossBugPrefab, bossSpawnPoint.position, bossSpawnPoint.rotation);
                currentWave++;
            }
            else
            {
                SpawnVariant();
            }
            timer = spawnInterval;
        }
    }

    private void SpawnVariant()
    {
        if (spawnZones.Length == 0) return;

        var zone = spawnZones[Random.Range(0, spawnZones.Length)];
        Vector3 pos = zone.GetSpawnPoint();
        float r = Random.value;

        GameObject toSpawn = wallDotPrefab;
        if (r < tankProbability)
            toSpawn = tankBugPrefab;
        else if (r < tankProbability + fastProbability)
            toSpawn = fastBugPrefab;
        else if (r < tankProbability + fastProbability + healProbability)
            toSpawn = healthBugPrefab;

        Instantiate(toSpawn, pos, Quaternion.identity);
    }

    /// <summary>
    /// Call this from your GameManager when advancing to the next wave.
    /// </summary>
    public void NextWave()
    {
        currentWave++;
    }
}
