using System.Collections;
using UnityEngine;

public class EnemyMantisScript : MonoBehaviour
{
    [Header("Refencers")]
    Enemy enemy;
    EnemyMeleeAttack melee;
    EnemyPlungeAttack plunge;
    Animator anim;

    bool patternRunning = false;

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

        if (anim != null)
        {
            anim.SetFloat("yVelocity", enemy.rb.linearVelocity.y);
            anim.SetBool("isGrounded", enemy.isGrounded);
        }

        if (!patternRunning)
        {
            StartCoroutine(AttackPattern());  
        }
    }

    IEnumerator AttackPattern()
    {
        patternRunning = true;

        // MELEE SLASH SLASH SLASH
        melee.tryMelee(); // called by ANIMATION MY SAVIOUR


        yield return new WaitUntil(() => !melee.isAttacking); // wait until all 3 attacks done

        yield return new WaitForSeconds(0.2f);

        // GO HIGH AND DOWN LOW (PLUNGE)
        plunge.TryPlunge();
        yield return new WaitUntil(() => !plunge.isAttacking);

        patternRunning = false;
    }
}
