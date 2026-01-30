using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("References")]
    public Transform attackPos;

    [Header("Abilities")]
    public bool canMAttack = false;

    [Header("Settings")]
    public float startTimeBtwAttack;
    public float attackRange;
    public int damageAmount;
    public LayerMask whatIsEnemies;

    private float timeBtwAttack;

    public void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

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
        AudioManager.Instance.Play("Attack");

        // 1. Detect enemies
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);

        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            Collider2D hit = enemiesToDamage[i];

            // --- CHECK 1: Normal Enemy ---
            IAttackable attackable = hit.GetComponent<IAttackable>();
            if (attackable != null)
            {
                attackable.OnHit(damageAmount);
            }

            // --- CHECK 2: Boss Body (Direct Hit) ---
            // This works if you hit the main red square
            BossController bossBody = hit.GetComponent<BossController>();
            if (bossBody != null)
            {
                bossBody.TakeDamage(damageAmount);
                Debug.Log("Boss Body Hit!");
            }

            // --- CHECK 3: Boss Arm (Arm Hit) ---
            // This works if you hit the blue squares
            BossArm bossArm = hit.GetComponent<BossArm>();
            if (bossArm != null)
            {
                // We hit an arm! Now we need to find the Brain (BossController) 
                // which is usually on the PARENT object of the arm.
                BossController parentBoss = bossArm.GetComponentInParent<BossController>();

                if (parentBoss != null)
                {
                    // Pass the damage to the controller, telling it which arm was hit
                    parentBoss.DamageArm(damageAmount, bossArm.isLeftArm);
                    Debug.Log("Boss Arm Hit!");
                }
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