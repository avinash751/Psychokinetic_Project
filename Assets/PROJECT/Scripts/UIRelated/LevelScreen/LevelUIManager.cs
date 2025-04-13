using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using VInspector;
using CustomInspector;
using TabAttribute = VInspector.TabAttribute;
using System;
using TMPro;
using TransitionsPlus;

public class LevelUIManager : MonoBehaviour
{
    public static Action OnLevelChange;

    [Tab("Level Selection Info")]
    [SerializeField] int levelSelectionSceneIndex;
    [SerializeField] List<LevelData> levelStats;
    [SerializeField]SerializedDictionary<LevelStatus, Sprite> levelStatDictionary = new SerializedDictionary<LevelStatus, Sprite>();
    [SerializeField]SerializedDictionary<Sprite,Color> levelStatusColor = new SerializedDictionary<Sprite, Color>();

    [HorizontalLine("Level Button in level Selection Scene", 5, FixedColor.BabyBlue)]
    [SerializeField] GameObject levelButtonsParent;
    [SerializeField][ReadOnly] List<GameObject> levelButtons;
    [SerializeField] List<TextMeshProUGUI> timingText;

    [Tab("Game Manager Info")]
    [SerializeField] Button nextLevelButtons;
    [SerializeField] List<Button> levelSelectionButtons;
    [SerializeField] TransitionAnimator animator;
 
    private void OnValidate()
    {
        if (levelButtonsParent == null) return;
        levelButtons.Clear();
        levelButtons = levelButtonsParent.GetComponentsInChildren<Button>().Select(button => button.gameObject).ToList();
    }

    private void OnEnable()
    {
        LevelManager.OnLevelStatsInitialized += InitializeLevelStats;
    }

    private void Start()
    {
        animator.gameObject.SetActive(true);
        for (int i = 0; i < levelButtons.Count; i++)
        {
            if (i == levelStats.Count || levelButtons.Count == 0) break;
            int levelIndex = levelStats[i].levelIndex;
            if (ShouldThisLevelBeLocked(i))
            {
                levelButtons[i].SetActive(false);
                continue;
            }
            levelButtons[i].SetActive(true);
            levelButtons[i].TryGetComponent(out LevelButtonUi _levelButton);
            Sprite StatusSprite = levelStatDictionary[levelStats[i].status];
            Color statusColor = levelStatusColor[StatusSprite];
            _levelButton.InitializeButton(levelStats[i], StatusSprite,statusColor, () => TransitionScene(levelIndex));
            SetTimeText(i, levelStats[i].status);
        }

        if (nextLevelButtons != null)
        {
            nextLevelButtons.onClick.AddListener(LoadNextLevelScene);
            nextLevelButtons.onClick.AddListener(() => nextLevelButtons.interactable = false);
        }

        if (levelSelectionButtons.Count == 0) return;
        foreach (var button in levelSelectionButtons)
        {
            button.onClick.AddListener(LoadLevelSelectionScene);
            button.onClick.AddListener(() => button.interactable = false);
        }
    }

    void SetTimeText(int index, LevelStatus status)
    {
        if (status == LevelStatus.Completed)
        {
            string rankText = levelStats[index].rank == FinalRank.Unranked ? levelStats[index].rank.ToString() : levelStats[index].rank.ToString() + " Rank";
            timingText[index].text = rankText + "\nTime " + string.Format("{0:0.00}", levelStats[index].completionTime);
        }
        else
        {
            timingText[index].gameObject.SetActive(false);
        }
    }

    bool ShouldThisLevelBeLocked(int iterationIndex)
    {
        int levelIndex = levelStats[iterationIndex].levelIndex;
        if (levelIndex == levelStats[0].levelIndex)
        { return false; }

        bool isPreviousLevelCompleted = levelStats[iterationIndex - 1].status == LevelStatus.Completed;
        if (isPreviousLevelCompleted)
        { return false; }

        return true;
    }

    void InitializeLevelStats(List<LevelData> stats) => levelStats = stats;

    public void LoadLevelSelectionScene() => TransitionScene(levelSelectionSceneIndex);

    public void LoadNextLevelScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            TransitionScene(nextSceneIndex);
        }
        else
        {
            LoadLevelSelectionScene();
        }
    }

    private void TransitionScene(int sceneIndex)
    {
        animator.profile.invert = !animator.profile.invert;
        animator.SetProgress(0);
        animator.enabled = true;
        AudioManager.Instance?.PlayAudio("LevelExit");
        OnLevelChange.Invoke();
        animator.onTransitionEnd.AddListener(() => 
            {
                animator.profile.invert = !animator.profile.invert;
                SceneManager.LoadScene(sceneIndex);
            });
    }

    private void OnDisable()
    {
        if (nextLevelButtons != null)
        {
            nextLevelButtons.onClick.RemoveListener(LoadNextLevelScene);
        }

        if (levelSelectionButtons.Count == 0) return;
        levelSelectionButtons.ForEach(button => button.onClick.RemoveListener(LoadLevelSelectionScene));
    }
}