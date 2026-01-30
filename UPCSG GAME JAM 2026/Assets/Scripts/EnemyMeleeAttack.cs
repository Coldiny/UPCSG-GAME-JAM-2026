using System.Collections;
using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour
{
    [Header("References")]
    public Transform attackPoint;
    Enemy enemy;
    Animator anim;

    [Header("Melee Settings")]
    public float attackRadius = 0.8f;
    public int damage = 1;
    public float attackCooldown = 1f;
    public float windUpTime = 0.15f;

    private float nextAttackTime;
    public bool isAttacking { get; private set; }

    public bool useAnimationEvents = false;

    // Update is called once per frame

    private void Start()
    {
        enemy = GetComponent<Enemy>();
        anim = GetComponent<Animator>();
    }

    public void tryMelee()
    {
        if (isAttacking)
        {
            return;
        }

        if(Time.time < nextAttackTime)
        {
            return;
        }

        if(enemy.currentState != Enemy.EnemyState.Attack)
        {
            return;
        }

        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown; // ATTACK cooldown, cause I was doin hit cooldown before :<
        anim.SetTrigger("Attack");

        if(!useAnimationEvents)
        {
            StartCoroutine(MeleeRoutine()); // simple enemies (AKA NO COMBOS LIKE MANTIS AAAAAAAAAAAAAAAAAAAAAAA)
        }
        
    }

    private IEnumerator MeleeRoutine()
    {
        yield return new WaitForSeconds(windUpTime);
        DoHit();
        yield return new WaitForSeconds(0.2f);
        isAttacking = false;
    }

    public void DoHit()
    {
        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackRadius, LayerMask.GetMask("Player"));

        if (hit != null)
        {
            hit.GetComponent<PlayerHealth>()?.PlayerTakeDamage(damage); // the ? is not me losing my mind, it makes it null if no PlayerHealth is found = no crash
        }
    }

    public void EndMelee()
    {
        isAttacking = false;
    }

    public void CancelAttack()
    {
        StopAllCoroutines();
        isAttacking = false;
    }

    //void Update()
    //{
    //    if(Time.time < nextAttackTime)
    //    {
    //        return;
    //    }
    //    if (enemy.currentState == Enemy.EnemyState.Attack)
    //    {
    //        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackRadius, LayerMask.GetMask("Player
    //        if (hit != null)
    //        {
    //            hit.GetComponent<PlayerHealth>()?.PlayerTakeDamage(damage); // the ? is not me losing my mind, it makes it null if no PlayerHealth is found = no crash
    //            nextAttackTime = Time.time + attackCooldown;
    //        }
    //    
    //}

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
