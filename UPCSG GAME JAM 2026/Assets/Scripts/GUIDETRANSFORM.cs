using UnityEngine;
using UnityEngine.SceneManagement; // Needed to load scenes
using TMPro; // Use this if you are using TextMeshPro, otherwise use UnityEngine.UI

public class GUIDETRANSFORM : MonoBehaviour
{
    [Header("Settings")]
    public int maxHits = 5;
    private int currentHits = 0;
    public string sceneToLoad = "SecretEnding"; // EXACT name of the scene to load

    [Header("Dialogue")]
    public TextMeshProUGUI dialogueText; // Drag your UI Text or Speech Bubble here
    public GameObject dialogueBox;       // Drag the background box (if you have one) to hide/show it
    public float dialogueDuration = 2f;  // How long the text stays on screen

    // You can type the sentences in the Inspector
    [TextArea]
    public string[] hitSentences;

    private void Start()
    {
        // Hide dialogue at the start
        if (dialogueBox != null) dialogueBox.SetActive(false);
        if (dialogueText != null) dialogueText.text = "";
    }

    // This function is called by your PlayerAttack script
    public void TakeHit()
    {
        currentHits++;

        // 1. Check if we reached the limit
        if (currentHits >= maxHits)
        {
            LoadSecretScene();
            return;
        }

        // 2. Show Dialogue for the current hit
        // We use (currentHits - 1) to get the correct index (0, 1, 2, 3...)
        if (hitSentences.Length > 0)
        {
            int index = Mathf.Clamp(currentHits - 1, 0, hitSentences.Length - 1);
            string sentence = hitSentences[index];

            StopAllCoroutines(); // Stop old text if user hits fast
            StartCoroutine(ShowDialogueRoutine(sentence));
        }
    }

    System.Collections.IEnumerator ShowDialogueRoutine(string text)
    {
        if (dialogueBox != null) dialogueBox.SetActive(true);
        if (dialogueText != null) dialogueText.text = text;

        Debug.Log("Guide says: " + text);

        yield return new WaitForSeconds(dialogueDuration);

        // Hide after waiting
        if (dialogueBox != null) dialogueBox.SetActive(false);
        if (dialogueText != null) dialogueText.text = "";
    }

    void LoadSecretScene()
    {
        Debug.Log("Loading Scene: " + sceneToLoad);
        SceneManager.LoadScene(sceneToLoad);
    }
}