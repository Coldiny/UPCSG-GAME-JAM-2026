using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    [Header("UI")]
    public Slider bossBodyHP;
    public Slider bossArmsHP;
        
    [Header("Components")]
    public BossArm leftArm;
    public BossArm rightArm;

    [Header("Stats")]
    public int maxHealth = 20;
    public int currentHealth;

    // The "Shield" is the HP of the arms combined (10 each)
    public int armShieldHP;

    private bool isPhase2 = false;
    private bool isAttacking = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (bossBodyHP != null)
        {
            bossBodyHP.maxValue = maxHealth;
            bossBodyHP.value = currentHealth;
        }
        if (bossArmsHP != null)
        {
            bossArmsHP.maxValue = armShieldHP;
            bossArmsHP.value = armShieldHP;
        }

        // Start the attack loop
        StartCoroutine(BossLogicLoop());
    }

    IEnumerator BossLogicLoop()
    {
        while (currentHealth > 0)
        {
            if (isAttacking)
            {
                yield return null;
                continue;
            }

            yield return new WaitForSeconds(2f); // Cooldown between attacks

            if (!isPhase2)
            {
                // --- PHASE 1: Random Arm Attacks ---
                int coinFlip = Random.Range(0, 2); // 0 or 1

                // 50/50 Chance to Smash or Swipe
                // Also ensures we don't try to use a broken arm if we partially broke one
                if (coinFlip == 0 && !leftArm.isBroken)
                {
                    isAttacking = true;
                    // Randomly choose Swipe or Smash
                    if (Random.value > 0.5f) StartCoroutine(RunAttack(leftArm.SwipeAttack()));
                    else StartCoroutine(RunAttack(leftArm.SmashAttack()));
                }
                else if (!rightArm.isBroken)
                {
                    isAttacking = true;
                    if (Random.value > 0.5f) StartCoroutine(RunAttack(rightArm.SwipeAttack()));
                    else StartCoroutine(RunAttack(rightArm.SmashAttack()));
                }
            }
            else
            {
                // --- PHASE 2: Rain Fire ---
                isAttacking = true;
                // Both arms do the rain pattern
                StartCoroutine(rightArm.Phase2RainPattern());
                yield return StartCoroutine(leftArm.Phase2RainPattern()); // Wait for one to finish
                isAttacking = false;
            }
        }
    }

    // Wrapper to handle the isAttacking flag
    IEnumerator RunAttack(IEnumerator attackRoutine)
    {
        yield return StartCoroutine(attackRoutine);
        isAttacking = false;
    }

    // Call this when Player hits the Boss Body
    public void TakeDamage(int damage)
    {
        if (armShieldHP > 0)
        {
            Debug.Log("Shield blocked the damage! Break the arms!");
            return;
        }

        currentHealth -= damage;

        if (bossBodyHP != null)
        {
            bossBodyHP.value = currentHealth;
        }

        Debug.Log("Boss HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Call this when Player hits an Arm
    public void DamageArm(int damage, bool hitLeftArm)
    {
        if (armShieldHP <= 0) return; // Arms are already "dead"

        armShieldHP -= damage;
        if (bossArmsHP != null)
        {
            bossArmsHP.value = armShieldHP;
        }
        Debug.Log("Arm/Shield HP: " + armShieldHP);

        // Visual break logic: If shield is halfway gone, break one arm
        if (armShieldHP <= 0 && !leftArm.isBroken && hitLeftArm)
        {
            leftArm.isBroken = true;
            leftArm.gameObject.SetActive(false); // Hide arm or change sprite to broken
        }
        else if (armShieldHP <= 0 && !rightArm.isBroken && !hitLeftArm)
        {
            rightArm.isBroken = true;
            rightArm.gameObject.SetActive(false);
        }

        // Trigger Phase 2 if Shield is 0
        if (armShieldHP <= 0)
        {
            EnterPhase2();
        }
    }

    void EnterPhase2()
    {
        Debug.Log("ENTERING PHASE 2!");
        isPhase2 = true;

        // Revive arms for the ghostly phase 2 attack
        leftArm.isBroken = false;
        rightArm.isBroken = false;
        leftArm.gameObject.SetActive(true);
        rightArm.gameObject.SetActive(true);

        // Change their color or alpha to look "ghostly" if you want
        leftArm.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        rightArm.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
    }

    void Die()
    {
        Debug.Log("Boss Defeated!");
        leftArm.gameObject.SetActive(true);
        rightArm.gameObject.SetActive(true);
        // Add death animation or scene load here
        Destroy(gameObject);
    }
}