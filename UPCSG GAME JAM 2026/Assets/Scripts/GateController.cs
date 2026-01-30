using UnityEngine;

public class GateController : MonoBehaviour
{
    private Rigidbody2D rb;
    public bool moveGate = false;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (moveGate)
        {
            rb.linearVelocity = new Vector2(0, -5f);
        }
    }
}
