using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    // Singleton instance
    public static CameraShake Instance { get; private set; }

    private Transform camTransform;
    private Vector3 originalPos;
    private Coroutine shakeRoutine;

    [RuntimeInitializeOnLoadMethod]
    private static void InitializeOnLoad()
    {
        if (Instance == null)
        {
            var go = new GameObject("CameraShake");
            Instance = go.AddComponent<CameraShake>();
            DontDestroyOnLoad(go);
        }
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        camTransform = Camera.main != null 
            ? Camera.main.transform 
            : transform;
        originalPos = camTransform.localPosition;
    }

    /// <summary>
    /// Shake the camera for <paramref name="duration"/> seconds at <paramref name="intensity"/>.
    /// </summary>
    public static void Shake(float duration, float intensity)
    {
        if (Instance == null)
            InitializeOnLoad();
        Instance.StartShake(duration, intensity);
    }

    private void StartShake(float duration, float intensity)
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);
        shakeRoutine = StartCoroutine(DoShake(duration, intensity));
    }

    private IEnumerator DoShake(float duration, float intensity)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;
            camTransform.localPosition = originalPos + new Vector3(x, y, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        camTransform.localPosition = originalPos;
        shakeRoutine = null;
    }
}
