using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Bug Prefabs")]
    public GameObject tankPrefab;
    public GameObject fastPrefab;
    public GameObject healthPrefab;
    public GameObject bossPrefab;

    [Header("Spawn Odds (must sum to 1.0)")]
    [Range(0f, 1f)] public float tankChance   = 0.33f;
    [Range(0f, 1f)] public float fastChance   = 0.33f;
    [Range(0f, 1f)] public float healthChance = 0.34f;
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
        float r = Random.value;
        Debug.Log($"TrySpawn() r={r:F2}, wave={currentWave}");

        // boss
        if (currentWave == bossWave)
        {
            Debug.Log(" → BossBug");
            Spawn(bossPrefab);
            currentWave++;
            return;
        }

        // tank
        if (r < tankChance)
        {
            Debug.Log(" → TankBug");
            Spawn(tankPrefab);
        }
        // fast
        else if (r < tankChance + fastChance)
        {
            Debug.Log(" → FastBug");
            Spawn(fastPrefab);
        }
        // health
        else
        {
            Debug.Log(" → HealthBug");
            Spawn(healthPrefab);
        }

        currentWave++;
    }

    private void Spawn(GameObject prefab)
    {
        Vector2 circle = Random.insideUnitCircle * 5f;
        Vector3 spawnPos = new Vector3(circle.x, 0f, circle.y);
        Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}
