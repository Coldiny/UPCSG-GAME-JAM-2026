using UnityEngine;

public class Lever : MonoBehaviour
{
    [Header("Settings")]
    public bool isActivated = false;

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    public Sprite leverOnSprite;  // Drag green lever image here
    public Sprite leverOffSprite; // Drag red lever image here

    [Header("Target")]
    // 1. Reference the SPECIFIC script you want to change
    public GateController targetGate;

    private void Start()
    {
        UpdateVisuals();
    }

    // This built-in Unity function runs when you click the Collider
    private void OnMouseDown()
    {
        // Toggle the state (if true becomes false, if false becomes true)
        // Or just set to true: isActivated = true;
        isActivated = !isActivated;

        // Update the visuals
        UpdateVisuals();

        // CHANGE THE OTHER OBJECT'S BOOL
        if (targetGate != null)
        {
            targetGate.moveGate = isActivated;
            Debug.Log("Door is now: " + isActivated);
        }
    }

    void UpdateVisuals()
    {
        if (isActivated && leverOnSprite != null)
        {
            spriteRenderer.sprite = leverOnSprite;
        }
        else if (!isActivated && leverOffSprite != null)
        {
            spriteRenderer.sprite = leverOffSprite;
        }
    }
}