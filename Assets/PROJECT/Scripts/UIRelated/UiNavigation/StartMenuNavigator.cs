using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StartMenuNavigator : MonoBehaviour
{
    [SerializeField] GameObject StartMenuFirstSelected;
    [SerializeField] GameObject SettingsMenuFirstSelected;
    [SerializeField] GameObject LevelSelectionFirstSelected;

    [SerializeField] Button[] ToTriggerStartMenuFirstSelection;
    [SerializeField] Button ToTriggerSettingsMenuFirstSelection;
    [SerializeField] Button ToTriggerLevelSelectionFirstSelection;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(StartMenuFirstSelected);

        foreach (Button button in ToTriggerStartMenuFirstSelection)
        {
            button.onClick.AddListener(() => EventSystem.current.SetSelectedGameObject(StartMenuFirstSelected));
        }
        
        ToTriggerSettingsMenuFirstSelection.onClick.AddListener(() => EventSystem.current.SetSelectedGameObject(SettingsMenuFirstSelected));
        ToTriggerLevelSelectionFirstSelection.onClick.AddListener(() => EventSystem.current.SetSelectedGameObject(LevelSelectionFirstSelected));
    }

}
