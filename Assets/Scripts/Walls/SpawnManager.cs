using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject wallDotPrefab;
    public GameObject tankPrefab;
    public GameObject fastPrefab;
    public GameObject healthPrefab;
    public GameObject bossPrefab;

    [Header("Spawn Odds (sum ≤ 1.0)")]
    [Range(0f, 1f)] public float tankChance   = 0.15f;
    [Range(0f, 1f)] public float fastChance   = 0.12f;
    [Range(0f, 1f)] public float healthChance = 0.08f;
    public int bossWave = 6;

    [Header("Timing")]
    public float spawnInterval = 0.8f;

    private float timer = 0f;
    private int currentWave = 1;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer < spawnInterval) return;
        timer = 0f;
        TrySpawn();
    }

    private void TrySpawn()
    {
        // Boss appears on its designated wave
        if (currentWave == bossWave)
        {
            Spawn(bossPrefab);
            currentWave++;
            return;
        }

        // Otherwise pick based on odds
        float r = Random.value;
        if (r < tankChance)
            Spawn(tankPrefab);
        else if (r < tankChance + fastChance)
            Spawn(fastPrefab);
        else if (r < tankChance + fastChance + healthChance)
            Spawn(healthPrefab);
        else
            Spawn(wallDotPrefab);
    }

    private void Spawn(GameObject prefab)
    {
        // Example spawn logic: random point within 5 units on XZ-plane
        Vector2 circle = Random.insideUnitCircle * 5f;
        Vector3 spawnPos = new Vector3(circle.x, 0f, circle.y);
        Instantiate(prefab, spawnPos, Quaternion.identity);
    }

    /// <summary>
    /// Call this from your wave manager to advance waves manually.
    /// </summary>
    public void NextWave()
    {
        currentWave++;
    }
}
