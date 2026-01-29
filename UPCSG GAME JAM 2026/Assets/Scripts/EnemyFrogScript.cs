using UnityEngine;

public class EnemyFrogScript : MonoBehaviour
{
    [Header("Refencers")]
    Enemy enemy;
    EnemyMeleeAttack melee;
    Rigidbody2D rb;

    [Header("Pattern Settings")]
    public int meleeRepeats = 1;

    int meleeCount;

    [Header("Hop Settings")]
    public float hopForceY = 7f;
    public float hopForceX = 4f;
    public float hopCooldown = 1.2f;

    float nextHopTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemy = GetComponent<Enemy>();
        melee = GetComponent<EnemyMeleeAttack>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

        if (enemy.currentState != Enemy.EnemyState.Attack)
        {
            return;
        }

        if (AnyAttackRunning())
        {
            return;
        }

        ExecutePattern();
    }

    private void FixedUpdate()
    {
        if(enemy.isGrounded)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        HandleHopping();
    }

    void HandleHopping()
    {
        if(!enemy.isGrounded)
        {
            return;
        }

        if(Time.time < nextHopTime)
        {
            return;
        }


        if(enemy.currentState == Enemy.EnemyState.Patrol)
        {
            if(Vector2.Distance(transform.position, enemy.GetCurrentPatrolTarget().position) < 1f)
            {
                enemy.SwitchPatrolTarget();
                return;
            }
        }

        // decides whether patrol or enemy
        Transform target = enemy.currentState == Enemy.EnemyState.Patrol ? enemy.GetCurrentPatrolTarget() : enemy.player;

        // direction of HOP
        float dir = Mathf.Sign(target.position.x - transform.position.x);

        // LUNGE when in ATTACK state
        float finalX = enemy.currentState == Enemy.EnemyState.Chase ? hopForceX * 3f : hopForceX;
        
        //Debug.Log("FinalX " + finalX);
        Vector2 hop = new Vector2(dir * finalX, hopForceY);

        //Debug.Log("Hop dir " + dir);
        //Debug.Log("Hop val " + hop);
        
        rb.linearVelocity = new Vector2(dir * finalX, hopForceY);

        nextHopTime = Time.time + hopCooldown;
    }

    bool AnyAttackRunning()
    {
        if ((melee != null && melee.isAttacking))
        {
            return true;
        }

        return false;
    }

    void ExecutePattern()
    {
        if (meleeCount < meleeRepeats && melee != null)
        {
            melee.tryMelee();
            meleeCount++;
            return;
        }

        meleeCount = 0;
    }
}
