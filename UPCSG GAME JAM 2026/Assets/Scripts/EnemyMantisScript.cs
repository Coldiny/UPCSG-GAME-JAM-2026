using System.Collections;
using UnityEngine;

public class EnemyMantisScript : MonoBehaviour
{
    [Header("Refencers")]
    Enemy enemy;
    EnemyMeleeAttack melee;
    EnemyPlungeAttack plunge;

    [Header("Pattern Settings")]
    public int meleeRepeats = 3;

    bool patternRunning;

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

        if(!patternRunning)
        {
            StartCoroutine(AttackPattern());  
        }
    }

    IEnumerator AttackPattern()
    {
        patternRunning = true;

        // MELEE SLASH SLASH SLASH
        for(int i = 0; i < meleeRepeats; i++)
        {
            melee.tryMelee();
            Debug.Log("Try Melee");
            yield return new WaitUntil(() => !melee.isAttacking);
        }

        yield return new WaitForSeconds(0.2f); // small pause before doing the plunge attack routine

        // GO HIGH AND DOWN LOW (PLUNGE)
        plunge.TryPlunge();
        yield return new WaitUntil(() => !plunge.isAttacking);

        patternRunning = false;
    }
}
