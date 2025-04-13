using UnityEngine;

// This is meant to be a placeholder, but realistically we'll just add to it later?
public class Item : MonoBehaviour, IResettable
{
    GroundMovement player;
    [Resettable] bool collected = false;
    public bool Collected => collected;
    [Resettable] GameObject go => gameObject;

    string soundKey = "ItemCollect";

    private void Start()
    {
        player = FindObjectOfType<GroundMovement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (player.gameObject == other.gameObject)
        {
            collected = true;
            AudioManager.Instance.PlayAudio(soundKey);
            gameObject.SetActive(false);
        }
    }
}