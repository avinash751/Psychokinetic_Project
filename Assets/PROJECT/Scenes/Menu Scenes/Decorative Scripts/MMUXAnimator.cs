using System.Collections;
using System.Collections.Generic;
using TransitionsPlus;
using UnityEngine;
using UnityEngine.UI;

public class MMUXAnimator : MonoBehaviour
{
    [SerializeField] private Canvas menuCanv;
    [SerializeField] private RawImage mask;
    [SerializeField] private RawImage maskedImage;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Animator bippiAnimator;
    [SerializeField] private TransitionAnimator transition;
    [SerializeField] public TransitionProfile transitionProfile;
    [SerializeField] private Image filterImage;
    [SerializeField] private Image transitionalImage;
    [SerializeField] private float fadeDuration;
    [SerializeField] private float fadeTime;
    private float currentState;
    private float targetState;
    private Matrix4x4 originalProjectionMatrix;

    //button stuff
    [SerializeField] private Button button;
    [SerializeField] private Animator buttonAnimator;


    private void Start()
    {
        bippiAnimator.SetBool("SettingsClicked", false);
        transition.SetProfile(transitionProfile);
        originalProjectionMatrix = Camera.main.projectionMatrix;
    }
    public void StartPressed()
    {
        //goes to level select
        bippiAnimator.SetBool("SettingsClicked", false);
        bippiAnimator.SetFloat("MenuState", 2f);
        buttonAnimator.SetInteger("ButtonState", 2);
    }

   public void SettingsPressed()
    {
        //goes to settingss
        bippiAnimator.SetFloat("MenuState", 5f);
        bippiAnimator.SetBool("SettingsClicked", true);
    }

    public void BackButtonPressed()
    {
        //goes back to landing page
        if (bippiAnimator.GetBool("SettingsClicked") == false)
        {
            //TransitionAnimator.Start(transitionProfile, false, 0.8f);
        }
        bippiAnimator.SetBool("SettingsClicked", false);
        bippiAnimator.SetFloat("MenuState", -2f);
        buttonAnimator.SetInteger("ButtonState", -1);
    }
    public IEnumerator FadeToAlpha(Image image, float startAlpha, float endAlpha, float duration)
    {
        Color color = image.color;
        color.a = startAlpha;
        image.color = color;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            image.color = color;
            yield return null;
        }

        // Ensure the final alpha is set
        color.a = endAlpha;
        image.color = color;
    }

    public void StartFadeIn()
    {
        StopAllCoroutines(); // Stop any ongoing fade
        StartCoroutine(FadeIn());
    }

    public void StartFadeOut()
    {
        StopAllCoroutines(); // Stop any ongoing fade
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn()
    {
        yield return FadeToAlpha(filterImage, 0f, 120f / 255f, fadeDuration);
    }

    private IEnumerator FadeOut()
    {
        yield return FadeToAlpha(filterImage, 120f / 255f, 0f, fadeDuration);
    }

    public void InvertCamera()
    {
        Matrix4x4 mat = Camera.main.projectionMatrix;
        mat *= Matrix4x4.Scale(new Vector3(-1, 1, 1));
        Camera.main.projectionMatrix = mat;

    }

    public void ResetCam()
    {
        Camera.main.projectionMatrix = originalProjectionMatrix;
    }
}
