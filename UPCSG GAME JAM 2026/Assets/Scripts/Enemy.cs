using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Settings")]
    public int health;
    public float speed;
    public int Edamage;

    void Update()
    {
        // FIX 1: Multiply by 'speed' so the enemy actually moves at the right speed
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Enemy took " + damage + " damage. Remaining health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Enemy died!");
        // Optional: Play death animation or sound here
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. Check if we hit the Player
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Enemy collided with Player!");

            // FIX 2: Get the Player's Health script, NOT the Enemy script
            // (Make sure your Player object has a script named 'PlayerHealth')
            PlayerHealth playerScript = collision.gameObject.GetComponent<PlayerHealth>();

            // 3. Apply damage if the script exists
            if (playerScript != null)
            {
                // FIX 3: Use 'Edamage' (your variable), not 'damage'
                playerScript.PlayerTakeDamage(Edamage);
            }
        }
    }
}