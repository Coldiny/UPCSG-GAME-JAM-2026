using UnityEngine;
using UnityEngine.Tilemaps;

public class LightPlatform : MonoBehaviour, ILightReceiver
{

    [Header("References")]
    TilemapCollider2D platformCollider;
    TilemapRenderer tr;

    public Color activeColor;
    public Color inactiveColor = new Color(1, 1, 1, 0.3f);

    void Awake()
    {
        platformCollider = GetComponent<TilemapCollider2D>();
        tr = GetComponent<TilemapRenderer>();
        SetActive(false);
    }

    public void OnlightOn()
    { 
        SetActive(true);
    }

    public void OnlightOff()
    {
        SetActive(false);
    }

    void SetActive(bool active)
    {
        Debug.Log(active);
        platformCollider.enabled = active;
        tr.enabled = active;
    }
}
