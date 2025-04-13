using CustomInspector;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreenSetup : MonoBehaviour, IResettable
{
    [Resettable][SerializeField] Transform winScreen;
    [Resettable][SerializeField] GameObject gameplayUI;
    [SerializeField] AnimationCurve curve;
    [SerializeField] Transform rankUIParent;
    [SerializeField] Transform[] personalBestParents;
    [SerializeField] TextMeshProUGUI completionRank;
    [SerializeField] TextMeshProUGUI completionTime;
    [SerializeField] GameObject rankUIPrefab;
    [SerializeField] float animationTime = 0.2f;
    [HorizontalLine("Start Screen", 1, FixedColor.Gray)]
    [SerializeField] TextMeshProUGUI levelName;
    float personalBest;
    RankSystem rankSystem;

    void Start()
    {
        rankSystem = FindObjectOfType<RankSystem>();
        levelName.text = SaveManager.Instance.Profile.allLevelData[SceneManager.GetActiveScene().buildIndex - 1].levelName;
        SetUpPersonalBest();
        SetUpRankUI();
        GameManager.Instance.OnLevelCompleted.AddListener(() => StartCoroutine(AnimateWinScreen()));
        GameManager.Instance.OnLevelCompleted.AddListener(UpdateCompletionUI);
    }

    IEnumerator AnimateWinScreen()
    {
        float time = 0;
        gameplayUI.SetActive(false);
        while (time < animationTime)
        {
            if (time > 0.01f)
            {
                SetUpPersonalBest();
            }
            float position = Extrapolate(-1000, 0, curve.Evaluate(time / animationTime));
            winScreen.transform.localPosition = new Vector3(0, position, 0);
            time += Time.unscaledDeltaTime;
            yield return null;
        }
        winScreen.transform.localPosition = Vector3.zero;
    }

    private void SetUpRankUI()
    {
        foreach (var rank in rankSystem.LevelRanks)
        {
            GameObject rankUI = Instantiate(rankUIPrefab, rankUIParent);
            rankUI.SetActive(true);

            TextMeshProUGUI rankText = rankUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI timeText = rankUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

            rankText.text = $"{rank.Key} Rank";
            timeText.text = $"{rank.Value} s";
        }
    }

    private void SetUpPersonalBest()
    {
        foreach (var personalBestParent in personalBestParents)
        {
            TextMeshProUGUI rankPBText = personalBestParent.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI timePBText = personalBestParent.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

            FinalRank pbRank = SaveManager.Instance.Profile.allLevelData[SceneManager.GetActiveScene().buildIndex - 1].rank;
            personalBest = SaveManager.Instance.Profile.allLevelData[SceneManager.GetActiveScene().buildIndex - 1].completionTime;

            rankPBText.text = pbRank == FinalRank.Unranked ? pbRank.ToString() : $"{pbRank} Rank";
            timePBText.text = personalBest == 0 ? "---" : $"{personalBest} s";
        }
    }

    void UpdateCompletionUI()
    {
        float time = ReferenceManager.Instance.GameTimer.CurrentTime;
        string completionText = string.Format("{0:0.00}", time) +"s";

        if (personalBest > time || personalBest == 0)
        {
            completionText += "\n<i><size=50><color=#FFFF00>NEW RECORD!";
        }

        FinalRank rank = rankSystem.GetRank(time);
        completionRank.text = rank.ToString();
        if (rankSystem.RankColors.TryGetValue(rank, out Color color))
        {
            completionRank.fontMaterial.SetColor("_GlowColor", color);
        }
        completionTime.text = completionText;
    }

    float Extrapolate(float startValue,  float endValue, float t)
    {
        return startValue + t * (endValue - startValue);
    }
}