using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] List<LevelData> allLevelData = new();
    [SerializeField] int indexOffset;
    GhostPlayer player;
    GhostRecorder recorder;
    RankSystem rankSystem;
    public static Action<List<LevelData>> OnLevelStatsInitialized;

    private void Awake()
    {
        player = FindObjectOfType<GhostPlayer>();
        recorder = FindObjectOfType<GhostRecorder>();
        rankSystem = FindObjectOfType<RankSystem>();

        if (SaveManager.Instance != null)
        {
            List<LevelData> savedLevelData = SaveManager.Instance.Profile.allLevelData;
            if (savedLevelData.Count == 0)
            {
                for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
                {
                    int index = i + indexOffset;
                    if (index > SceneManager.sceneCountInBuildSettings - 1) break;
                    savedLevelData.Add(new LevelData(index, LevelStatus.NotStarted));
                }
            }
            else if (SceneManager.GetActiveScene().buildIndex - indexOffset >= 0)
            {
                int sceneIndex = SceneManager.GetActiveScene().buildIndex;
                GhostData ghostData = savedLevelData[sceneIndex - indexOffset].ghostData;
                player.SetGhostData(ghostData);
            }
            allLevelData = savedLevelData;
        }

        ChangeCurrentLevelStatToInProgress();
        OnLevelStatsInitialized?.Invoke(allLevelData);
    }
        
    private void Start()
    {
        if(GameManager.Instance == null) return;
        GameManager.Instance.OnLevelCompleted.AddListener(CompleteLevel);
    }

    private void OnDisable()
    {
        if(GameManager.Instance == null) return;
        GameManager.Instance.OnLevelCompleted.RemoveListener(CompleteLevel);
    }

    void ChangeCurrentLevelStatToInProgress()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        if (index < indexOffset || index > allLevelData.Count - 1) return;
        if (allLevelData[index - indexOffset].status == LevelStatus.Completed) return;
        allLevelData[index - indexOffset].status = LevelStatus.InProgress;
        SaveManager.Instance.SaveData(allLevelData);
    }

    public void CompleteLevel()
    {
        int index = SceneManager.GetActiveScene().buildIndex;

        LevelData levelData = allLevelData[index - indexOffset];
        GhostData ghostData = levelData.ghostData;
        float completionTime = levelData.completionTime;
        FinalRank rank = levelData.rank;
        float newTime = ReferenceManager.Instance.GameTimer.CurrentTime;
        
        if (completionTime == 0 || completionTime > newTime)
        {
            if (recorder != null) { ghostData = recorder.GetRecording(); }
            completionTime = newTime;
            if (rankSystem != null) { rank = rankSystem.GetRank(completionTime); }
        }
        
        LevelData newData = new(index, LevelStatus.Completed, ghostData, completionTime, rank);
        player.SetGhostData(ghostData);
        allLevelData[index - indexOffset] = newData;
        
        SaveManager.Instance.SaveData(allLevelData);
    }

    public LevelStatus GetLevelStatus(int index)
    {
        index -= indexOffset;
        if (index < 0 || index >= allLevelData.Count)
        {
            Debug.LogError("Invalid level index, wtf?");
            return LevelStatus.NotStarted;
        }

        return allLevelData[index].status;
    }
}