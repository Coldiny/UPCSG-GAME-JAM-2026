using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static System.Net.WebRequestMethods;
public class Enemy : MonoBehaviour, IAttackable
{
    [Header("References")] 
    public Rigidbody2D rb;
    private Collider2D col;
    public Transform player; 
    public LayerMask groundLayer;
    public Animator anim;
    
    [Header("Settings")] 
    public int health; 
    public float speed; 
    public int Edamage; 
    public float detectionRange = 8f;
    public float attackRange = 1.6f;
    public float attackExitRange = 2.2f;

    public bool isGrounded; 

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
    public bool canJump = false;

    [Header("Knockback Settings")]
    public float knockbackForce = 5f;
    public float knockbackUpForce = 2f;
    public float knockbackDuration = 0.2f;
    private float knockbackTimer = 1f;
    public bool canTakeKnockback = true;

    [Header("Status Effects")]
    public bool isFeared;

    [Header("Item Dependency")]
    public BoolItemSO requiredItem;
    public bool hostileWithoutItem = true;


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
        anim = GetComponent<Animator>();
        spriteRenderer.color = normalColor;
        currentTarget = patrolPointB; // 1st patrol target
    }
    void Update()
    {

        Debug.Log($"{name} item state: {requiredItem?.value}");


        // Determine the distance between enemy and player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if(isFeared)
        {
            isChasing = false;
        } else
        {
            isChasing = distanceToPlayer <= detectionRange;
        }

        // Adding a bit extra range for attack, so that exiting attack range wont immediately stop attack state
        // Decide state

        // If enemy depends on an item and player has it → force passive
        if (requiredItem != null && requiredItem.value)
        {
            currentState = EnemyState.Patrol;
            isChasing = false;
        }
        else if (isFeared)
        {
            currentState = EnemyState.Patrol;
        }
        else
        {
            if (currentState == EnemyState.Attack)
            {
                if (distanceToPlayer > attackExitRange)
                {
                    currentState = EnemyState.Chase;
                }
            }
            else
            {
                if (distanceToPlayer <= attackRange)
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

        // Patrol target switching
        if (!isChasing && Vector2.Distance(transform.position, currentTarget.position) < 0.5f)
        {
            currentTarget = currentTarget == patrolPointA ? patrolPointB : patrolPointA;
        }

        // Decide target
        Transform target = currentState == EnemyState.Patrol ? currentTarget : player;

        // Target Direction
        float direction = Mathf.Sign(target.position.x - transform.position.x);

        // Flippity the spritey and face the targettyyy YEAAAAH
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction; // if negative direction becomes positive and vice versa, thus switching
        transform.localScale = scale;

        // isGrounded?
        Vector2 feetPos = new Vector2(col.bounds.center.x, col.bounds.min.y); // so we can locate feet
        isGrounded = Physics2D.Raycast(feetPos, Vector2.down, 0.05f, groundLayer);

        

        // DEBUG
        Debug.DrawRay(feetPos, Vector2.down * 0.05f, Color.yellow); // isGrounded

        

    } 
    private void FixedUpdate() 
    {
        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.fixedDeltaTime;
        }

        // if attacking, not default movement or knocked no moving allowed
        if (currentState == EnemyState.Attack || !useDefaultMovement || knockbackTimer > 0f)
        {
            return;
        }

        // Determine Target to lock on, hohoho
        Transform target = (currentState == EnemyState.Patrol) ? currentTarget : player;

        // Target Direction
        float direction = Mathf.Sign(target.position.x - transform.position.x);

        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);

        
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Enemy took " + damage + " damage. Remaining health: " + health);

        anim.SetTrigger("takeDmg");

        if (health <= 0)
        {
            Die();
        }

        if(player != null && canTakeKnockback && isGrounded)
        {
            float dir = Mathf.Sign(transform.position.x - player.position.x); // this is pushing away from player

            rb.linearVelocity = new Vector2(dir * knockbackForce, knockbackUpForce);
            knockbackTimer = knockbackDuration;
        }
    }

    public void OnHurtAnimationEnd()
    {
        anim.SetBool("isHurt", false);
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
        if (collision.gameObject.CompareTag("Player") && currentState == EnemyState.Attack)
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