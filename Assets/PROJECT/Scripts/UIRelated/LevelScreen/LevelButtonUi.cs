using CustomInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButtonUi : MonoBehaviour
{
    [SerializeField] Button levelButton;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Image levelStatusImage;
    [SerializeField][ReadOnly] LevelData levelData;
    [SerializeField] string levelTextAddon = "Level ";


    private void Awake()
    {
        if (levelButton == null)
        { TryGetComponent(out levelButton); }

        if (levelText == null)
        { transform.GetChild(0).TryGetComponent(out levelText); }

        if(levelStatusImage == null)
        { transform.GetChild(1).TryGetComponent(out levelStatusImage); }

        gameObject.SetActive(false);
    }

    public void InitializeButton(LevelData stat,Sprite statusSprite,Color statusColor, Action onClick)
    {
        levelData = stat;
        levelButton.onClick.AddListener(() =>
        {
            onClick();
            levelButton.interactable = false;
        });
        levelText.text = levelTextAddon + stat.levelIndex.ToString();
        levelStatusImage.sprite = statusSprite;
        levelStatusImage.color = statusColor;
    }
}
