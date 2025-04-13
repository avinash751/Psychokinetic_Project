using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeveloperDebugManager : MonoBehaviour
{
    public static DeveloperDebugManager Instance { get; private set; }
    public bool DebugMode;

    int totalScenes;
    int currentSceneIndex;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        totalScenes = SceneManager.sceneCountInBuildSettings;

    }

    private void Update()
    {
        if (!DebugMode) return;
        LoadPreviousSceneWithInput();
        LoadNextSceneWithInput();
        TriggerLevelCompletion();
        TogglePlayerUI();
        TogglePlayerInvincibility();
    }


    private void TogglePlayerInvincibility()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            PlayerHealth playerHealth = ReferenceManager.Instance.PlayerHealth;
            playerHealth.IsInvincible = !playerHealth.IsInvincible;
            if(playerHealth.IsInvincible)
            {
                Debug.Log("Player is invincible");
            }
            else
            {
                Debug.Log("Player is not invincible");
            }
        }
    }

    private void TogglePlayerUI()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            UiManager uiManager = ReferenceManager.Instance.UiManager;
            uiManager.playerUi.SetActive(!uiManager.playerUi.activeSelf);
        }
    }

    private void TriggerLevelCompletion()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            GameManager.Instance.Win();
        }
    }

    private void LoadPreviousSceneWithInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int PreviousSceneIndex = currentSceneIndex - 1;
            if (PreviousSceneIndex < 0) return;

            SceneManager.LoadScene(PreviousSceneIndex);
        }
    }

    public void LoadNextSceneWithInput()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneIndex = currentSceneIndex + 1;
            if (nextSceneIndex >= totalScenes) return;

            SceneManager.LoadScene(nextSceneIndex);
        }
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(DeveloperDebugManager))]
public class DeveloperDebugManagerEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DeveloperDebugManager developerDebugManager = (DeveloperDebugManager)target;
        if(GUILayout.Button("Clear Save Profiles Data"))
        {
            SaveManager.ClearCurrentData();
            Debug.Log("Save Data Cleared");
        }
    }
}
#endif
