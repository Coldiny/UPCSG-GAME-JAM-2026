using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour
{
    [Header("References")]
    public Transform attackPoint;

    [Header("Melee Settings")]
    public float attackRadius = 0.8f;
    public int damage = 1;
    public float attackCooldown = 1f;

    private float nextAttackTime;

    // Update is called once per frame
    void Update()
    {
        if(Time.time < nextAttackTime)
        {
            return;
        }

        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackRadius, LayerMask.GetMask("Player"));

        if(hit != null)
        {
            hit.GetComponent<PlayerHealth>()?.PlayerTakeDamage(damage); // the ? is not me losing my mind, it makes it null of no PlayerHealth is found = no crash
            nextAttackTime = Time.time + attackCooldown;
        }

    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
