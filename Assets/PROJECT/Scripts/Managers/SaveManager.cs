using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    string filePath;
    Profile profile;
    public Profile Profile => profile;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        filePath = "SaveData.json"; // Later make it so it's Application.persistentDataPath + "/SaveData.json", this is just handier for easily deleting save files
        LoadData();
    }

    public void LoadData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            profile = JsonUtility.FromJson<Profile>(json);
        }
        else
        {
            profile = new Profile { allLevelData = new List<LevelData>() };
        }
    }

    public void SaveData(List<LevelData> levelData)
    {
        profile.allLevelData = levelData;
        string json = JsonUtility.ToJson(profile);
        File.WriteAllText(filePath, json);
    }

    public static void  ClearCurrentData()
    {
        if (File.Exists("SaveData.json"))
        {
            File.Delete("SaveData.json");
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SaveManager))]
public class SaveManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Clear Save Data"))
        {
            SaveManager.ClearCurrentData();
            Debug.Log("Save Data Cleared");
        }
    }
}
#endif