using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;

public class GuideSystem : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject guidePanel;
    public GameObject contButton;
    public TextMeshProUGUI guideText;

    [Header("Settings")]
    [TextArea(3, 10)]
    public string[] guideMessages;
    public float wordSpeed = 0.05f;

    // State Variables
    private int currentMessageIndex = 0;
    public bool playerInZone;
    private bool isTyping = false;

    void Update()
    {
        // Check for 'E' press and if player is nearby
        if (playerInZone && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (!guidePanel.activeInHierarchy)
            {
                guidePanel.SetActive(true);
                // Ensure button is hidden when we first open the panel
                contButton.SetActive(false);
                StartCoroutine(Typing());
            }
            else
            {
                if (!isTyping)
                {
                    NextLine();
                }
            }
        }

        // DELETED: The string comparison check here was inefficient.
        // We now handle the button directly in the Typing() coroutine below.
    }

    public void zeroText()
    {
        guideText.text = "";
        currentMessageIndex = 0;
        guidePanel.SetActive(false);
        contButton.SetActive(false); // Make sure button hides when closing
        isTyping = false;
    }

    IEnumerator Typing()
    {
        isTyping = true;
        contButton.SetActive(false); // Hide button while typing
        guideText.text = "";

        foreach (char letter in guideMessages[currentMessageIndex].ToCharArray())
        {
            guideText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }

        isTyping = false;

        // NEW: Show the arrow button specifically when typing finishes!
        contButton.SetActive(true);
    }

    public void NextLine()
    {
        StopAllCoroutines();
        // No need to hide button here, the Coroutine will do it at the start

        if (currentMessageIndex < guideMessages.Length - 1)
        {
            currentMessageIndex++;
            StartCoroutine(Typing());
        }
        else
        {
            zeroText();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInZone = false;
            StopAllCoroutines();
            zeroText();
        }
    }
}