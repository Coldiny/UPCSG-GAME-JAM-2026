using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue System/NPC Dialogue")]
public class NPCDialogue : ScriptableObject
{
    public string npcName;
    public Sprite npcPortrait;

    // Typing settings
    public float typingSpeed = 0.05f;

    // The list of all possible "states" in this conversation
    public List<DialogueNode> nodes;
}

[System.Serializable]
public class DialogueNode
{
    [TextArea(3, 10)]
    public string npcText; // What the NPC says

    // If this is -1, the conversation ends.
    // If "choices" is empty, pressing Interact jumps to this ID.
    public int defaultNextNode = -1;

    public List<DialogueOption> choices; // List of buttons to show
}

[System.Serializable]
public struct DialogueOption
{
    public string buttonText; // Text on the player button (e.g., "Who are you?")
    public int nextNodeID;    // The ID of the node to jump to
}