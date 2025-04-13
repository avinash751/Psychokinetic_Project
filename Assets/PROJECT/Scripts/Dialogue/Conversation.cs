using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Conversation", menuName = "Custom/Conversation")]
public class Conversation : ScriptableObject
{
    public Line[] lines;

    public void StartDialogue()
    {
        DialogueManager.Instance.StartDialogue(lines);
    }
}

[Serializable]
public class Line
{
    [SerializeField] Character character;
    [SerializeField][TextArea] string message;

    public Character Character => character;
    public string Message => message;
}
