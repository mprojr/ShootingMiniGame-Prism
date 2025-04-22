using UnityEngine;
using UnityEngine.UI;

public class ScreenTintController : MonoBehaviour
{
    
    public Image tintOverlay;

    private void Awake()
    {
        if (tintOverlay != null)
        {
            tintOverlay.gameObject.SetActive(false);
        }
    }

    public void ShowTint(Color tintColor, float duration)
    {
        if (tintOverlay == null) return;
        tintColor.a = 0.07f;
        tintOverlay.color = tintColor;
        tintOverlay.gameObject.SetActive(true);
        CancelInvoke(nameof(HideTint)); 
        Invoke(nameof(HideTint), duration);
    }

    private void HideTint()
    {
        if (tintOverlay != null)
        {
            tintOverlay.gameObject.SetActive(false);
        }
    }


}
