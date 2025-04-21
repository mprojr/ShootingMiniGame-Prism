using UnityEngine;
using System.Collections;

public class WallScriptSides : MonoBehaviour, ISpawnZone
{
    public GameObject WallDot;
    public int numberOfDots = 5;
    public float spawnDuration = 3.0f;
    Collider wall_collider;
    Vector3 wall_size;

    void Start()
    {
        wall_collider = GetComponent<MeshCollider>();
        wall_size = wall_collider.bounds.size;
        //StartCoroutine(SpawnDotsOverTime());
    }

    void Update()
    {

    }

    public void Configure(int dotsToSpawn, float duration)
    {
        numberOfDots = dotsToSpawn;
        spawnDuration = duration;
        StartCoroutine(SpawnDotsOverTime());
    }

    IEnumerator SpawnDotsOverTime()
    {
        float totalTime = 0f;
        float maxDelay = spawnDuration / numberOfDots * 2;

        for (int i = 0; i < numberOfDots; i++)
        {

            float z = Random.Range(-wall_size.z / 2, wall_size.z / 2);
            float y = Random.Range(-wall_size.y / 2, wall_size.y / 2);

            Vector3 spawnPosition = transform.position + new Vector3(0, y, z);

            Instantiate(WallDot, spawnPosition, Quaternion.identity);


            float randomDelay = Random.Range(0, maxDelay);
            totalTime += randomDelay;

            if (totalTime > spawnDuration) break;

            yield return new WaitForSeconds(randomDelay);


        }
    }

}
