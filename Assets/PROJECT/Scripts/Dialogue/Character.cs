using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Custom/Character")]
public class Character : ScriptableObject
{
    [SerializeField] string characterName;
    [SerializeField] Sprite avatar;

    public string CharacterName => characterName;
    public Sprite Avatar => avatar;
}