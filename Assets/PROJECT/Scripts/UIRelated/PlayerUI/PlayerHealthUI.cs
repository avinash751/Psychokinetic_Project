using CustomInspector;
using MoreMountains.Feedbacks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityRandom = UnityEngine.Random;

public class PlayerHealthUI : UiGridSpawner, INeedPlayerRefs
{
    GameObject player;

    [HorizontalLine("Player Health Ui Settings", 2, FixedColor.CherryRed)]
    [Tooltip("Represents the health amount  of one heart\r\n")]
    [Resettable][SerializeField] int heartUIHealthValue;

    [Resettable][SerializeField][ReadOnly] int healthLeftInCurrentHeart;
    [SerializeField][ReadOnly] MMF_Player healthSpriteDamageFeedback;
    [SerializeField][ReadOnly] List<GameObject> deactivatedHealthSprites;


    private void OnEnable()
    {
        IDamageable.OnDamageTaken += UpdateHealthUI;
        PlayerHealth.OnPlayerHealthInitialized += InitializePlayerHealthUI;
        healthLeftInCurrentHeart = heartUIHealthValue;
    }
    private void OnDisable()
    {
        IDamageable.OnDamageTaken -= UpdateHealthUI;
        PlayerHealth.OnPlayerHealthInitialized -= InitializePlayerHealthUI;
        ReferenceManager.Instance.Resetter.OnReset -= ResetHealthSprites;
    }

    private void Start()
    {
        ReferenceManager.Instance.Resetter.OnReset += ResetHealthSprites;
        player = ReferenceManager.Instance.PlayerTransform.gameObject;
    }

    void InitializePlayerHealthUI(int _maxHealth, GameObject _player)
    {
        if (_player != player.gameObject) return;
        if (heartUIHealthValue == 0) return;

        int amountOfHeartsToSpawn = _maxHealth / heartUIHealthValue;
        SpawnUiInBulk(uiPrefabToSpawn, amountOfHeartsToSpawn);
    }

    void UpdateHealthUI(int damageTaken, GameObject _player)
    {
        if (_player != player.gameObject) return;
        if (spawnedUI.Count == 0) { return; }

        healthLeftInCurrentHeart -= damageTaken;
        InitializingHealthVisualFeedback();
        RemoveSpriteAndUpdateFeedback();
    }

    private void RemoveSpriteAndUpdateFeedback()
    {
        if (healthLeftInCurrentHeart <= 0)
        {
            deactivatedHealthSprites.Add(spawnedUI.Last());
            spawnedUI.RemoveAt(spawnedUI.Count - 1);

            if (healthLeftInCurrentHeart < 0)
            { healthLeftInCurrentHeart = heartUIHealthValue - Mathf.Abs(healthLeftInCurrentHeart); }

            else if (healthLeftInCurrentHeart == 0)
            { healthLeftInCurrentHeart = heartUIHealthValue; }

            InitializingHealthVisualFeedback();
            if (healthLeftInCurrentHeart <= 0)
            { RemoveSpriteAndUpdateFeedback(); }
        }
    }

    private void InitializingHealthVisualFeedback()
    {
        if (spawnedUI.Count == 0) return;
        healthSpriteDamageFeedback = spawnedUI.Last().transform.GetChild(1).GetChild(1).GetComponent<MMF_Player>();
        List<MMF_ImageFill> imageFillFeedbacks = healthSpriteDamageFeedback.FeedbacksList.OfType<MMF_ImageFill>().ToList();
        float recorrectedHealthRemainingInCurrentHeart = healthLeftInCurrentHeart > 0 ? healthLeftInCurrentHeart : 0;
        float destinationFillValue = recorrectedHealthRemainingInCurrentHeart / (float)heartUIHealthValue;
        imageFillFeedbacks.ForEach(x => x.DestinationFill = destinationFillValue);
        healthSpriteDamageFeedback.PlayFeedbacks();
    }

    void ResetHealthSprites()
    {
        int maxHealth = (deactivatedHealthSprites.Count + spawnedUI.Count) * heartUIHealthValue;
        deactivatedHealthSprites.ForEach(x => Destroy(x));
        deactivatedHealthSprites.Clear();
        spawnedUI.ForEach(x => Destroy(x));
        spawnedUI.Clear();
        InitializePlayerHealthUI(maxHealth, player);
    }
}