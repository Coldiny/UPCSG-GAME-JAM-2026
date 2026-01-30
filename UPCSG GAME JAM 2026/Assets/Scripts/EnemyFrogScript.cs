using System.Collections;
using UnityEngine;

public class EnemyFrogScript : MonoBehaviour
{
    [Header("Refencers")]
    Enemy enemy;
    EnemyMeleeAttack melee;
    Animator anim;

    [Header("Pattern Settings")]
    public int meleeRepeats = 1;

    int meleeCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemy = GetComponent<Enemy>();
        melee = GetComponent<EnemyMeleeAttack>();
        anim = GetComponent<Animator>();
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
        return (melee != null && melee.isAttacking);
    }

    void ExecutePattern()
    {
        if (meleeCount < meleeRepeats && melee != null)
        {
            anim.SetBool("isAttacking", true);
            melee.tryMelee();
            meleeCount++;

            StartCoroutine(ResetAttackBool());
            return;
        }

        meleeCount = 0;
    }

    private IEnumerator ResetAttackBool()
    {
        yield return new WaitUntil(() => !melee.isAttacking);
        anim.SetBool("isAttacking", false);
    }
}
