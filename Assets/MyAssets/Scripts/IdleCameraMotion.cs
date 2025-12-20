using UnityEngine;

public class IdleCameraMotion : MonoBehaviour
{
    public float amplitude;
    public float frequency;

    private Vector3 startPos;
    private Quaternion startRot;

    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
       float offsetY = Mathf.Sin(Time.time * frequency) * amplitude;
       float offsetX = Mathf.Cos(Time.time * frequency) * amplitude;
       transform.position = startPos + new Vector3(offsetX, offsetY, 0);
       
       float rotY = Mathf.Sin(Time.time * 0.3f) * 2f;
       transform.rotation = startRot * Quaternion.Euler(0, rotY, 0);
    }
}
