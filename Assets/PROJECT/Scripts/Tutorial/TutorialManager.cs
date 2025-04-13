using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour, IResettable
{
    [SerializeField] TextMeshProUGUI tutorialText;
    [SerializeField] CanvasGroup tutorialWindow;
    [SerializeField] float fadeTime;
    [SerializeField] RectTransform origin;
    RectTransform windowRect;
    TutorialTask currentTask;
    Coroutine fadeCoroutine;
    Vector2 startPosition;
    Vector2 endPosition;
    [Resettable] GameObject _ => tutorialWindow.gameObject;

    public static TutorialManager Instance { get; private set; }

    private void Start()
    {
        if (Instance == null) { Instance = this; } else { Destroy(this); }
        windowRect = tutorialWindow.GetComponent<RectTransform>();
        endPosition = windowRect.anchoredPosition;
        startPosition = origin.anchoredPosition + endPosition;
        tutorialWindow.gameObject.SetActive(false);
        tutorialWindow.alpha = 0;
    }

    public void StartTutorial(TutorialTask task)
    {
        if (currentTask != null)
        {
            currentTask.TaskComplete -= TaskCompleted;
            if (task != currentTask)
            {
                currentTask.CancelTask();
            }
        }
        currentTask = task;
        currentTask.TaskComplete += TaskCompleted;
        ToggleUI(true, task.TutorialText);
    }

    void TaskCompleted()
    {
        ToggleUI(false);
    }

    private void OnDisable()
    {
        if (currentTask != null)
        {
            currentTask.TaskComplete -= TaskCompleted;
        }
    }

    void ToggleUI(bool enable, string text = "")
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
        fadeCoroutine = StartCoroutine(DialogueManager.ToggleUI(enable, tutorialWindow, windowRect, startPosition, endPosition, fadeTime));
        if (text != "") tutorialText.text = text;
    }
}