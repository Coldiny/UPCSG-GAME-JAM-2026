using System.Collections;
using UnityEngine;

public class EnemyCowScript : MonoBehaviour
{
    [Header("Refencers")]
    Enemy enemy;
    EnemyMeleeAttack melee;
    EnemyChargeAttack charge;

    [Header("Pattern Settings")]
    public int meleeRepeats = 2;

    bool patternRunning;

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

        if (!patternRunning)
        {
            StartCoroutine(AttackPattern());
        }
    }

    IEnumerator AttackPattern()
    {
        patternRunning = true;

        // MELEE SLASH SLASH
        for(int i = 0; i < meleeRepeats; i++)
        {
            melee.tryMelee();
            yield return new WaitUntil(() => !melee.isAttacking);
        }

        // PUSH FORWARD
        charge.TryCharge();
        yield return new WaitUntil(() => !charge.isAttacking);

        patternRunning = false;
    }
}
