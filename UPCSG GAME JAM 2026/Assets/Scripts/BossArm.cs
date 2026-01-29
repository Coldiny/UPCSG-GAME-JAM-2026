using System.Collections;
using UnityEngine;

public class BossArm : MonoBehaviour
{
    [Header("Settings")]
    public bool isLeftArm; // CHECK THIS BOX for the Left Arm object
    public float moveSpeed = 5f;
    public int contactDamage = 1;

    [Header("Phase 2 Settings")]
    public GameObject projectilePrefab;
    public float fireRate = 0.5f;

    [HideInInspector] public bool isBroken = false;
    private Vector3 startPosition;
    private Transform player;

    void Start()
    {
        startPosition = transform.position;
        // Find player automatically (ensure Player has "Player" tag)
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    // --- PHASE 1 ATTACKS ---

    public IEnumerator SmashAttack()
    {
        if (player == null) yield break;

        // 1. Hover above player
        Vector3 hoverPos = new Vector3(player.position.x, startPosition.y, 0);
        yield return MoveTo(hoverPos, 1.0f);

        yield return new WaitForSeconds(0.2f); // Telegraphed pause

        // 2. Smash Down
        Vector3 smashPos = new Vector3(player.position.x, player.position.y, 0);
        yield return MoveTo(smashPos, 0.2f); // Fast smash!

        // 3. Wait on ground
        yield return new WaitForSeconds(0.5f);

        // 4. Return to start
        yield return MoveTo(startPosition, 1.5f);
    }

    public IEnumerator SwipeAttack()
    {
        // 1. Wind up (Left arm goes far left, Right arm goes far right)
        float windUpX = isLeftArm ? -8f : 8f;
        Vector3 readyPos = new Vector3(windUpX, player.position.y, 0);
        yield return MoveTo(readyPos, 1.0f);

        yield return new WaitForSeconds(0.2f);

        // 2. Swipe across to the opposite side
        float endX = isLeftArm ? 8f : -8f;
        Vector3 endPos = new Vector3(endX, player.position.y, 0);
        yield return MoveTo(endPos, 0.5f); // Fast swipe

        // 3. Return home
        yield return MoveTo(startPosition, 1.5f);
    }

    // --- PHASE 2 BEHAVIOR ---

    public IEnumerator Phase2RainPattern()
    {
        // Move from Left Screen Edge (-9) to Right Screen Edge (9)
        // You might need to adjust -9 and 9 based on your camera size
        Vector3 startSweep = new Vector3(-9f, 4f, 0);
        Vector3 endSweep = new Vector3(9f, 4f, 0);

        // Go to start
        yield return MoveTo(startSweep, 1.0f);

        // Move across slowly while shooting
        float duration = 4f;
        float elapsed = 0f;
        float shootTimer = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startSweep, endSweep, elapsed / duration);
            elapsed += Time.deltaTime;

            // Shooting logic
            shootTimer += Time.deltaTime;
            if (shootTimer >= fireRate)
            {
                Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                shootTimer = 0f;
            }
            yield return null;
        }

        // Return to idle spot briefly
        yield return MoveTo(startPosition, 1.0f);
    }

    // Helper function to move smoothly
    IEnumerator MoveTo(Vector3 target, float duration)
    {
        Vector3 start = transform.position;
        float time = 0;
        while (time < duration)
        {
            transform.position = Vector3.Lerp(start, target, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = target;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Deal damage to player on contact
            other.GetComponent<PlayerHealth>().PlayerTakeDamage(contactDamage);
        }
    }
}