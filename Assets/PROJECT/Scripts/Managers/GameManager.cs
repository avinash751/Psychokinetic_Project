using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IResettable
{
    public static GameManager Instance { get; private set; }
    [SerializeField] bool TimeScaleZeroOnAwake = true;
    [Header("Events")]
    public UnityEvent OnLevelCompleted;
    public UnityEvent onStart;
    public UnityEvent onLose;
    public UnityEvent onPause;
    public UnityEvent onResume;
    public UnityEvent onReset;
    [Resettable] bool hasGameStarted = false;
    [Resettable] bool isGamePaused = false;
    [Resettable] bool hasGameEnded = false;
    [Resettable] float timeScale;
    CursorSettings cursorSettings;

    string levelCompletedKey = "LevelCompleted";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Time.timeScale = TimeScaleZeroOnAwake ? 0 : 1;
        PlayerHealth.OnPlayerDeath += Lose;
    }

    private void Start()
    {
        cursorSettings = ReferenceManager.Instance?.CursorSettings;
        cursorSettings?.ActivateMouseCursor(true);
    }

    private void OnDisable()=> PlayerHealth.OnPlayerDeath -= Lose;

    private void Update()
    {
        if (InputManager.GetKeyState("Jump") && !hasGameStarted)
        { StartGame(); }

        if (InputManager.GetKeyState("Pause"))
        { TogglePause();}

        if (InputManager.GetKeyState("Reset") && !isGamePaused && hasGameStarted)
        { Restart();}
    }

    public void Win()
    {
        OnLevelCompleted.Invoke();
        AudioManager.Instance.PlayAudio(levelCompletedKey);
        EndGame();
    }

    public void Lose()
    {
        if (!hasGameEnded)
        {
            onLose.Invoke();
            EndGame();
        }
    }

    public void TogglePause()
    {
        if (hasGameEnded || !hasGameStarted) return;

        isGamePaused = !isGamePaused;

        if (isGamePaused)
        { PauseGame(); }
        else
        { ResumeGame(); }
    }

    public void StartGame()
    {
        Time.timeScale = 1;
        hasGameStarted = true;
        onStart.Invoke();
        cursorSettings?.ActivateMouseCursor(false);
    }

    private void PauseGame()
    {
        if (Time.timeScale > 0)
        {
            timeScale = Time.timeScale;
            Time.timeScale = 0;
            cursorSettings.ActivateMouseCursor(true);
        }
        onPause.Invoke();
    }

    private void ResumeGame()
    {
        Time.timeScale = timeScale;
        onResume.Invoke();
        cursorSettings.ActivateMouseCursor(false);
    }

    public void Restart()
    {
        if (ReferenceManager.Instance.Resetter != null)
        {
            cursorSettings.ActivateMouseCursor(true);
            onReset.Invoke();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void EndGame()
    {
        hasGameEnded = true;
        Time.timeScale = 0;
        cursorSettings.ActivateMouseCursor(true);
    }

    public void OpenSurveyLink()
    {
        Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSe2qh3TcpU-mZ61yPm0qhUTq9mMfTfllRIOUPIZNqVFylZQbQ/viewform");
    }
    public void QuitGame() => Application.Quit();
}
