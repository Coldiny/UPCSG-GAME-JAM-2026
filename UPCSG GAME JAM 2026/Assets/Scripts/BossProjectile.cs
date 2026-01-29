using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public float speed = 5f;
    public int damage = 1;

    void Update()
    {
        // Move straight down
        transform.rotation = Quaternion.Euler(0, 0, 180);
        transform.Translate(Vector2.up * speed * Time.deltaTime);

        // Destroy if it goes too far down (cleanup)
        if (transform.position.y < -10f) Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Assuming your player has a tag "Player" and a script with TakeDamage
        if (other.CompareTag("Player"))
        {
            // other.GetComponent<PlayerController>().TakeDamage(damage); 
            Debug.Log("Player Hit by Projectile!");
            Destroy(gameObject);
        }
    }
}