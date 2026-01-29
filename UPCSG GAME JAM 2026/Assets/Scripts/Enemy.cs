using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static System.Net.WebRequestMethods;
public class Enemy : MonoBehaviour, IAttackable
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
    public float attackExitRange = 2.2f;

    public bool isGrounded; 
    
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

    [Header("Movement Options")]
    public bool useDefaultMovement = true;

    [Header("Knockback Settings")]
    public float knockbackForce = 5f;
    public float knockbackUpForce = 2f;
    public float knockbackDuration = 0.2f;
    private float knockbackTimer = 0f;
    public bool canTakeKnockback = true;

    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack
    }

    public void OnHit(int damage)
    {
        TakeDamage(damage);
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

        // Adding a bit extra range for attack, so that exiting attack range wont immediately stop attack state
        // Decide state
        if(currentState == EnemyState.Attack)
        {
            if(distanceToPlayer > attackExitRange)
            {
                currentState = EnemyState.Chase;
            }
        } else
        {
            if(distanceToPlayer <= attackRange)
            {
                currentState = EnemyState.Attack;
            }
            else if (distanceToPlayer <= detectionRange)
            {
                currentState = EnemyState.Chase;
            }
            else
            {
                currentState = EnemyState.Patrol;
            }
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

        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.fixedDeltaTime;
        }

        // No moving when attacking
        if(currentState != EnemyState.Attack && useDefaultMovement && knockbackTimer <= 0f)
        {
            // Chase player
            float horizontalVelocity = direction * speed;

            // Stop if grounded and wall ahead
            if (isGrounded && wallAhead)
            {
                horizontalVelocity = 0f; // we stop before jumping over wall :D
            }

            Debug.Log(useDefaultMovement);
            rb.linearVelocity = new Vector2(horizontalVelocity, rb.linearVelocity.y);
        }
        
        // JUMP GOOOO, PLUS ULTRA
        if (currentState != EnemyState.Attack && isGrounded && shouldJump && Time.time >= nextJumpTime) 
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

        if(player != null && canTakeKnockback)
        {
            float dir = Mathf.Sign(transform.position.x - player.position.x); // this is pushing away from player

            rb.linearVelocity = new Vector2(dir * knockbackForce, knockbackUpForce);
            knockbackTimer = knockbackDuration;
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

    // for outside forces to get the patrol target location, like Mr. Frog
    public Transform GetCurrentPatrolTarget()
    {
        return currentTarget;
    }

    // to switch patrol outside
    public void SwitchPatrolTarget()
    {
        currentTarget = currentTarget == patrolPointA ? patrolPointB : patrolPointA;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackExitRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}