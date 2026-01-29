using UnityEngine;

public class EnemyCrocodileScript : MonoBehaviour
{
    [Header("Refencers")]
    Enemy enemy;
    EnemyChargeAttack charge;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemy = GetComponent<Enemy>();
        charge = GetComponent<EnemyChargeAttack>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.currentState != Enemy.EnemyState.Attack)
        {
            return;
        }

        if (AnyAttackRunning())
        {
            return;
        }

        ExecutePattern();
    }

    bool AnyAttackRunning()
    {
        if ((charge != null && charge.isAttacking))
        {
            return true;
        }

        return false;
    }

    void ExecutePattern()
    {
        if (charge != null)
        {
            charge.TryCharge();
        }
    }
}
