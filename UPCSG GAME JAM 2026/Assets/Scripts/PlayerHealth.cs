using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;



public class PlayerHealth : MonoBehaviour

{
    [Header("References")]
    public Rigidbody2D rb;
    GameManager gameManager;
    PlayerMovement playerMovement;
    public GameObject[] hearts;
    Animator anim;

    [Header("i-Frame Settings")]
    public float iFrameDuration = 1.0f;     // How long to be invincible
    public int numberOfFlashes = 5;         // How many times to blink
    private bool isInvincible = false;      // Internal check

    private SpriteRenderer spriteRend;      // To control the flashing

    [Header("Settings")]
    public int health;

    public bool IsDead { get; private set; }

    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }
    private void Start()
    {
        // Get the SpriteRenderer component
        spriteRend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        IsDead = false;
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
            if(IsDead)
            {
                return;
            }

            Debug.Log("Player is dead.");
            IsDead = true;
            playerMovement.canMove = false;
            anim.SetTrigger("Die");

            rb.linearVelocity = Vector2.zero;
            // gameObject.SetActive(false);
            // gameManager.PlayerHasDied(); moved to a function to be called by animation event
        }

        else
        {
            // 3. Trigger Invincibility
            StartCoroutine(InvincibilityRoutine());
        }

    }

    public void OnDeathAnimationFinished()
    {
        Debug.Log("GameManager got called");
        gameManager.PlayerHasDied(); // this is where it got moved
    }

    public void TestEvent()
    {
        Debug.Log("Test called");
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