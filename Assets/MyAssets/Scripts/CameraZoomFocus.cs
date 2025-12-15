using System;
using System.Collections;
using UnityEngine;

public class CameraZoomFocus : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float targetFOV = 30f;
    [SerializeField] private float zoomSpeed = 2f;

    private float _originalFOV;

    private void Awake()
    {
        if (cam == null)
            cam = GetComponent<Camera>();
        
        _originalFOV = cam.fieldOfView;
    }

    public void Focus()
    {
        StartCoroutine(ZoomIn());
    }

    private IEnumerator ZoomIn()
    {
        while (cam.fieldOfView > targetFOV)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, zoomSpeed * Time.unscaledDeltaTime);
            
            yield return null;
        }
    }
}
