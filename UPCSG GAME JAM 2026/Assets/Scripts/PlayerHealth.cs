using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerHealth : MonoBehaviour

{
    [Header("References")]
    public Rigidbody2D rb;
    GameManager gameManager;
    public GameObject[] hearts;

    [Header("i-Frame Settings")]
    public float iFrameDuration = 1.0f;     // How long to be invincible
    public int numberOfFlashes = 5;         // How many times to blink
    private bool isInvincible = false;      // Internal check

    private SpriteRenderer spriteRend;      // To control the flashing

    [Header("Settings")]
    public int health;


    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }
    private void Start()
    {
        // Get the SpriteRenderer component
        spriteRend = GetComponent<SpriteRenderer>();
        // Ensure UI is correct at start
        UpdateHeartUI();
    }

    void UpdateHeartUI()
    {
        // Loop through all heart objects
        for (int i = 0; i < hearts.Length; i++)
        {
            // If the heart index is less than our current health, show it.
            // Example: Health is 2. 
            // i=0 (Heart 1) < 2 -> TRUE (Show)
            // i=1 (Heart 2) < 2 -> TRUE (Show)
            // i=2 (Heart 3) < 2 -> FALSE (Hide)
            if (i < health)
            {
                hearts[i].SetActive(true);
            }
            else
            {
                hearts[i].SetActive(false);
            }
        }
    }


    public void PlayerTakeDamage(int damage)

    {
        if (isInvincible) return;
        health -= damage;
        rb.linearVelocity = Vector2.up * 5f; // Knockback effect
        UpdateHeartUI();

        Debug.Log("Player took " + damage + " damage. Remaining health: " + health);
        if (health <= 0)

        {

            Debug.Log("Player is dead.");
            gameObject.SetActive(false);
            gameManager.PlayerHasDied();
        }

        else
        {
            // 3. Trigger Invincibility
            StartCoroutine(InvincibilityRoutine());
        }

    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;

        //Optional: Ignore collisions with enemies during this time
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);

        for (int i = 0; i < numberOfFlashes; i++)
        {
            // Turn partially transparent (or Red)
            spriteRend.color = new Color(1, 1, 1, 0.5f);
            yield return new WaitForSeconds(iFrameDuration / (numberOfFlashes * 2));

            // Turn back to normal
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(iFrameDuration / (numberOfFlashes * 2));
        }

        isInvincible = false;

        //Re-enable collisions
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
    }
}