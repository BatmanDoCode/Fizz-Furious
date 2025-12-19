using UnityEngine;

public class TVlightFlicker : MonoBehaviour
{
    public Light tvLight;
    public float flickerSpeed;
    public float minIntensity;
    public float maxIntensity;

    void Update()
    {
        if (tvLight != null)
        {
            float t = Mathf.PingPong(Time.time * flickerSpeed, 1f);
            tvLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
        }
    }
}
