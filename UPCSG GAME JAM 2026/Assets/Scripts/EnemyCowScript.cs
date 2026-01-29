using UnityEngine;

public class EnemyCowScript : MonoBehaviour
{
    [Header("Refencers")]
    Enemy enemy;
    EnemyMeleeAttack melee;
    EnemyChargeAttack charge;

    [Header("Pattern Settings")]
    public int meleeRepeats = 2;

    int meleeCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemy = GetComponent<Enemy>();
        melee = GetComponent<EnemyMeleeAttack>();
        charge = GetComponent<EnemyChargeAttack>();
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

    bool AnyAttackRunning()
    {
        if ((melee != null && melee.isAttacking) ||
           (charge != null && charge.isAttacking))
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

        if (charge != null)
        {
            charge.TryCharge();
        }

        meleeCount = 0;
    }
}
