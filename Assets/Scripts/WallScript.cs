using UnityEngine;
using System.Collections;

public class WallScript : MonoBehaviour
{
    public GameObject WallDot;
    public int numberOfDots = 5;
    public float spawnDuration = 3.0f;

    void Start()
    {
        StartCoroutine(SpawnDotsOverTime());
    }


    void Update()
    {
        
    }

    IEnumerator SpawnDotsOverTime()
    {
        float totalTime = 0f;
        float maxDelay = spawnDuration / numberOfDots * 2;

        for (int i = 0; i < numberOfDots; i++)
        {

            float x = Random.Range(-4.5f, 4.5f);
            float y = Random.Range(-2.0f, 2.0f);
            Vector3 spawnPosition = transform.position + new Vector3(x, y, 0);

            Instantiate(WallDot, spawnPosition, Quaternion.identity);

            float randomDelay = Random.Range(0, maxDelay);
            totalTime += randomDelay;

            if (totalTime > spawnDuration) break;

            yield return new WaitForSeconds(randomDelay);
            
            
        }
    }

}
