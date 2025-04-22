using Unity.Hierarchy;
using UnityEngine;

public class SprayScript : MonoBehaviour
{
    public float growRate = 1.5f;
    public float maxSize = 100f;
    public float lifeTime = 5f;

    private float currentSize = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentSize = transform.localScale.x;
        
        // Check if layers exist before using them
        int bulletLayer = LayerMask.NameToLayer("Bullet");
        int defaultLayer = LayerMask.NameToLayer("Default");
        
        // Only ignore collisions if the layers exist (not -1)
        if (bulletLayer != -1 && defaultLayer != -1)
        {
            Physics.IgnoreLayerCollision(bulletLayer, bulletLayer, true);
            Physics.IgnoreLayerCollision(bulletLayer, defaultLayer, true);
        }
        else
        {
            Debug.LogWarning("Layer 'Bullet' or 'Default' not found. Please create these layers in your project settings.");
        }

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
