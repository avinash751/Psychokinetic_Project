using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class TutorialTask : MonoBehaviour, IResettable
{
    [field: SerializeField][TextArea] string tutorialText;
    [SerializeField] Conversation start;
    [SerializeField] Conversation end;
    bool active = true;
    LevelManager levelManager;
    [Resettable] protected bool complete = false;
    [Resettable] protected bool started = false;
    public string TutorialText => tutorialText;
    public Action TaskComplete;

    protected virtual void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        ReferenceManager.Instance.Resetter.OnReset += ToggleActivation;
        ToggleActivation();
    }

    void ToggleActivation()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (levelManager.GetLevelStatus(sceneIndex) == LevelStatus.Completed)
        {
            active = false;
        }
        else
        {
            active = true;
        }
    }

    protected virtual void CompleteTask()
    {
        complete = true;
        end.StartDialogue();
        TaskComplete?.Invoke();
    }

    // This is OnTriggerStay cuz the reset system doesn't make you set off the trigger if you never left it. Edge case, but it is what it is.
    void OnTriggerStay(Collider other)
    {
        if (!started && other.TryGetComponent(out BippiTutorialNavigator bippi))
        {
            bippi.TriggerBippiToStopMoving();
        }

        if (active && other.gameObject == ReferenceManager.Instance.PlayerMovement.gameObject && !started)
        {
            StartTask();
        }
    }

    protected virtual void StartTask()
    {
        started = true;
        start.StartDialogue();
        TutorialManager.Instance.StartTutorial(this);
    }

    public abstract void CancelTask();
}