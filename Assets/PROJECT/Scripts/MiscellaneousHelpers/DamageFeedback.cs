using CustomInspector;
using System.Collections;
using System.Linq;
using UnityEngine;

public class DamageFeedback : MonoBehaviour
{
    [Header("Color Animation Settings")]
    [SerializeField] float duration;
    [SerializeField] Color takeDamageColor;
    [SerializeField] float targetIntensity;

    [Header("scale Animation  Settings")]
    [SerializeField] float takeDamageScale;
    [SerializeField] AnimationCurve damageCurve;
    [SerializeField] bool animateScale;
    [SerializeField] Transform scaleParent;

    [SerializeField] bool getMultipleRenderers = false;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Renderer[] renderers;

    float startZScale;
    float startXScale;

    float[] startZScales;
    float[] startXScales;

    float currentIntensity;

    bool isTakingDamage;
    float elapsedTime;
    Color startColor;
    Color startEmisiveColor;
    Color[] startColors;
    Color[] startEmisiveColors;


    private void OnEnable()
    {
        IDamageable.OnDamageTaken += EnableTakeDamageEffects;
    }

    private void OnDisable()
    {
        IDamageable.OnDamageTaken -= EnableTakeDamageEffects;
    }
    private void Start()
    {
        if (scaleParent != null)
        {
            startZScale = scaleParent.localScale.z;
            startXScale = scaleParent.localScale.x;
        }

        if (meshRenderer == null && !getMultipleRenderers)
        {
            TryGetComponent(out meshRenderer);
            if (meshRenderer != null) return;

            foreach (Transform child in transform)
            {
                if (scaleParent == null)
                {
                    startZScale = transform.localScale.z;
                    startXScale = transform.localScale.x;
                }


                if (!child.TryGetComponent(out meshRenderer)) continue;

                if (meshRenderer.material.HasProperty("_EmissionColor"))
                { startEmisiveColor = meshRenderer.material.GetColor("_EmissionColor"); }

                if (meshRenderer.material.HasProperty("_BaseColor"))
                {
                    startColor = meshRenderer.material.GetColor("_BaseColor");
                }
                else
                {
                    startColor = meshRenderer.material.color;
                }

                return;
            }
        }

        if (!getMultipleRenderers) return;

        renderers = transform.GetComponentsInChildren<Renderer>();

        startZScales = new float[renderers.Length];
        startXScales = new float[renderers.Length];

        startColors = new Color[renderers.Length];
        startEmisiveColors = new Color[renderers.Length];

        if (renderers.Length == 0) return;

        for (int i = 0; i < renderers.Length; i++)
        {
            // this for getting the toon shader property , can be replaced with any other shader property
            if (renderers[i].material.HasProperty("_BaseColor"))
            {
                startColors[i] = renderers[i].material.GetColor("_BaseColor");
            }
            else
            {
                startColors[i] = renderers[i].material.color;
            }

            if (renderers[i].material.HasProperty("_EmissionColor"))
            { startEmisiveColors[i] = renderers[i].material.GetColor("_EmissionColor"); }

            startZScales[i] = renderers[i].transform.localScale.z;
            startXScales[i] = renderers[i].transform.localScale.x;
        }
    }

    void EnableTakeDamageEffects(int damage, GameObject _damageable)
    {
        if (_damageable == gameObject && damage != 0)
        {
            elapsedTime = 0;
            isTakingDamage = true;
        }
    }

    private void Update()
    {
        ShowTakeDamageEffects();
    }

    void ShowTakeDamageEffects()
    {
        if (!isTakingDamage) return;
        if (meshRenderer == null && renderers.Length == 0) return;
        if (duration >= elapsedTime)
        {
            elapsedTime += Time.deltaTime;
            float _lerpValue = damageCurve.Evaluate(elapsedTime / duration);
            if (getMultipleRenderers && renderers[0])
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    SetScaleAndColorOfRenderers(renderers[i], renderers[i].transform, startColors[i],
                    startEmisiveColors[i], _lerpValue, startZScales[i], startZScales[i]);
                }
            }
            else
            {
                SetScaleAndColorOfRenderers(meshRenderer, meshRenderer.transform, startColor,
                startEmisiveColor, _lerpValue, startZScale, startXScale);
            }
        }
        else { isTakingDamage = false; }
    }

    void SetScaleAndColorOfRenderers(Renderer renderer, Transform meshScale, Color targetStartColor, Color targetStartEmisiveColor,
        float _lerpValue, float startZScale, float startXScale)
    {
        if (!animateScale) return;

        if (scaleParent == null)
        {
            float _localScaleZ = Mathf.Lerp(takeDamageScale, startZScale, _lerpValue);
            float _localScaleX = Mathf.Lerp(takeDamageScale, startXScale, _lerpValue);
            meshScale.localScale = new Vector3(_localScaleX, meshScale.localScale.y, _localScaleZ);
        }
        else
        {
            float _localScaleZ = Mathf.Lerp(takeDamageScale, this.startXScale, _lerpValue);
            float _localScaleX = Mathf.Lerp(takeDamageScale, this.startXScale, _lerpValue);
            scaleParent.localScale = new Vector3(_localScaleX, scaleParent.localScale.y, _localScaleZ);
        }

        foreach (Material material in renderer.materials)
        {
            Color targetColor = Color.Lerp(takeDamageColor, targetStartColor, _lerpValue);
            currentIntensity = Mathf.Lerp(targetIntensity, 0, _lerpValue);
            material.SetColor("_EmissionColor", Color.Lerp(takeDamageColor / 3f, targetStartEmisiveColor, _lerpValue));
            // this for getting the toon shader property , can be replaced with any other shader property
            if (renderer.material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", targetColor);
            }
            else
            {
                material.color = targetColor;
            }
        }
    }
}