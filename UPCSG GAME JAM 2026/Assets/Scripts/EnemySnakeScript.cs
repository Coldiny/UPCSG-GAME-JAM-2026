using UnityEngine;

public class EnemySnakeScript : MonoBehaviour
{
    [Header("Refencers")]
    Enemy enemy;
    EnemyMeleeAttack melee;

    [Header("Pattern Settings")]
    public int meleeRepeats = 1;

    int meleeCount;

    [Header("Fear of Light")]
    public bool isInLight;
    public float fearCooldown = 1f;

    float fearEndTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemy = GetComponent<Enemy>();
        melee = GetComponent<EnemyMeleeAttack>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isInLight || Time.time < fearEndTime)
        {
            return;
        }

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Light"))
        {
            EnterFear();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("Light"))
        {
            EnterFear();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Light"))
        {
            isInLight = false;
        }
    }

    void EnterFear()
    {
        isInLight = true;
        fearEndTime = Time.time + fearCooldown;

        // born to chase, FORCED TO PATROL
        enemy.currentState = Enemy.EnemyState.Patrol;

        // no more attacks FORCED TO PATROL
        if(melee != null)
        {
            melee.CancelAttack();
        }

        meleeCount = 0;
    }
}
