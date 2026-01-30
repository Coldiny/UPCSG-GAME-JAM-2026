using System.Collections;
using UnityEngine;

public class EnemyPlungeAttack : MonoBehaviour
{
    [Header("References")]
    Rigidbody2D rb;
    Enemy enemy;

    [Header("Plunge Settings")]
    public float plungeJumpForce = 10f;
    public float plungeForce = 10f;
    public float hangTime = 0.25f;
    public float plungeCooldown = 2f;

    public bool isAttacking { get; private set;}
    float nextPlungeTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemy = GetComponent<Enemy>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void TryPlunge()
    {
        if(isAttacking) {
            return;
        }

        if(Time.time < nextPlungeTime)
        {
            return;
        }

        if(enemy.currentState != Enemy.EnemyState.Attack)
        {
            return;
        }

        if(!enemy.isGrounded)
        {
            return;
        }

        StartCoroutine(PlungeRoutine());
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    if(enemy == null) // if non existent dont do anything
    //    {
    //        return;
    //    }

    //    if(enemy.currentState != Enemy.EnemyState.Attack) // if not in attack state dont do anything
    //    {
    //        return;
    //    }

    //    if(!enemy.isGrounded) // if not grounded dont do anything
    //    {
    //        return;
    //    }

    //    if (isPlunging) // if is plunging dont do anything
    //    {
    //        return;
    //    }

    //    if(Time.time < nextPlungeTime) // if cooldown not done yet dont do anything
    //    {
    //        return;
    //    }

    //    bool playerBelow = enemy.player.position.y < transform.position.y + 0.2f; // check if player below enemy in Y level, with 0.2f padding

    //    if(playerBelow || enemy.currentState == Enemy.EnemyState.Attack)
    //    {
    //        StartCoroutine(PlungeRoutine());
    //    }
    //}

    private IEnumerator PlungeRoutine()
    {
        isAttacking = true;
        nextPlungeTime = Time.time + plungeCooldown;

        // no other linear movement, AND  PLUS ULTRA JUMPPPPPPPPPPP
        Debug.Log("JUMP");
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(Vector2.up * plungeJumpForce, ForceMode2D.Impulse);

        Debug.Log("AURA");
        // aura farm in the air for a moment
        yield return new WaitForSeconds(hangTime);

        //float horizontalDir = Mathf.Sign(enemy.player.position.x - transform.position.x) * 0.5f;

        Vector2 direction = (enemy.player.position - transform.position).normalized;
        direction.x *= 0.5f;
        direction.y = -1f;


        Debug.Log("SMASH");
        // SMAAAAAAAAAASH
        //rb.linearVelocity = new Vector2(horizontalDir * plungeForce, -plungeForce);

        rb.AddForce(direction * plungeForce, ForceMode2D.Impulse);

        yield return new WaitUntil(() => enemy.isGrounded);

        rb.linearVelocity = Vector2.zero;
        isAttacking = false;
    }
}
