using UnityEngine;

public class EnemyChargeAttack : MonoBehaviour
{
    [Header("References")]
    Enemy enemy;
    Rigidbody2D rb;

    [Header("Charge Settings")]
    public float chargeMoveSpeed = 12f;
    public float chargeDuration;
    public float chargeCooldown = 2f;

    float chargeTimer;
    float nextChargeTime;
    bool isCharging;

    float chargeDirection;

    private void Start()
    {
        enemy = GetComponent<Enemy>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(enemy.currentState != Enemy.EnemyState.Attack)
        {
            isCharging = false;
            return;
        }

        if(!isCharging && Time.time >= nextChargeTime)
        {
            StartCharge();
        }

        if(isCharging)
        {
            chargeTimer -= Time.deltaTime;
            

            if(chargeTimer <= 0f)
            {
                rb.linearVelocity = new Vector2(chargeDirection * chargeMoveSpeed, rb.linearVelocity.y);
                StopCharge();
            }
        }
    }

    void StartCharge()
    {
        isCharging = true;
        chargeTimer = chargeDuration;
        nextChargeTime = Time.time + chargeCooldown;

        // this locks the charging direction, so the enemy doesnt just change directions mid charge
        chargeDirection = Mathf.Sign(enemy.player.position.x - transform.position.x);
    }

    void StopCharge()
    {
        isCharging = false;
    }
}
