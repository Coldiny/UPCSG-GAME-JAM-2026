using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CrystalLight : MonoBehaviour
{
    [Header("references")]
    public GameObject lightZone;
    SpriteRenderer sr;
    Light2D light2D;

    [Header("Light Settings")]
    public Color inactiveColor = Color.gray;
    public Color activeColor = Color.white;
    public float lightDuration = 5f;
    public float lightIntensityOn = 1.2f;
    public float lightIntensityOff = 0f;

    bool isLit;
    float lightEndTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        light2D = GetComponent<Light2D>();
        light2D.intensity = 0f;
        sr.color = inactiveColor;

        if(lightZone != null)
        {
            lightZone.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isLit && Time.time >= lightEndTime)
        {
            TurnOff();
        }
    }

    // Player bonk the crystal TO LIGHT IT
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerAttack"))
        {
            Debug.Log("Hit crystal");
            LightUp();
        }
    }

    void LightUp()
    {
        if(isLit)
        {
            return;
        }

        isLit = true;
        lightEndTime = Time.time + lightDuration;

        sr.color = activeColor;

        if(lightZone != null)
        {
            Debug.Log("Light Up");
            light2D.intensity = lightIntensityOn;
            lightZone.SetActive(true);
        }
    }

    void TurnOff()
    {
        isLit = false;

        sr.color = inactiveColor;

        if(lightZone != null)
        {
            Debug.Log("Turn Off");
            light2D.intensity = lightIntensityOff;
            lightZone.SetActive(false);
        }
    }
}
