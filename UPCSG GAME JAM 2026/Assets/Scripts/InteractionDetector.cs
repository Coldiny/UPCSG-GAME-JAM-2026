using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null;
    public GameObject interactionIcon;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        if (interactableInRange != null)
        {
            // FIX 1: Use 'GetComponentInParent'. 
            // This searches the current object AND any parent objects for the script.
            PlayerMovement movementScript = GetComponentInParent<PlayerMovement>();

            if (movementScript != null)
            {
                interactableInRange.Interact(movementScript);
            }
            else
            {
                // FIX 2: Added a clear error message.
                // If we can't find the player script, there is no point passing 'this'
                // because the Unlockable script needs PlayerMovement to work.
                Debug.LogError("Found an item, but cannot find 'PlayerMovement' script on parent object!");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            interactableInRange = interactable;
            if (interactionIcon != null) interactionIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange)
        {
            interactableInRange = null;
            if (interactionIcon != null) interactionIcon.SetActive(false);
        }
    }
}