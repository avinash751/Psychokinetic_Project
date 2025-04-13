using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class SaveFile
{
    public List<Profile> profiles;
}

[Serializable]
public class Profile
{
    public List<LevelData> allLevelData;
}

[Serializable]
public class LevelData
{
    public int levelIndex;
    public string levelName;
    public LevelStatus status;
    public float completionTime;
    public FinalRank rank;
    public GhostData ghostData;

    public LevelData(int levelIndex, LevelStatus status, GhostData ghostData = null, float completionTime = 0, FinalRank rank = default)
    {
        this.levelIndex = levelIndex;
        this.status = status;
        this.ghostData = ghostData;
        this.completionTime = completionTime;
        this.rank = rank;
        string scenePath = SceneUtility.GetScenePathByBuildIndex(levelIndex);
        int lastSlashIndex = scenePath.LastIndexOf('/');
        levelName = (lastSlashIndex >= 0 && lastSlashIndex < scenePath.Length - 1) ? scenePath.Substring(lastSlashIndex + 1) : scenePath;
        int extensionIndex = levelName.LastIndexOf('.');
        levelName = (extensionIndex >= 0) ? levelName.Substring(0, extensionIndex) : levelName;
    }
}

[Serializable]
public class GhostData
{
    public float timestep;
    public List<GhostFrame> ghostFrames;

    public GhostData(float timestep, List<GhostFrame> ghostFrames)
    {
        this.timestep = timestep;
        this.ghostFrames = ghostFrames;
    }
}

[Serializable]
public class GhostFrame
{
    public Vector3 position;
    public Quaternion rotation;
    public List<KeyValuePair> animationValues;

    public GhostFrame(Vector3 position, Quaternion rotation, List<KeyValuePair> animationValues)
    {
        this.position = position;
        this.rotation = rotation;
        this.animationValues = animationValues;
    }
}

public enum LevelStatus
{
    NotStarted,
    InProgress,
    Completed,
}