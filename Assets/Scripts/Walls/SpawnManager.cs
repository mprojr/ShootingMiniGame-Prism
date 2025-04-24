using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private ISpawnZone[] spawnZones;

    private int baseDotCount = 30;
    public float baseSpawnDuration = 90f;

    void Start()
    {
        spawnZones = FindObjectsOfType<MonoBehaviour>().OfType<ISpawnZone>().ToArray();
        Debug.Log($"[SpawnManager] Found {spawnZones.Length} spawn zones.");

        int difficulty = GameManager.Instance.diffcultLevel;
        Debug.Log($"[SpawnManager] Current difficulty from GameManager: {difficulty}");

        int totalDots = baseDotCount * difficulty + 40;
        Debug.Log($"[SpawnManager] Total dots to distribute: {totalDots}");

        float spawnDuration = baseSpawnDuration;

        DistributeDots(totalDots, spawnDuration);
    }

    void Update()
    {
        
    }

    void DistributeDots(int totalDots, float spawnDuration)
    {
        if (spawnZones.Length == 0)
        {
            Debug.LogWarning("No spawn zones found!");
            return;
        }

        int dotsPerZone = totalDots / spawnZones.Length;
        int extraDots = totalDots % spawnZones.Length;

        for (int i = 0; i < spawnZones.Length; i++)
        {
            int finalDots = dotsPerZone + (i < extraDots ? 1 : 0);
            spawnZones[i].Configure(finalDots, spawnDuration);
        }
    }
}
