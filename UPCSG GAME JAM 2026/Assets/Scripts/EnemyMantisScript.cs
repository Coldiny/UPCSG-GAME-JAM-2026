using UnityEngine;

public class EnemyMantisScript : MonoBehaviour
{
    [Header("Refencers")]
    Enemy enemy;
    EnemyMeleeAttack melee;
    EnemyPlungeAttack plunge;

    [Header("Pattern Settings")]
    public int meleeRepeats = 3;

    int meleeCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemy = GetComponent<Enemy>();
        melee = GetComponent<EnemyMeleeAttack>();
        plunge = GetComponent<EnemyPlungeAttack>();
    }

    // Update is called once per frame
    void Update()
    {
        if(enemy.currentState != Enemy.EnemyState.Attack)
        {
            return;
        }

        if(AnyAttackRunning())
        {
            return;
        }

        ExecutePattern();
    }

    bool AnyAttackRunning()
    {
        if((melee != null && melee.isAttacking) || 
           (plunge != null && plunge.isAttacking))
        {
            return true;
        }

        return false;
    }

    void ExecutePattern()
    {
        if(meleeCount < meleeRepeats && melee != null)
        {
            melee.tryMelee();
            meleeCount++;
            return;
        }

        if(plunge != null)
        {
            plunge.TryPlunge();
        }

        meleeCount = 0;
    }
}
