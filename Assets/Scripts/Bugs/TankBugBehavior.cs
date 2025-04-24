using System.Collections;
using UnityEngine;

[RequireComponent(typeof(WallDotMovement))]
public class TankBugBehavior : MonoBehaviour
{
    [Header("TankBug Flash")]
    public Material flashMaterial;
    public float flashDuration = 0.1f;

    private WallDotMovement wallDot;
    private Renderer rend;
    private Material originalMaterial;
    private int hitCount;

    void Awake()
    {
        wallDot = GetComponent<WallDotMovement>();
        rend = GetComponentInChildren<Renderer>();
        originalMaterial = rend.material;
    }

    void OnEnable() => wallDot.OnHit += HandleHit;
    void OnDisable() => wallDot.OnHit -= HandleHit;

    private void HandleHit()
    {
        hitCount++;
        StartCoroutine(FlashRoutine());

        if (hitCount >= 2)
        {
            Camera.main.GetComponent<CameraShaker>()?.Shake(0.2f, 0.1f);
            wallDot.Kill(Color.red);
        }
    }

    private IEnumerator FlashRoutine()
    {
        rend.material = flashMaterial;
        yield return new WaitForSeconds(flashDuration);
        rend.material = originalMaterial;
    }
}
