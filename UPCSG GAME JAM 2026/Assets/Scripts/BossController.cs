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
    public int contactDamage = 1;
    public int maxHealth = 20;
    public int currentHealth;

    // NEW: Individual HP for each arm
    public int maxArmHP = 10;
    private int leftArmHP;
    private int rightArmHP;

    private bool isPhase2 = false;
    private bool isAttacking = false;

    void Start()
    {
        currentHealth = maxHealth;

        // Initialize arms separately
        leftArmHP = maxArmHP;
        rightArmHP = maxArmHP;

        if (bossBodyHP != null)
        {
            bossBodyHP.maxValue = maxHealth;
            bossBodyHP.value = currentHealth;
        }

        // The Shield Slider represents the TOTAL HP of both arms
        if (bossArmsHP != null)
        {
            bossArmsHP.maxValue = maxArmHP * 2;
            bossArmsHP.value = leftArmHP + rightArmHP;
        }

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

            yield return new WaitForSeconds(2f);

            if (!isPhase2)
            {
                // --- PHASE 1 ---
                // Only try to attack if at least one arm is alive
                if (!leftArm.isBroken || !rightArm.isBroken)
                {
                    int coinFlip = Random.Range(0, 2);

                    // Logic: Try Left, if broken, force Right. Try Right, if broken, force Left.

                    if (coinFlip == 0)
                    {
                        if (!leftArm.isBroken)
                        {
                            isAttacking = true;
                            if (Random.value > 0.5f) StartCoroutine(RunAttack(leftArm.SwipeAttack()));
                            else StartCoroutine(RunAttack(leftArm.SmashAttack()));
                        }
                        else if (!rightArm.isBroken) // Left is broken, use Right instead
                        {
                            isAttacking = true;
                            if (Random.value > 0.5f) StartCoroutine(RunAttack(rightArm.SwipeAttack()));
                            else StartCoroutine(RunAttack(rightArm.SmashAttack()));
                        }
                    }
                    else // coinFlip == 1
                    {
                        if (!rightArm.isBroken)
                        {
                            isAttacking = true;
                            if (Random.value > 0.5f) StartCoroutine(RunAttack(rightArm.SwipeAttack()));
                            else StartCoroutine(RunAttack(rightArm.SmashAttack()));
                        }
                        else if (!leftArm.isBroken) // Right is broken, use Left instead
                        {
                            isAttacking = true;
                            if (Random.value > 0.5f) StartCoroutine(RunAttack(leftArm.SwipeAttack()));
                            else StartCoroutine(RunAttack(leftArm.SmashAttack()));
                        }
                    }
                }
            }
            else
            {
                // --- PHASE 2 ---
                isAttacking = true;
                StartCoroutine(rightArm.Phase2RainPattern());
                yield return StartCoroutine(leftArm.Phase2RainPattern());
                isAttacking = false;
            }
        }
    }

    IEnumerator RunAttack(IEnumerator attackRoutine)
    {
        yield return StartCoroutine(attackRoutine);
        isAttacking = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().PlayerTakeDamage(contactDamage);
        }
    }

    // Call this when Player hits the Boss Body
    public void TakeDamage(int damage)
    {
        // Shield is active if EITHER arm is still alive
        if (leftArmHP > 0 || rightArmHP > 0)
        {
            Debug.Log("Shield blocked! Destroy both arms first!");
            return;
        }

        currentHealth -= damage;

        if (bossBodyHP != null) bossBodyHP.value = currentHealth;

        Debug.Log("Boss HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Call this when Player hits an Arm
    public void DamageArm(int damage, bool hitLeftArm)
    {
        // 1. Handle Left Arm Hit
        if (hitLeftArm)
        {
            if (leftArmHP > 0)
            {
                leftArmHP -= damage;
                Debug.Log("Left Arm HP: " + leftArmHP);

                if (leftArmHP <= 0)
                {
                    leftArmHP = 0; // Clamp to 0
                    if (!leftArm.isBroken)
                    {
                        Debug.Log("Left Arm Destroyed!");
                        leftArm.isBroken = true;
                        leftArm.gameObject.SetActive(false);
                    }
                }
            }
        }
        // 2. Handle Right Arm Hit
        else
        {
            if (rightArmHP > 0)
            {
                rightArmHP -= damage;
                Debug.Log("Right Arm HP: " + rightArmHP);

                if (rightArmHP <= 0)
                {
                    rightArmHP = 0; // Clamp to 0
                    if (!rightArm.isBroken)
                    {
                        Debug.Log("Right Arm Destroyed!");
                        rightArm.isBroken = true;
                        rightArm.gameObject.SetActive(false);
                    }
                }
            }
        }

        // 3. Update the UI Slider (Total Shield)
        if (bossArmsHP != null)
        {
            bossArmsHP.value = leftArmHP + rightArmHP;
        }

        // 4. Trigger Phase 2 if BOTH are dead
        if (leftArmHP <= 0 && rightArmHP <= 0 && !isPhase2)
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

        // Visual change
        if (leftArm.GetComponent<SpriteRenderer>())
            leftArm.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        if (rightArm.GetComponent<SpriteRenderer>())
            rightArm.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
    }

    void Die()
    {
        Debug.Log("Boss Defeated!");
        Destroy(gameObject);
    }
}