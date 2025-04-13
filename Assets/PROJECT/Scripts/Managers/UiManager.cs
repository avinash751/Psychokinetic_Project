using System.Collections;
using TransitionsPlus;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour, IResettable
{
    [Resettable] public GameObject winScreen;
    [Resettable] public GameObject loseScreen;
    [Resettable] public GameObject startScreen;
    [Resettable] public GameObject pauseScreenCanvas;
    [SerializeField] TransitionAnimator animator;
    public GameObject playerUi;
    private float time;
    private bool isAnimating;
    private bool isForward;

    private void Awake()
    {
        startScreen.SetActive(true);
    }

    private void Start()
    {
        time = 0;
        isForward = true;
    }

    private void Update()
    {
        if (!isAnimating) return;

        if (isForward)
        {
            time += Time.unscaledDeltaTime;
            if (time > animator.profile.duration)
            {
                time = animator.profile.duration;
                animator.SetProgress(1);
                Time.timeScale = 0;
                ReferenceManager.Instance.Resetter.ResetAll();
                StartCoroutine(DelayReverse());
                isAnimating = false; // Temporarily stop animating to wait for delay
            }
        }
        else
        {
            time -= Time.unscaledDeltaTime;
            if (time < 0)
            {
                time = 0;
                animator.SetProgress(0);
                isAnimating = false; // Stop animating once completed
            }
        }

        animator.SetProgress(time / animator.profile.duration);
    }

    private IEnumerator DelayReverse()
    {
        yield return new WaitForSecondsRealtime(animator.playDelay);
        isForward = false;
        isAnimating = true;
    }

    public void ShowWinScreen()
    {
        winScreen.SetActive(true);
    }

    public void ShowLoseScreen()
    {
        loseScreen.SetActive(true);
    }

    public void ShowPauseScreen()
    {
        pauseScreenCanvas.SetActive(true);
    }

    public void HidePauseScreen()
    {
        pauseScreenCanvas.SetActive(false);
    }

    public void HideStartScreen()
    {
        startScreen.SetActive(false);
    }

    public void AnimateReset()
    {
        isAnimating = true;
        isForward = true;
    }
}