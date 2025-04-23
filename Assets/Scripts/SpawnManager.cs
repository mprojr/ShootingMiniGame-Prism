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
    [Range(0, 1)] public float tankChance   = 0.15f;
    [Range(0, 1)] public float fastChance   = 0.12f;
    [Range(0, 1)] public float healthChance = 0.08f;
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
        // Boss on its special wave
        if (currentWave == bossWave)
        {
            Spawn(bossPrefab);
            currentWave++;
            return;
        }

        // Otherwise random pick
        float r = Random.value;
        if (r < tankChance)          Spawn(tankPrefab);
        else if (r < tankChance + fastChance)
                                    Spawn(fastPrefab);
        else if (r < tankChance + fastChance + healthChance)
                                    Spawn(healthPrefab);
        else                         Spawn(wallDotPrefab);
    }

    private void Spawn(GameObject prefab)
    {
        // You can customize spawn location logic here
        Vector3 spawnPos = /* e.g. Random.insideUnitCircle or a fixed point */;
        Instantiate(prefab, spawnPos, Quaternion.identity);
    }

    // Call this from your wave-complete logic to bump up the difficulty
    public void NextWave()
    {
        currentWave++;
    }
}
