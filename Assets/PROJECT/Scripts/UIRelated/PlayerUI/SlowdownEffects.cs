using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SlowdownEffects : MonoBehaviour
{
    [SerializeField] Volume volume;
    [SerializeField] Color filterColor = Color.white;
    [SerializeField] float exposure = 0.3f;
    [SerializeField] float hueShift = 20f;
    [SerializeField] float vignetteStrength = 0.4f;
    ColorAdjustments colorAdjustments;
    Vignette vignette;

    void Start()
    {
        ReferenceManager.Instance.Resetter.OnReset += () => UpdateEffects(0.2f, true);
        if (volume.profile.TryGet<ColorAdjustments>(out var colorAdjustments))
        {
            this.colorAdjustments = colorAdjustments;
        }
        if (volume.profile.TryGet<Vignette>(out var vignette))
        {
            this.vignette = vignette;
        }
    }

    public void UpdateEffects(float minTimeScale, bool forceOff = false)
    {
        float timeScale = Time.timeScale;
        if (forceOff) { timeScale = 1; }
        float t = 1 - Mathf.InverseLerp(minTimeScale, 1, timeScale);
        colorAdjustments.colorFilter.value = Color.Lerp(Color.white, filterColor, t);
        colorAdjustments.postExposure.value = Mathf.Lerp(0, exposure, t);
        colorAdjustments.hueShift.value = Mathf.Lerp(0, hueShift, t);
        vignette.intensity.value = Mathf.Lerp(0, vignetteStrength, t);  
    }
}