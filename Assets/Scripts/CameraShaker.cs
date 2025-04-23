using UnityEngine;
public class CameraShaker : MonoBehaviour {
  public void Shake(float duration, float magnitude) {
    StartCoroutine(ShakeRoutine(duration, magnitude));
  }
  private System.Collections.IEnumerator ShakeRoutine(float dur, float mag) {
    Vector3 originalPos = transform.localPosition;
    float elapsed = 0f;
    while (elapsed < dur) {
      float x = Random.Range(-1f,1f) * mag;
      float y = Random.Range(-1f,1f) * mag;
      transform.localPosition = originalPos + new Vector3(x,y,0);
      elapsed += Time.deltaTime;
      yield return null;
    }
    transform.localPosition = originalPos;
  }
}
