using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static System.Net.WebRequestMethods;
public class Enemy : MonoBehaviour
{
    [Header("References")] 
    private Rigidbody2D rb;
    private Collider2D col;
    public Transform player; 
    public LayerMask groundLayer; 
    
    [Header("Settings")] 
    public int health; 
    public float speed; 
    public int Edamage; 
    public float jumpForce;
    public float detectionRange = 8f;
    public float attackRange = 1.6f;


    private bool isGrounded; 
    private bool shouldJump;
    private bool wallAhead;
    private float jumpCooldown = 1f; // seconds between jumps
    private float nextJumpTime = 0f;

    private bool isChasing = false;

    [Header("Visual Feedback")]
    private SpriteRenderer spriteRenderer;
    public Color normalColor = Color.red;
    public Color attackColor = Color.white;
    public float normalAlpha = 1f;
    public float attackAlpha = 0.5f;
    public float colorLerpSpeed = 6f;

    [Header("Patrol Settings")]
    public Transform patrolPointA; // left / right patrol points
    public Transform patrolPointB;
    private Transform currentTarget; // To switch between patrol and player

    [Header("Enemy AI")]
    public EnemyState currentState;

    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack
    }

    private void Start() 
    { 
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = normalColor;
        currentTarget = patrolPointB; // 1st patrol target
    }
    void Update()
    {
        // Determine the distance between enemy and player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        isChasing = distanceToPlayer <= detectionRange;

        // Decide state
        if(distanceToPlayer <= attackRange)
        {
            currentState = EnemyState.Attack;
        } else if(distanceToPlayer <= detectionRange)
        {
            currentState = EnemyState.Chase;
        } else
        {
            currentState = EnemyState.Patrol;
        }

        // Change color and alpha depending on state
        Color targetColor;
        if (currentState == EnemyState.Attack)
        {
            targetColor = attackColor;
            targetColor.a = attackAlpha;
        } else
        {
            targetColor = normalColor;
            targetColor.a = normalAlpha;
        }
        spriteRenderer.color = Color.Lerp(spriteRenderer.color, targetColor, Time.deltaTime * colorLerpSpeed);

        // Decide target
        Transform target = isChasing ? player : currentTarget;

        // Target Direction
        float direction = Mathf.Sign(target.position.x - transform.position.x);

        // Flippity the spritey and face the targettyyy YEAAAAH
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction; // if negative direction becomes positive and vice versa, thus switching
        transform.localScale = scale;

        // isGrounded?
        Vector2 feetPos = new Vector2(col.bounds.center.x, col.bounds.min.y); // so we can locate feet
        isGrounded = Physics2D.Raycast(feetPos, Vector2.down, 0.05f, groundLayer);

        // Front of feet
        float frontOffset = col.bounds.extents.x + 0.05f;
        Vector2 frontFeetPos = feetPos + new Vector2(direction * frontOffset, 0);

        // Player above detection
        bool isPlayerAbove = isChasing && player.position.y > transform.position.y + 0.7f;

        // Jump if there's gap ahead && no ground infront 
        // else if Jump if wall infront 
        // else if there's player above and platform above 

        // Check ground infront (aka checking for gaps)
        bool groundAhead = Physics2D.Raycast(frontFeetPos, Vector2.down, 0.2f, groundLayer);

        // Check wall infront of enemy
        Vector2 wallCheckPos = new Vector2(col.bounds.center.x + direction * col.bounds.extents.x, col.bounds.center.y);
        wallAhead = Physics2D.Raycast(wallCheckPos, new Vector2(direction, 0), 0.1f, groundLayer);

        // Check if platform above enemy
        bool platformAbove = Physics2D.Raycast(transform.position, Vector2.up, 3f, groundLayer);

        shouldJump = false;

        if (isGrounded)
        { 
            if(!groundAhead || wallAhead || (isPlayerAbove && platformAbove))
            {
                shouldJump = true;
            }
        }

        // DEBUG
        Debug.DrawRay(feetPos, Vector2.down * 0.05f, Color.yellow); // isGrounded
        Debug.DrawRay(frontFeetPos, Vector2.down * 0.2f, Color.blue); // groundAhead
        Debug.DrawRay(wallCheckPos, new Vector2(direction, 0) * 0.1f, Color.red); // wallAhead

        // Patrol target switching
        if (!isChasing)
        {
            if(Vector2.Distance(transform.position, currentTarget.position) < 0.2f)
            {
                currentTarget = currentTarget == patrolPointA ? patrolPointB : patrolPointA;
            }
        }
    } 
    private void FixedUpdate() 
    {
        // Determine Target to lock on, hohoho
        Transform target = isChasing ? player : currentTarget;

        // Target Direction
        float direction = Mathf.Sign(target.position.x - transform.position.x);

        // Chase player
        float horizontalVelocity = direction * speed;

        // Stop if grounded and wall ahead
        if (isGrounded && wallAhead)
        {
            horizontalVelocity = 0f; // we stop before jumping over wall :D
        }

        // No moving when attacking
        if(currentState != EnemyState.Attack)
        {
            rb.linearVelocity = new Vector2(horizontalVelocity, rb.linearVelocity.y);
        }
        

        // JUMP GOOOO
        if (isGrounded && shouldJump && Time.time >= nextJumpTime) 
        {
            nextJumpTime = Time.time + jumpCooldown;

            shouldJump = false;

            float horizontalBoost = wallAhead ? speed * 1.8f : speed * 0.8f; // To increase jump power for walls hehehehe

            Vector2 jumpVector = new Vector2(direction * horizontalBoost, jumpForce); 
            rb.AddForce(jumpVector, ForceMode2D.Impulse);
        } 
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

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}