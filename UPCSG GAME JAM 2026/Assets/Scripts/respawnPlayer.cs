using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class respawnPlayer : MonoBehaviour
{
    public GameObject spawnPoint;
    public GameObject Player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rb = Player.GetComponent<Rigidbody2D>();
            rb.linearVelocity = Vector2.zero;
            Player.transform.position = spawnPoint.transform.position;
        }
    }
}
