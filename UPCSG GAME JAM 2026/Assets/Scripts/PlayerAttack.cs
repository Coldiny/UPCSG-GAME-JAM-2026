using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("References")]
    public Transform attackPos;

    [Header("Abilities")]
    public bool canMAttack = false;
    public bool canRAttack = false;

    [Header("Settings")]
    public float startTimeBtwAttack;
    public float attackRange;
    public int damageAmount;
    public LayerMask whatIsEnemies;

    private float timeBtwAttack;

    public void Update()
    {
        if (timeBtwAttack <= 0 && canMAttack)
        {
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                MAttack();
            }
        }
        else
        {
            timeBtwAttack -= Time.deltaTime;
        }
    }

    void MAttack()
    {

        Debug.Log("Attack!");

        // 3. Physics Logic (Converted to 2D)
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);

        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            Enemy enemyScript = enemiesToDamage[i].GetComponent<Enemy>();

            if (enemyScript != null)
            {
                enemyScript.TakeDamage(damageAmount);
            }
        }

        timeBtwAttack = startTimeBtwAttack;
    }

    public void OnDrawGizmosSelected()
    {
        if (attackPos == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }
}