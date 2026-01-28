using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour, IInteractable
{
    [Header("Data")]
    public NPCDialogue dialogueData;

    [Header("UI Elements")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;

    [Header("Buttons")]
    // Drag your 4 Button GameObjects here
    public Button[] choiceButtons;
    // Drag the TMP_Text objects INSIDE those buttons here
    public TMP_Text[] choiceButtonTexts;

    private int currentNodeID = 0;
    private bool isTyping, isDialogueActive;

    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    public void Interact(MonoBehaviour player)
    {
        if (dialogueData == null || (PauseController.IsGamePaused && !isDialogueActive))
            return;

        if (!isDialogueActive)
        {
            StartDialogue();
        }
        else
        {
            // If typing, finish immediately
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.SetText(dialogueData.nodes[currentNodeID].npcText);
                isTyping = false;
            }
            // If NOT typing and NO choices (linear), go to next
            else if (dialogueData.nodes[currentNodeID].choices.Count == 0)
            {
                AdvanceNode(dialogueData.nodes[currentNodeID].defaultNextNode);
            }
        }
    }

    void StartDialogue()
    {
        isDialogueActive = true;
        currentNodeID = 0; // Always start at the first node

        nameText.SetText(dialogueData.npcName);
        portraitImage.sprite = dialogueData.npcPortrait;

        dialoguePanel.SetActive(true);
        PauseController.SetPause(true);

        DisplayNode(currentNodeID);
    }

    void DisplayNode(int id)
    {
        currentNodeID = id;

        // Hide buttons while typing
        SetButtonsActive(false);

        // Start typing the text for this node
        StartCoroutine(TypeLine(dialogueData.nodes[id]));
    }

    // --- KEY FEATURE: TURNING BUTTONS ON/OFF ---
    void ShowChoices()
    {
        DialogueNode node = dialogueData.nodes[currentNodeID];

        // If we have choices, loop through our UI buttons
        if (node.choices.Count > 0)
        {
            for (int i = 0; i < choiceButtons.Length; i++)
            {
                if (i < node.choices.Count)
                {
                    // 1. Activate Button
                    choiceButtons[i].gameObject.SetActive(true);

                    // 2. Set Text (e.g., "Who are you?")
                    choiceButtonTexts[i].text = node.choices[i].buttonText;

                    // 3. Link Button to the specific Next Node
                    // We need to copy 'i' to a local variable for the click listener
                    int choiceIndex = i;
                    int nextID = node.choices[choiceIndex].nextNodeID;

                    choiceButtons[i].onClick.RemoveAllListeners();
                    choiceButtons[i].onClick.AddListener(() => OnOptionSelected(nextID));
                }
                else
                {
                    // 4. Deactivate unused buttons
                    choiceButtons[i].gameObject.SetActive(false);
                }
            }
        }
    }

    // Use this helper to hide everything when moving or closing
    void SetButtonsActive(bool active)
    {
        foreach (Button btn in choiceButtons)
            btn.gameObject.SetActive(active);
    }

    // Called when a button is clicked
    void OnOptionSelected(int nextID)
    {
        AdvanceNode(nextID);
    }

    void AdvanceNode(int nextID)
    {
        if (nextID == -1)
        {
            EndDialogue();
        }
        else
        {
            DisplayNode(nextID);
        }
    }

    IEnumerator TypeLine(DialogueNode node)
    {
        isTyping = true;
        dialogueText.SetText("");

        foreach (char letter in node.npcText)
        {
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(dialogueData.typingSpeed);
        }

        isTyping = false;

        // Only show buttons AFTER typing finishes
        ShowChoices();
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueText.SetText("");
        dialoguePanel.SetActive(false);
        PauseController.SetPause(false);
    }
}