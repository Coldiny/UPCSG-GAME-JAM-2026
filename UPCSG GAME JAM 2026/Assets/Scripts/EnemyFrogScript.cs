using UnityEngine;

public class EnemyFrogScript : MonoBehaviour
{
    [Header("Refencers")]
    Enemy enemy;
    EnemyMeleeAttack melee;

    [Header("Pattern Settings")]
    public int meleeRepeats = 1;

    int meleeCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemy = GetComponent<Enemy>();
        melee = GetComponent<EnemyMeleeAttack>();
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
