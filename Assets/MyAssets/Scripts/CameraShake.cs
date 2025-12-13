using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraShake : MonoBehaviour
{
    private Vector3 _originalLocalPosition;
    private Coroutine _shakeCoroutine;

    private void Awake()
    {
        _originalLocalPosition = transform.localPosition;
    }

    public void Shake(float intensity, float duration)
    {
        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
        }

        _shakeCoroutine = StartCoroutine(ShakeRoutine(intensity, duration));
    }

    private IEnumerator ShakeRoutine(float intensity, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector3 randomOffset = Random.insideUnitSphere * intensity;
            transform.localPosition = _originalLocalPosition + randomOffset;

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        
        transform.localPosition = _originalLocalPosition;
    }
}
