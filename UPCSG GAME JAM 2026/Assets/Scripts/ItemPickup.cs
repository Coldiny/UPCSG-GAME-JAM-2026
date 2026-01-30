using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public BoolItemSO itemState;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    public void Collect()
    {
        Debug.Log("Item collected: " + itemState.name);

        itemState.Set(true);
        Destroy(gameObject);
    }
}
