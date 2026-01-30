using System.Collections;
using UnityEngine;

public class EnemyChargeAttack : MonoBehaviour
{
    [Header("References")]
    Enemy enemy;
    Rigidbody2D rb;

    [Header("Charge Settings")]
    public float chargeMoveSpeed = 12f;
    public float chargeDuration = 0.5f;
    public float chargeCooldown = 2f;
    public float windUpTime = 0.25f;

    float nextChargeTime;
    public bool isAttacking { get; private set; }
    
    float chargeDirection;

    private void Start()
    {
        enemy = GetComponent<Enemy>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void TryCharge()
    {
        if(isAttacking)
        {
            return;
        }

        if(Time.time < nextChargeTime)
        {
            return;
        }

        if(enemy.currentState != Enemy.EnemyState.Attack)
        {
            return;
        }

        StartCoroutine(ChargeRoutine());
    }

    private IEnumerator ChargeRoutine()
    {
        isAttacking = true;
        nextChargeTime = Time.time + chargeCooldown;

        // NO CHANGING DIRECTIONS WHILE CHARGGINNNG
        chargeDirection = Mathf.Sign(enemy.player.position.x - transform.position.x);

        // WIND IT UP
        yield return new WaitForSeconds(windUpTime);

        float timer = chargeDuration;

        // CHARGE FORWARD
        while(timer > 0f)
        {
            timer -= Time.deltaTime;
            rb.linearVelocity = new Vector2(chargeDirection * chargeMoveSpeed, rb.linearVelocity.y);
            yield return null;
        }

        // ok stop that charge
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        isAttacking = false;

        
    }

    //void Update()
    //{
    //    if(enemy.currentState != Enemy.EnemyState.Attack)
    //    {
    //        isCharging = false;
    //        return;
    //    }

    //    if(!isCharging && Time.time >= nextChargeTime)
    //    {
    //        StartCharge();
    //    }

    //    if(isCharging)
    //    {
    //        chargeTimer -= Time.deltaTime;
            
    //        if(chargeTimer <= 0f)
    //        {
    //            rb.linearVelocity = new Vector2(chargeDirection * chargeMoveSpeed, rb.linearVelocity.y);
    //            StopCharge();
    //        }
    //    }
    //}

    //void StartCharge()
    //{
    //    isCharging = true;
    //    chargeTimer = chargeDuration;
    //    nextChargeTime = Time.time + chargeCooldown;

    //    // this locks the charging direction, so the enemy doesnt just change directions mid charge
    //    chargeDirection = Mathf.Sign(enemy.player.position.x - transform.position.x);
    //}

    //void StopCharge()
    //{
    //    isCharging = false;
    //}
}
