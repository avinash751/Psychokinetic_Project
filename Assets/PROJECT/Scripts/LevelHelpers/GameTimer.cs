using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using CustomInspector;


public class GameTimer : MonoBehaviour, IResettable
{
    [SerializeField] float colorChangeIntervel;
    [SerializeField] float colorChangeDuration;
    [SerializeField] Color ColorChangeIntervelColor;
    [Resettable][SerializeField] string addonText;
    [SerializeField] bool debugMode;

    [Resettable][SerializeField] TimerUtilities gameTimer;
    [SerializeField][ReadOnly]bool updateTextAllowed;
    public float CurrentTime;
   

    private void Start()
    {
       
        gameTimer = new TimerUtilities(gameTimer.showDecimal, GetComponent<TextMeshProUGUI>(), addonText);
        gameTimer.InitializeUnlimitedStopWatch();
        CanUpdateText();
        GameManager.Instance?.OnLevelCompleted.AddListener(() => gameTimer.EndStopWatch(false));
        GameManager.Instance?.onLose.AddListener(() => gameTimer.EndStopWatch(false));
        ReferenceManager.Instance.Resetter.OnReset += OnReset;
    }

    private void CanUpdateText()
    {
        int levelIndex = SceneManager.GetActiveScene().buildIndex - 1;
        updateTextAllowed = SaveManager.Instance.Profile.allLevelData[levelIndex].status == LevelStatus.Completed;
        if(!updateTextAllowed) gameTimer.textReference.text = "---";
    }

    private void OnDisable()
    {
        GameManager.Instance?.OnLevelCompleted.RemoveListener(() => gameTimer.EndStopWatch(false));
        GameManager.Instance?.onLose.RemoveListener(() => gameTimer.EndStopWatch(false));
    }

    private void Update()
    {   bool showTimer = debugMode ? true : updateTextAllowed;
        CurrentTime = gameTimer.UpdateUnlimitedStopWatch(showTimer);
        gameTimer.ChangeTextColor(colorChangeIntervel, ColorChangeIntervelColor, colorChangeDuration);
    }
    
    void OnReset()
    {
        CanUpdateText();
    }
}