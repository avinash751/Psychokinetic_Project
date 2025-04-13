using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using CustomInspector;

public class InGameUINavigators : MonoBehaviour
{
    [SerializeField][SelfFill] GameManager gameManager;

    [SerializeField] GameObject StartScreenFirstSelection;
    [SerializeField] GameObject PauseScreenFirstSelection;
    [SerializeField] GameObject WinScreenFirstSelection;
    [SerializeField] GameObject LoseScreenFirstSelection;



    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(StartScreenFirstSelection);
        gameManager.onPause.AddListener(() => EventSystem.current.SetSelectedGameObject(PauseScreenFirstSelection));
        gameManager.OnLevelCompleted.AddListener(() => EventSystem.current.SetSelectedGameObject(WinScreenFirstSelection));
        gameManager.onLose.AddListener(() => EventSystem.current.SetSelectedGameObject(LoseScreenFirstSelection));
        gameManager.onReset.AddListener(() => EventSystem.current.SetSelectedGameObject(StartScreenFirstSelection));
      
    }
}
