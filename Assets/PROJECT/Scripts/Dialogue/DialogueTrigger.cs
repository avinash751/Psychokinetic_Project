using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueTrigger : MonoBehaviour, IResettable
{
    [SerializeField] Conversation conversation;
    [Resettable] bool triggered = false;
    bool bippiIn;

    private void Start()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (FindObjectOfType<LevelManager>().GetLevelStatus(sceneIndex) == LevelStatus.Completed)
        {
            enabled = false;
        }
        #if UNITY_EDITOR
        enabled = true;
        #endif
    }

    private void OnTriggerStay(Collider other)
    {
        if (!bippiIn && other.TryGetComponent(out BippiTutorialNavigator bippi))
        {
            bippi.TriggerBippi();
            bippiIn = true;
        }


        if (enabled && !triggered && other.gameObject == ReferenceManager.Instance.PlayerMovement.gameObject)
        {
            conversation.StartDialogue();
            triggered = true;
        }

       

    }
}