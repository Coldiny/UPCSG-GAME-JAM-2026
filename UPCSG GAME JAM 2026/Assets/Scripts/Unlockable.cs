using UnityEngine;

public class Unlockable : MonoBehaviour, IInteractable
{
    public enum abilityType { DoubleJump, Dash, MeleeAttack, RangedAttack};
    public abilityType abilityName;
    public bool isUnlocked;
    public string unlockableID { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        unlockableID ??= GlobalHelper.GenerateUniqueID(gameObject);
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Check if the object colliding is the Player
        if (collision.CompareTag("Player"))
        {
            // 2. Grab the PlayerMovement script from the player object
            PlayerMovement player = collision.GetComponent<PlayerMovement>();
            

            if (player != null)
            {
                Interact(player);

                // 3. Destroy this item so it can't be picked up twice
                Destroy(gameObject);
            }
        }
    }
    public bool CanInteract()
    {
        return !isUnlocked;
    }

    public void Interact(MonoBehaviour controller)
    {
        if (!CanInteract()) return;

        // 1. FIX: You must FIND the PlayerMovement script first!
        // We look for it on the same object that triggered the interaction.
        PlayerMovement player = controller.GetComponent<PlayerMovement>();

        // 2. Safety Check: Now 'player' actually exists to be checked.
        if (player == null)
        {
            Debug.LogWarning("Interaction failed: No PlayerMovement script found on " + controller.name);
            return;
        }

        // 3. Find the attack script (neighbor to the movement script)
        PlayerAttack playerA = player.GetComponent<PlayerAttack>();

        // Unlock the ability
        switch (abilityName)
        {
            case abilityType.DoubleJump:
                // FIX: Use 'player', not 'controller'
                player.canDoubleJump = true;
                break;

            case abilityType.Dash:
                // FIX: Use 'player', not 'controller'
                player.canDash = true;
                break;

            case abilityType.MeleeAttack:
                if (playerA != null) playerA.canMAttack = true;
                break;
        }
        
        isUnlocked = true;
    }
}
