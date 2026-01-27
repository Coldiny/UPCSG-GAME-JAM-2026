using UnityEngine;

using UnityEngine.InputSystem;



public class PlayerHealth : MonoBehaviour

{
    [Header("References")]
    public Rigidbody2D rb;

    public GameObject[] hearts;

    [Header("Settings")]
    public int health;
    private void Start()
    {
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

        health -= damage;
        UpdateHeartUI();
        rb.AddForce(new Vector2(-10, 0), ForceMode2D.Impulse);

        Debug.Log("Player took " + damage + " damage. Remaining health: " + health);
        if (health <= 0)

        {

            Debug.Log("Player is dead.");
            gameObject.SetActive(false);

        }

    }
}