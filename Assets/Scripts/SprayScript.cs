using Unity.Hierarchy;
using UnityEngine;

public class SprayScript : MonoBehaviour
{
    public float growRate = 1.5f;
    public float maxSize = 3f;
    public float lifeTime = 5f;

    private float currentSize = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentSize = transform.localScale.x;
        int bulletLayer = LayerMask.NameToLayer("Bullet");
        int defaultLayer = LayerMask.NameToLayer("Default");
        Physics.IgnoreLayerCollision(bulletLayer, bulletLayer, true);
        Physics.IgnoreLayerCollision(bulletLayer, defaultLayer, true);

        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentSize < maxSize)
        {
            currentSize += growRate * Time.deltaTime;
            transform.localScale = new Vector3(currentSize, transform.localScale.y, currentSize);
        }
    }
}
