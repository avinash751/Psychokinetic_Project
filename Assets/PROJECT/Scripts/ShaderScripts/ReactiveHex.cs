using CustomInspector;
using System;
using UnityEngine;

public class ReactiveHex : MonoBehaviour, INeedPlayerRefs
{
    [HorizontalLine("Grid Material Settings", 4, FixedColor.DustyBlue)]
    public Material gridMaterial;
    [SerializeField] Color interactionColor = Color.red;
    [SerializeField] Color FadeOutColor = Color.clear;
    [SerializeField] float gradientSpeed;
    [SerializeField] float colorFadeDuration = 1f;

    [HorizontalLine("Debug Information", 4, FixedColor.DustyBlue)]
    [SerializeField][ReadOnly] Color currentInteractionColor = Color.clear;
    [SerializeField][ReadOnly] Color FadedColor;
    [SerializeField][ReadOnly] Vector3 lastPlayerPosition;
    [SerializeField][ReadOnly] Transform playerTransform;
    [SerializeField][ReadOnly] bool isFadingOut = true;
    [SerializeField][ReadOnly] bool FadingT;
    [SerializeField][ReadOnly] float elapsedTime;

    void Start()
    {
        playerTransform = ReferenceManager.Instance.PlayerTransform;
        lastPlayerPosition = playerTransform.position;
        gridMaterial.SetFloat("_LightSpeed", gradientSpeed);
    }

    void Update()
    {
        if (!gridMaterial && !playerTransform) return;
        gridMaterial.SetVector("_PlayerPos", playerTransform.position);

        // Check if player position has changed
        if (playerTransform.position != lastPlayerPosition)
        {
            lastPlayerPosition = playerTransform.position;
            isFadingOut = false;
            currentInteractionColor = FadedColor;
            elapsedTime = 0;
        }

        if (!isFadingOut)
        {
            FadeMaterialColor(interactionColor, true);
        }
        else if (isFadingOut)
        {
            FadeMaterialColor(FadeOutColor,true);
        }
    }

    private void FadeMaterialColor(Color fadeTransitionColor, bool fadeBoolAfterTimer)
    {

        if (elapsedTime >= colorFadeDuration)
        {
            isFadingOut = fadeBoolAfterTimer;
            elapsedTime = 0;
            currentInteractionColor = fadeTransitionColor;
        }
        else
        {
            elapsedTime += Time.deltaTime;
            // Calculate the fade amount based on time
            float t = elapsedTime / colorFadeDuration; // Fading over 2 seconds
                                           // Interpolate between the interaction color and clear color
            Color fadedColor = Color.Lerp(currentInteractionColor, fadeTransitionColor, t);
            FadedColor = fadedColor;
            gridMaterial.SetColor("_InteractionCol", fadedColor);
        }
    }
}

