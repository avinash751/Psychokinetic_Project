using System;
using CustomInspector;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;
using MoreMountains.Feedbacks;

public class ObjectiveManager : MonoBehaviour, IResettable
{
    public Action ObjectiveComplete;
    [Resettable] bool isObjectiveComplete;
    [field: SerializeField] public ObjectiveType objectiveType { get; private set; }
    [SerializeField, ReadOnly] Objective objective;
    [SerializeField] protected bool customTarget = false;
    [ShowIf(nameof(customTarget))]
    [SerializeField] protected int targetNumber = 1;
    [HorizontalLine("UI", 1, FixedColor.Gray)]
    [SerializeField] TextMeshProUGUI objectiveText;
    [SerializeField] string description;
    [SerializeField] bool progressBar;
    [SerializeField, ShowIf(nameof(progressBar))] Image bar;
    [SerializeField] MMF_Player objectiveFeedback;
    [SerializeField] GameObject objectiveBarrier;

    string soundKey = "ObjectiveComplete";


    private void OnEnable()
    {
        ReferenceManager.Instance.Resetter.OnReset += OnReset;
    }

    private void OnDisable()
    {
        ReferenceManager.Instance.Resetter.OnReset -= OnReset;
    }

    private void Start()
    {
        objectiveFeedback = GetComponent<MMF_Player>();
        if (objectiveType == ObjectiveType.None)
        {
            objectiveText.transform.parent.gameObject.SetActive(false);
        }
        else if (objective == null)
        {
            objective = GetComponent<Objective>();
        }

        if (description == "")
        {
            description = objectiveType.ToString().Replace("Destroy", "").Replace("Collect", "");
        }

        if (objective != null)
        {
            objective.SetTotal(customTarget, targetNumber);
        }
    }

    void Update()
    {
        if (objective == null) { return; }

        if (objective.Current == objective.Total) { return; }
        if (progressBar && objective.Total != 0)
        {
            bar.transform.parent.gameObject.SetActive(objective.Current <= objective.Total);
            objectiveText.text = $"{description} \n \n";
            bar.fillAmount = (float)objective.Current / objective.Total;
        }
        else
        {     
            objectiveText.text = $"{description} \n {objective.Current} / {objective.Total}";
            bar.transform.parent.gameObject.SetActive(false);
        }

        if (!isObjectiveComplete && objective.CheckCompletion())
        {
            AudioManager.Instance.PlayAudio(soundKey);
            if(objectiveBarrier!=null)
            { objectiveBarrier.SetActive(false); }
            if(objectiveFeedback!=null)
            { objectiveFeedback.PlayFeedbacks(); }

            isObjectiveComplete = true;
            ObjectiveComplete?.Invoke();
            Debug.Log($"Objective '{objectiveType}' complete!");
        }

      
    }

    void OnReset()
    {
        isObjectiveComplete = false;
        if (objective != null)
        {
            objectiveText.text = $"{description} \n {0} / {objective.Total}";
            if(objectiveBarrier!=null)
            { objectiveBarrier.SetActive(true); }
        }
        
    }

    public enum ObjectiveType
    {
        None,
        DestroyEnemies,
        DestroyTargets,
        CollectItems
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        EditorApplication.delayCall += () => UpdateObjective();
    }

    private void UpdateObjective()
    {
        if (Application.isPlaying) { return; }
        
        if (objectiveType == ObjectiveType.None)
        {
            if (objective != null)
            {
                DestroyImmediate(objective, true);
                objective = null;
            }
            return;
        }

        Type currentObjective = objective == null ? null : objective.GetType();

        if (objectiveType.ToString() == nameof(currentObjective)) { return; }

        if (currentObjective != null)
        {
            DestroyImmediate(objective, true);
            objective = null;
        }

        Type desiredObjective = System.Type.GetType(objectiveType.ToString());

        if (desiredObjective != null)
        {
            objective = TryGetComponent(desiredObjective, out var newObjective) ? (Objective) newObjective : (Objective) gameObject.AddComponent(desiredObjective);
        }
    }
#endif
}